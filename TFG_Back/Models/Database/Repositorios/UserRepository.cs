using TFG_Back.Models.Database.Entidades;

namespace TFG_Back.Models.Database.Repositorios
{
    // Repositorio específico para la entidad User.
    public class UserRepository : Repository<User>
    {
        public UserRepository(DBContext context) : base(context) { }
    }
}