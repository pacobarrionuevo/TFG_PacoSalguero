using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TFG_Back.Models.Database;
using TFG_Back.Models.Database.Entidades;
using System.IO;
using Microsoft.EntityFrameworkCore;

namespace TFG_Back.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FriendRequestController : ControllerBase
    {
        private readonly UnitOfWork _unitOfWork;

        public FriendRequestController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendRequest([FromQuery] int receiverId)
        {
            var senderId = int.Parse(User.FindFirst("id")?.Value);
            if (senderId == receiverId)
                return BadRequest("No puedes enviarte una solicitud a ti mismo.");

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
            var requests = await _unitOfWork._friendRequestRepository.GetQueryable()
                .Where(f => !f.IsAccepted && f.UserFriendship.Any(uf => uf.UserId == userId && !uf.Requestor))
                .SelectMany(f => f.UserFriendship.Where(uf => uf.Requestor))
                .Select(uf => new
                {
                    FriendshipId = uf.FriendshipId,
                    UserId = uf.User.UserId,
                    UserNickname = uf.User.UserNickname,
                    UserProfilePhoto = Path.GetFileName(uf.User.UserProfilePhoto)
                })
                .ToListAsync();
            return Ok(requests);
        }

        [HttpGet("friends")]
        public async Task<IActionResult> GetFriendsList()
        {
            var userId = int.Parse(User.FindFirst("id")?.Value);
            var friends = await _unitOfWork._friendRequestRepository.GetFriendsList(userId);
            var friendDtos = friends.Select(f => new {
                f.UserId,
                f.UserNickname,
                UserProfilePhoto = Path.GetFileName(f.UserProfilePhoto),
                f.IsOnline,
                f.LastSeen
            }).ToList();
            return Ok(friendDtos);
        }
    }
}