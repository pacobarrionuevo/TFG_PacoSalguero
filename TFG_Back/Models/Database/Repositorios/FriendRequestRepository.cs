using TFG_Back.Models.Database.Entidades;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TFG_Back.Models.Database.Repositorios;

namespace TFG_Back.Models.Database.Repositorios
{
    public class FriendRequestRepository : Repository<Friendship>
    {
        private readonly DBContext _context;

        public FriendRequestRepository(DBContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<User>> GetFriendsList(int usuarioId)
        {
            return await _context.UserHasFriendship
                .Include(uhf => uhf.Friendship)
                .Include(uhf => uhf.User)
                .Where(uhf => uhf.Friendship.IsAccepted &&
                              uhf.UserId != usuarioId &&
                              uhf.Friendship.UserFriendship.Any(au => au.UserId == usuarioId))
                .Select(uhf => uhf.User)
                .ToListAsync();
        }
    }
}
