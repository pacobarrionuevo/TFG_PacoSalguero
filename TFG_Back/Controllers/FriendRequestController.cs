using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;
using TFG_Back.Models.Database;
using TFG_Back.Models.Database.Entidades;
using TFG_Back.Models.DTO;
using TFG_Back.WebSocketAdvanced;

namespace TFG_Back.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FriendRequestController : ControllerBase
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly WebSocketNetwork _webSocketNetwork;

        public FriendRequestController(UnitOfWork unitOfWork, WebSocketNetwork webSocketNetwork)
        {
            _unitOfWork = unitOfWork;
            _webSocketNetwork = webSocketNetwork;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendRequest([FromQuery] int receiverId)
        {
            var senderId = int.Parse(User.FindFirst("id")?.Value);
            if (senderId == receiverId)
                return BadRequest("No puedes enviarte una solicitud a ti mismo.");

            var existingFriendship = await _unitOfWork._context.UserHasFriendship
                .Where(u => u.UserId == senderId || u.UserId == receiverId)
                .GroupBy(u => u.FriendshipId)
                .Where(g => g.Count() == 2)
                .Select(g => g.First().Friendship)
                .FirstOrDefaultAsync();

            if (existingFriendship != null)
                return BadRequest("Ya existe una relación de amistad o una solicitud pendiente.");

            var friendship = new FriendShip();
            await _unitOfWork._friendRequestRepository.InsertAsync(friendship);
            await _unitOfWork.SaveAsync();

            var senderLink = new UserHasFriendship { UserId = senderId, FriendshipId = friendship.FriendShipId, Requestor = true };
            var receiverLink = new UserHasFriendship { UserId = receiverId, FriendshipId = friendship.FriendShipId, Requestor = false };

            _unitOfWork._context.UserHasFriendship.Add(senderLink);
            _unitOfWork._context.UserHasFriendship.Add(receiverLink);
            await _unitOfWork.SaveAsync();

            return Ok(new { success = true, amistadId = friendship.FriendShipId });
        }

        [HttpPost("accept")]
        public async Task<IActionResult> AcceptRequest([FromQuery] int amistadId)
        {
            var currentUserId = int.Parse(User.FindFirst("id")?.Value);
            var friendship = await _unitOfWork._friendRequestRepository.GetByIdAsync(amistadId);
            if (friendship == null) return NotFound("Solicitud no encontrada.");

            var usersInFriendship = await _unitOfWork._context.UserHasFriendship
                .Where(u => u.FriendshipId == amistadId)
                .ToListAsync();

            if (!usersInFriendship.Any(u => u.UserId == currentUserId && !u.Requestor))
                return Forbid("No puedes aceptar esta solicitud.");

            friendship.IsAccepted = true;
            _unitOfWork._friendRequestRepository.Update(friendship);
            await _unitOfWork.SaveAsync();

            var user1Id = usersInFriendship.First(u => u.UserId != currentUserId).UserId;
            var user2Id = currentUserId;

            var user1 = await _unitOfWork._userRepository.GetByIdAsync(user1Id);
            var user2 = await _unitOfWork._userRepository.GetByIdAsync(user2Id);

            var user1Dto = ToUserDTO(user1);
            var user2Dto = ToUserDTO(user2);

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

            var notificationToUser1 = new { type = "new_friend_notification", payload = user2Dto };
            var notificationToUser2 = new { type = "new_friend_notification", payload = user1Dto };

            await _webSocketNetwork.SendToUserAsync(user1Id, JsonSerializer.Serialize(notificationToUser1, options));
            await _webSocketNetwork.SendToUserAsync(user2Id, JsonSerializer.Serialize(notificationToUser2, options));

            return Ok(new { success = true });
        }

        [HttpPost("reject")]
        public async Task<IActionResult> RejectRequest([FromQuery] int amistadId)
        {
            var friendship = await _unitOfWork._friendRequestRepository.GetByIdAsync(amistadId);
            if (friendship == null) return NotFound();

            _unitOfWork._friendRequestRepository.Delete(friendship);
            await _unitOfWork.SaveAsync();

            return Ok(new { success = true });
        }

        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingRequests()
        {
            var userId = int.Parse(User.FindFirst("id")?.Value);
            var requests = await _unitOfWork._context.UserHasFriendship
                .Where(u => u.UserId == userId && !u.Requestor)
                .Select(u => u.Friendship)
                .Where(f => !f.IsAccepted)
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

        [HttpGet("friends")]
        public async Task<IActionResult> GetFriendsList()
        {
            var userId = int.Parse(User.FindFirst("id")?.Value);
            var friends = await _unitOfWork._friendRequestRepository.GetFriendsList(userId);
            var friendDtos = friends.Select(f => ToUserDTO(f)).ToList();
            return Ok(friendDtos);
        }

        private UserDTO ToUserDTO(User user)
        {
            return new UserDTO
            {
                UserId = user.UserId,
                UserNickname = user.UserNickname,
                UserProfilePhoto = user.UserProfilePhoto,
                UserEmail = user.UserEmail,
                IsOnline = user.IsOnline,
                LastSeen = user.LastSeen,
                Role = user.Role
            };
        }
    }
}