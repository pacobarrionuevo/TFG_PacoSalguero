
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
        private readonly DBContext _context;

        public AgendaController(DBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EntradaAgenda>>> GetEntradas()
        {
            return await _context.EntradasAgenda
                .OrderByDescending(e => e.Fecha)
                .ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<EntradaAgenda>> CrearEntrada([FromBody] EntradaAgenda entrada)
        {
            // Validar que el servicio existe
            var servicioExistente = await _context.Servicios.FindAsync(entrada.ServiceId);
            if (servicioExistente == null)
            {
                return BadRequest("El servicio especificado no existe");
            }

            entrada.Fecha = DateTime.Today;
            entrada.Hora = DateTime.Now.TimeOfDay;

            _context.EntradasAgenda.Add(entrada);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEntradas), new { id = entrada.Id }, entrada);
        }

        [HttpGet("mes/{year}/{month}")]
        public async Task<ActionResult<IEnumerable<EntradaAgenda>>> GetEntradasPorMes(int year, int month)
        {
            var entradas = await _context.EntradasAgenda
                .Where(e => e.Fecha.Year == year && e.Fecha.Month == month)
                .Include(e => e.Service) // Para incluir el servicio relacionado
                .OrderBy(e => e.Fecha)
                .ThenBy(e => e.Hora)
                .ToListAsync();

            return Ok(entradas);
        }
    }
}