using Proyecto_Final.Models;

namespace Proyecto_Final.Services;

/// <summary>
/// Servicio encargado de proveer el catálogo de obras.
/// Las imágenes se sirven directamente desde almacenamiento en la nube (CDN)
/// mediante URLs HTTPS públicas y optimizadas.
/// </summary>
public class ProyectoService : IProyectoService
{
    // =========================================================================
    // ⚠️ GUÍA PARA REEMPLAZAR CON TUS URLS EN LA NUBE (Cloudinary / Supabase):
    // =========================================================================
    // 1. Sube tus fotos de obra a tu cuenta de Cloudinary (https://cloudinary.com)
    //    o Supabase Storage (https://supabase.com/storage).
    // 2. Copia la URL pública HTTPS de cada imagen.
    // 3. Pégalas en la propiedad ImagenUrl de cada proyecto abajo.
    // =========================================================================

    public async Task<IEnumerable<Proyecto>> ObtenerProyectosAsync()
    {
        // Simulación de latencia de red para demostrar carga asíncrona fluida
        await Task.Delay(300);

        return new List<Proyecto>
        {
            new()
            {
                Id = 1,
                Titulo = "Residencia Moderna Las Palmas",
                Categoria = "Construcción Residencial",
                Descripcion = "Edificación unifamiliar contemporánea de 2 niveles, estructura sismo-resistente y acabados en microcemento y madera.",
                Ubicacion = "Zona Norte - Residencial Las Palmas",
                MetrosCuadrados = 320,
                TiempoEjecucion = "8 meses",
                // ☁️ URL en la nube (CDN de alta resolución optimizada para móvil):
                ImagenUrl = "https://images.unsplash.com/photo-1600585154340-be6161a56a0c?q=80&w=1200&auto=format&fit=crop"
            },
            new()
            {
                Id = 2,
                Titulo = "Edificio Corporativo Alianza",
                Categoria = "Obra Comercial",
                Descripcion = "Complejo de 4 niveles para oficinas corporativas, fachada flotante de cristal termoacústico y subsuelo técnico.",
                Ubicacion = "Av. Empresarial #450",
                MetrosCuadrados = 850,
                TiempoEjecucion = "14 meses",
                // ☁️ URL en la nube:
                ImagenUrl = "https://images.unsplash.com/photo-1541888946425-d0fbb1861593?q=80&w=1200&auto=format&fit=crop"
            },
            new()
            {
                Id = 3,
                Titulo = "Remodelación & Penthouse Loft",
                Categoria = "Remodelación Integral",
                Descripcion = "Transformación total de planta abierta, refuerzo de losa, integración de cocina gourmet y terraza con pérgola metálica.",
                Ubicacion = "Condominio Mirador Real",
                MetrosCuadrados = 190,
                TiempoEjecucion = "3 meses",
                // ☁️ URL en la nube:
                ImagenUrl = "https://images.unsplash.com/photo-1503387762-592deb58ef4e?q=80&w=1200&auto=format&fit=crop"
            },
            new()
            {
                Id = 4,
                Titulo = "Estructura de Hormigón & Obra Gruesa",
                Categoria = "Obra Gruesa",
                Descripcion = "Fundaciones profundas con pilotes, columnas de hormigón armado H25 y losas alivianadas para nave industrial.",
                Ubicacion = "Parque Industrial Sector B",
                MetrosCuadrados = 1200,
                TiempoEjecucion = "5 meses",
                // ☁️ URL en la nube:
                ImagenUrl = "https://images.unsplash.com/photo-1589939705384-5185137a7f0f?q=80&w=1200&auto=format&fit=crop"
            },
            new()
            {
                Id = 5,
                Titulo = "Vivienda Minimalista Los Ceibos",
                Categoria = "Construcción desde Cero",
                Descripcion = "Diseño bioclimático llave en mano con iluminación natural cenital, piscina y domótica integrada.",
                Ubicacion = "Urb. Los Ceibos",
                MetrosCuadrados = 260,
                TiempoEjecucion = "7 meses",
                // ☁️ URL en la nube:
                ImagenUrl = "https://images.unsplash.com/photo-1613490493576-7fde63acd811?q=80&w=1200&auto=format&fit=crop"
            }
        };
    }
}

