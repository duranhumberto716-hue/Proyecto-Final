using Proyecto_Final.Models;

namespace Proyecto_Final.Services;

/// <summary>
/// Guarda el usuario que inició sesión y controla los permisos de acceso por rol.
/// </summary>
public class SesionService
{
    private static readonly HashSet<string> RutasCliente = new(StringComparer.OrdinalIgnoreCase)
    {
        "MainPage",
        "CotizacionPage"
    };

    private static readonly HashSet<string> RutasTrabajador = new(StringComparer.OrdinalIgnoreCase)
    {
        "MainPage",
        "MaterialesPage",
        "AvancesPage",
        "ReportePage?tipo=materiales",
        "ReportePage?tipo=avances",
        "ReportePage?tipo=obras",
        "OdsPage"
    };

    public Usuario? UsuarioActual { get; private set; }

    public bool HaySesion => UsuarioActual is not null;

    public string RolActual => NormalizarRol(UsuarioActual?.Rol);

    public bool EsAdministrador => string.Equals(RolActual, Roles.Administrador, StringComparison.OrdinalIgnoreCase);

    public bool EsTrabajador => string.Equals(RolActual, Roles.Trabajador, StringComparison.OrdinalIgnoreCase);

    public bool EsCliente => !EsAdministrador && !EsTrabajador;

    public void Iniciar(Usuario usuario) => UsuarioActual = usuario;

    public void Cerrar() => UsuarioActual = null;

    /// <summary>
    /// Verifica si el rol activo tiene autorización para navegar a la ruta solicitada.
    /// </summary>
    public bool PuedeAccederARuta(string? ruta)
    {
        if (!HaySesion || string.IsNullOrWhiteSpace(ruta))
            return false;

        if (EsAdministrador)
            return true;

        var destino = ruta.Trim();
        if (EsTrabajador)
            return RutasTrabajador.Contains(destino);

        return RutasCliente.Contains(destino);
    }

    private static string NormalizarRol(string? rol)
    {
        if (string.Equals(rol, Roles.Administrador, StringComparison.OrdinalIgnoreCase))
            return Roles.Administrador;
        if (string.Equals(rol, Roles.Trabajador, StringComparison.OrdinalIgnoreCase))
            return Roles.Trabajador;
        return Roles.Cliente;
    }
}
