using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using proyectogranja1.Data;
using proyectogranja1.Dtos;
using proyectogranja1.Models;

namespace proyectogranja1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmpleadosController : ControllerBase
    {
        private readonly proyectogranja1Context _context;

        public EmpleadosController(proyectogranja1Context context)
        {
            _context = context;
        }

        // GET: api/Empleados
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetEmpleados()
        {
            return await _context.Empleado
                .Select(e => new
                {
                    e.EmpleadoID,
                    e.CI,
                    e.Nombre,
                    e.Cargo,
                    e.EsVeterinario,
                    e.Activo
                })
                .ToListAsync();
        }

        // GET: api/Empleados/activos
        [HttpGet("activos")]
        public async Task<ActionResult<IEnumerable<object>>> GetEmpleadosActivos()
        {
            return await _context.Empleado
                .Where(e => e.Activo)
                .Select(e => new
                {
                    e.EmpleadoID,
                    e.CI,
                    e.Nombre,
                    e.Cargo,
                    e.EsVeterinario
                })
                .ToListAsync();
        }

        // GET: api/Empleados/inactivos
        [HttpGet("inactivos")]
        public async Task<ActionResult<IEnumerable<object>>> GetEmpleadosInactivos()
        {
            return await _context.Empleado
                .Where(e => !e.Activo)
                .Select(e => new
                {
                    e.EmpleadoID,
                    e.CI,
                    e.Nombre,
                    e.Cargo,
                    e.EsVeterinario
                })
                .ToListAsync();
        }

        // GET: api/Empleados/5
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetEmpleado(int id)
        {
            var empleado = await _context.Empleado
                .Where(e => e.EmpleadoID == id)
                .Select(e => new
                {
                    e.EmpleadoID,
                    e.CI,
                    e.Nombre,
                    e.Cargo,
                    e.EsVeterinario,
                    e.Activo
                })
                .FirstOrDefaultAsync();

            if (empleado == null)
            {
                return NotFound(new { message = $"No se encontró el empleado con ID {id}" });
            }

            return empleado;
        }

        // GET: api/Empleados/veterinarios
        [HttpGet("veterinarios")]
        public async Task<ActionResult<IEnumerable<object>>> GetVeterinarios()
        {
            return await _context.Empleado
                .Where(e => e.EsVeterinario && e.Activo)
                .Select(e => new
                {
                    e.EmpleadoID,
                    e.CI,
                    e.Nombre,
                    e.Cargo
                })
                .ToListAsync();
        }

        // PUT: api/Empleados/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmpleado(int id, CreateEmpleadoDto updateEmpleadoDto) 
        {
            var empleado = await _context.Empleado.FindAsync(id);
            if (empleado == null)
            {
                return NotFound(new { message = $"No se encontró el empleado con ID {id}" });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Datos del empleado no válidos", errors = ModelState.Values.SelectMany(v => v.Errors) });
            }

            // Validar que la CI sea única
            if (await _context.Empleado.AnyAsync(e => e.CI == updateEmpleadoDto.CI && e.EmpleadoID != id))
            {
                return BadRequest(new { message = "Ya existe un empleado con esta cédula de identidad" });
            }

            // Actualizar solo los campos permitidos
            empleado.CI = updateEmpleadoDto.CI;
            empleado.Nombre = updateEmpleadoDto.Nombre;
            empleado.Cargo = updateEmpleadoDto.Cargo;
            empleado.EsVeterinario = updateEmpleadoDto.EsVeterinario;

            _context.Entry(empleado).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                
                var result = new
                {
                    empleado.EmpleadoID,
                    empleado.CI,
                    empleado.Nombre,
                    empleado.Cargo,
                    empleado.EsVeterinario,
                    empleado.Activo
                };
                
                return Ok(new { message = "Empleado actualizado correctamente", data = result });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmpleadoExists(id))
                {
                    return NotFound(new { message = $"No se encontró el empleado con ID {id}" });
                }
                else
                {
                    throw;
                }
            }
        }

        // POST: api/Empleados
        [HttpPost]
        public async Task<ActionResult<object>> PostEmpleado(CreateEmpleadoDto createEmpleadoDto) 
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Datos del empleado no válidos", errors = ModelState.Values.SelectMany(v => v.Errors) });
            }

            // Validar que la CI sea única
            if (await _context.Empleado.AnyAsync(e => e.CI == createEmpleadoDto.CI))
            {
                return BadRequest(new { message = "Ya existe un empleado con esta cédula de identidad" });
            }

            var empleado = new Empleado
            {
                CI = createEmpleadoDto.CI,
                Nombre = createEmpleadoDto.Nombre,
                Cargo = createEmpleadoDto.Cargo,
                EsVeterinario = createEmpleadoDto.EsVeterinario,
                Activo = true
            };

            _context.Empleado.Add(empleado);
            await _context.SaveChangesAsync();

            var result = new
            {
                empleado.EmpleadoID,
                empleado.CI,
                empleado.Nombre,
                empleado.Cargo,
                empleado.EsVeterinario,
                empleado.Activo
            };

            return CreatedAtAction(nameof(GetEmpleado), new { id = empleado.EmpleadoID },
                new { message = "Empleado creado correctamente", data = result });
        }

        // DELETE: api/Empleados/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmpleado(int id)
        {
            var empleado = await _context.Empleado.FindAsync(id);
            if (empleado == null)
            {
                return NotFound(new { message = $"No se encontró el empleado con ID {id}" });
            }

            // Verificar si el empleado tiene producciones o distribuciones asociadas
            bool tieneProducciones = await _context.Produccion.AnyAsync(p => p.EmpleadoID == id);
            bool tieneDistribuciones = await _context.Distribucion.AnyAsync(d => d.EmpleadoID == id);

            if (tieneProducciones || tieneDistribuciones)
            {
                // En lugar de eliminar, marcamos como inactivo
                empleado.Activo = false;
                _context.Entry(empleado).State = EntityState.Modified;
                
                await _context.SaveChangesAsync();
                return Ok(new { message = "El empleado tiene registros asociados y ha sido marcado como inactivo" });
            }
            else
            {
                _context.Empleado.Remove(empleado);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Empleado eliminado correctamente" });
            }
        }

        private bool EmpleadoExists(int id)
        {
            return _context.Empleado.Any(e => e.EmpleadoID == id);
        }
    }
}
