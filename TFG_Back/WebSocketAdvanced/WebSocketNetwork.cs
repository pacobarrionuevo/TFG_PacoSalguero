using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TFG_Back.Models.Database;
using TFG_Back.Services;

namespace TFG_Back.WebSocketAdvanced
{
    public class WebSocketNetwork
    {
        private readonly ConcurrentDictionary<int, WebSocketHandler> _handlers = new ConcurrentDictionary<int, WebSocketHandler>();
        private readonly IServiceScopeFactory _scopeFactory;

        public WebSocketNetwork(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public async Task HandleAsync(WebSocket webSocket, int userId, string username)
        {
            var handler = new WebSocketHandler(userId, webSocket, username);

            handler.MessageReceived += OnMessageReceived;
            handler.Disconnected += OnDisconnected;

            if (_handlers.TryAdd(userId, handler))
            {
                await UpdateUserStatus(userId, true);
                await BroadcastUserStatusUpdate();
                Console.WriteLine($"WebSocket: Usuario {username} (ID: {userId}) conectado. Total: {_handlers.Count}");
                await handler.HandleAsync();
            }
            else
            {
                await webSocket.CloseAsync(WebSocketCloseStatus.PolicyViolation, "Usuario ya conectado.", CancellationToken.None);
            }
        }

        private async Task OnMessageReceived(WebSocketHandler sender, string messageJson)
        {
            var messageOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var message = JsonSerializer.Deserialize<WebSocketMessage>(messageJson, messageOptions);

            if (string.IsNullOrEmpty(message?.Type))
            {
                Console.WriteLine("WebSocket: Mensaje recibido con formato inválido.");
                return;
            }

            using (var scope = _scopeFactory.CreateScope())
            {
                var friendRequestService = scope.ServiceProvider.GetRequiredService<FriendRequestService>();

                switch (message.Type)
                {
                    case "sendFriendRequest":
                        var sendRequestMsg = JsonSerializer.Deserialize<SendFriendRequestMessage>(messageJson, messageOptions);
                        if (sendRequestMsg != null)
                            await friendRequestService.SendRequestAsync(sender.Id, sendRequestMsg.ReceiverId);
                        break;

                    case "acceptFriendRequest":
                        var acceptRequestMsg = JsonSerializer.Deserialize<AcceptFriendRequestMessage>(messageJson, messageOptions);
                        if (acceptRequestMsg != null)
                            await friendRequestService.AcceptRequestAsync(acceptRequestMsg.RequestId, sender.Id);
                        break;

                    case "rejectFriendRequest":
                        var rejectRequestMsg = JsonSerializer.Deserialize<RejectFriendRequestMessage>(messageJson, messageOptions);
                        if (rejectRequestMsg != null)
                            await friendRequestService.RejectRequestAsync(rejectRequestMsg.RequestId, sender.Id);
                        break;

                    default:
                        Console.WriteLine($"WebSocket: Tipo de mensaje no manejado: '{message.Type}'");
                        break;
                }
            }
        }

        private async Task OnDisconnected(WebSocketHandler handler)
        {
            if (_handlers.TryRemove(handler.Id, out _))
            {
                await UpdateUserStatus(handler.Id, false);
                await BroadcastUserStatusUpdate();
                Console.WriteLine($"WebSocket: Usuario {handler.Username} (ID: {handler.Id}) desconectado. Total: {_handlers.Count}");
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
                    user.LastSeen = DateTime.UtcNow;
                    await unitOfWork.SaveAsync();
                }
            }
        }

        public async Task SendMessageToUserAsync(int userId, string message)
        {
            if (_handlers.TryGetValue(userId, out var handler) && handler.IsOpen)
            {
                await handler.SendAsync(message);
            }
        }

        public async Task BroadcastUserStatusUpdate()
        {
            var message = JsonSerializer.Serialize(new { type = "userStatusChanged" });
            var allHandlers = _handlers.Values.ToList();
            foreach (var handler in allHandlers)
            {
                if (handler.IsOpen)
                {
                    await handler.SendAsync(message);
                }
            }
        }

        public List<object> GetConnectedUsers()
        {
            return _handlers.Values.Select(h => new { userId = h.Id, username = h.Username }).ToList<object>();
        }
    }

    public class WebSocketMessage { public string Type { get; set; } }
    public class SendFriendRequestMessage { public int ReceiverId { get; set; } }
    public class AcceptFriendRequestMessage { public int RequestId { get; set; } }
    public class RejectFriendRequestMessage { public int RequestId { get; set; } }
}