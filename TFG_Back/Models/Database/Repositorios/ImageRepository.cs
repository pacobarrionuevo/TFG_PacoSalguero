using TFG_Back.Models.Database.Entidades;

namespace TFG_Back.Models.Database.Repositorios
{
    public class ImageRepository : Repository<Image>
    {
        public ImageRepository(DBContext context) : base(context) { }
    }
}
