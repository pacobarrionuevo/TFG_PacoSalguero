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
        private readonly CustomerService _service;

        public CustomerController(CustomerService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetAll()
        {
            var customers = await _service.GetAllAsync();
            return Ok(customers);
        }

        [HttpPost]
        public async Task<ActionResult<Customer>> Create(CustomerDTO dto)
        {
            var created = await _service.CreateAsync(dto);
            if (created == null)
            {
                return BadRequest("The specified payment method does not exist.");
            }

            return CreatedAtAction(nameof(Create), new { id = created.Id }, created);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);
            return result ? NoContent() : NotFound();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Customer>> Update(int id, CustomerDTO customer)
        {
            if (id != customer.Id) return BadRequest("ID mismatch");

            var updated = await _service.UpdateAsync(customer);
            return updated == null ? NotFound() : Ok(updated);
        }
    }
}
