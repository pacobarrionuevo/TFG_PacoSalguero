using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using TFG_Back.Models.Database;
using TFG_Back.Models.Database.Entidades;

namespace TFG_Back.WebSocketAdvanced
{
    public class WebSocketNetwork
    {
        private readonly ConcurrentDictionary<int, WebSocket> _sockets = new();
        private readonly IServiceScopeFactory _scopeFactory;

        public WebSocketNetwork(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public async Task HandleAsync(WebSocket webSocket, int userId, string username)
        {
            _sockets.TryAdd(userId, webSocket);
            await UpdateAndNotifyUserStatus(userId, true);

            try
            {
                var buffer = new byte[1024 * 4];
                WebSocketReceiveResult result;
                do
                {
                    result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                } while (!result.CloseStatus.HasValue);

                await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
            }
            catch (WebSocketException)
            {
            }
            finally
            {
                _sockets.TryRemove(userId, out _);
                await UpdateAndNotifyUserStatus(userId, false);
            }
        }

        private async Task UpdateAndNotifyUserStatus(int userId, bool isOnline)
        {
            using var scope = _scopeFactory.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<UnitOfWork>();

            var user = await unitOfWork._userRepository.GetByIdAsync(userId);
            if (user == null) return;

            user.IsOnline = isOnline;
            user.LastSeen = isOnline ? null : DateTime.UtcNow;
            unitOfWork._userRepository.Update(user);
            await unitOfWork.SaveAsync();

            var friends = await unitOfWork._friendRequestRepository.GetFriendsList(userId);

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var statusUpdate = new
            {
                type = "friend_status_update",
                payload = new
                {
                    UserId = userId,
                    IsOnline = isOnline,
                    LastSeen = user.LastSeen
                }
            };
            var message = JsonSerializer.Serialize(statusUpdate, options);

            foreach (var friend in friends)
            {
                if (_sockets.TryGetValue(friend.UserId, out var friendSocket) && friendSocket.State == WebSocketState.Open)
                {
                    var buffer = Encoding.UTF8.GetBytes(message);
                    await friendSocket.SendAsync(new ArraySegment<byte>(buffer, 0, buffer.Length), WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
        }

        public async Task SendToUserAsync(int userId, string message)
        {
            if (_sockets.TryGetValue(userId, out var socket) && socket.State == WebSocketState.Open)
            {
                var buffer = Encoding.UTF8.GetBytes(message);
                await socket.SendAsync(new ArraySegment<byte>(buffer, 0, buffer.Length), WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }

        public ICollection<int> GetConnectedUsers()
        {
            return _sockets.Keys;
        }
    }
}