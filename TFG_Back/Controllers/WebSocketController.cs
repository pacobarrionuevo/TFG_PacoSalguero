﻿using System.Net.WebSockets;
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
        private readonly WebSocketNetwork _websocketNetwork;

        public WebSocketController(WebSocketNetwork websocketNetwork)
        {
            _websocketNetwork = websocketNetwork;
        }

        [HttpGet("connect")]
        public async Task ConnectAsync()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                // Obtener userId del token JWT
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var username = User.FindFirst("apodo")?.Value
                      ?? User.FindFirst("unique_name")?.Value
                      ?? "Desconocido";

                WebSocket webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                await _websocketNetwork.HandleAsync(webSocket, userId, username);
            }
            else
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }
        [HttpGet("online-users")]
        public IActionResult GetOnlineUsers()
        {
            var connectedUsers = _websocketNetwork.GetConnectedUsers();
            return Ok(new
            {
                onlineUsers = connectedUsers, // Propiedad "onlineUsers"
                total = connectedUsers.Count
            });
        }
    }
}