using Proyecto_Final.Models;
using Proyecto_Final.Services;

namespace Proyecto_Final.ViewModels;

/// <summary>CRUD de Clientes (Integrante 1).</summary>
public class ClientesViewModel : CrudViewModel<Cliente>
{
    public ClientesViewModel(DatabaseService db) : base(db)
    {
        Title = "Gestión de Clientes";
    }

    protected override string? Validar(Cliente c) => Validador.ValidarCliente(c);

    protected override void Normalizar(Cliente c)
    {
        c.Nombre = c.Nombre?.Trim() ?? string.Empty;
        c.Ci = c.Ci?.Trim().ToUpperInvariant() ?? string.Empty;
        c.Telefono = c.Telefono?.Trim() ?? string.Empty;
        c.Correo = c.Correo?.Trim().ToLowerInvariant() ?? string.Empty;
        c.Direccion = c.Direccion?.Trim() ?? string.Empty;
    }

    protected override bool Coincide(Cliente c, string filtro) =>
        c.Nombre.ToLowerInvariant().Contains(filtro) ||
        c.Ci.ToLowerInvariant().Contains(filtro) ||
        c.Telefono.Contains(filtro);

    protected override string Describir(Cliente c) => c.Nombre;
}

