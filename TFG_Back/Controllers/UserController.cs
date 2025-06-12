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
        // Parámetros para la validación y creación de tokens JWT.
        private readonly TokenValidationParameters _tokenParameters;
        private readonly UnitOfWork _unitOfWork;

        public UserController(DBContext _dbContext, IOptionsMonitor<JwtBearerOptions> jwtOptions, UnitOfWork unitOfWork)
        {
            _context = _dbContext;
            // Obtenemos los parámetros de validación de JWT configurados en Program.cs.
            _tokenParameters = jwtOptions.Get(JwtBearerDefaults.AuthenticationScheme).TokenValidationParameters;
            _unitOfWork = unitOfWork;
        }

        // Convierte una entidad User a un DTO de registro.
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

        // Endpoint para obtener todos los usuarios. Principalmente para el panel de administración.
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _unitOfWork._userRepository.GetAllAsync();
            var userDtos = users.Select(u => ToUserDTO(u)).ToList();
            return Ok(userDtos);
        }

        // Convierte una entidad User a un DTO público, ocultando información sensible.
        private UserDTO ToUserDTO(User user)
        {
            return new UserDTO
            {
                UserId = user.UserId,
                UserNickname = user.UserNickname,
                UserProfilePhoto = user.UserProfilePhoto,
                UserEmail = user.UserEmail,
                IsOnline = user.IsOnline,
                LastSeen = user.LastSeen,
                Role = user.Role
            };
        }

        // Endpoint para el registro de nuevos usuarios.
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] UserSignUpDTO user)
        {
            // Valida que el email no esté ya en uso.
            if (_context.Users.Any(u => u.UserEmail == user.UserEmail))
            {
                return BadRequest("El nombre del usuario ya está en uso");
            }

            // Valida que las contraseñas coincidan.
            if (user.UserPassword != user.UserConfirmPassword)
            {
                return BadRequest("Las contraseñas no coinciden");
            }

            if (user.UserProfilePhoto == null || user.UserProfilePhoto.Length == 0)
            {
                return BadRequest("No se ha elegido foto de perfil");
            }

            // Genera un nombre de archivo único y lo guarda.
            string nombreArchivo = $"{Guid.NewGuid()}_{user.UserProfilePhoto.FileName}";
            string rutaRelativaCompleta = $"fotos/{nombreArchivo}";

            await StoreImageAsync(rutaRelativaCompleta, user.UserProfilePhoto);

            // Crea la nueva entidad de usuario con la contraseña hasheada.
            User newUser = new User()
            {
                UserNickname = user.UserNickname,
                UserEmail = user.UserEmail,
                UserPassword = PasswordHelper.Hash(user.UserPassword),
                UserConfirmPassword = PasswordHelper.Hash(user.UserConfirmPassword),
                UserProfilePhoto = rutaRelativaCompleta,
                Role = "user" // Rol por defecto para nuevos usuarios.
            };

            await _context.Users.AddAsync(newUser);
            await _context.SaveChangesAsync();

            // Genera un token JWT para el nuevo usuario para que inicie sesión automáticamente.
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
            return Ok(new { StringToken = accessToken, UserId = newUser.UserId });
        }

        // Endpoint para el inicio de sesión.
        [HttpPost("login")]
        public IActionResult Login([FromBody] UserLoginDTO usuarioLoginDto)
        {
            // Permite el login tanto con email como con nickname.
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

            // Compara el hash de la contraseña proporcionada con el hash almacenado.
            if (!PasswordHelper.Hash(usuarioLoginDto.UserPassword).Equals(user.UserPassword))
            {
                return Unauthorized("Contraseña incorrecta");
            }

            // Si las credenciales son correctas, genera un token JWT.
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

        // Endpoint para obtener los detalles de un usuario por su ID.
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

            // Mapea a un DTO para enviar al cliente.
            var usuarioDto = new UserDTO
            {
                UserId = usuario.UserId,
                UserNickname = usuario.UserNickname,
                UserEmail = usuario.UserEmail,
                UserProfilePhoto = usuario.UserProfilePhoto,
                IsOnline = usuario.IsOnline,
                Role = usuario.Role,
                IsFriend = usuario.UserFriendship.Any(ua => ua.Friendship.IsAccepted)
            };

            return Ok(usuarioDto);
        }

        // Endpoint para que un usuario actualice su propia información.
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

            // Si se proporciona una nueva contraseña, se hashea y actualiza.
            if (!string.IsNullOrEmpty(dto.UserPassword))
            {
                if (dto.UserPassword != dto.UserPassword) // Parece un error, debería ser dto.UserConfirmPassword
                {
                    return BadRequest("Las contraseñas no coinciden");
                }
                usuario.UserPassword = PasswordHelper.Hash(dto.UserPassword);
            }

            await _context.SaveChangesAsync();
            return Ok(usuario);
        }

        // Método auxiliar para guardar un archivo de imagen en el servidor.
        private async Task StoreImageAsync(string relativePath, IFormFile file)
        {
            using Stream stream = file.OpenReadStream();
            await FileHelper.SaveAsync(stream, relativePath);
        }
    }
}