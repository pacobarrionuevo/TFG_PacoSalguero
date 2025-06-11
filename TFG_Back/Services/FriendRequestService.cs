using TFG_Back .Models.Database;
using TFG_Back.Models.Database.Entidades;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TFG_Back.Services
{
    public class FriendRequestService
    {
        private readonly DBContext _context;

        public FriendRequestService(DBContext context)
        {
            _context = context;
        }

        public class FriendRequestResult
        {
            public bool Success { get; set; }
            public int FriendshipId { get; set; }
            public string SenderName { get; set; }
        }

        public async Task<FriendRequestResult> SendFriendRequest(int senderId, int receiverId)
        {
            if (senderId == receiverId)
                return new FriendRequestResult { Success = false };

            var exists = await _context.Friendships
                .Include(a => a.UserFriendship)
                .AnyAsync(a =>
                    !a.IsAccepted &&
                    a.UserFriendship.Any(ua => ua.UserId == senderId) &&
                    a.UserFriendship.Any(ua => ua.UserId == receiverId)
                );
            if (exists)
                return new FriendRequestResult { Success = false };

            var amistad = new FriendShip() { IsAccepted = false };
            _context.Friendships.Add(amistad);
            await _context.SaveChangesAsync();

            var senderRelation = new UserHasFriendship()
            {
                UserId = senderId,
                FriendshipId = amistad.FriendShipId,
                Requestor = true
            };

            var receiverRelation = new UserHasFriendship()
            {
                UserId = receiverId,
                FriendshipId = amistad.FriendShipId,
                Requestor = false
            };

            _context.UserHasFriendship.AddRange(senderRelation, receiverRelation);
            await _context.SaveChangesAsync();

            var sender = await _context.Users.FindAsync(senderId);

            return new FriendRequestResult
            {
                Success = true,
                FriendshipId = amistad.FriendShipId,
                SenderName = sender?.UserNickname
            };
        }
        public async Task<FriendRequestDetails> GetRequestDetails(int requestId)
        {
            return await _context.Friendships
                .Include(a => a.UserFriendship)
                .Where(a => a.FriendShipId == requestId)
                .Select(a => new FriendRequestDetails
                {
                    SenderId = a.UserFriendship.First(ua => ua.Requestor).UserId,
                    ReceiverId = a.UserFriendship.First(ua => !ua.Requestor).UserId
                })
                .FirstOrDefaultAsync();
        }
        public class FriendRequestDetails
        {
            public int SenderId { get; set; }
            public int ReceiverId { get; set; }
        }
        public async Task<bool> AcceptFriendRequest(int amistadId, int receiverId)
        {
            var amistad = await _context.Friendships
                .Include(a => a.UserFriendship)
                .FirstOrDefaultAsync(a => a.FriendShipId == amistadId && !a.IsAccepted);

            if (amistad == null)
                return false;

            var receiverRecord = amistad.UserFriendship
                .FirstOrDefault(ua => ua.UserId == receiverId && ua.Requestor == false);

            if (receiverRecord == null)
                return false;

            amistad.IsAccepted = true;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RejectFriendRequest(int amistadId, int receiverId)
        {
            var amistad = await _context.Friendships
                .Include(a => a.UserFriendship)
                .FirstOrDefaultAsync(a => a.FriendShipId == amistadId && !a.IsAccepted);

            if (amistad == null)
                return false;

            var receiverRecord = amistad.UserFriendship
                .FirstOrDefault(ua => ua.UserId == receiverId && ua.Requestor == false);

            if (receiverRecord == null)
                return false;

            _context.UserHasFriendship.RemoveRange(amistad.UserFriendship);
            _context.Friendships.Remove(amistad);
            await _context.SaveChangesAsync();

            return true;
        }
        public async Task<List<User>> GetFriendsList(int usuarioId)
        {
            var friendships = await _context.Friendships
                .Include(a => a.UserFriendship)
                    .ThenInclude(ua => ua.User)
                .Where(a => a.IsAccepted && a.UserFriendship.Any(ua => ua.UserId == usuarioId))
                .ToListAsync();

            List<User> friends = new List<User>();
            foreach (var amistad in friendships)
            {
                var friendRelation = amistad.UserFriendship.FirstOrDefault(ua => ua.UserId != usuarioId);
                if (friendRelation != null && friendRelation.User != null)
                    friends.Add(friendRelation.User);
            }
            return friends;
        }
        public async Task<List<FriendShip>> GetPendingFriendRequests(int usuarioId)
        {
            var pendingRequests = await _context.Friendships
                .Include(a => a.UserFriendship)
                .Where(a => !a.IsAccepted &&
                       a.UserFriendship.Any(ua => ua.UserId == usuarioId && ua.Requestor == false))
                .ToListAsync();

            return pendingRequests;
        }
    }
}
