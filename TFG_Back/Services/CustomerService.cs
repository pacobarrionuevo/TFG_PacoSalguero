using TFG_Back.Models.Database.Entidades;
using TFG_Back.Models.Database;
using TFG_Back.Models.DTO;

namespace TFG_Back.Services
{
    // Servicio para la gestión de la lógica de negocio de los clientes.
    public class CustomerService
    {
        private readonly UnitOfWork _unitOfWork;

        public CustomerService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // Obtiene todos los clientes.
        public async Task<ICollection<Customer>> GetAllAsync()
        {
            return await _unitOfWork._customerRepository.GetAllAsync();
        }

        // Crea un nuevo cliente a partir de un DTO.
        public async Task<Customer?> CreateAsync(CustomerDTO dto)
        {
            // Verifica que el método de pago especificado en el DTO exista.
            var paymentMethod = await _unitOfWork._paymentMethodRepository.GetByIdAsync(dto.PaymentMethodId);
            if (paymentMethod == null) return null;

            // Mapea el DTO a la entidad Customer.
            var customer = new Customer
            {
                Id = dto.Id,
                CIF = dto.CIF,
                Name = dto.Name,
                Adress = dto.Adress,
                PostalCode = dto.PostalCode,
                PlaceOfResidence = dto.PlaceOfResidence,
                PhoneNumber = dto.PhoneNumber,
                Email = dto.Email,
                AdminEmail = dto.AdminEmail,
                PaymentMethodId = dto.PaymentMethodId
            };

            var created = await _unitOfWork._customerRepository.InsertAsync(customer);
            await _unitOfWork.SaveAsync();
            return created;
        }

        // Elimina un cliente por su ID.
        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _unitOfWork._customerRepository.GetByIdAsync(id);
            if (entity == null) return false;

            _unitOfWork._customerRepository.Delete(entity);
            await _unitOfWork.SaveAsync();
            return true;
        }

        // Actualiza un cliente existente.
        public async Task<Customer?> UpdateAsync(CustomerDTO customer)
        {
            var existing = await _unitOfWork._customerRepository.GetByIdAsync(customer.Id);
            if (existing == null) return null;

            // Actualiza las propiedades de la entidad existente con los valores del DTO.
            existing.CIF = customer.CIF;
            existing.Name = customer.Name;
            existing.Adress = customer.Adress;
            existing.PostalCode = customer.PostalCode;
            existing.PlaceOfResidence = customer.PlaceOfResidence;
            existing.PhoneNumber = customer.PhoneNumber;
            existing.Email = customer.Email;
            existing.AdminEmail = customer.AdminEmail;
            existing.PaymentMethodId = customer.PaymentMethodId;

            _unitOfWork._customerRepository.Update(existing);
            await _unitOfWork.SaveAsync();
            return existing;
        }
    }
}