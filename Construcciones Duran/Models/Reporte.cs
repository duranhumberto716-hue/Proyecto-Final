namespace Proyecto_Final.Models;

/// <summary>
/// Tarjeta de indicador (KPI) que se muestra en los formularios de información.
/// </summary>
public class Indicador
{
    public string Etiqueta { get; set; } = string.Empty;
    public string Titulo { get; set; } = string.Empty;
    public string Valor { get; set; } = string.Empty;
}

/// <summary>
/// Fila de detalle de un reporte.
/// </summary>
public class FilaReporte
{
    public string Titulo { get; set; } = string.Empty;
    public string Detalle { get; set; } = string.Empty;
    public string Valor { get; set; } = string.Empty;
    public bool Destacado { get; set; }
}
