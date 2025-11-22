using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace proyectogranja1.Models;

public class Produccion
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ProduccionID { get; set; }

    [Required]
    public int VacaID { get; set; }

    [Required]
    public int EmpleadoID { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime Fecha { get; set; } = DateTime.Today;

    [Required]
    [Column(TypeName = "decimal(6,2)")]
    public decimal TotalLitros { get; set; }

    [StringLength(20)]
    public string? Calidad { get; set; }

    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("VacaID")]
    public Vaca Vaca { get; set; } = null!;

    [ForeignKey("EmpleadoID")]
    public Empleado Empleado { get; set; } = null!;
}
