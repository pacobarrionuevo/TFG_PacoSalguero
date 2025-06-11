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
                UserFriendship = new List<UserHasFriendship>(),
                Role = "admin",
                IsOnline = false
            
            };
            User usuario2 = new User()
            {

                //UserId = 1,
                UserNickname = "prueba",
                UserEmail = "prueba",
                UserPassword = PasswordHelper.Hash("prueba"),
                UserConfirmPassword = PasswordHelper.Hash("prueba"),
                UserProfilePhoto = "Perfil_Deffault.png",
                UserFriendship = new List<UserHasFriendship>(),
                Role = "admin",
                IsOnline = false
            };
            FriendShip amistad1 = new FriendShip()
            {
                FriendShipId = 1,
                IsAccepted = true,
                UserFriendship = new List<UserHasFriendship>()
            };
            UserHasFriendship uhf1 = new UserHasFriendship()
            {
                UserId = usuario1.UserId,
                FriendshipId = amistad1.FriendShipId,
                User = usuario1,
                Friendship = amistad1,
                Requestor = true
            };
            UserHasFriendship uhf2 = new UserHasFriendship()
            {
                UserId = usuario2.UserId,
                FriendshipId = amistad1.FriendShipId,
                User = usuario2,
                Friendship = amistad1,
                Requestor = false
            };

            amistad1.UserFriendship.Add(uhf1);
            amistad1.UserFriendship.Add(uhf2);

            usuario1.UserFriendship.Add(uhf1);
            usuario2.UserFriendship.Add(uhf2);
            // Insertar usuarios y relaciones en la base de datos
            await _dBContext.AddRangeAsync(usuario1, usuario2, amistad1,uhf1,uhf2);
            await _dBContext.SaveChangesAsync();
        }
    }
}
