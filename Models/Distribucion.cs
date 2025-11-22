using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace proyectogranja1.Models;

public class Distribucion
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int DistribucionID { get; set; }

    [Required]
    public int EmpleadoID { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime Fecha { get; set; } = DateTime.Today;

    [Required]
    [Column(TypeName = "decimal(8,2)")]
    public decimal LitrosEntregados { get; set; }

    [Required]
    [StringLength(100)]
    public string Destino { get; set; } = string.Empty;

    [StringLength(20)]
    public string Estado { get; set; } = "Pendiente";

    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

    // Navigation property
    [ForeignKey("EmpleadoID")]
    public Empleado Empleado { get; set; } = null!;
}
