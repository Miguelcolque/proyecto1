using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using proyectogranja1.Data;
using proyectogranja1.Dtos;
using proyectogranja1.Models;

namespace proyectogranja1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SolicitudesFabricaController : ControllerBase
    {
        private readonly proyectogranja1Context _context;

        public SolicitudesFabricaController(proyectogranja1Context context)
        {
            _context = context;
        }

        // GET: api/SolicitudesFabrica
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetSolicitudesFabrica()
        {
            return await _context.SolicitudFabrica
                .Where(s => s.Activo)
                .Select(s => new {
                    s.LitrosSolicitados,
                    s.Fecha,
                    s.Estado
                })
                .ToListAsync();
        }

        // GET: api/SolicitudesFabrica/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SolicitudFabrica>> GetSolicitudFabrica(int id)
        {
            var solicitud = await _context.SolicitudFabrica.FindAsync(id);

            if (solicitud == null || !solicitud.Activo)
            {
                return NotFound();
            }

            return solicitud;
        }

        // GET: api/SolicitudesFabrica/borradas
        [HttpGet("borradas")]
        public async Task<ActionResult<IEnumerable<SolicitudFabrica>>> GetSolicitudesBorradas()
        {
            return await _context.SolicitudFabrica
                .Where(s => !s.Activo)
                .ToListAsync();
        }

        // POST: api/SolicitudesFabrica
        [HttpPost]
        public async Task<ActionResult<SolicitudFabrica>> PostSolicitudFabrica(CreateSolicitudFabricaDto dto)
        {
            var solicitud = new SolicitudFabrica
            {
                LitrosSolicitados = dto.LitrosSolicitados,
                Fecha = dto.Fecha,
                Estado = dto.Estado,
                Activo = true
            };

            _context.SolicitudFabrica.Add(solicitud);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSolicitudFabrica), 
                new { id = solicitud.SolicitudFabricaID }, 
                new { 
                    message = "Solicitud creada correctamente",
                    data = new {
                        solicitud.LitrosSolicitados,
                        solicitud.Fecha,
                        solicitud.Estado
                    }
                });
        }

        // PUT: api/SolicitudesFabrica/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSolicitudFabrica(int id, CreateSolicitudFabricaDto dto)
        {
            var solicitud = await _context.SolicitudFabrica.FindAsync(id);
            if (solicitud == null || !solicitud.Activo)
            {
                return NotFound();
            }

            solicitud.LitrosSolicitados = dto.LitrosSolicitados;
            solicitud.Fecha = dto.Fecha;
            solicitud.Estado = dto.Estado;

            _context.Entry(solicitud).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SolicitudFabricaExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/SolicitudesFabrica/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSolicitudFabrica(int id)
        {
            var solicitud = await _context.SolicitudFabrica.FindAsync(id);
            if (solicitud == null)
            {
                return NotFound();
            }

            // Baja lógica
            solicitud.Activo = false;
            _context.Entry(solicitud).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SolicitudFabricaExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        private bool SolicitudFabricaExists(int id)
        {
            return _context.SolicitudFabrica.Any(e => e.SolicitudFabricaID == id);
        }
    }
}
