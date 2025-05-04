using Microsoft.AspNetCore.Mvc;
using TFG_Back.Models.Database.Entidades;
using TFG_Back.Servicios;

namespace TFG_Back.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentMethodController : ControllerBase
    {
        private readonly PaymentMethodService _service;

        public PaymentMethodController(PaymentMethodService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PaymentMethod>>> GetAll()
        {
            var servicios = await _service.GetAllAsync();
            return Ok(servicios);
        }

        [HttpPost]
        public async Task<ActionResult<PaymentMethod>> Create(PaymentMethod method)
        {
            var created = await _service.CreateAsync(method);
            return CreatedAtAction(nameof(Create), new { id = created.Id }, created);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);
            return result ? NoContent() : NotFound();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<PaymentMethod>> Update(int id, PaymentMethod method)
        {
            if (id != method.Id) return BadRequest("ID mismatch");

            var updated = await _service.UpdateAsync(method);
            return updated == null ? NotFound() : Ok(updated);
        }
    }
}
