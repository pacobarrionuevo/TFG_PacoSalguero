using Microsoft.EntityFrameworkCore;
using TFG_Back.Models.Database.Entidades;
using TFG_Back.Recursos;

namespace TFG_Back.Models.Database
{
    public class Seeder
    {
        private readonly DBContext _dBContext;

        public Seeder(DBContext dbContext)
        {
            _dBContext = dbContext;
        }

        public async Task SeedAsync()
        {
            await SeedImagesAsync();
            await _dBContext.SaveChangesAsync();
        }

        private async Task SeedImagesAsync()
        {
            // Todos los usuarios
            User usuario1 = new User()
            {
                
                //UserId = 1,
                UserNickname = "Jose",
                UserEmail = "jose777@gmail.com",
                UserPassword = PasswordHelper.Hash("jose777"),
                UserConfirmPassword = PasswordHelper.Hash("jose777"),
                UserProfilePhoto = "Perfil_Deffault.png",
                Role = "admin",
            
            };    

            // Insertar usuarios y relaciones en la base de datos
            await _dBContext.AddRangeAsync(usuario1);
            await _dBContext.SaveChangesAsync();
        }
    }
}
