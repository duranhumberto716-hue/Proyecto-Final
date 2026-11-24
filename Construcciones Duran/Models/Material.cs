using SQLite;

namespace Proyecto_Final.Models;

/// <summary>
/// Material de construcción del inventario (tabla Material). CRUD del Integrante 2.
/// Los precios se registran en bolivianos (Bs).
/// </summary>
[Table("Material")]
public class Material : EntidadBase
{
    [NotNull, MaxLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [NotNull, MaxLength(50)]
    public string Categoria { get; set; } = string.Empty;

    [NotNull, MaxLength(20)]
    public string Unidad { get; set; } = string.Empty;

    public double Cantidad { get; set; }

    public double PrecioUnitario { get; set; }

    public double StockMinimo { get; set; }

    [Ignore]
    public double ValorTotal => Cantidad * PrecioUnitario;

    [Ignore]
    public bool StockBajo => Cantidad <= StockMinimo;

    [Ignore]
    public string Resumen => $"{Categoria}  •  {Cantidad:N2} {Unidad}  •  Bs {PrecioUnitario:N2} c/u";

    [Ignore]
    public string Detalle => StockBajo
        ? $"⚠️ Stock bajo (mínimo {StockMinimo:N0})  •  Valor: Bs {ValorTotal:N2}"
        : $"✅ Stock suficiente  •  Valor: Bs {ValorTotal:N2}";
}

