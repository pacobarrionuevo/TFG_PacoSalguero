
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks; 

namespace TFG_Back.WebSocketAdvanced
{
    public class WebSocketHandler : IAsyncDisposable
    {
        private readonly WebSocket _webSocket;
        public int Id { get; init; }
        public string Username { get; set; }
        public bool IsOpen => _webSocket.State == WebSocketState.Open;

        public event Func<WebSocketHandler, string, Task> MessageReceived;
        public event Func<WebSocketHandler, Task> Disconnected;

        public WebSocketHandler(int userId, WebSocket webSocket, string username)
        {
            Id = userId;
            _webSocket = webSocket;
            Username = username;
        }

        public async Task HandleAsync()
        {
            var buffer = new byte[1024 * 4];
            while (_webSocket.State == WebSocketState.Open)
            {
                try
                {
                    var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        if (MessageReceived != null)
                        {
                            await MessageReceived.Invoke(this, message);
                        }
                    }
                    else if (result.MessageType == WebSocketMessageType.Close)
                    {
                        break;
                    }
                }
                catch (WebSocketException)
                {
                    break;
                }
            }
            if (Disconnected != null)
            {
                await Disconnected.Invoke(this);
            }
        }

        public async Task SendAsync(string message)
        {
            if (IsOpen)
            {
                var buffer = Encoding.UTF8.GetBytes(message);
                await _webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }

        // --- CAMBIO: Método de limpieza asíncrono ---
        public async ValueTask DisposeAsync()
        {
            if (_webSocket.State == WebSocketState.Open || _webSocket.State == WebSocketState.CloseReceived)
            {
                try
                {
                    // Notificar al cliente que cerramos la conexión de forma normal
                    await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                }
                catch (WebSocketException)
                {
                    // Ignorar errores si el socket ya está cerrado o en un estado inválido
                }
            }
            _webSocket.Dispose();
        }
    }
}