using Proyecto_Final.Models;
using Proyecto_Final.Services;

namespace Proyecto_Final.ViewModels;

/// <summary>CRUD de Materiales / Inventario (Integrante 3).</summary>
public class MaterialesViewModel : CrudViewModel<Material>
{
    public MaterialesViewModel(DatabaseService db) : base(db)
    {
        Title = "Base de Datos - Materiales";
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

    protected override string RutaReporte => "ReportePage?tipo=materiales";
    protected override string NombreArchivoCsv => "materiales_construcciones_duran";
    protected override string EncabezadoCsv => "ID,Material,Categoria,Unidad,Cantidad,PrecioBs,StockMinimo,ValorTotalBs,Estado";

    protected override string FormatearFilaCsv(Material m) =>
        $"{m.Id},{EscaparCsv(m.Nombre)},{EscaparCsv(m.Categoria)},{EscaparCsv(m.Unidad)},{m.Cantidad:F2},{m.PrecioUnitario:F2},{m.StockMinimo:F2},{m.ValorTotal:F2},{EscaparCsv(m.EstadoStock)}";

    protected override string? Validar(Material m) => Validador.ValidarMaterial(m);

    protected override void Normalizar(Material m)
    {
        m.Nombre = m.Nombre?.Trim() ?? string.Empty;
    }

    protected override bool Coincide(Material m, string filtro) =>
        m.Nombre.ToLowerInvariant().Contains(filtro) ||
        m.Categoria.ToLowerInvariant().Contains(filtro);

    protected override string Describir(Material m) => m.Nombre;
}
