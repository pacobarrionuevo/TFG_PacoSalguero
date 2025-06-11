using Microsoft.AspNetCore.Mvc;
using TFG_Back.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TFG_Back.Models.Database.Entidades;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using TFG_Back.Models.DTO;

namespace TFG_Back.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FriendRequestController : ControllerBase
    {
        private readonly FriendRequestService _friendRequestService;

        public FriendRequestController(FriendRequestService friendRequestService)
        {
            _friendRequestService = friendRequestService;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendFriendRequest([FromQuery] int receiverId)
        {
            if (!int.TryParse(User.Claims.FirstOrDefault(c => c.Type == "id")?.Value, out var senderId))
                return Unauthorized("Usuario no autenticado.");

            if (senderId == receiverId)
                return BadRequest("No puedes enviarte una solicitud a ti mismo.");

            var result = await _friendRequestService.SendFriendRequest(senderId, receiverId);

            if (result.Success)
                return Ok(result);
            else
                return StatusCode(500, "Error al enviar la solicitud.");
        }



        [HttpPost("accept")]
        public async Task<IActionResult> AcceptFriendRequest([FromQuery] int amistadId)
        {
            if (!int.TryParse(User.Claims.FirstOrDefault(c => c.Type == "id")?.Value, out var receiverId))
                return Unauthorized("Usuario no autenticado.");

            var result = await _friendRequestService.AcceptFriendRequest(amistadId, receiverId);
            return result ? Ok() : BadRequest("No se pudo aceptar la solicitud.");
        }

        [HttpPost("reject")]
        public async Task<IActionResult> RejectFriendRequest([FromQuery] int amistadId)
        {
            if (!int.TryParse(User.Claims.FirstOrDefault(c => c.Type == "id")?.Value, out var receiverId))
                return Unauthorized("Usuario no autenticado.");

            var result = await _friendRequestService.RejectFriendRequest(amistadId, receiverId);
            return result ? Ok() : BadRequest("No se pudo rechazar la solicitud.");
        }


        [HttpGet("friends")]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetFriendsList()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
            {
                return Unauthorized("User ID not found in token.");
            }

            var friends = await _friendRequestService.GetFriendsList(userId);

            var friendDtos = friends.Select(f => new UserDTO
            {
                UserId = f.UserId,
                UserNickname = f.UserNickname,
                UserProfilePhoto = f.UserProfilePhoto,
                UserEmail = f.UserEmail,
                IsOnline = f.IsOnline,
                LastConnection = f.LastConnection,
                IsFriend = true
            }).ToList();

            return Ok(friendDtos);
        }


        [HttpGet("pending")]
        public async Task<ActionResult<List<FriendShip>>> GetPendingFriendRequests()
        {
            if (!int.TryParse(User.Claims.FirstOrDefault(c => c.Type == "id")?.Value, out var usuarioId))
                return Unauthorized("Usuario no autenticado.");

            var solicitudesPendientes = await _friendRequestService.GetPendingFriendRequests(usuarioId);
            return Ok(solicitudesPendientes);
        }

    }
}
