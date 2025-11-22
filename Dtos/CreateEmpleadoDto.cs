using System.ComponentModel.DataAnnotations;

namespace proyectogranja1.Dtos;

public class CreateEmpleadoDto
{
    [Required(ErrorMessage = "El CI es obligatorio")]
    [StringLength(15, ErrorMessage = "El CI no puede exceder los 15 caracteres")]
    public string CI { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres")]
    public string Nombre { get; set; } = string.Empty;

    [StringLength(50, ErrorMessage = "El cargo no puede exceder los 50 caracteres")]
    public string? Cargo { get; set; }

    public bool EsVeterinario { get; set; } = false;
}
