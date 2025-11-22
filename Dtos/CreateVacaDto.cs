using System.ComponentModel.DataAnnotations;

namespace proyectogranja1.Dtos;

public class CreateVacaDto
{
    [Required(ErrorMessage = "El código de la vaca es obligatorio")]
    [StringLength(20, ErrorMessage = "El código no puede exceder los 20 caracteres")]
    public string CodigoVaca { get; set; } = string.Empty;

    [StringLength(50, ErrorMessage = "La raza no puede exceder los 50 caracteres")]
    public string? Raza { get; set; }

    [Required(ErrorMessage = "El peso es obligatorio")]
    [Range(0.01, double.MaxValue, ErrorMessage = "El peso debe ser mayor a 0")]
    public decimal Peso { get; set; }

    [StringLength(30, ErrorMessage = "El estado de salud no puede exceder los 30 caracteres")]
    public string? EstadoSalud { get; set; }
}
