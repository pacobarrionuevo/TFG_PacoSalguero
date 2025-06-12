using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TFG_Back.Models.Database;
using TFG_Back.Models.DTO;
using TFG_Back.Recursos;
using System.IO;
using System.Threading.Tasks;

namespace TFG_Back.Services
{
    // Servicio que encapsula la lógica de negocio para las operaciones del administrador.
    public class AdminService
    {
        private readonly UnitOfWork _unitOfWork;

        public AdminService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // Cambia el rol de un usuario.
        public async Task<bool> ChangeUserRoleAsync(int userId, string newRole)
        {
            // Validación para asegurar que el rol sea uno de los permitidos.
            if (newRole != "admin" && newRole != "user")
            {
                return false;
            }

            var user = await _unitOfWork._userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            user.Role = newRole;
            _unitOfWork._userRepository.Update(user);
            return await _unitOfWork.SaveAsync();
        }

        // Actualiza los datos de un usuario.
        public async Task<bool> UpdateUserAsync(int userId, UpdateUserByAdminDTO dto)
        {
            var user = await _unitOfWork._userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            user.UserNickname = dto.UserNickname;
            user.UserEmail = dto.UserEmail;

            _unitOfWork._userRepository.Update(user);
            return await _unitOfWork.SaveAsync();
        }

        // Actualiza el avatar de un usuario.
        public async Task<string?> UpdateUserAvatarAsync(int userId, IFormFile newAvatar)
        {
            var user = await _unitOfWork._userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return null;
            }

            // Genera un nombre de archivo único y guarda la nueva imagen.
            string nombreArchivo = $"{Guid.NewGuid()}_{newAvatar.FileName}";
            string rutaRelativaCompleta = $"fotos/{nombreArchivo}";

            await FileHelper.SaveAsync(newAvatar.OpenReadStream(), rutaRelativaCompleta);

            // Actualiza la ruta del avatar en la entidad del usuario.
            user.UserProfilePhoto = rutaRelativaCompleta;
            _unitOfWork._userRepository.Update(user);
            await _unitOfWork.SaveAsync();

            return rutaRelativaCompleta;
        }

        // Elimina un usuario.
        public async Task<bool> DeleteUserAsync(int userId)
        {
            var user = await _unitOfWork._userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            _unitOfWork._userRepository.Delete(user);
            return await _unitOfWork.SaveAsync();
        }

        // Obtiene las estadísticas para el dashboard.
        public async Task<DashboardStatsDto> GetDashboardStatsAsync()
        {
            // Realiza consultas para contar el total de cada entidad.
            var usersCount = await _unitOfWork._userRepository.GetQueryable().CountAsync();
            var customersCount = await _unitOfWork._customerRepository.GetQueryable().CountAsync();
            var servicesCount = await _unitOfWork._serviceRepository.GetQueryable().CountAsync();
            var agendaEntriesCount = await _unitOfWork._context.EntradasAgenda.CountAsync();

            // Devuelve un DTO con los resultados.
            return new DashboardStatsDto
            {
                TotalUsers = usersCount,
                TotalCustomers = customersCount,
                TotalServices = servicesCount,
                TotalAgendaEntries = agendaEntriesCount
            };
        }
    }
}