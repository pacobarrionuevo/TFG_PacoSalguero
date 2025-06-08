using TFG_Back.Models.Database.Entidades;
using TFG_Back.Models.Database;

namespace TFG_Back.WebSocketAdvanced
{
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
