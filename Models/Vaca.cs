using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace proyectogranja1.Models;

public class Vaca
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int VacaID { get; set; }

    [Required]
    [StringLength(20)]
    public string CodigoVaca { get; set; } = string.Empty;

    [StringLength(50)]
    public string? Raza { get; set; }

    [Required]
    [Column(TypeName = "decimal(6,2)")]
    public decimal Peso { get; set; }

    [StringLength(20)]
    public string? EstadoSalud { get; set; } = "Saludable";

    // Método helper para validar
    public static readonly string[] EstadosValidos =
    {
        "Saludable", "Enferma", "En Terapia", "Peñada",
        "En Producción", "Retirada", "Muerta"
    };

    public bool Activo { get; set; } = true;

    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    // Navigation property
    public ICollection<Produccion> Producciones { get; set; } = new List<Produccion>();
}
