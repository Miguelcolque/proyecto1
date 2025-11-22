using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using proyectogranja1.Data;
using proyectogranja1.Dtos;
using proyectogranja1.Models;

namespace proyectogranja1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DistribucionesController : ControllerBase
    {
        private readonly proyectogranja1Context _context;

        public DistribucionesController(proyectogranja1Context context)
        {
            _context = context;
        }

        // GET: api/Distribuciones
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetDistribuciones()
        {
            return await _context.Distribucion
                .Include(d => d.Empleado)
                .Select(d => new
                {
                    d.DistribucionID,
                    d.EmpleadoID,
                    EmpleadoNombre = d.Empleado.Nombre,
                    d.Fecha,
                    d.LitrosEntregados,
                    d.Destino,
                    d.Estado,
                    d.FechaRegistro
                })
                .OrderByDescending(d => d.Fecha)
                .ToListAsync();
        }

        // GET: api/Distribuciones/por-fecha?fechaInicio=2023-01-01&fechaFin=2023-12-31
        [HttpGet("por-fecha")]
        public async Task<ActionResult<IEnumerable<object>>> GetDistribucionesPorRangoFechas(
            [FromQuery] DateTime fechaInicio,
            [FromQuery] DateTime fechaFin)
        {
            return await _context.Distribucion
                .Where(d => d.Fecha >= fechaInicio && d.Fecha <= fechaFin)
                .Include(d => d.Empleado)
                .Select(d => new
                {
                    d.DistribucionID,
                    d.EmpleadoID,
                    EmpleadoNombre = d.Empleado.Nombre,
                    d.Fecha,
                    d.LitrosEntregados,
                    d.Destino,
                    d.Estado
                })
                .OrderByDescending(d => d.Fecha)
                .ToListAsync();
        }

        // GET: api/Distribuciones/por-empleado/5
        [HttpGet("por-empleado/{empleadoId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetDistribucionesPorEmpleado(int empleadoId)
        {
            return await _context.Distribucion
                .Where(d => d.EmpleadoID == empleadoId)
                .Include(d => d.Empleado)
                .Select(d => new
                {
                    d.DistribucionID,
                    d.EmpleadoID,
                    EmpleadoNombre = d.Empleado.Nombre,
                    d.Fecha,
                    d.LitrosEntregados,
                    d.Destino,
                    d.Estado
                })
                .OrderByDescending(d => d.Fecha)
                .ToListAsync();
        }

        // GET: api/Distribuciones/por-estado/Pendiente
        [HttpGet("por-estado/{estado}")]
        public async Task<ActionResult<IEnumerable<object>>> GetDistribucionesPorEstado(string estado)
        {
            return await _context.Distribucion
                .Where(d => d.Estado == estado)
                .Include(d => d.Empleado)
                .Select(d => new
                {
                    d.DistribucionID,
                    d.EmpleadoID,
                    EmpleadoNombre = d.Empleado.Nombre,
                    d.Fecha,
                    d.LitrosEntregados,
                    d.Destino,
                    d.Estado
                })
                .OrderByDescending(d => d.Fecha)
                .ToListAsync();
        }

        // GET: api/Distribuciones/5
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetDistribucion(int id)
        {
            var distribucion = await _context.Distribucion
                .Include(d => d.Empleado)
                .Where(d => d.DistribucionID == id)
                .Select(d => new
                {
                    d.DistribucionID,
                    d.EmpleadoID,
                    EmpleadoNombre = d.Empleado.Nombre,
                    d.Fecha,
                    d.LitrosEntregados,
                    d.Destino,
                    d.Estado,
                    d.FechaRegistro
                })
                .FirstOrDefaultAsync();

            if (distribucion == null)
            {
                return NotFound(new { message = $"No se encontró la distribución con ID {id}" });
            }

            return distribucion;
        }

        // POST: api/Distribuciones
        [HttpPost]
        public async Task<ActionResult<object>> PostDistribucion(CreateDistribucionDto distribucionDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Datos de distribución no válidos", errors = ModelState.Values.SelectMany(v => v.Errors) });
            }

            // Validar que el empleado existe y está activo
            var empleado = await _context.Empleado.FindAsync(distribucionDto.EmpleadoID);
            if (empleado == null || !empleado.Activo)
            {
                return BadRequest(new { message = "El empleado especificado no existe o no está activo" });
            }

            var distribucion = new Distribucion
            {
                EmpleadoID = distribucionDto.EmpleadoID,
                Fecha = distribucionDto.Fecha,
                LitrosEntregados = distribucionDto.LitrosEntregados,
                Destino = distribucionDto.Destino,
                Estado = distribucionDto.Estado,
                FechaRegistro = DateTime.UtcNow
            };

            _context.Distribucion.Add(distribucion);
            await _context.SaveChangesAsync();

            // Obtener los datos relacionados para la respuesta
            var distribucionResponse = await _context.Distribucion
                .Where(d => d.DistribucionID == distribucion.DistribucionID)
                .Select(d => new
                {
                    d.DistribucionID,
                    d.EmpleadoID,
                    EmpleadoNombre = d.Empleado.Nombre,
                    d.Fecha,
                    d.LitrosEntregados,
                    d.Destino,
                    d.Estado,
                    d.FechaRegistro
                })
                .FirstOrDefaultAsync();

            return CreatedAtAction(nameof(GetDistribucion), new { id = distribucion.DistribucionID },
                new { message = "Distribución registrada correctamente", data = distribucionResponse });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutDistribucion(int id, CreateDistribucionDto distribucionDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Datos de distribución no válidos", errors = ModelState.Values.SelectMany(v => v.Errors) });
            }

            // Validar que la distribución existe
            var distribucionExistente = await _context.Distribucion.FindAsync(id);
            if (distribucionExistente == null)
            {
                return NotFound(new { message = $"No se encontró la distribución con ID {id}" });
            }

            // Validar que el empleado existe y está activo
            var empleado = await _context.Empleado.FindAsync(distribucionDto.EmpleadoID);
            if (empleado == null || !empleado.Activo)
            {
                return BadRequest(new { message = "El empleado especificado no existe o no está activo" });
            }

            // Validar que la fecha no sea futura
            if (distribucionDto.Fecha > DateTime.Today)
            {
                return BadRequest(new { message = "La fecha de distribución no puede ser futura" });
            }

            // Validar que los litros sean positivos
            if (distribucionDto.LitrosEntregados <= 0)
            {
                return BadRequest(new { message = "La cantidad de litros debe ser mayor a cero" });
            }

            // Validar que el estado sea válido
            var estadosPermitidos = new[] { "Pendiente", "En camino", "Entregado", "Cancelado" };
            if (!estadosPermitidos.Contains(distribucionDto.Estado))
            {
                return BadRequest(new { message = "El estado de la distribución no es válido" });
            }

            // Actualizar solo los campos permitidos
            distribucionExistente.EmpleadoID = distribucionDto.EmpleadoID;
            distribucionExistente.Fecha = distribucionDto.Fecha;
            distribucionExistente.LitrosEntregados = distribucionDto.LitrosEntregados;
            distribucionExistente.Destino = distribucionDto.Destino;
            distribucionExistente.Estado = distribucionDto.Estado;

            try
            {
                await _context.SaveChangesAsync();

                // Obtener los datos actualizados para la respuesta
                var distribucionActualizada = await _context.Distribucion
                    .Where(d => d.DistribucionID == id)
                    .Include(d => d.Empleado)
                    .Select(d => new
                    {
                        d.DistribucionID,
                        d.EmpleadoID,
                        EmpleadoNombre = d.Empleado.Nombre,
                        d.Fecha,
                        d.LitrosEntregados,
                        d.Destino,
                        d.Estado,
                        d.FechaRegistro
                    })
                    .FirstOrDefaultAsync();

                return Ok(new { message = "Distribución actualizada correctamente", data = distribucionActualizada });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DistribucionExists(id))
                {
                    return NotFound(new { message = $"No se encontró la distribución con ID {id}" });
                }
                else
                {
                    throw;
                }
            }
        }

        // DELETE: api/Distribuciones/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDistribucion(int id)
        {
            var distribucion = await _context.Distribucion.FindAsync(id);
            if (distribucion == null)
            {
                return NotFound(new { message = $"No se encontró la distribución con ID {id}" });
            }

            // Solo permitir eliminar distribuciones con estado "Pendiente" o "Cancelado"
            if (distribucion.Estado == "En camino" || distribucion.Estado == "Entregado")
            {
                return BadRequest(new { message = "No se puede eliminar una distribución que ya está en camino o ha sido entregada" });
            }

            _context.Distribucion.Remove(distribucion);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Distribución eliminada correctamente" });
        }

        private bool DistribucionExists(int id)
        {
            return _context.Distribucion.Any(e => e.DistribucionID == id);
        }
    }
}
