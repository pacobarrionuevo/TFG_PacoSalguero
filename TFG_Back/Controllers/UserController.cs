using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using TFG_Back.Models.Database.Entidades;
using TFG_Back.Models.Database.Repositorios;
using TFG_Back.Models.DTO;
using TFG_Back.Models.Recursos;

namespace TFG_Back.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly DBContext _context;
        private readonly PasswordHelper passwordHelper;
        private readonly TokenValidationParameters _tokenParameters;
        public UserController(DBContext _dbContext, IOptionsMonitor<JwtBearerOptions> jwtOptions)
        {
            _context = _dbContext;
            _tokenParameters = jwtOptions.Get(JwtBearerDefaults.AuthenticationScheme).TokenValidationParameters;
        }
        private UserSignUpDTO ToDtoRegistro(User users)
        {
            return new UserSignUpDTO()
            {
                UserNickname = users.UserNickname,
                UserEmail = users.UserEmail,
                UserPassword = users.UserPassword,
                UserConfirmPassword = users.UserConfirmPassword,
                UserProfilePhoto = null
            };
        }
        private UserDTO ToDto(User users)
        {
            return new UserDTO()
            {
                UserId = users.UserId,
                UserNickname = users.UserNickname,
                UserEmail = users.UserEmail,
                UserPassword = users.UserPassword,
                UserConfirmPassword = users.UserConfirmPassword,
                UserProfilePhoto = users.UserProfilePhoto,
            
            };
        }
        [HttpGet("users")]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsuarios()
        {
            return await _context.Users
                .Select(u => new UserDTO
                {
                    UserId = u.UserId,
                    UserNickname = u.UserNickname,
                    UserEmail = u.UserEmail,
                    UserProfilePhoto = u.UserProfilePhoto,
                    Role = u.Role
                    
                })
                .ToListAsync();
        }
        [HttpPost("Registro")]
        public async Task<IActionResult> Register([FromForm] UserSignUpDTO user)
        {
            if (_context.Users.Any(User => User.UserEmail == user.UserEmail))
            {
                return BadRequest("El nombre del usuario ya está en uso");
            }

            if (user.UserPassword!= user.UserConfirmPassword)
            {
                return BadRequest("Las contraseñas no coinciden");
            }

            if (user.UserProfilePhoto == null || user.UserProfilePhoto.Length == 0)
            {
                return BadRequest("No se ha elegido foto de perfil");
            }
                
            //Eso guarda la ruta
            string rutaFotoPerfil = $"{Guid.NewGuid()}_{user.UserProfilePhoto.FileName}";
            await StoreImageAsync("fotos/" + rutaFotoPerfil, user.UserProfilePhoto);

            User newUser = new User()
            {
                UserNickname = user.UserNickname,
                UserEmail = user.UserEmail,
                UserPassword = PasswordHelper.Hash(user.UserPassword),
                UserConfirmPassword = PasswordHelper.Hash(user.UserConfirmPassword),
                UserProfilePhoto = rutaFotoPerfil,
                
            };

            await _context.Users.AddAsync(newUser);
            await _context.SaveChangesAsync();
            UserSignUpDTO userCreated = ToDtoRegistro(newUser);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Claims = new Dictionary<string, object>
                {
                    {"id", newUser.UserId},
                    {"Nombre", newUser.UserNickname},
                    {"Email", newUser.UserEmail},
                    {"FotoPerfil", newUser.UserProfilePhoto},
                    {"Rol", newUser.Role},
                },
                Expires = DateTime.UtcNow.AddDays(5),
                SigningCredentials = new SigningCredentials(
                    _tokenParameters.IssuerSigningKey,
                    SecurityAlgorithms.HmacSha256Signature)
            };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
            string accessToken = tokenHandler.WriteToken(token);
            return Ok(new { StringToken = accessToken });
        }
        private async Task StoreImageAsync(string relativePath, IFormFile file)
        {
            using Stream stream = file.OpenReadStream();

            await FileHelper.SaveAsync(stream, relativePath);
        }
    }

}
