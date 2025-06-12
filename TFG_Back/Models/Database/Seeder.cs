using Microsoft.EntityFrameworkCore;
using TFG_Back.Models.Database.Entidades;
using TFG_Back.Recursos;

namespace TFG_Back.Models.Database
{
    // Clase responsable de poblar la base de datos con datos iniciales (seeding).
    // Es útil para tener datos de prueba al iniciar la aplicación por primera vez.
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

        // Método para crear usuarios y relaciones de amistad de prueba.
        private async Task SeedImagesAsync()
        {
            // Crea dos usuarios de prueba.
            User usuario1 = new User()
            {
                UserNickname = "Profe",
                UserEmail = "profe@gmail.com",
                UserPassword = PasswordHelper.Hash("profe777"),
                UserConfirmPassword = PasswordHelper.Hash("profe777"),
                UserProfilePhoto = "Perfil_Deffault.png",
                UserFriendship = new List<UserHasFriendship>(),
                Role = "admin", // Asigna el rol de administrador.
                IsOnline = true
            };
            

            

           

            // Añade todas las nuevas entidades al contexto de la base de datos.
            await _dBContext.AddRangeAsync(usuario1);
            await _dBContext.SaveChangesAsync();
        }
    }
}