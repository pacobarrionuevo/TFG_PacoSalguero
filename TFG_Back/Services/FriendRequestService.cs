using Microsoft.EntityFrameworkCore;
using TFG_Back.Models.Database;
using TFG_Back.Models.Database.Entidades;

namespace TFG_Back.Services
{
    public class FriendRequestService
    {
        private readonly UnitOfWork _unitOfWork;

        public FriendRequestService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<(bool success, User sender, User receiver)> AcceptRequestAsync(int friendshipId, int acceptorId)
        {
            var friendship = await _unitOfWork._context.Friendships
                .Include(f => f.UserFriendship)
                    .ThenInclude(uf => uf.User)
                .FirstOrDefaultAsync(f => f.FriendShipId == friendshipId && f.UserFriendship.Any(uf => uf.UserId == acceptorId && !uf.Requestor));

            if (friendship == null || friendship.IsAccepted)
            {
                return (false, null, null);
            }

            friendship.IsAccepted = true;
            await _unitOfWork.SaveAsync();

            var sender = friendship.UserFriendship.FirstOrDefault(uf => uf.Requestor)?.User;
            var receiver = friendship.UserFriendship.FirstOrDefault(uf => !uf.Requestor)?.User;

            return (true, sender, receiver);
        }
    }
}   