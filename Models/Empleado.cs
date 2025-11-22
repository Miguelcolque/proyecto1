using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace proyectogranja1.Models;

public class Empleado
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int EmpleadoID { get; set; }

    [Required]
    [StringLength(15)]
    public string CI { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [StringLength(50)]
    public string? Cargo { get; set; }

    public bool EsVeterinario { get; set; }

    public bool Activo { get; set; } = true;

    // Navigation properties
    public ICollection<Produccion> Producciones { get; set; } = new List<Produccion>();
    public ICollection<Distribucion> Distribuciones { get; set; } = new List<Distribucion>();
}
