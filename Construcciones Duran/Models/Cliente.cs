using SQLite;

namespace Proyecto_Final.Models;

/// <summary>
/// Cliente de Construcciones Durán (tabla Cliente). CRUD del Integrante 1.
/// </summary>
[Table("Cliente")]
public class Cliente : EntidadBase
{
    [NotNull, MaxLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [Unique, NotNull, MaxLength(15)]
    public string Ci { get; set; } = string.Empty;

    [NotNull, MaxLength(15)]
    public string Telefono { get; set; } = string.Empty;

    [MaxLength(100)]
    public string Correo { get; set; } = string.Empty;

    [MaxLength(150)]
    public string Direccion { get; set; } = string.Empty;

    public DateTime FechaRegistro { get; set; } = DateTime.Now;

    [Ignore]
    public string Resumen => $"CI: {Ci}  •  📞 {Telefono}";

    [Ignore]
    public string Detalle => string.IsNullOrWhiteSpace(Correo)
        ? $"📍 {Direccion}"
        : $"✉️ {Correo}  •  📍 {Direccion}";
}

