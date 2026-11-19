namespace Proyecto_Final.Models;

/// <summary>
/// Modelo de datos para la solicitud de cotización que ingresa el cliente.
/// </summary>
public class Cotizacion
{
    public string NombreCliente { get; set; } = string.Empty;
    public string TelefonoCliente { get; set; } = string.Empty;
    public string TipoProyecto { get; set; } = string.Empty;
    public double MetrosCuadrados { get; set; }
    public string Ubicacion { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public decimal PresupuestoEstimadoReferencial { get; set; }
    public DateTime FechaSolicitud { get; set; } = DateTime.Now;

    /// <summary>
    /// Genera el texto estructurado y formal listo para ser enviado al WhatsApp del contratista.
    /// </summary>
    public string GenerarMensajeWhatsApp()
    {
        return $"""
                🏗️ *SOLICITUD DE COTIZACIÓN - CONSTRUCCIONES DURAN* 🏗️
                ━━━━━━━━━━━━━━━━━━━━━
                👤 *Cliente:* {NombreCliente.Trim()}
                📞 *Teléfono:* {(string.IsNullOrWhiteSpace(TelefonoCliente) ? "No especificado" : TelefonoCliente.Trim())}
                📍 *Ubicación de la Obra:* {Ubicacion.Trim()}
                🛠️ *Tipo de Proyecto:* {TipoProyecto.Trim()}
                📐 *Área Aproximada:* {MetrosCuadrados:N0} m²
                💰 *Estimado Referencial:* ${(PresupuestoEstimadoReferencial > 0 ? PresupuestoEstimadoReferencial.ToString("N2") : "A evaluar en sitio")} USD
                
                📝 *Detalles y Requerimientos:*
                {Descripcion.Trim()}
                ━━━━━━━━━━━━━━━━━━━━━
                _Enviado desde la App Oficial de Construcciones Duran_
                """;
    }
}

