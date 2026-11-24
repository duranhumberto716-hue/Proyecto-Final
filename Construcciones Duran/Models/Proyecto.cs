namespace Proyecto_Final.Models;

/// <summary>
/// Representa una obra o proyecto ejecutado por Construcciones Duran.
/// Las imágenes se alojan en la nube (ej. Cloudinary, Supabase o Azure Blob Storage)
/// para optimizar el peso de la app y permitir actualización remota.
/// </summary>
public class Proyecto
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty; // Ej: "Residencial", "Comercial", "Obra Gruesa", "Acabados"
    public string Descripcion { get; set; } = string.Empty;
    public string Ubicacion { get; set; } = string.Empty;
    
    /// <summary>
    /// URL directa HTTPS al archivo de imagen alojado en la nube (CDN).
    /// </summary>
    public string ImagenUrl { get; set; } = string.Empty;
    public double MetrosCuadrados { get; set; }
    public string TiempoEjecucion { get; set; } = string.Empty;
    public string BadgeTexto => $"{MetrosCuadrados:F0} m² • {TiempoEjecucion}";
}

