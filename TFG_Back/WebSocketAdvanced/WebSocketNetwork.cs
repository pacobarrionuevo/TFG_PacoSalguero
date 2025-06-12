using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using TFG_Back.Models.Database;
using TFG_Back.Models.Database.Entidades;
using TFG_Back.Services;
using System.IO;
using TFG_Back.Models.DTO;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace TFG_Back.WebSocketAdvanced
{
    // Clase central que gestiona todas las conexiones WebSocket activas.
    public class WebSocketNetwork
    {
        // Diccionario concurrente para almacenar los manejadores de WebSocket por ID de usuario.
        private readonly ConcurrentDictionary<int, WebSocketHandler> _handlers = new();
        // Fábrica para crear scopes de inyección de dependencias, necesario para usar servicios Scoped en un Singleton.
        private readonly IServiceScopeFactory _scopeFactory;

        public WebSocketNetwork(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
            Console.WriteLine("[WebSocketNetwork] Instancia creada.");
        }

        // Maneja una nueva conexión WebSocket.
        public async Task HandleAsync(WebSocket webSocket, int userId, string username)
        {
            Console.WriteLine($"[WebSocket] HandleAsync: Intento de conexión para Usuario ID: {userId}, Username: {username}");
            var handler = new WebSocketHandler(userId, webSocket, username);

            // Si ya existe una conexión para este usuario, la cierra antes de crear la nueva.
            if (_handlers.TryRemove(userId, out var oldHandler))
            {
                await oldHandler.DisposeAsync();
                Console.WriteLine($"[WebSocket] HandleAsync: Conexión antigua para Usuario ID: {userId} eliminada.");
            }

            _handlers[userId] = handler;
            Console.WriteLine($"[WebSocket] HandleAsync: Conexión establecida. Usuario ID: {userId} añadido a los handlers. Total: {_handlers.Count}");

            // Se suscribe a los eventos del manejador.
            handler.MessageReceived += OnMessageReceived;
            handler.Disconnected += OnDisconnected;

            await UpdateUserStatus(userId, true);
            await handler.HandleAsync();
        }

        // Se ejecuta cuando se recibe un mensaje de un cliente.
        private async Task OnMessageReceived(WebSocketHandler sender, string messageJson)
        {
            Console.WriteLine($"[WebSocket] OnMessageReceived: Mensaje recibido de Usuario ID: {sender.Id}. Mensaje: {messageJson}");
            var messageOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var message = JsonSerializer.Deserialize<WebSocketMessage>(messageJson, messageOptions);

            if (string.IsNullOrEmpty(message?.Type))
            {
                Console.WriteLine("[WebSocket] OnMessageReceived ERROR: Mensaje sin tipo definido.");
                return;
            }

            Console.WriteLine($"[WebSocket] OnMessageReceived: Mensaje tipo '{message.Type}' detectado. Procesando...");
            // Crea un nuevo scope para resolver dependencias Scoped (como UnitOfWork).
            using var scope = _scopeFactory.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<UnitOfWork>();

            switch (message.Type)
            {
                case "sendFriendRequest":
                    // Lógica para enviar una solicitud de amistad.
                    var sendMsg = JsonSerializer.Deserialize<SendFriendRequestMessage>(messageJson, messageOptions);
                    if (sendMsg != null)
                    {
                        // ... (Lógica de creación de amistad) ...
                        // Notifica al receptor de la solicitud.
                        var senderUser = await unitOfWork._userRepository.GetByIdAsync(sender.Id);
                        var notification = new { type = "new_friend_request", payload = new { /* ... */ } };
                        await SendMessageToUserAsync(sendMsg.ReceiverId, JsonSerializer.Serialize(notification));
                    }
                    break;

                case "acceptFriendRequest":
                    // Lógica para aceptar una solicitud de amistad.
                    var acceptMsg = JsonSerializer.Deserialize<AcceptFriendRequestMessage>(messageJson, messageOptions);
                    if (acceptMsg != null)
                    {
                        var friendRequestService = scope.ServiceProvider.GetRequiredService<FriendRequestService>();
                        var (success, senderUser, receiverUser) = await friendRequestService.AcceptRequestAsync(acceptMsg.RequestId, sender.Id);

                        if (success && senderUser != null && receiverUser != null)
                        {
                            // Notifica a ambos usuarios que ahora son amigos.
                            var notificationToSender = new { type = "new_friend_notification", payload = ToUserDTO(receiverUser) };
                            var notificationToReceiver = new { type = "new_friend_notification", payload = ToUserDTO(senderUser) };
                            await SendMessageToUserAsync(senderUser.UserId, JsonSerializer.Serialize(notificationToSender));
                            await SendMessageToUserAsync(receiverUser.UserId, JsonSerializer.Serialize(notificationToReceiver));
                        }
                    }
                    break;
                default:
                    Console.WriteLine($"[WebSocket] OnMessageReceived ADVERTENCIA: Tipo de mensaje no manejado: {message.Type}");
                    break;
            }
        }

        // Se ejecuta cuando un cliente se desconecta.
        private async Task OnDisconnected(WebSocketHandler handler)
        {
            if (_handlers.TryRemove(handler.Id, out _))
            {
                Console.WriteLine($"[WebSocket] OnDisconnected: Usuario ID: {handler.Id} desconectado. Total handlers: {_handlers.Count}");
                await UpdateUserStatus(handler.Id, false);
            }
        }

        // Actualiza el estado de conexión del usuario en la base de datos y notifica a sus amigos.
        private async Task UpdateUserStatus(int userId, bool isOnline)
        {
            Console.WriteLine($"[WebSocket] UpdateUserStatus: Actualizando estado de Usuario ID: {userId} a IsOnline: {isOnline}");
            using var scope = _scopeFactory.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<UnitOfWork>();
            var user = await unitOfWork._userRepository.GetByIdAsync(userId);
            if (user == null) return;

            user.IsOnline = isOnline;
            user.LastSeen = isOnline ? null : DateTime.UtcNow;
            await unitOfWork.SaveAsync();

            // Notifica a todos los amigos del cambio de estado.
            var friends = await unitOfWork._friendRequestRepository.GetFriendsList(userId);
            var statusUpdate = new { type = "friend_status_update", payload = new { userId, isOnline, lastSeen = user.LastSeen } };
            var message = JsonSerializer.Serialize(statusUpdate);

            foreach (var friend in friends)
            {
                await SendMessageToUserAsync(friend.UserId, message);
            }
        }

        // Envía un mensaje a un usuario específico si está conectado.
        public async Task SendMessageToUserAsync(int userId, string message)
        {
            if (_handlers.TryGetValue(userId, out var handler) && handler.IsOpen)
            {
                await handler.SendAsync(message);
            }
        }

        public List<int> GetConnectedUsers()
        {
            return _handlers.Keys.ToList();
        }

        private UserDTO ToUserDTO(User user)
        {
            return new UserDTO { /* ... */ };
        }
    }

    // Clases auxiliares para la deserialización de mensajes WebSocket.
    public class WebSocketMessage { public string Type { get; set; } }
    public class SendFriendRequestMessage { public int ReceiverId { get; set; } }
    public class AcceptFriendRequestMessage { public int RequestId { get; set; } }
}