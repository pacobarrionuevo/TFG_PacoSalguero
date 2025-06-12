using System.Net.WebSockets;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TFG_Back.WebSocketAdvanced;
using System.Security.Claims;

namespace TFG_Back.Controllers
{
    [Route("ws")]
    [ApiController]
    public class WebSocketController : ControllerBase
    {
        // Inyectamos la clase que gestiona la red de WebSockets.
        private readonly WebSocketNetwork _websocketNetwork;

        public WebSocketController(WebSocketNetwork websocketNetwork)
        {
            _websocketNetwork = websocketNetwork;
        }

        // Endpoint para establecer la conexión WebSocket.
        [HttpGet("connect")]
        public async Task ConnectAsync()
        {
            // Verifica si la petición es una solicitud de WebSocket.
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                // Extrae la información del usuario del token JWT que ya ha sido validado por el middleware.
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var username = User.FindFirst("nickname")?.Value
                      ?? User.FindFirst("unique_name")?.Value
                      ?? "Desconocido";

                // Acepta la conexión y la delega al gestor de la red de WebSockets.
                WebSocket webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                await _websocketNetwork.HandleAsync(webSocket, userId, username);
            }
            else
            {
                // Si no es una petición WebSocket, devuelve un error.
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }

        // Endpoint HTTP para obtener la lista de usuarios actualmente conectados por WebSocket.
        [HttpGet("online-users")]
        public IActionResult GetOnlineUsers()
        {
            var connectedUsers = _websocketNetwork.GetConnectedUsers();
            return Ok(new
            {
                onlineUsers = connectedUsers,
                total = connectedUsers.Count
            });
        }
    }
}