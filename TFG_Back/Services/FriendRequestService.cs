using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TFG_Back.Models.Database;
using TFG_Back.Models.Database.Entidades;
using TFG_Back.WebSocketAdvanced;

namespace TFG_Back.Services
{
    public class FriendRequestService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly WebSocketNetwork _webSocketNetwork;

        public FriendRequestService(UnitOfWork unitOfWork, WebSocketNetwork webSocketNetwork)
        {
            _unitOfWork = unitOfWork;
            _webSocketNetwork = webSocketNetwork;
        }

        public async Task SendRequestAsync(int senderId, int receiverId)
        {
            if (senderId == receiverId) return;

            var exists = await _unitOfWork._context.UserHasFriendship
                .AnyAsync(uhf =>
                    (uhf.UserId == senderId && uhf.Friendship.UserFriendship.Any(other => other.UserId == receiverId)) ||
                    (uhf.UserId == receiverId && uhf.Friendship.UserFriendship.Any(other => other.UserId == senderId))
                );

            if (exists) return;

            var friendship = new FriendShip { IsAccepted = false };
            _unitOfWork._context.Friendships.Add(friendship);
            await _unitOfWork.SaveAsync();

            var senderRelation = new UserHasFriendship { UserId = senderId, FriendshipId = friendship.FriendShipId, Requestor = true };
            var receiverRelation = new UserHasFriendship { UserId = receiverId, FriendshipId = friendship.FriendShipId, Requestor = false };
            _unitOfWork._context.UserHasFriendship.AddRange(senderRelation, receiverRelation);
            await _unitOfWork.SaveAsync();

            var sender = await _unitOfWork._userRepository.GetByIdAsync(senderId);
            if (sender == null) return;

            var notification = new
            {
                type = "friendRequest",
                requestId = friendship.FriendShipId,
                senderId,
                senderName = sender.UserNickname
            };
            await _webSocketNetwork.SendMessageToUserAsync(receiverId, JsonSerializer.Serialize(notification));
        }

        public async Task AcceptRequestAsync(int friendshipId, int acceptorId)
        {
            var friendship = await _unitOfWork._context.Friendships
                .Include(f => f.UserFriendship)
                .FirstOrDefaultAsync(f => f.FriendShipId == friendshipId && f.UserFriendship.Any(uf => uf.UserId == acceptorId && !uf.Requestor));

            if (friendship == null || friendship.IsAccepted) return;

            friendship.IsAccepted = true;
            await _unitOfWork.SaveAsync();

            var notification = new { type = "friendListUpdate" };
            foreach (var userFriendship in friendship.UserFriendship)
            {
                await _webSocketNetwork.SendMessageToUserAsync(userFriendship.UserId, JsonSerializer.Serialize(notification));
            }
        }

        public async Task RejectRequestAsync(int friendshipId, int rejectorId)
        {
            var friendship = await _unitOfWork._context.Friendships
                .Include(f => f.UserFriendship)
                .FirstOrDefaultAsync(f => f.FriendShipId == friendshipId && f.UserFriendship.Any(uf => uf.UserId == rejectorId));

            if (friendship == null) return;

            _unitOfWork._context.UserHasFriendship.RemoveRange(friendship.UserFriendship);
            _unitOfWork._context.Friendships.Remove(friendship);
            await _unitOfWork.SaveAsync();
        }
    }
}