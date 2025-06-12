using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TFG_Back.Models.DTO;
using TFG_Back.Services;
using System.Threading.Tasks;

namespace TFG_Back.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // Protegemos este controlador para que solo los usuarios con el rol "admin" puedan acceder.
    [Authorize(Roles = "admin")]
    public class AdminController : ControllerBase
    {
        // Inyectamos el servicio de administración que contiene la lógica de negocio para las operaciones de admin.
        private readonly AdminService _adminService;

        public AdminController(AdminService adminService)
        {
            _adminService = adminService;
        }

        // Endpoint para cambiar el rol de un usuario específico.
        [HttpPut("users/{id}/role")]
        public async Task<IActionResult> ChangeUserRole(int id, [FromBody] ChangeRoleDto dto)
        {
            var result = await _adminService.ChangeUserRoleAsync(id, dto.Role);
            if (!result)
            {
                return NotFound("Usuario no encontrado o rol no válido.");
            }
            return NoContent(); // Devuelve 204 No Content si la operación fue exitosa.
        }

        // Endpoint para que un administrador actualice los detalles básicos de un usuario.
        [HttpPut("users/{id}/details")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserByAdminDTO dto)
        {
            var result = await _adminService.UpdateUserAsync(id, dto);
            if (!result)
            {
                return NotFound("Usuario no encontrado.");
            }
            return NoContent();
        }

        // Endpoint para actualizar el avatar de un usuario por parte de un administrador.
        [HttpPut("users/{id}/avatar")]
        public async Task<IActionResult> UpdateUserAvatar(int id, [FromForm] AvatarUploadDTO dto)
        {
            if (dto.Avatar == null || dto.Avatar.Length == 0)
            {
                return BadRequest("No se ha proporcionado un archivo de imagen.");
            }

            var newPath = await _adminService.UpdateUserAvatarAsync(id, dto.Avatar);
            if (newPath == null)
            {
                return NotFound("Usuario no encontrado.");
            }

            // Devuelve la nueva ruta del archivo para que el frontend pueda actualizar la vista.
            return Ok(new { filePath = newPath });
        }

        // Endpoint para eliminar un usuario del sistema.
        [HttpDelete("users/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var result = await _adminService.DeleteUserAsync(id);
            if (!result)
            {
                return NotFound("Usuario no encontrado.");
            }
            return NoContent();
        }

        // Endpoint para obtener estadísticas clave para el dashboard del panel de administración.
        [HttpGet("dashboard-stats")]
        public async Task<ActionResult<DashboardStatsDto>> GetDashboardStats()
        {
            var stats = await _adminService.GetDashboardStatsAsync();
            return Ok(stats);
        }
    }
}