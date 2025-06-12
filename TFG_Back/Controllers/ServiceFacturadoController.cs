using Microsoft.AspNetCore.Mvc;
using TFG_Back.Models.Database.Entidades;
using TFG_Back.Services;

namespace TFG_Back.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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

        // Endpoint para crear una nueva entrada de servicio facturado.
        [HttpPost]
        public async Task<IActionResult> CrearFactura([FromBody] ServiceFacturado servicio)
        {
            if (servicio == null)
                return BadRequest("No se han enviado servicio.");

            // La lógica de negocio (guardar en BD, etc.) está en el servicio.
            await _service.CreateAsync(servicio);

            return Ok();
        }

        // Endpoint para generar un PDF a partir de una lista de servicios facturados.
        [HttpPost("generar-pdf")]
        public async Task<ActionResult<string>> GenerarFacturaPDF(IEnumerable<ServiceFacturado> servicios)
        {
            var pdfResult = await _service.GenerarFacturaPDFAsync(servicios);
            // Devuelve la ruta o información sobre el PDF generado.
            return Ok(pdfResult);
        }
    }
}