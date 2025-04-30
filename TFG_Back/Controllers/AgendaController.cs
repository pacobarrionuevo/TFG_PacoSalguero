
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
            entrada.Fecha = DateTime.Today;
            entrada.Hora = DateTime.Now.TimeOfDay;
            _context.EntradasAgenda.Add(entrada);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEntradas), new { id = entrada.Id }, entrada);
        }
    }
}