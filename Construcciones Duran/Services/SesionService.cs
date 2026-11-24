using Proyecto_Final.Models;

namespace Proyecto_Final.Services;

/// <summary>
/// Guarda el usuario que inició sesión mientras la app está abierta.
/// </summary>
public class SesionService
{
    public Usuario? UsuarioActual { get; private set; }

    public bool HaySesion => UsuarioActual is not null;

    public void Iniciar(Usuario usuario) => UsuarioActual = usuario;

    public void Cerrar() => UsuarioActual = null;
}

