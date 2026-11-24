using SQLite;

namespace Proyecto_Final.Models;

/// <summary>
/// Registro de avance físico de una obra (tabla Avance). CRUD del Integrante 4.
/// </summary>
[Table("Avance")]
public class Avance : EntidadBase
{
    [NotNull, MaxLength(120)]
    public string NombreObra { get; set; } = string.Empty;

    [NotNull, MaxLength(80)]
    public string Etapa { get; set; } = string.Empty;

    /// <summary>Porcentaje de avance acumulado (0 a 100).</summary>
    public double Porcentaje { get; set; }

    [NotNull, MaxLength(100)]
    public string Responsable { get; set; } = string.Empty;

    public DateTime Fecha { get; set; } = DateTime.Today;

    [MaxLength(250)]
    public string Observaciones { get; set; } = string.Empty;

    [Ignore]
    public double ProgresoNormalizado => Math.Clamp(Porcentaje, 0, 100) / 100.0;

    [Ignore]
    public string PorcentajeTexto => $"{Porcentaje:N0}%";

    [Ignore]
    public bool Completado => Porcentaje >= 100;

    [Ignore]
    public string Resumen => $"{Etapa}  •  Fecha: {Fecha:dd/MM/yyyy}";

    [Ignore]
    public string Detalle => string.IsNullOrWhiteSpace(Observaciones)
        ? $"Responsable: {Responsable}"
        : $"Responsable: {Responsable}  •  {Observaciones}";
}

