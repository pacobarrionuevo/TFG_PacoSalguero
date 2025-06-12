using TFG_Back.Models.Database;
using TFG_Back.Models.Database.Entidades;

namespace TFG_Back.Servicios
{
    // Servicio para la lógica de negocio de los métodos de pago.
    public class PaymentMethodService
    {
        private readonly UnitOfWork _unitOfWork;

        public PaymentMethodService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ICollection<PaymentMethod>> GetAllAsync()
        {
            return await _unitOfWork._paymentMethodRepository.GetAllAsync();
        }

        public async Task<PaymentMethod> CreateAsync(PaymentMethod method)
        {
            var created = await _unitOfWork._paymentMethodRepository.InsertAsync(method);
            await _unitOfWork.SaveAsync();
            return created;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _unitOfWork._paymentMethodRepository.GetByIdAsync(id);
            if (entity == null) return false;

            _unitOfWork._paymentMethodRepository.Delete(entity);
            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<PaymentMethod?> UpdateAsync(PaymentMethod method)
        {
            var existing = await _unitOfWork._paymentMethodRepository.GetByIdAsync(method.Id);
            if (existing == null) return null;

            // Actualiza las propiedades de la entidad existente.
            existing.Method = method.Method;
            existing.Installments = method.Installments;
            existing.FirstPaymentDays = method.FirstPaymentDays;
            existing.DaysBetweenPayments = method.DaysBetweenPayments;

            _unitOfWork._paymentMethodRepository.Update(existing);
            await _unitOfWork.SaveAsync();
            return existing;
        }
    }
}