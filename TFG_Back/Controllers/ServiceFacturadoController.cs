using Microsoft.AspNetCore.Mvc;
using TFG_Back.Models.Database.Entidades;
using TFG_Back.Services;

namespace TFG_Back.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServiceFacturadoController : ControllerBase
    {
        private readonly ServiceFacturadoService _service;

        public ServiceFacturadoController(ServiceFacturadoService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<ICollection<ServiceFacturado>>> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CrearFactura([FromBody] ServiceFacturado servicio)
        {
            if (servicio == null)
                return BadRequest("No se han enviado servicio.");

            // Aquí podrías guardar en base de datos, generar el PDF, etc.
            await _service.CreateAsync(servicio);

            return Ok();
        }

        [HttpPost("generar-pdf")]
        public async Task<ActionResult<string>> GenerarFacturaPDF(IEnumerable<ServiceFacturado> servicios)
        {
            var pdfResult = await _service.GenerarFacturaPDFAsync(servicios);
            return Ok(pdfResult); // Puedes devolver un enlace, base64, etc.
        }
    }
}
