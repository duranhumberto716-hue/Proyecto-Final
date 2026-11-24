using SQLite;

namespace Proyecto_Final.Models;

/// <summary>
/// Obra o proyecto de construcción registrado (tabla Obra). CRUD del Integrante 1.
/// </summary>
[Table("Obra")]
public class Obra : EntidadBase
{
    [NotNull, MaxLength(120)]
    public string Nombre { get; set; } = string.Empty;

    [NotNull, MaxLength(100)]
    public string Cliente { get; set; } = string.Empty;

    [NotNull, MaxLength(150)]
    public string Ubicacion { get; set; } = string.Empty;

    [NotNull, MaxLength(60)]
    public string TipoObra { get; set; } = string.Empty;

    public double Presupuesto { get; set; }

    public DateTime FechaInicio { get; set; } = DateTime.Today;

    [NotNull, MaxLength(30)]
    public string Estado { get; set; } = "Planificación";

    [Ignore]
    public bool EnEjecucion => string.Equals(Estado, "En ejecución", StringComparison.OrdinalIgnoreCase);

    [Ignore]
    public string Resumen => $"{TipoObra}  •  Cliente: {Cliente}";

    [Ignore]
    public string Detalle => $"Ubicación: {Ubicacion}  •  Bs {Presupuesto:N2}  •  Inicio: {FechaInicio:dd/MM/yyyy}";
}

