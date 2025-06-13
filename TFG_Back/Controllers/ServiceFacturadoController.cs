using Microsoft.AspNetCore.Mvc;
using TFG_Back.Extensions;
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
            // 1. El servicio sigue devolviendo la URL relativa, lo cual es correcto.
            var pdfRelativeUrl = await _service.GenerarFacturaPDFAsync(servicios);

            // 2. Comprobamos si se generó la URL.
            if (string.IsNullOrEmpty(pdfRelativeUrl))
            {
                return BadRequest("No se pudo generar la factura.");
            }

            // 3. Convertimos la URL relativa en una URL absoluta usando la extensión.
            var absoluteUrl = Request.GetAbsoluteUrl(pdfRelativeUrl);

            // 4. Devolvemos la URL absoluta al frontend.
            return Ok(new { url = absoluteUrl });
        }

    }
}