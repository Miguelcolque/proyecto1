using System.ComponentModel.DataAnnotations;

namespace proyectogranja1.Dtos;

public class CreateSolicitudFabricaDto
{
    [Required(ErrorMessage = "Los litros solicitados son obligatorios")]
    [Range(0.1, double.MaxValue, ErrorMessage = "Los litros deben ser mayores a 0")]
    public decimal LitrosSolicitados { get; set; }

    [Required(ErrorMessage = "La fecha es obligatoria")]
    public DateTime Fecha { get; set; } = DateTime.Today;

    [StringLength(20, ErrorMessage = "El estado no puede exceder los 20 caracteres")]
    public string Estado { get; set; } = "Pendiente"; // Valor por defecto
}
