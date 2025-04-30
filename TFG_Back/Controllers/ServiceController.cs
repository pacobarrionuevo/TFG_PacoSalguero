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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Service>>> GetAll()
        {
            var servicios = await _services.GetAllAsync();
            return Ok(servicios);
        }

        [HttpPost]
        public async Task<ActionResult<Service>> Create([FromBody] Service servicio)
        {
            var nuevoServicio = await _services.CreateAsync(servicio);
            await _unitOfWork.SaveAsync();
            return CreatedAtAction(nameof(Create), new { id = nuevoServicio.Id }, nuevoServicio);
        }
    }
}
