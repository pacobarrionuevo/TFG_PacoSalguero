using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using TFG_Back.Models.Database.Entidades;
using TFG_Back.Models.Database;
using TFG_Back.Models.DTO;
using Microsoft.Extensions.DependencyInjection;

using Microsoft.AspNetCore.Mvc;
using TFG_Back.Services;
using static TFG_Back.WebSocketAdvanced.WebSocketNetwork;
using System.Collections.Concurrent;
using System.Text;

namespace TFG_Back.WebSocketAdvanced
{

    public class WebSocketNetwork
    {
        private int _idCounter;
        private readonly List<WebSocketHandler> _handlers = new List<WebSocketHandler>();
        private readonly List<WebSocketHandler> _connectedPlayers = new List<WebSocketHandler>();
        private readonly List<WebSocketHandler> _waitingPlayers = new List<WebSocketHandler>();
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private readonly SemaphoreSlim _connectedSemaphore = new SemaphoreSlim(1, 1);
        private readonly SemaphoreSlim _waitingSemaphore = new SemaphoreSlim(1, 1);
        private readonly ConcurrentDictionary<int, WebSocket> _sockets = new();
        private readonly ConcurrentDictionary<int, string> _users = new();
        private readonly IServiceScopeFactory _scopeFactory;

        public WebSocketNetwork(IServiceScopeFactory scopeFactory, IServiceProvider serviceProvider)
        {
            _scopeFactory = scopeFactory;
            _serviceProvider = serviceProvider;
            OnActiveConnectionsChanged += count => NotifyActiveConnectionsChanged(count);
        }
        private readonly Dictionary<int, WebSocketHandler> _connectedUsers = new Dictionary<int, WebSocketHandler>();

        private readonly IServiceProvider _serviceProvider;
        private static int _activeConnections = 0;
        public event Action<int> OnActiveConnectionsChanged;

        
        public async Task BroadcastMessage(string message)
        {
            await _semaphore.WaitAsync();
            try
            {
                var handlersSnapshot = _handlers.ToList();
                Console.WriteLine($"Enviando mensaje a {handlersSnapshot.Count} clientes: {message}");
                foreach (var handler in handlersSnapshot)
                {
                    if (handler.IsOpen)
                    {
                        await handler.SendAsync(message);
                    }
                }
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async void NotifyActiveConnectionsChanged(int count)
        {
            var message = new
            {
                Type = "activeConnections",
                Count = count
            };

            var handlersSnapshot = _handlers.ToList();
            foreach (var handler in handlersSnapshot)
            {
                if (handler.IsOpen)
                {
                    await handler.SendAsync(JsonSerializer.Serialize(message));
                }
            }
        }

        public async Task HandleAsync(WebSocket webSocket, int userId, string username)
        {
            _sockets.TryAdd(userId, webSocket);
            _users.TryAdd(userId, username);

            await UpdateUserStatus(userId, true);
            await BroadcastStatusUpdate(userId, true);

            try
            {
                var buffer = new byte[1024 * 4];
                WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                while (!result.CloseStatus.HasValue)
                {
                    // Mantener la conexión viva, se pueden manejar mensajes entrantes aquí si es necesario.
                    result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                }
            }
            catch (WebSocketException)
            {
                // El cliente se desconectó abruptamente.
            }
            finally
            {
                _sockets.TryRemove(userId, out _);
                _users.TryRemove(userId, out _);
                if (webSocket.State != WebSocketState.Closed && webSocket.State != WebSocketState.Aborted)
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Connection closed", CancellationToken.None);
                }

                await UpdateUserStatus(userId, false);
                await BroadcastStatusUpdate(userId, false);
            }
        }
        private async Task UpdateUserStatus(int userId, bool isOnline)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var unitOfWork = scope.ServiceProvider.GetRequiredService<UnitOfWork>();
                var user = await unitOfWork._userRepository.GetByIdAsync(userId);
                if (user != null)
                {
                    user.IsOnline = isOnline;
                    if (!isOnline)
                    {
                        user.LastConnection = DateTime.UtcNow;
                    }
                    unitOfWork._userRepository.Update(user);
                    await unitOfWork.SaveAsync();
                }
            }
        }

        private async Task BroadcastStatusUpdate(int userId, bool isOnline)
        {
            var messagePayload = new { type = "status_update", userId, isOnline };
            var messageString = JsonSerializer.Serialize(messagePayload);
            var bytes = Encoding.UTF8.GetBytes(messageString);
            var messageSegment = new ArraySegment<byte>(bytes);

            var broadcastTasks = _sockets.Values.Select(socket =>
            {
                if (socket.State == WebSocketState.Open)
                {
                    return socket.SendAsync(messageSegment, WebSocketMessageType.Text, true, CancellationToken.None);
                }
                return Task.CompletedTask;
            });

            await Task.WhenAll(broadcastTasks);
        }

        public IReadOnlyDictionary<int, string> GetConnectedUsers()
        {
            return _users;
        }
    

private async Task AddUser(int userId, WebSocketHandler handler)
        {
            _connectedUsers[userId] = handler;
            Console.WriteLine($"Usuario {userId} conectado.");
        }

        private async Task<WebSocketHandler> CreateHandlerAsync(int userId, WebSocket webSocket, string username)
        {
            await _connectedSemaphore.WaitAsync();
            using var scope = _serviceProvider.CreateScope();
            var wsMethods = scope.ServiceProvider.GetRequiredService<WebSocketMethods>();

            // Aquí sí podemos usar userId para buscar al usuario
            var user = await wsMethods.GetUserById(userId);
            try
            {
                var handler = new WebSocketHandler(userId, webSocket, username: user?.UserNickname ?? "Usuario desconocido");

                handler.Disconnected += OnDisconnectedHandler;
                handler.MessageReceived += OnMessageReceivedHandler;

                // Agregar el handler a la lista de handlers
                _handlers.Add(handler);

                // Agregar el handler a la lista de jugadores conectados
                _connectedPlayers.Add(handler);

                Console.WriteLine($"Nuevo cliente conectado. ID: {handler.Id}, Total de clientes: {_handlers.Count}");
                return handler;
            }
            finally
            {
                _connectedSemaphore.Release();
            }
        }

        private async Task OnDisconnectedHandler(WebSocketHandler handler)
        {
            Console.WriteLine($"Evento de desconexión disparado para el usuario {handler.Id}.");
            await NotifyFriendsAsync(handler, false);
        }
        

        private async Task OnMessageReceivedHandler(WebSocketHandler handler, string message)
        {
            Console.WriteLine($"Mensaje recibido de {handler.Id}: {message}");

            try
            {
                // Deserializar a un objeto base con la propiedad Type
                var baseMsg = JsonSerializer.Deserialize<WebSocketMessage>(message);
                if (baseMsg == null)
                {
                    Console.WriteLine("No se pudo deserializar el mensaje o es nulo.");
                    return;
                }

                switch (baseMsg.Type.ToLower())
                {
                    case "sendfriendrequest":
                        var sendFriendRequestMessage = JsonSerializer.Deserialize<SendFriendRequestMessage>(message);
                        await ProcessFriendRequest(handler, sendFriendRequestMessage.ReceiverId);
                        break;

                    case "acceptfriendrequest":
                        var acceptFriendRequestMessage = JsonSerializer.Deserialize<AcceptFriendRequestMessage>(message);
                        await ProcessAcceptFriendRequest(handler, acceptFriendRequestMessage.RequestId);
                        break;

                    case "rejectfriendrequest":
                        var rejectFriendRequestMessage = JsonSerializer.Deserialize<RejectFriendRequestMessage>(message);
                        await ProcessRejectFriendRequest(handler, rejectFriendRequestMessage.RequestId);
                        break;

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error procesando mensaje: {ex.Message}");
            }
        }

        // por si lo usamos
        public class ChatMessage : WebSocketMessage
        {
            [JsonPropertyName("sender")]
            public string Sender { get; set; }

            [JsonPropertyName("text")]
            public string Text { get; set; }
        }

        public class WebSocketMessage
        {
            [JsonPropertyName("type")]
            public string Type { get; set; }
        }

        public class InviteFriendMessage : WebSocketMessage
        {
            [JsonPropertyName("friendId")]
            public int FriendId { get; set; }
        }

        public class AcceptInvitationMessage : WebSocketMessage
        {
            [JsonPropertyName("inviterId")]
            public int InviterId { get; set; }
        }

        public class SendFriendRequestMessage : WebSocketMessage
        {
            [JsonPropertyName("receiverId")]
            public int ReceiverId { get; set; }
        }

        public class AcceptFriendRequestMessage : WebSocketMessage
        {
            [JsonPropertyName("requestId")]
            public int RequestId { get; set; }
        }

        public class RejectFriendRequestMessage : WebSocketMessage
        {
            [JsonPropertyName("requestId")]
            public int RequestId { get; set; }
        }

        private async Task ProcessFriendRequest(WebSocketHandler handler, int receiverId)
        {
            using var scope = _serviceProvider.CreateScope();
            var friendService = scope.ServiceProvider.GetRequiredService<FriendRequestService>();

            var result = await friendService.SendFriendRequest(handler.Id, receiverId);

            if (result.Success)
            {
                if (_connectedUsers.TryGetValue(receiverId, out var receiver))
                {
                    await receiver.SendAsync(JsonSerializer.Serialize(new
                    {
                        type = "friendRequest",
                        requestId = result.FriendshipId,
                        senderId = handler.Id,
                        senderName = result.SenderName
                    }));
                }
            }
        }
        public class TurnTimeoutMessage : WebSocketMessage
        {
            [JsonPropertyName("gameId")]
            public string GameId { get; set; }

            [JsonPropertyName("playerId")]
            public int PlayerId { get; set; }
        }


        private async Task ProcessAcceptFriendRequest(WebSocketHandler handler, int requestId)
        {
            using var scope = _serviceProvider.CreateScope();
            var friendService = scope.ServiceProvider.GetRequiredService<FriendRequestService>();

            var request = await friendService.GetRequestDetails(requestId);
            Console.WriteLine($"GetRequestDetails: sender={request?.SenderId}, receiver={request?.ReceiverId}, handler.Id={handler.Id}");

            if (request == null) return;

            var success = await friendService.AcceptFriendRequest(requestId, handler.Id);
            if (success)
            {
                var notifyPayload = new { type = "friendListUpdate" };
                await handler.SendAsync(JsonSerializer.Serialize(notifyPayload));

                if (_connectedUsers.TryGetValue(request.SenderId, out var sender))
                {
                    await sender.SendAsync(JsonSerializer.Serialize(notifyPayload));
                }
            }
        }

        private async Task ProcessRejectFriendRequest(WebSocketHandler handler, int requestId)
        {
            using var scope = _serviceProvider.CreateScope();
            var friendService = scope.ServiceProvider.GetRequiredService<FriendRequestService>();
            var requestDetails = await friendService.GetRequestDetails(requestId);
            Console.WriteLine($"GetRequestDetails: sender={requestDetails?.SenderId}, receiver={requestDetails?.ReceiverId}, handler.Id={handler.Id}");

            if (requestDetails == null) return;

            var success = await friendService.RejectFriendRequest(requestId, handler.Id);
            if (success)
            {
                if (_connectedUsers.TryGetValue(requestDetails.SenderId, out var sender))
                {
                    await sender.SendAsync(JsonSerializer.Serialize(new
                    {
                        type = "friendRequestRejected",
                        requestId = requestId,
                        reason = "La solicitud fue rechazada"
                    }));
                }
            }
        }

        


        private async Task NotifyFriendsAsync(WebSocketHandler handler, bool isConnected)
        {
            using var scope = _serviceProvider.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<UnitOfWork>();
            var friends = await unitOfWork._friendRequestRepository.GetFriendsList(handler.Id);

            var message = JsonSerializer.Serialize(new
            {
                type = isConnected ? "friendConnected" : "friendDisconnected",
                friendId = handler.Id
            });

            foreach (var friend in _connectedPlayers.Where(p => friends.Any(f => f.UserId == p.Id)))
            {
                await friend.SendAsync(message);
            }
        }


        public int GetActiveConnections() => _activeConnections;

        public WebSocketHandler GetHandlerById(int userId)
        {
            return _handlers.FirstOrDefault(h => h.Id == userId);
        }
    }
}