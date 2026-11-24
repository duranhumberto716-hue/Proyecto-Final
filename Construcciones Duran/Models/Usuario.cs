using SQLite;

namespace Proyecto_Final.Models;

/// <summary>
/// Usuario del sistema (tabla Usuario). La contraseña se guarda como hash SHA-256, nunca en texto plano.
/// </summary>
[Table("Usuario")]
public class Usuario : EntidadBase
{
    [Unique, NotNull, MaxLength(30)]
    public string NombreUsuario { get; set; } = string.Empty;

    [NotNull, MaxLength(100)]
    public string NombreCompleto { get; set; } = string.Empty;

    [NotNull]
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>Administrador o Usuario.</summary>
    public string Rol { get; set; } = "Usuario";

    public DateTime FechaRegistro { get; set; } = DateTime.Now;
}

