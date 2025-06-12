using TFG_Back.Models.Database.Entidades;

namespace TFG_Back.Models.Database.Repositorios
{
    // Repositorio específico para la entidad Image.
    public class ImageRepository : Repository<Image>
    {
        public ImageRepository(DBContext context) : base(context) { }
    }
}