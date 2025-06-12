using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace TFG_Back.WebSocketAdvanced
{
    // Representa una única conexión WebSocket de un cliente.
    public class WebSocketHandler : IAsyncDisposable
    {
        private readonly WebSocket _webSocket;
        public int Id { get; init; } // ID del usuario asociado a esta conexión.
        public string Username { get; set; }
        public bool IsOpen => _webSocket.State == WebSocketState.Open;

        // Eventos para notificar la recepción de mensajes y la desconexión.
        public event Func<WebSocketHandler, string, Task> MessageReceived;
        public event Func<WebSocketHandler, Task> Disconnected;

        public WebSocketHandler(int userId, WebSocket webSocket, string username)
        {
            Id = userId;
            _webSocket = webSocket;
            Username = username;
        }

        // Bucle principal que escucha los mensajes entrantes del cliente.
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
                        // Invoca el evento MessageReceived si hay suscriptores.
                        if (MessageReceived != null)
                        {
                            await MessageReceived.Invoke(this, message);
                        }
                    }
                    else if (result.MessageType == WebSocketMessageType.Close)
                    {
                        break; // Sale del bucle si el cliente solicita cerrar la conexión.
                    }
                }
                catch (WebSocketException)
                {
                    // Ocurre si la conexión se interrumpe abruptamente.
                    break;
                }
            }
            // Notifica que la conexión se ha cerrado.
            if (Disconnected != null)
            {
                await Disconnected.Invoke(this);
            }
        }

        // Envía un mensaje de texto al cliente.
        public async Task SendAsync(string message)
        {
            if (IsOpen)
            {
                var buffer = Encoding.UTF8.GetBytes(message);
                await _webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }

        // Método de limpieza para cerrar la conexión de forma segura y liberar recursos.
        public async ValueTask DisposeAsync()
        {
            if (_webSocket.State == WebSocketState.Open || _webSocket.State == WebSocketState.CloseReceived)
            {
                try
                {
                    // Notifica al cliente que estamos cerrando la conexión de forma normal.
                    await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                }
                catch (WebSocketException)
                {
                    // Ignora errores si el socket ya está cerrado o en un estado inválido.
                }
            }
            _webSocket.Dispose();
        }
    }
}