using Microsoft.AspNetCore.Mvc;
using TFG_Back.Models.Database.Entidades;
using TFG_Back.Models.DTO;
using TFG_Back.Services;

namespace TFG_Back.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        // Inyectamos el servicio de clientes que encapsula la lógica de negocio.
        private readonly CustomerService _service;

        public CustomerController(CustomerService service)
        {
            _service = service;
        }

        // Endpoint para obtener todos los clientes.
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetAll()
        {
            var customers = await _service.GetAllAsync();
            return Ok(customers);
        }

        // Endpoint para crear un nuevo cliente.
        [HttpPost]
        public async Task<ActionResult<Customer>> Create(CustomerDTO dto)
        {
            var created = await _service.CreateAsync(dto);
            if (created == null)
            {
                // Si el servicio devuelve null, significa que el método de pago no existía.
                return BadRequest("The specified payment method does not exist.");
            }

            return CreatedAtAction(nameof(Create), new { id = created.Id }, created);
        }

        // Endpoint para eliminar un cliente por su ID.
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);
            return result ? NoContent() : NotFound();
        }

        // Endpoint para actualizar un cliente existente.
        [HttpPut("{id}")]
        public async Task<ActionResult<Customer>> Update(int id, CustomerDTO customer)
        {
            // Validación para asegurar que el ID de la ruta y el del cuerpo coinciden.
            if (id != customer.Id) return BadRequest("ID mismatch");

            var updated = await _service.UpdateAsync(customer);
            return updated == null ? NotFound() : Ok(updated);
        }
    }
}