using System.Diagnostics;

namespace Proyecto_Final.Services;

/// <summary>
/// Servicio para integración directa con WhatsApp mediante la API Click-to-Chat (wa.me)
/// y el Launcher de .NET MAUI.
/// </summary>
public class WhatsAppService : IWhatsAppService
{
    // =========================================================================
    // ⚠️ NÚMERO OFICIAL DE WHATSAPP: 77661522
    // =========================================================================
    // Para la API de WhatsApp (wa.me) se requiere el formato internacional
    // con el código de país de Bolivia (591): 59177661522.
    // El método SanitizarNumero antepone automáticamente '591' si se ingresan 8 dígitos.
    public const string NUMERO_VISIBLE = "77661522";
    private const string NUMERO_WHATSAPP_PREDETERMINADO = "59177661522";

    /// <summary>
    /// Envía un mensaje estructurado directamente al chat de WhatsApp del contratista.
    /// </summary>
    public async Task<bool> EnviarMensajeAsync(string mensaje, string? numeroTelefono = null)
    {
        var numeroDestino = string.IsNullOrWhiteSpace(numeroTelefono)
            ? NUMERO_WHATSAPP_PREDETERMINADO
            : SanitizarNumero(numeroTelefono);

        // 1. Codificación segura según el estándar RFC 3986
        var mensajeCodificado = Uri.EscapeDataString(mensaje);

        // 2. Construcción de la URL oficial de la API Click-to-Chat de WhatsApp
        var uriString = $"https://wa.me/{numeroDestino}?text={mensajeCodificado}";

        return await AbrirUriAsync(uriString);
    }

    /// <summary>
    /// Abre un chat directo de consulta rápida o saludo sin cotización previa.
    /// </summary>
    public async Task<bool> AbrirChatDirectoAsync(string? numeroTelefono = null)
    {
        const string saludoGeneral = "¡Hola Construcciones Durán! 👋 Me comunico desde su aplicación móvil para consultar sobre sus servicios de obras y cotizaciones.";
        return await EnviarMensajeAsync(saludoGeneral, numeroTelefono);
    }

    /// <summary>
    /// Invoca el lanzador nativo de la plataforma (Launcher.Default) con manejo de errores robusto.
    /// </summary>
    private static async Task<bool> AbrirUriAsync(string uriString)
    {
        try
        {
            if (!Uri.TryCreate(uriString, UriKind.Absolute, out var uri))
            {
                Debug.WriteLine($"[WhatsAppService] URI inválida: {uriString}");
                return false;
            }

            return await Launcher.Default.OpenAsync(uri);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[WhatsAppService Error]: No fue posible abrir WhatsApp. Detalle: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Limpia el número telefónico y asegura el prefijo internacional necesario para wa.me.
    /// Si recibe los 8 dígitos locales (ej: 77661522), añade el prefijo de Bolivia (591)
    /// para que WhatsApp reconozca el número correctamente.
    /// </summary>
    private static string SanitizarNumero(string numero)
    {
        var soloDigitos = new string(numero.Where(char.IsDigit).ToArray());

        // Si es número móvil boliviano de 8 dígitos (inicia con 7 o 6), anteponer 591
        if (soloDigitos.Length == 8 && (soloDigitos.StartsWith("7") || soloDigitos.StartsWith("6")))
        {
            return "591" + soloDigitos;
        }

        return soloDigitos;
    }
}
