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
    [Authorize(Roles = "admin")]
    public class AdminController : ControllerBase
    {
        private readonly AdminService _adminService;

        public AdminController(AdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpPut("users/{id}/role")]
        public async Task<IActionResult> ChangeUserRole(int id, [FromBody] ChangeRoleDto dto)
        {
            var result = await _adminService.ChangeUserRoleAsync(id, dto.Role);
            if (!result)
            {
                return NotFound("Usuario no encontrado o rol no válido.");
            }
            return NoContent();
        }

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

       ç
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

            return Ok(new { filePath = newPath });
        }

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

        [HttpGet("dashboard-stats")]
        public async Task<ActionResult<DashboardStatsDto>> GetDashboardStats()
        {
            var stats = await _adminService.GetDashboardStatsAsync();
            return Ok(stats);
        }
    }
}