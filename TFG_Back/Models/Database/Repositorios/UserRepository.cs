using TFG_Back.Models.Database.Entidades;

namespace TFG_Back.Models.Database.Repositorios
{
    public class UserRepository : Repository<User>
    {
        public UserRepository(DBContext context) : base(context) { }
    }
}
