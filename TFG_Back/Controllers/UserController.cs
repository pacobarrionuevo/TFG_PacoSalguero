using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text.RegularExpressions;
using TFG_Back.Models.Database;
using TFG_Back.Models.Database.Entidades;
using TFG_Back.Models.DTO;
using TFG_Back.Recursos;

namespace TFG_Back.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly DBContext _context;
        private readonly PasswordHelper passwordHelper;
        private readonly TokenValidationParameters _tokenParameters;
        private readonly UnitOfWork _unitOfWork;

        public UserController(DBContext _dbContext, IOptionsMonitor<JwtBearerOptions> jwtOptions, UnitOfWork unitOfWork)
        {
            _context = _dbContext;
            _tokenParameters = jwtOptions.Get(JwtBearerDefaults.AuthenticationScheme).TokenValidationParameters;
            _unitOfWork = unitOfWork;
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

        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _unitOfWork._userRepository.GetAllAsync();
            var userDtos = users.Select(u => ToUserDTO(u)).ToList();
            return Ok(userDtos);
        }

        private UserDTO ToUserDTO(User user)
        {
            return new UserDTO
            {
                UserId = user.UserId,
                UserNickname = user.UserNickname,
                UserProfilePhoto = Path.GetFileName(user.UserProfilePhoto), 
                UserEmail = user.UserEmail,
                IsOnline = user.IsOnline,
                LastSeen = user.LastSeen,
                Role = user.Role
            };
        }
        [HttpPost("register")]
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
                
            //Eso guarda la ruta de la foto
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
            
            // Aqui es donde se hace el login dentro del registro //
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Claims = new Dictionary<string, object>
                {
                    {"id", newUser.UserId},
                    {"nickname", newUser.UserNickname},
                    {"email", newUser.UserEmail},
                    {"profilephoto", newUser.UserProfilePhoto},
                    {"role", newUser.Role},
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

        [HttpPost("login")]
        public IActionResult Login([FromBody] UserLoginDTO usuarioLoginDto)
        {
            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            User user;

            if (Regex.IsMatch(usuarioLoginDto.UserEmailOrNickname, emailPattern))
            {
                user = _context.Users.FirstOrDefault(u => u.UserEmail == usuarioLoginDto.UserEmailOrNickname);
            }
            else
            {
                user = _context.Users.FirstOrDefault(u => u.UserNickname == usuarioLoginDto.UserEmailOrNickname);
            }

            if (user == null)
            {
                return Unauthorized("Usuario no existe");
            }
            Console.WriteLine("Hashed input: " + PasswordHelper.Hash(usuarioLoginDto.UserPassword));
            Console.WriteLine("Stored password: " + user.UserPassword);

            if (!PasswordHelper.Hash(usuarioLoginDto.UserPassword).Equals(user.UserPassword))
            {
                return Unauthorized("Contraseña incorrecta");
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Claims = new Dictionary<string, object>
                {
                    {"id", user.UserId},
                    {"nickname", user.UserNickname},
                    {"email", user.UserEmail},
                    {"profilephoto", user.UserProfilePhoto},
                    {"role", user.Role},

                },
                Expires = DateTime.UtcNow.AddDays(5),
                SigningCredentials = new SigningCredentials(
                    _tokenParameters.IssuerSigningKey,
                    SecurityAlgorithms.HmacSha256Signature)
            };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
            string accessToken = tokenHandler.WriteToken(token);

            return Ok(new { StringToken = accessToken, user.UserId });
        }
        [HttpGet("usuarios/{id}")]
        public async Task<ActionResult<UserDTO>> GetUsuarioById(int id)
        {
            var usuario = await _context.Users
                .Include(u => u.UserFriendship)
                .ThenInclude(ua => ua.Friendship)
                .FirstOrDefaultAsync(u => u.UserId == id);

            if (usuario == null)
            {
                return NotFound("Usuario no encontrado");
            }

            var usuarioDto = new UserDTO
            {
                UserId = usuario.UserId,
                UserNickname = usuario.UserNickname,
                UserEmail = usuario.UserEmail,
                UserProfilePhoto = usuario.UserProfilePhoto,
                IsOnline=usuario.IsOnline,
                Role = usuario.Role,
                IsFriend = usuario.UserFriendship.Any(ua => ua.Friendship.IsAccepted)
            };

            return Ok(usuarioDto);
        }

        [HttpPut("usuarios/{id}")]
        public async Task<IActionResult> ActualizarUsuario(int id, [FromBody] UpdateUserDTO dto)
        {
            var usuario = await _context.Users.FindAsync(id);
            if (usuario == null)
            {
                return NotFound("Usuario no encontrado");
            }

            if (!string.IsNullOrEmpty(dto.UserNickname))
            {
                usuario.UserNickname = dto.UserNickname;
            }

            if (!string.IsNullOrEmpty(dto.UserEmail))
            {
                usuario.UserEmail = dto.UserEmail;
            }

            if (!string.IsNullOrEmpty(dto.UserPassword))
            {
                if (dto.UserPassword != dto.UserPassword)
                {
                    return BadRequest("Las contraseñas no coinciden");
                }
                usuario.UserPassword = PasswordHelper.Hash(dto.UserPassword);
            }

            await _context.SaveChangesAsync();
            return Ok(usuario);
        }

        // Método para crear guardar las imagenes en su formato correcto //
        private async Task StoreImageAsync(string relativePath, IFormFile file)
        {
            using Stream stream = file.OpenReadStream();

            await FileHelper.SaveAsync(stream, relativePath);
        }
    }

}
