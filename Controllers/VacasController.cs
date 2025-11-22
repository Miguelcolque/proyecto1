using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using proyectogranja1.Data;
using proyectogranja1.Dtos;
using proyectogranja1.Models;

namespace proyectogranja1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VacasController : ControllerBase
    {
        private readonly proyectogranja1Context _context;

        public VacasController(proyectogranja1Context context)
        {
            _context = context;
        }

        // GET: api/Vacas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Vaca>>> GetVacas()
        {
            return await _context.Vaca.ToListAsync();
        }

        // GET: api/Vacas/activas
        [HttpGet("activas")]
        public async Task<ActionResult<IEnumerable<Vaca>>> GetVacasActivas()
        {
            return await _context.Vaca.Where(v => v.Activo).ToListAsync();
        }

        // GET: api/Vacas/borradas
        [HttpGet("borradas")]
        public async Task<ActionResult<IEnumerable<Vaca>>> GetVacasBorradas()
        {
            return await _context.Vaca.Where(v => !v.Activo).ToListAsync();
        }

        // GET: api/Vacas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Vaca>> GetVaca(int id)
        {
            var vaca = await _context.Vaca.FindAsync(id);

            if (vaca == null)
            {
                return NotFound(new { message = $"No se encontró la vaca con ID {id}" });
            }

            return vaca;
        }

        // PUT: api/Vacas/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutVaca(int id, Vaca vaca)
        {
            if (id != vaca.VacaID)
            {
                return BadRequest(new { message = "El ID de la URL no coincide con el ID de la vaca" });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Datos de la vaca no válidos", errors = ModelState.Values.SelectMany(v => v.Errors) });
            }

            _context.Entry(vaca).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new { message = "Vaca actualizada correctamente", data = vaca });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VacaExists(id))
                {
                    return NotFound(new { message = $"No se encontró la vaca con ID {id}" });
                }
                else
                {
                    throw;
                }
            }
        }

        // POST: api/Vacas
        [HttpPost]
        public async Task<ActionResult<Vaca>> PostVaca(CreateVacaDto createVacaDto) 
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Datos de la vaca no válidos", errors = ModelState.Values.SelectMany(v => v.Errors) });
            }

            // Validar que el código de vaca sea único
            if (await _context.Vaca.AnyAsync(v => v.CodigoVaca == createVacaDto.CodigoVaca))
            {
                return BadRequest(new { message = "Ya existe una vaca con este código" });
            }

            var vaca = new Vaca
            {
                CodigoVaca = createVacaDto.CodigoVaca,
                Raza = createVacaDto.Raza,
                Peso = createVacaDto.Peso,
                EstadoSalud = createVacaDto.EstadoSalud,
                Activo = true,
                FechaCreacion = DateTime.UtcNow
            };

            _context.Vaca.Add(vaca);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetVaca), new { id = vaca.VacaID }, 
                new { message = "Vaca creada correctamente", data = vaca });
        }

        // DELETE: api/Vacas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVaca(int id)
        {
            var vaca = await _context.Vaca.FindAsync(id);
            if (vaca == null)
            {
                return NotFound(new { message = $"No se encontró la vaca con ID {id}" });
            }

            // En lugar de eliminar, marcamos como inactiva
            vaca.Activo = false;
            _context.Entry(vaca).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new { message = "Vaca marcada como inactiva correctamente" });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VacaExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        private bool VacaExists(int id)
        {
            return _context.Vaca.Any(e => e.VacaID == id);
        }
        
    }
}
