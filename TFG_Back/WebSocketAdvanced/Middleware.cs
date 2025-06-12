using System.Net.WebSockets;
using System.Security.Claims;

namespace TFG_Back.WebSocketAdvanced
{
    // Middleware para gestionar las conexiones WebSocket.
    public class Middleware : IMiddleware
    {
        private readonly WebSocketNetwork _webSocketNetwork;

        public Middleware(WebSocketNetwork webSocketNetwork)
        {
            _webSocketNetwork = webSocketNetwork;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            // Comprueba si la petición es para establecer una conexión WebSocket.
            if (context.WebSockets.IsWebSocketRequest)
            {
                // Extrae el token de la URL y lo agrega al header para que la autenticación funcione.
                string token = context.Request.Query["token"];
                if (!string.IsNullOrEmpty(token))
                {
                    context.Request.Headers["Authorization"] = $"Bearer {token}";
                }
                var username = context.User.FindFirst("nickname")?.Value
                       ?? context.User.FindFirst("unique_name")?.Value
                       ?? "Desconocido";

                // Acepta la conexión WebSocket.
                using WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
                int userId = ObtenerUserId(context);
                // Delega el manejo de la conexión a la clase WebSocketNetwork.
                await _webSocketNetwork.HandleAsync(webSocket, userId, username);
                return;
            }
            else
            {
                // Si no es una petición WebSocket, pasa al siguiente middleware en el pipeline.
                await next(context);
            }
        }

        // Obtiene el ID del usuario desde los claims del token JWT.
        private int ObtenerUserId(HttpContext context)
        {
            if (!context.User.Identity.IsAuthenticated)
            {
                throw new UnauthorizedAccessException("Usuario no autenticado");
            }
            var userIdClaim = context.User.FindFirst("id");
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                throw new UnauthorizedAccessException("Claim 'id' no encontrada o inválida");
            }

            return userId;
        }
    }
}