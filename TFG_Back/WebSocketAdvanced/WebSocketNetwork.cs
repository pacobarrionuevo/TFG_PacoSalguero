
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
    public class WebSocketNetwork
    {
        private readonly ConcurrentDictionary<int, WebSocketHandler> _handlers = new();
        private readonly IServiceScopeFactory _scopeFactory;

        public WebSocketNetwork(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
            Console.WriteLine("[WebSocketNetwork] Instancia creada.");
        }

        public async Task HandleAsync(WebSocket webSocket, int userId, string username)
        {
            Console.WriteLine($"[WebSocket] HandleAsync: Intento de conexión para Usuario ID: {userId}, Username: {username}");
            var handler = new WebSocketHandler(userId, webSocket, username);

            if (_handlers.TryRemove(userId, out var oldHandler))
            {
                await oldHandler.DisposeAsync();
                Console.WriteLine($"[WebSocket] HandleAsync: Conexión antigua para Usuario ID: {userId} eliminada.");
            }

            _handlers[userId] = handler;
            Console.WriteLine($"[WebSocket] HandleAsync: Conexión establecida. Usuario ID: {userId} añadido a los handlers. Total: {_handlers.Count}");

            handler.MessageReceived += OnMessageReceived;
            handler.Disconnected += OnDisconnected;

            await UpdateUserStatus(userId, true);
            await handler.HandleAsync();
        }

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
            using var scope = _scopeFactory.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<UnitOfWork>();

            switch (message.Type)
            {
                case "sendFriendRequest":
                    var sendMsg = JsonSerializer.Deserialize<SendFriendRequestMessage>(messageJson, messageOptions);
                    if (sendMsg != null)
                    {
                        Console.WriteLine($"[WebSocket] OnMessageReceived: Procesando 'sendFriendRequest' de Sender ID: {sender.Id} a Receiver ID: {sendMsg.ReceiverId}");

                        var existingFriendship = await unitOfWork._context.UserHasFriendship
                            .FirstOrDefaultAsync(u => (u.UserId == sender.Id && u.Friendship.UserFriendship.Any(uf => uf.UserId == sendMsg.ReceiverId)) ||
                                                      (u.UserId == sendMsg.ReceiverId && u.Friendship.UserFriendship.Any(uf => uf.UserId == sender.Id)));

                        if (existingFriendship != null)
                        {
                            Console.WriteLine($"[WebSocket] OnMessageReceived ADVERTENCIA: Ya existe una relación entre {sender.Id} y {sendMsg.ReceiverId}.");
                            return;
                        }

                        var friendship = new FriendShip { IsAccepted = false };
                        await unitOfWork._friendRequestRepository.InsertAsync(friendship);
                        await unitOfWork.SaveAsync();

                        var senderLink = new UserHasFriendship { UserId = sender.Id, FriendshipId = friendship.FriendShipId, Requestor = true };
                        var receiverLink = new UserHasFriendship { UserId = sendMsg.ReceiverId, FriendshipId = friendship.FriendShipId, Requestor = false };
                        await unitOfWork._context.UserHasFriendship.AddRangeAsync(senderLink, receiverLink);
                        await unitOfWork.SaveAsync();

                        var senderUser = await unitOfWork._userRepository.GetByIdAsync(sender.Id);
                        var notification = new
                        {
                            type = "new_friend_request",
                            payload = new
                            {
                                friendshipId = friendship.FriendShipId,
                                userId = senderUser.UserId,
                                userNickname = senderUser.UserNickname,
                                UserProfilePhoto = senderUser.UserProfilePhoto, 
                            }
                        };
                        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                        await SendMessageToUserAsync(sendMsg.ReceiverId, JsonSerializer.Serialize(notification, options));
                    }
                    break;

                case "acceptFriendRequest":
                    var acceptMsg = JsonSerializer.Deserialize<AcceptFriendRequestMessage>(messageJson, messageOptions);
                    if (acceptMsg != null)
                    {
                        var friendRequestService = scope.ServiceProvider.GetRequiredService<FriendRequestService>();
                        Console.WriteLine($"[WebSocket] OnMessageReceived: Procesando 'acceptFriendRequest' para RequestId: {acceptMsg.RequestId} por Usuario ID: {sender.Id}");
                        var (success, senderUser, receiverUser) = await friendRequestService.AcceptRequestAsync(acceptMsg.RequestId, sender.Id);

                        if (success && senderUser != null && receiverUser != null)
                        {
                            Console.WriteLine($"[WebSocket] OnMessageReceived: Amistad aceptada en DB. Notificando a Sender ID: {senderUser.UserId} y Receiver ID: {receiverUser.UserId}");
                            var senderDto = ToUserDTO(senderUser);
                            var receiverDto = ToUserDTO(receiverUser);

                            var notificationToSender = new { type = "new_friend_notification", payload = receiverDto };
                            var notificationToReceiver = new { type = "new_friend_notification", payload = senderDto };

                            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

                            await SendMessageToUserAsync(senderUser.UserId, JsonSerializer.Serialize(notificationToSender, options));
                            await SendMessageToUserAsync(receiverUser.UserId, JsonSerializer.Serialize(notificationToReceiver, options));
                        }
                        else
                        {
                            Console.WriteLine($"[WebSocket] OnMessageReceived ERROR al procesar 'acceptFriendRequest'. Success: {success}, Sender: {senderUser?.UserId}, Receiver: {receiverUser?.UserId}");
                        }
                    }
                    break;
                default:
                    Console.WriteLine($"[WebSocket] OnMessageReceived ADVERTENCIA: Tipo de mensaje no manejado: {message.Type}");
                    break;
            }
        }

        private async Task OnDisconnected(WebSocketHandler handler)
        {
            if (_handlers.TryRemove(handler.Id, out _))
            {
                Console.WriteLine($"[WebSocket] OnDisconnected: Usuario ID: {handler.Id} desconectado. Total handlers: {_handlers.Count}");
                await UpdateUserStatus(handler.Id, false);
            }
        }

        private async Task UpdateUserStatus(int userId, bool isOnline)
        {
            Console.WriteLine($"[WebSocket] UpdateUserStatus: Actualizando estado de Usuario ID: {userId} a IsOnline: {isOnline}");
            using var scope = _scopeFactory.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<UnitOfWork>();
            var user = await unitOfWork._userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                Console.WriteLine($"[WebSocket] UpdateUserStatus ERROR: No se encontró al usuario {userId} para actualizar estado.");
                return;
            }

            user.IsOnline = isOnline;
            user.LastSeen = isOnline ? null : DateTime.UtcNow;

            await unitOfWork.SaveAsync();
            Console.WriteLine($"[WebSocket] UpdateUserStatus: Estado de Usuario ID: {userId} guardado en DB.");

            var friends = await unitOfWork._friendRequestRepository.GetFriendsList(userId);
            Console.WriteLine($"[WebSocket] UpdateUserStatus: Usuario ID: {userId} tiene {friends.Count} amigos a notificar.");
            var statusUpdate = new
            {
                type = "friend_status_update",
                payload = new { userId, isOnline, lastSeen = user.LastSeen }
            };
            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var message = JsonSerializer.Serialize(statusUpdate, options);

            foreach (var friend in friends)
            {
                await SendMessageToUserAsync(friend.UserId, message);
            }
        }

        public async Task SendMessageToUserAsync(int userId, string message)
        {
            if (_handlers.TryGetValue(userId, out var handler) && handler.IsOpen)
            {
                Console.WriteLine($"[WebSocket] SendMessageToUserAsync: OK - Enviando mensaje a Usuario ID: {userId}. Mensaje: {message}");
                await handler.SendAsync(message);
            }
            else
            {
                Console.WriteLine($"[WebSocket] SendMessageToUserAsync ADVERTENCIA: No se pudo enviar mensaje a Usuario ID: {userId}. No está conectado o no se encontró en la lista de handlers.");
            }
        }

        public List<int> GetConnectedUsers()
        {
            return _handlers.Keys.ToList();
        }

        private UserDTO ToUserDTO(User user)
        {
            return new UserDTO
            {
                UserId = user.UserId,
                UserNickname = user.UserNickname,
                UserProfilePhoto = user.UserProfilePhoto,
                UserEmail = user.UserEmail,
                IsOnline = user.IsOnline,
                LastSeen = user.LastSeen,
                Role = user.Role
            };
        }
    }

    public class WebSocketMessage { public string Type { get; set; } }
    public class SendFriendRequestMessage { public int ReceiverId { get; set; } }
    public class AcceptFriendRequestMessage { public int RequestId { get; set; } }
}