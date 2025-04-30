using TFG_Back.Models.Database.Repositorios;

namespace TFG_Back.Models.Database
{
    public class UnitOfWork
    {
        
        public readonly DBContext _context;

        public ImageRepository _imageRepository { get; init; }
        public UserRepository _userRepository { get; init; }
        public ServiceRepository _serviceRepository { get; init; }

        public UnitOfWork(DBContext context, ImageRepository imageRepository, UserRepository userRepository, ServiceRepository serviceRepository)
        {
            _context = context;
            _imageRepository = imageRepository;
            _userRepository = userRepository;
            _serviceRepository = serviceRepository;
        }

        public async Task<bool> SaveAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
