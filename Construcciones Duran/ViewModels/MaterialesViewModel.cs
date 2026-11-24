using Proyecto_Final.Models;
using Proyecto_Final.Services;

namespace Proyecto_Final.ViewModels;

/// <summary>CRUD de Materiales / Inventario (Integrante 2).</summary>
public class MaterialesViewModel : CrudViewModel<Material>
{
    public MaterialesViewModel(DatabaseService db) : base(db)
    {
        Title = "Inventario de Materiales";
    }

    public List<string> Categorias { get; } = new()
    {
        "Cemento y agregados", "Acero y fierro", "Ladrillos y bloques",
        "Madera", "Pinturas y acabados", "Eléctricos", "Sanitarios", "Herramientas"
    };

    public List<string> Unidades { get; } = new()
    {
        "Bolsa", "Barra", "Unidad", "m³", "m²", "Metro", "Galón", "Kg", "Caja"
    };

    protected override string? Validar(Material m) => Validador.ValidarMaterial(m);

    protected override void Normalizar(Material m)
    {
        m.Nombre = m.Nombre?.Trim() ?? string.Empty;
    }

    protected override bool Coincide(Material m, string filtro) =>
        m.Nombre.ToLowerInvariant().Contains(filtro) ||
        m.Categoria.ToLowerInvariant().Contains(filtro);

    protected override IEnumerable<Material> Ordenar(IEnumerable<Material> datos) =>
        datos.OrderByDescending(m => m.StockBajo).ThenBy(m => m.Nombre);

    protected override string Describir(Material m) => m.Nombre;
}

