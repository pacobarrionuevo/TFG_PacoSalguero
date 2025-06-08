using Microsoft.AspNetCore.Mvc;
using TFG_Back.Models.Database;
using TFG_Back.Models.Database.Entidades;
using TFG_Back.Services;

namespace TFG_Back.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServiceController : ControllerBase
    {
        private readonly ServicesService _services;
        private readonly UnitOfWork _unitOfWork;

        public ServiceController(ServicesService servicioService, UnitOfWork unitOfWork)
        {
            _services = servicioService;
            _unitOfWork = unitOfWork;
        }

        [HttpGet("get_services")]
        public async Task<ActionResult<IEnumerable<Service>>> GetAll()
        {
            var servicios = await _services.GetAllAsync();
            return Ok(servicios);
        }

        [HttpPost("post_services")]
        public async Task<ActionResult<Service>> Create([FromBody] Service servicio)
        {
            var nuevoServicio = await _services.CreateAsync(servicio);
            await _unitOfWork.SaveAsync();
            return CreatedAtAction(nameof(Create), new { id = nuevoServicio.Id }, nuevoServicio);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult<Service>> Update(int id, [FromBody] Service servicio)
        {
            if (id != servicio.Id) return BadRequest("ID mismatch");

            var updated = await _services.UpdateAsync(servicio);
            await _unitOfWork.SaveAsync();

            return updated != null ? Ok(updated) : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _services.DeleteAsync(id);
            await _unitOfWork.SaveAsync();

            return result ? NoContent() : NotFound();
        }
    }
}
