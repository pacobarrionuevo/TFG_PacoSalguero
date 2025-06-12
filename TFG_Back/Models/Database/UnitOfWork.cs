using TFG_Back.Models.Database.Repositorios;

namespace TFG_Back.Models.Database
{
    // Implementación del patrón Unit of Work.
    // Agrupa múltiples operaciones de repositorios en una única transacción de base de datos.
    public class UnitOfWork
    {
        public readonly DBContext _context;

        // Propiedades para acceder a los diferentes repositorios.
        public ImageRepository _imageRepository { get; init; }
        public UserRepository _userRepository { get; init; }
        public ServiceRepository _serviceRepository { get; init; }
        public PaymentMethodRepository _paymentMethodRepository { get; init; }
        public CustomerRepository _customerRepository { get; init; }
        public FriendRequestRepository _friendRequestRepository { get; init; }

        // El constructor recibe el DbContext y todos los repositorios a través de inyección de dependencias.
        public UnitOfWork(DBContext context, ImageRepository imageRepository, UserRepository userRepository
                            , ServiceRepository serviceRepository, PaymentMethodRepository paymentMethodRepository,
                            CustomerRepository customerRepository, FriendRequestRepository friendRequestRepository)
        {
            _context = context;
            _imageRepository = imageRepository;
            _userRepository = userRepository;
            _serviceRepository = serviceRepository;
            _paymentMethodRepository = paymentMethodRepository;
            _customerRepository = customerRepository;
            _friendRequestRepository = friendRequestRepository;
        }

        // Guarda todos los cambios pendientes en la base de datos.
        // Devuelve true si se guardó al menos un cambio.
        public async Task<bool> SaveAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}