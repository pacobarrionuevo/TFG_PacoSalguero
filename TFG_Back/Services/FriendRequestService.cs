using Microsoft.EntityFrameworkCore;
using TFG_Back.Models.Database;
using TFG_Back.Models.Database.Entidades;

namespace TFG_Back.Services
{
    // Servicio para manejar la lógica de las solicitudes de amistad.
    public class FriendRequestService
    {
        private readonly UnitOfWork _unitOfWork;

        public FriendRequestService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // Acepta una solicitud de amistad.
        public async Task<(bool success, User sender, User receiver)> AcceptRequestAsync(int friendshipId, int acceptorId)
        {
            // Busca la solicitud de amistad asegurándose de que el usuario que acepta es el receptor.
            var friendship = await _unitOfWork._context.Friendships
                .Include(f => f.UserFriendship)
                    .ThenInclude(uf => uf.User)
                .FirstOrDefaultAsync(f => f.FriendShipId == friendshipId && f.UserFriendship.Any(uf => uf.UserId == acceptorId && !uf.Requestor));

            // Si la solicitud no existe o ya fue aceptada, la operación falla.
            if (friendship == null || friendship.IsAccepted)
            {
                return (false, null, null);
            }

            // Marca la amistad como aceptada y guarda los cambios.
            friendship.IsAccepted = true;
            await _unitOfWork.SaveAsync();

            // Obtiene los usuarios implicados para poder notificarlos.
            var sender = friendship.UserFriendship.FirstOrDefault(uf => uf.Requestor)?.User;
            var receiver = friendship.UserFriendship.FirstOrDefault(uf => !uf.Requestor)?.User;

            return (true, sender, receiver);
        }
    }
}