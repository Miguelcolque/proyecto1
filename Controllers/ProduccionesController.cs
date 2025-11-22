using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using proyectogranja1.Data;
using proyectogranja1.Dtos;
using proyectogranja1.Models;

namespace proyectogranja1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProduccionesController : ControllerBase
    {
        private readonly proyectogranja1Context _context;

        public ProduccionesController(proyectogranja1Context context)
        {
            _context = context;
        }

        // GET: api/Producciones
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetProducciones()
        {
            return await _context.Produccion
                .Include(p => p.Vaca)
                .Include(p => p.Empleado)
                .Select(p => new
                {
                    p.ProduccionID,
                    p.VacaID,
                    VacaCodigo = p.Vaca.CodigoVaca,
                    p.EmpleadoID,
                    EmpleadoNombre = p.Empleado.Nombre,
                    p.Fecha,
                    p.TotalLitros,
                    p.Calidad,
                    p.FechaRegistro
                })
                .ToListAsync();
        }

        // GET: api/Producciones/por-fecha?fechaInicio=2023-01-01&fechaFin=2023-12-31
        [HttpGet("por-fecha")]
        public async Task<ActionResult<IEnumerable<object>>> GetProduccionesPorRangoFechas(
            [FromQuery] DateTime fechaInicio, 
            [FromQuery] DateTime fechaFin)
        {
            return await _context.Produccion
                .Where(p => p.Fecha >= fechaInicio && p.Fecha <= fechaFin)
                .Include(p => p.Vaca)
                .Include(p => p.Empleado)
                .Select(p => new
                {
                    p.ProduccionID,
                    p.VacaID,
                    VacaCodigo = p.Vaca.CodigoVaca,
                    p.EmpleadoID,
                    EmpleadoNombre = p.Empleado.Nombre,
                    p.Fecha,
                    p.TotalLitros,
                    p.Calidad
                })
                .ToListAsync();
        }

        // GET: api/Producciones/5
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetProduccion(int id)
        {
            var produccion = await _context.Produccion
                .Include(p => p.Vaca)
                .Include(p => p.Empleado)
                .Where(p => p.ProduccionID == id)
                .Select(p => new
                {
                    p.ProduccionID,
                    p.VacaID,
                    VacaCodigo = p.Vaca.CodigoVaca,
                    p.EmpleadoID,
                    EmpleadoNombre = p.Empleado.Nombre,
                    p.Fecha,
                    p.TotalLitros,
                    p.Calidad,
                    p.FechaRegistro
                })
                .FirstOrDefaultAsync();

            if (produccion == null)
            {
                return NotFound(new { message = $"No se encontró la producción con ID {id}" });
            }

            return produccion;
        }

        // GET: api/Producciones/por-vaca/5
        [HttpGet("por-vaca/{vacaId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetProduccionesPorVaca(int vacaId)
        {
            return await _context.Produccion
                .Where(p => p.VacaID == vacaId)
                .Include(p => p.Vaca)
                .Include(p => p.Empleado)
                .Select(p => new
                {
                    p.ProduccionID,
                    p.VacaID,
                    VacaCodigo = p.Vaca.CodigoVaca,
                    p.EmpleadoID,
                    EmpleadoNombre = p.Empleado.Nombre,
                    p.Fecha,
                    p.TotalLitros,
                    p.Calidad
                })
                .ToListAsync();
        }

        // GET: api/Producciones/por-empleado/5
        [HttpGet("por-empleado/{empleadoId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetProduccionesPorEmpleado(int empleadoId)
        {
            return await _context.Produccion
                .Where(p => p.EmpleadoID == empleadoId)
                .Include(p => p.Vaca)
                .Include(p => p.Empleado)
                .Select(p => new
                {
                    p.ProduccionID,
                    p.VacaID,
                    VacaCodigo = p.Vaca.CodigoVaca,
                    p.EmpleadoID,
                    EmpleadoNombre = p.Empleado.Nombre,
                    p.Fecha,
                    p.TotalLitros,
                    p.Calidad
                })
                .ToListAsync();
        }

        // POST: api/Producciones
        [HttpPost]
        public async Task<ActionResult<object>> PostProduccion(CreateProduccionDto createProduccionDto) 
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Datos de producción no válidos", errors = ModelState.Values.SelectMany(v => v.Errors) });
            }

            // Validar que la vaca existe y está activa
            var vaca = await _context.Vaca.FindAsync(createProduccionDto.VacaID);
            if (vaca == null || !vaca.Activo)
            {
                return BadRequest(new { message = "La vaca especificada no existe o no está activa" });
            }

            // Validar que el empleado existe y está activo
            var empleado = await _context.Empleado.FindAsync(createProduccionDto.EmpleadoID);
            if (empleado == null || !empleado.Activo)
            {
                return BadRequest(new { message = "El empleado especificado no existe o no está activo" });
            }

            // Validar que la fecha no sea futura
            if (createProduccionDto.Fecha > DateTime.Today)
            {
                return BadRequest(new { message = "La fecha de producción no puede ser futura" });
            }

            // Validar que los litros sean positivos
            if (createProduccionDto.TotalLitros <= 0)
            {
                return BadRequest(new { message = "La cantidad de litros debe ser mayor a cero" });
            }

            var produccion = new Produccion
            {
                VacaID = createProduccionDto.VacaID,
                EmpleadoID = createProduccionDto.EmpleadoID,
                Fecha = createProduccionDto.Fecha,
                TotalLitros = createProduccionDto.TotalLitros,
                Calidad = createProduccionDto.Calidad,
                FechaRegistro = DateTime.UtcNow
            };

            _context.Produccion.Add(produccion);
            await _context.SaveChangesAsync();

            // Obtener los datos relacionados para la respuesta
            var produccionResponse = await _context.Produccion
                .Where(p => p.ProduccionID == produccion.ProduccionID)
                .Select(p => new
                {
                    p.ProduccionID,
                    p.VacaID,
                    VacaCodigo = p.Vaca.CodigoVaca,
                    p.EmpleadoID,
                    EmpleadoNombre = p.Empleado.Nombre,
                    p.Fecha,
                    p.TotalLitros,
                    p.Calidad,
                    p.FechaRegistro
                })
                .FirstOrDefaultAsync();

            return CreatedAtAction(nameof(GetProduccion), new { id = produccion.ProduccionID },
                new { message = "Producción registrada correctamente", data = produccionResponse });
        }

        // PUT: api/Producciones/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduccion(int id, CreateProduccionDto updateProduccionDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Datos de producción no válidos", errors = ModelState.Values.SelectMany(v => v.Errors) });
            }

            // Validar que la producción existe
            var produccionExistente = await _context.Produccion.FindAsync(id);
            if (produccionExistente == null)
            {
                return NotFound(new { message = $"No se encontró la producción con ID {id}" });
            }

            // Validar que la vaca existe y está activa
            var vaca = await _context.Vaca.FindAsync(updateProduccionDto.VacaID);
            if (vaca == null || !vaca.Activo)
            {
                return BadRequest(new { message = "La vaca especificada no existe o no está activa" });
            }

            // Validar que el empleado existe y está activo
            var empleado = await _context.Empleado.FindAsync(updateProduccionDto.EmpleadoID);
            if (empleado == null || !empleado.Activo)
            {
                return BadRequest(new { message = "El empleado especificado no existe o no está activo" });
            }

            // Validar que la fecha no sea futura
            if (updateProduccionDto.Fecha > DateTime.Today)
            {
                return BadRequest(new { message = "La fecha de producción no puede ser futura" });
            }

            // Validar que los litros sean positivos
            if (updateProduccionDto.TotalLitros <= 0)
            {
                return BadRequest(new { message = "La cantidad de litros debe ser mayor a cero" });
            }

            // Actualizar solo los campos permitidos
            produccionExistente.VacaID = updateProduccionDto.VacaID;
            produccionExistente.EmpleadoID = updateProduccionDto.EmpleadoID;
            produccionExistente.Fecha = updateProduccionDto.Fecha;
            produccionExistente.TotalLitros = updateProduccionDto.TotalLitros;
            produccionExistente.Calidad = updateProduccionDto.Calidad;

            // No es necesario validar nuevamente, ya se validó al inicio del método
            // Las variables vaca y empleado ya están definidas y validadas arriba

            // Marcar como modificado
            _context.Entry(produccionExistente).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();

                // Obtener los datos actualizados para la respuesta
                var produccionActualizada = await _context.Produccion
                    .Where(p => p.ProduccionID == id)
                    .Select(p => new
                    {
                        p.ProduccionID,
                        p.VacaID,
                        VacaCodigo = p.Vaca.CodigoVaca,
                        p.EmpleadoID,
                        EmpleadoNombre = p.Empleado.Nombre,
                        p.Fecha,
                        p.TotalLitros,
                        p.Calidad,
                        p.FechaRegistro
                    })
                    .FirstOrDefaultAsync();

                return Ok(new { message = "Producción actualizada correctamente", data = produccionActualizada });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProduccionExists(id))
                {
                    return NotFound(new { message = $"No se encontró la producción con ID {id}" });
                }
                else
                {
                    throw;
                }
            }
        }

        // DELETE: api/Producciones/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduccion(int id)
        {
            var produccion = await _context.Produccion.FindAsync(id);
            if (produccion == null)
            {
                return NotFound(new { message = $"No se encontró la producción con ID {id}" });
            }

            _context.Produccion.Remove(produccion);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Producción eliminada correctamente" });
        }

        private bool ProduccionExists(int id)
        {
            return _context.Produccion.Any(e => e.ProduccionID == id);
        }
    }
}
