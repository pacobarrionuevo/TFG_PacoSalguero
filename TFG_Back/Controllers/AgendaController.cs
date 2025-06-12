using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TFG_Back.Models.Database;
using TFG_Back.Models.Database.Entidades;

namespace TFG_Back.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AgendaController : ControllerBase
    {
        // Inyectamos el DbContext para interactuar directamente con la base de datos.
        private readonly DBContext _context;

        public AgendaController(DBContext context)
        {
            _context = context;
        }

        // Endpoint para obtener todas las entradas de la agenda, ordenadas por fecha y hora descendente.
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EntradaAgenda>>> GetEntradas()
        {
            return await _context.EntradasAgenda
                .OrderByDescending(e => e.FechaHora)
                .ToListAsync();
        }

        // Endpoint para crear una nueva entrada en la agenda.
        [HttpPost]
        public async Task<ActionResult<EntradaAgenda>> CrearEntrada([FromBody] EntradaAgenda entrada)
        {
            // Se valida que el servicio asociado a la entrada exista antes de crearla.
            var servicioExistente = await _context.Servicios.FindAsync(entrada.ServiceId);
            if (servicioExistente == null)
            {
                return BadRequest("El servicio especificado no existe");
            }

            // Se impide la creación de entradas con fechas que ya han pasado.
            if (entrada.FechaHora < DateTime.Now)
            {
                return BadRequest("No se pueden crear entradas con fechas pasadas");
            }

            _context.EntradasAgenda.Add(entrada);
            await _context.SaveChangesAsync();

            // Devuelve un 201 Created con la ubicación del nuevo recurso y el objeto creado.
            return CreatedAtAction(nameof(GetEntradas), new { id = entrada.Id }, entrada);
        }

        // Endpoint para obtener las entradas de la agenda filtradas por un mes y año específicos.
        // Utilizado principalmente por la vista de calendario.
        [HttpGet("mes/{year}/{month}")]
        public async Task<ActionResult<IEnumerable<EntradaAgenda>>> GetEntradasPorMes(int year, int month)
        {
            return await _context.EntradasAgenda
                .Where(e => e.FechaHora.Year == year && e.FechaHora.Month == month)
                .Include(e => e.Service) // Incluye la información del servicio relacionado.
                .OrderBy(e => e.FechaHora)
                .ToListAsync();
        }
    }
}