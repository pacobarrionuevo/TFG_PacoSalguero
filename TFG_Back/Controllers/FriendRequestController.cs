using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TFG_Back.Models.Database;
using TFG_Back.Models.Database.Entidades;
using System.IO;
using Microsoft.EntityFrameworkCore;

namespace TFG_Back.Controllers
{
    // Asegura que solo usuarios autenticados puedan acceder a estos endpoints.
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FriendRequestController : ControllerBase
    {
        // Inyectamos UnitOfWork para gestionar las operaciones de base de datos de forma transaccional.
        private readonly UnitOfWork _unitOfWork;

        public FriendRequestController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // Endpoint para enviar una solicitud de amistad a otro usuario.
        [HttpPost("send")]
        public async Task<IActionResult> SendRequest([FromQuery] int receiverId)
        {
            // Obtiene el ID del usuario que envía la solicitud desde el token JWT.
            var senderId = int.Parse(User.FindFirst("id")?.Value);
            if (senderId == receiverId)
                return BadRequest("No puedes enviarte una solicitud a ti mismo.");

            // Crea una nueva entidad Friendship y la guarda para obtener su ID.
            var friendship = new FriendShip();
            await _unitOfWork._friendRequestRepository.InsertAsync(friendship);
            await _unitOfWork.SaveAsync();

            // Crea los enlaces en la tabla intermedia UserHasFriendship para el emisor y el receptor.
            var senderLink = new UserHasFriendship { UserId = senderId, FriendshipId = friendship.FriendShipId, Requestor = true };
            var receiverLink = new UserHasFriendship { UserId = receiverId, FriendshipId = friendship.FriendShipId, Requestor = false };

            _unitOfWork._context.UserHasFriendship.Add(senderLink);
            _unitOfWork._context.UserHasFriendship.Add(receiverLink);
            await _unitOfWork.SaveAsync();

            return Ok(new { success = true, amistadId = friendship.FriendShipId });
        }

        // Endpoint para rechazar una solicitud de amistad.
        [HttpPost("reject")]
        public async Task<IActionResult> RejectRequest([FromQuery] int amistadId)
        {
            var friendship = await _unitOfWork._friendRequestRepository.GetByIdAsync(amistadId);
            if (friendship == null) return NotFound();

            // Elimina la solicitud de la base de datos.
            _unitOfWork._friendRequestRepository.Delete(friendship);
            await _unitOfWork.SaveAsync();

            return Ok(new { success = true });
        }

        // Endpoint para obtener las solicitudes de amistad pendientes del usuario actual.
        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingRequests()
        {
            var userId = int.Parse(User.FindFirst("id")?.Value);
            var requests = await _unitOfWork._friendRequestRepository.GetQueryable()
                // Filtra por amistades no aceptadas donde el usuario actual es el receptor.
                .Where(f => !f.IsAccepted && f.UserFriendship.Any(uf => uf.UserId == userId && !uf.Requestor))
                // Proyecta los datos del usuario que envió la solicitud.
                .SelectMany(f => f.UserFriendship.Where(uf => uf.Requestor))
                .Select(uf => new
                {
                    FriendshipId = uf.FriendshipId,
                    UserId = uf.User.UserId,
                    UserNickname = uf.User.UserNickname,
                    UserProfilePhoto = uf.User.UserProfilePhoto
                })
                .ToListAsync();
            return Ok(requests);
        }

        // Endpoint para obtener la lista de amigos del usuario actual.
        [HttpGet("friends")]
        public async Task<IActionResult> GetFriendsList()
        {
            var userId = int.Parse(User.FindFirst("id")?.Value);
            var friends = await _unitOfWork._friendRequestRepository.GetFriendsList(userId);
            // Mapea la lista de amigos a un DTO para no exponer toda la entidad User.
            var friendDtos = friends.Select(f => new {
                f.UserId,
                f.UserNickname,
                UserProfilePhoto = f.UserProfilePhoto,
                f.IsOnline,
                f.LastSeen
            }).ToList();
            return Ok(friendDtos);
        }
    }
}