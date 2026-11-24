using SQLite;

namespace Proyecto_Final.Models;

/// <summary>
/// Empleado / personal de obra (tabla Empleado). CRUD del Integrante 2.
/// </summary>
[Table("Empleado")]
public class Empleado : EntidadBase
{
    [NotNull, MaxLength(100)]
    public string NombreCompleto { get; set; } = string.Empty;

    [Unique, NotNull, MaxLength(15)]
    public string Ci { get; set; } = string.Empty;

    [NotNull, MaxLength(50)]
    public string Cargo { get; set; } = string.Empty;

    [MaxLength(15)]
    public string Telefono { get; set; } = string.Empty;

    public double SalarioMensual { get; set; }

    public DateTime FechaIngreso { get; set; } = DateTime.Today;

    public bool Activo { get; set; } = true;

    [Ignore]
    public string EstadoTexto => Activo ? "ACTIVO" : "INACTIVO";

    [Ignore]
    public string Resumen => string.IsNullOrWhiteSpace(Telefono)
        ? $"{Cargo}  •  CI: {Ci}"
        : $"{Cargo}  •  CI: {Ci}  •  Tel: {Telefono}";

    [Ignore]
    public string Detalle => $"Bs {SalarioMensual:N2}/mes  •  Ingreso: {FechaIngreso:dd/MM/yyyy}";
}
