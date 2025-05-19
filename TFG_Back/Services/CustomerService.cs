using TFG_Back.Models.Database.Entidades;
using TFG_Back.Models.Database;
using TFG_Back.Models.DTO;

namespace TFG_Back.Services
{
    public class CustomerService
    {
        private readonly UnitOfWork _unitOfWork;

        public CustomerService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ICollection<Customer>> GetAllAsync()
        {
            return await _unitOfWork._customerRepository.GetAllAsync();
        }

        public async Task<Customer?> CreateAsync(CustomerDTO dto)
        {
            // Verificar si existe el método de pago por ID
            var paymentMethod = await _unitOfWork._paymentMethodRepository.GetByIdAsync(dto.PaymentMethodId);

            if (paymentMethod == null) return null;

            var customer = new Customer
            {
                CIF = dto.CIF,
                Name = dto.Name,
                Adress = dto.Adress,
                PostalCode = dto.PostalCode,
                PlaceOfResidence = dto.PlaceOfResidence,
                PhoneNumber = dto.PhoneNumber,
                Email = dto.Email,
                AdminEmail = dto.AdminEmail, // Nombre consistente
                PaymentMethodId = dto.PaymentMethodId
            };

            var created = await _unitOfWork._customerRepository.InsertAsync(customer);
            await _unitOfWork.SaveAsync();
            return created;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _unitOfWork._customerRepository.GetByIdAsync(id);
            if (entity == null) return false;

            _unitOfWork._customerRepository.Delete(entity);
            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<Customer?> UpdateAsync(Customer customer)
        {
            var existing = await _unitOfWork._customerRepository.GetByIdAsync(customer.Id);
            if (existing == null) return null;

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
