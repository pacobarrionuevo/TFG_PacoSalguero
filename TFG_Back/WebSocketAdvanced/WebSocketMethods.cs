using TFG_Back.Models.Database.Entidades;
using TFG_Back.Models.Database;

namespace TFG_Back.WebSocketAdvanced
{
    // Clase de utilidad para realizar operaciones de base de datos desde la lógica de WebSocket.
    // Se inyecta como Scoped para asegurar que cada operación use un DbContext fresco.
    public class WebSocketMethods
    {
        private readonly UnitOfWork _unitOfWork;

        public WebSocketMethods(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<User> GetUserById(int id)
        {
            return await _unitOfWork._userRepository.GetByIdAsync(id);
        }

        public async Task UpdateUserAsync(User user)
        {
            _unitOfWork._userRepository.Update(user);
            await _unitOfWork.SaveAsync();
        }
    }
}