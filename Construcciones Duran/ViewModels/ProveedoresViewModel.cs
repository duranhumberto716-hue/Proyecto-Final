using Proyecto_Final.Models;
using Proyecto_Final.Services;

namespace Proyecto_Final.ViewModels;

/// <summary>CRUD de Proveedores (Integrante 4).</summary>
public class ProveedoresViewModel : CrudViewModel<Proveedor>
{
    public ProveedoresViewModel(DatabaseService db) : base(db)
    {
        Title = "Gestión de Proveedores";
    }

    public List<string> Rubros { get; } = new()
    {
        "Cemento y agregados", "Acero y fierro", "Ladrillos y bloques", "Madera",
        "Pinturas y acabados", "Eléctricos", "Sanitarios", "Alquiler de maquinaria", "Transporte"
    };

    public List<int> Calificaciones { get; } = new() { 1, 2, 3, 4, 5 };

    protected override string? Validar(Proveedor p) => Validador.ValidarProveedor(p);

    protected override void Normalizar(Proveedor p)
    {
        p.Empresa = p.Empresa?.Trim() ?? string.Empty;
        p.Nit = p.Nit?.Trim() ?? string.Empty;
        p.Contacto = p.Contacto?.Trim() ?? string.Empty;
        p.Telefono = p.Telefono?.Trim() ?? string.Empty;
        p.Correo = p.Correo?.Trim().ToLowerInvariant() ?? string.Empty;
    }

    protected override bool Coincide(Proveedor p, string filtro) =>
        p.Empresa.ToLowerInvariant().Contains(filtro) ||
        p.Nit.Contains(filtro) ||
        p.Rubro.ToLowerInvariant().Contains(filtro);

    protected override IEnumerable<Proveedor> Ordenar(IEnumerable<Proveedor> datos) =>
        datos.OrderByDescending(p => p.Calificacion).ThenBy(p => p.Empresa);

    protected override string Describir(Proveedor p) => p.Empresa;
}

