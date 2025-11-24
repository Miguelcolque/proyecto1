using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace proyectogranja1.Models;

public class SolicitudFabrica
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int SolicitudFabricaID { get; set; }

    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public decimal LitrosSolicitados { get; set; }

    [Required]
    public DateTime Fecha { get; set; } = DateTime.Today;

    [Required]
    [StringLength(20)]
    public string Estado { get; set; } = "Pendiente"; // "Pendiente" o "Entregado"

    public bool Activo { get; set; } = true;
}
