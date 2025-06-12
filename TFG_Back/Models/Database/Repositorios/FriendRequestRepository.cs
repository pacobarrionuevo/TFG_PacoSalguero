using TFG_Back.Models.Database.Entidades;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TFG_Back.Models.Database.Repositorios;

namespace TFG_Back.Models.Database.Repositorios
{
    // Repositorio para la entidad FriendShip, con lógica personalizada para consultas de amistad.
    public class FriendRequestRepository : Repository<FriendShip>
    {
        private readonly DBContext _context;

        public FriendRequestRepository(DBContext context) : base(context)
        {
            _context = context;
        }

        // Método personalizado para obtener la lista de amigos de un usuario específico.
        public async Task<List<User>> GetFriendsList(int usuarioId)
        {
            // Consulta compleja para encontrar a los amigos a través de la tabla intermedia.
            return await _context.UserHasFriendship
                .Include(uhf => uhf.Friendship)
                .Include(uhf => uhf.User)
                // Filtra por amistades aceptadas.
                .Where(uhf => uhf.Friendship.IsAccepted &&
                              // Excluye al propio usuario de la lista.
                              uhf.UserId != usuarioId &&
                              // Asegura que el usuario actual es parte de la amistad.
                              uhf.Friendship.UserFriendship.Any(au => au.UserId == usuarioId))
                // Selecciona la entidad User del otro lado de la relación.
                .Select(uhf => uhf.User)
                .ToListAsync();
        }
    }
}