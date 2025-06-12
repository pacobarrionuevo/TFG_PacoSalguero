using Microsoft.AspNetCore.Mvc;
using TFG_Back.Models.Database.Entidades;
using TFG_Back.Servicios;
using Microsoft.AspNetCore.Authorization;
namespace TFG_Back.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentMethodController : ControllerBase
    {
        // Inyectamos el servicio de métodos de pago.
        private readonly PaymentMethodService _service;

        public PaymentMethodController(PaymentMethodService service)
        {
            _service = service;
        }

        // Endpoint para obtener todos los métodos de pago.
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PaymentMethod>>> GetAll()
        {
            var servicios = await _service.GetAllAsync();
            return Ok(servicios);
        }

        // Endpoint para crear un nuevo método de pago.
        [HttpPost]
        public async Task<ActionResult<PaymentMethod>> Create(PaymentMethod method)
        {
            var created = await _service.CreateAsync(method);
            return CreatedAtAction(nameof(Create), new { id = created.Id }, created);
        }

        // Endpoint para eliminar un método de pago por su ID.
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);
            return result ? NoContent() : NotFound();
        }

        // Endpoint para actualizar un método de pago.
        [HttpPut("{id}")]
        public async Task<ActionResult<PaymentMethod>> Update(int id, PaymentMethod method)
        {
            if (id != method.Id) return BadRequest("ID mismatch");

            var updated = await _service.UpdateAsync(method);
            return updated == null ? NotFound() : Ok(updated);
        }
    }
}