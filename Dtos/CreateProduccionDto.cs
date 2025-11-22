using System.ComponentModel.DataAnnotations;

namespace proyectogranja1.Dtos;

public class CreateProduccionDto
{
    [Required(ErrorMessage = "El ID de la vaca es obligatorio")]
    public int VacaID { get; set; }

    [Required(ErrorMessage = "El ID del empleado es obligatorio")]
    public int EmpleadoID { get; set; }

    [Required(ErrorMessage = "La fecha es obligatoria")]
    public DateTime Fecha { get; set; } = DateTime.Today;

    [Required(ErrorMessage = "El total de litros es obligatorio")]
    [Range(0.01, double.MaxValue, ErrorMessage = "El total de litros debe ser mayor a 0")]
    public decimal TotalLitros { get; set; }

    [StringLength(20, ErrorMessage = "La calidad no puede exceder los 20 caracteres")]
    public string? Calidad { get; set; }
}
