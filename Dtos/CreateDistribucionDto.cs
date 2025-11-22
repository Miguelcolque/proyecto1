using System.ComponentModel.DataAnnotations;

namespace proyectogranja1.Dtos;

public class CreateDistribucionDto
{
    [Required(ErrorMessage = "El ID del empleado es obligatorio")]
    public int EmpleadoID { get; set; }

    [Required(ErrorMessage = "La fecha es obligatoria")]
    public DateTime Fecha { get; set; } = DateTime.Today;

    [Required(ErrorMessage = "Los litros entregados son obligatorios")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Los litros entregados deben ser mayores a 0")]
    public decimal LitrosEntregados { get; set; }

    [Required(ErrorMessage = "El destino es obligatorio")]
    [StringLength(100, ErrorMessage = "El destino no puede exceder los 100 caracteres")]
    public string Destino { get; set; } = string.Empty;

    [StringLength(20)]
    public string Estado { get; set; } = "Pendiente";
}
