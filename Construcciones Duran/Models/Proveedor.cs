using SQLite;

namespace Proyecto_Final.Models;

/// <summary>
/// Proveedor de materiales o servicios (tabla Proveedor). CRUD del Integrante 4.
/// </summary>
[Table("Proveedor")]
public class Proveedor : EntidadBase
{
    [NotNull, MaxLength(100)]
    public string Empresa { get; set; } = string.Empty;

    [Unique, NotNull, MaxLength(15)]
    public string Nit { get; set; } = string.Empty;

    [MaxLength(100)]
    public string Contacto { get; set; } = string.Empty;

    [NotNull, MaxLength(15)]
    public string Telefono { get; set; } = string.Empty;

    [NotNull, MaxLength(50)]
    public string Rubro { get; set; } = string.Empty;

    [MaxLength(100)]
    public string Correo { get; set; } = string.Empty;

    /// <summary>Calificación del proveedor de 1 a 5 estrellas.</summary>
    public int Calificacion { get; set; } = 3;

    [Ignore]
    public string Estrellas => new string('★', Math.Clamp(Calificacion, 0, 5)) + new string('☆', 5 - Math.Clamp(Calificacion, 0, 5));

    [Ignore]
    public string Resumen => $"{Rubro}  •  NIT: {Nit}  •  📞 {Telefono}";

    [Ignore]
    public string Detalle => string.IsNullOrWhiteSpace(Contacto)
        ? Estrellas
        : $"👤 {Contacto}  •  {Estrellas}";
}

