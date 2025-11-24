using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using proyectogranja1.Models;

namespace proyectogranja1.Data
{
    public class proyectogranja1Context : DbContext
    {
        public proyectogranja1Context (DbContextOptions<proyectogranja1Context> options)
            : base(options)
        {
        }

        public DbSet<proyectogranja1.Models.Vaca> Vaca { get; set; } = default!;
        public DbSet<proyectogranja1.Models.Empleado> Empleado { get; set; } = default!;
        public DbSet<proyectogranja1.Models.Produccion> Produccion { get; set; } = default!;
        public DbSet<proyectogranja1.Models.Distribucion> Distribucion { get; set; } = default!;
        public DbSet<proyectogranja1.Models.SolicitudFabrica> SolicitudFabrica { get; set; } = default!;
    }
}
