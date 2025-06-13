using Microsoft.AspNetCore.Mvc;
using TFG_Back.Models.Database.Entidades;
using TFG_Back.Services;

namespace TFG_Back.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceFacturadoController : ControllerBase
    {
        // Inyectamos el servicio para manejar la lógica de los servicios facturados.
        private readonly ServiceFacturadoService _service;

        public ServiceFacturadoController(ServiceFacturadoService service)
        {
            _service = service;
        }

        // Endpoint para obtener todos los servicios facturados.
        [HttpGet]
        public async Task<ActionResult<ICollection<ServiceFacturado>>> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }


        [HttpPost("generar-pdf")]
        public async Task<IActionResult> GenerarFacturaPDF([FromBody] IEnumerable<ServiceFacturado> servicios)
        {
            var pdfRelativeUrl = await _service.GenerarFacturaPDFAsync(servicios);
            return Ok(new { url = pdfRelativeUrl });
        }
    }
}