using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TFG_Back.Models.Database;
using TFG_Back.Models.DTO;
using TFG_Back.Services;

namespace TFG_Back.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FriendRequestController : ControllerBase
    {
        private readonly FriendRequestService _friendRequestService;
        private readonly UnitOfWork _unitOfWork;

        public FriendRequestController(FriendRequestService friendRequestService, UnitOfWork unitOfWork)
        {
            _friendRequestService = friendRequestService;
            _unitOfWork = unitOfWork;
        }

        private (bool, int) GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("id");
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
            {
                return (false, 0);
            }
            return (true, userId);
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendFriendRequest([FromQuery] int receiverId)
        {
            var (isAuthenticated, senderId) = GetCurrentUserId();
            if (!isAuthenticated)
            {
                return Unauthorized("Claim 'id' inválido o no encontrado en el token.");
            }

            if (senderId == receiverId)
            {
                return BadRequest("No puedes enviarte una solicitud a ti mismo.");
            }

            await _friendRequestService.SendRequestAsync(senderId, receiverId);
            return Ok(new { message = "Solicitud enviada correctamente." });
        }

        [HttpPost("accept")]
        public async Task<IActionResult> AcceptFriendRequest([FromQuery] int amistadId)
        {
            var (isAuthenticated, userId) = GetCurrentUserId();
            if (!isAuthenticated)
            {
                return Unauthorized("Claim 'id' inválido o no encontrado en el token.");
            }

            await _friendRequestService.AcceptRequestAsync(amistadId, userId);
            return Ok();
        }

        [HttpPost("reject")]
        public async Task<IActionResult> RejectFriendRequest([FromQuery] int amistadId)
        {
            var (isAuthenticated, userId) = GetCurrentUserId();
            if (!isAuthenticated)
            {
                return Unauthorized("Claim 'id' inválido o no encontrado en el token.");
            }

            await _friendRequestService.RejectRequestAsync(amistadId, userId);
            return Ok();
        }

        [HttpGet("friends")]
        public async Task<IActionResult> GetFriendsList()
        {
            var (isAuthenticated, userId) = GetCurrentUserId();
            if (!isAuthenticated)
            {
                return Unauthorized("Claim 'id' inválido o no encontrado en el token.");
            }

            var friends = await _unitOfWork._friendRequestRepository.GetFriendsList(userId);

            var friendDtos = friends.Select(u => new UserDTO
            {
                UserId = u.UserId,
                UserNickname = u.UserNickname,
                UserProfilePhoto = u.UserProfilePhoto,
                IsOnline = u.IsOnline,
                LastSeen = u.LastSeen
            });

            return Ok(friendDtos);
        }

        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingFriendRequests()
        {
            var (isAuthenticated, userId) = GetCurrentUserId();
            if (!isAuthenticated)
            {
                return Unauthorized("Claim 'id' inválido o no encontrado en el token.");
            }

            var pendingRequests = await _unitOfWork._context.UserHasFriendship
                .Where(uhf => uhf.UserId == userId && !uhf.Requestor && !uhf.Friendship.IsAccepted)
                .Select(uhf => new
                {
                    SenderRelation = uhf.Friendship.UserFriendship.FirstOrDefault(sender => sender.Requestor),
                    FriendshipId = uhf.FriendshipId
                })
                .Where(x => x.SenderRelation != null && x.SenderRelation.User != null)
                .Select(x => new
                {
                    FriendshipId = x.FriendshipId,
                    UserId = x.SenderRelation.User.UserId,
                    UserNickname = x.SenderRelation.User.UserNickname,
                    UserProfilePhoto = x.SenderRelation.User.UserProfilePhoto
                })
                .ToListAsync();

            return Ok(pendingRequests);
        }
    }
}