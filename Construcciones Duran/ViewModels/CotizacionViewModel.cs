using System.Windows.Input;
using Proyecto_Final.Models;
using Proyecto_Final.Services;

namespace Proyecto_Final.ViewModels;

public class CotizacionViewModel : BaseViewModel
{
    private readonly IWhatsAppService _whatsAppService;

    private string _nombre = string.Empty;
    private string _telefono = string.Empty;
    private string? _tipoProyectoSeleccionado;
    private string _metrosCuadradosTexto = string.Empty;
    private string _ubicacion = string.Empty;
    private string _descripcion = string.Empty;
    private decimal _estimadoReferencial;
    private string _estimadoReferencialFormateado = "$0.00 USD";

    public List<string> TiposProyecto { get; } = new()
    {
        "Construcción desde Cero",
        "Remodelación Integral",
        "Obra Gruesa",
        "Acabados y Fachadas",
        "Mantenimiento y Refacción"
    };

    #region Propiedades

    public string Nombre
    {
        get => _nombre;
        set => SetProperty(ref _nombre, value);
    }

    public string Telefono
    {
        get => _telefono;
        set => SetProperty(ref _telefono, value);
    }

    public string? TipoProyectoSeleccionado
    {
        get => _tipoProyectoSeleccionado;
        set
        {
            if (SetProperty(ref _tipoProyectoSeleccionado, value))
            {
                CalcularEstimadoReferencial();
            }
        }
    }

    public string MetrosCuadradosTexto
    {
        get => _metrosCuadradosTexto;
        set
        {
            if (SetProperty(ref _metrosCuadradosTexto, value))
            {
                CalcularEstimadoReferencial();
            }
        }
    }

    public string Ubicacion
    {
        get => _ubicacion;
        set => SetProperty(ref _ubicacion, value);
    }

    public string Descripcion
    {
        get => _descripcion;
        set => SetProperty(ref _descripcion, value);
    }

    public decimal EstimadoReferencial
    {
        get => _estimadoReferencial;
        private set => SetProperty(ref _estimadoReferencial, value);
    }

    public string EstimadoReferencialFormateado
    {
        get => _estimadoReferencialFormateado;
        private set => SetProperty(ref _estimadoReferencialFormateado, value);
    }

    #endregion

    #region Comandos

    public ICommand EnviarCotizacionCommand { get; }
    public ICommand VolverCommand { get; }

    #endregion

    public CotizacionViewModel(IWhatsAppService whatsAppService)
    {
        _whatsAppService = whatsAppService ?? throw new ArgumentNullException(nameof(whatsAppService));
        Title = "Cotizar Obra - Construcciones Durán";

        EnviarCotizacionCommand = new AsyncRelayCommand(EnviarCotizacionAsync);
        VolverCommand = new AsyncRelayCommand(async () => await Shell.Current.GoToAsync(".."));

        // Selección por defecto
        TipoProyectoSeleccionado = TiposProyecto.FirstOrDefault();
    }

    private void CalcularEstimadoReferencial()
    {
        if (!double.TryParse(MetrosCuadradosTexto?.Trim(), out var m2) || m2 <= 0)
        {
            EstimadoReferencial = 0;
            EstimadoReferencialFormateado = "$0.00 USD (Ingresa m² válidos)";
            return;
        }

        // Costo base referencial por m² en dólares
        decimal costoPorM2 = TipoProyectoSeleccionado switch
        {
            "Construcción desde Cero" => 480m,
            "Obra Gruesa" => 290m,
            "Remodelación Integral" => 230m,
            "Acabados y Fachadas" => 170m,
            "Mantenimiento y Refacción" => 95m,
            _ => 200m
        };

        EstimadoReferencial = (decimal)m2 * costoPorM2;
        EstimadoReferencialFormateado = $"~ ${EstimadoReferencial:N2} USD (Referencial)";
    }

    private async Task EnviarCotizacionAsync()
    {
        if (string.IsNullOrWhiteSpace(Nombre))
        {
            await MostrarAlertaAsync("Campo Requerido", "Por favor ingresa tu nombre completo.");
            return;
        }

        if (string.IsNullOrWhiteSpace(TipoProyectoSeleccionado))
        {
            await MostrarAlertaAsync("Campo Requerido", "Por favor selecciona el tipo de obra.");
            return;
        }

        if (!double.TryParse(MetrosCuadradosTexto?.Trim(), out var m2) || m2 <= 0)
        {
            await MostrarAlertaAsync("Dato Inválido", "Ingresa una cantidad válida de metros cuadrados (ej. 150).");
            return;
        }

        if (string.IsNullOrWhiteSpace(Ubicacion))
        {
            await MostrarAlertaAsync("Campo Requerido", "Por favor especifica la ubicación de la obra.");
            return;
        }

        var cotizacion = new Cotizacion
        {
            NombreCliente = Nombre,
            TelefonoCliente = Telefono,
            TipoProyecto = TipoProyectoSeleccionado,
            MetrosCuadrados = m2,
            Ubicacion = Ubicacion,
            Descripcion = string.IsNullOrWhiteSpace(Descripcion) ? "Sin observaciones adicionales." : Descripcion,
            PresupuestoEstimadoReferencial = EstimadoReferencial
        };

        try
        {
            IsBusy = true;
            var mensaje = cotizacion.GenerarMensajeWhatsApp();
            var exito = await _whatsAppService.EnviarMensajeAsync(mensaje);

            if (!exito)
            {
                await MostrarAlertaAsync("Aviso de WhatsApp",
                    "No se pudo abrir WhatsApp automáticamente en este dispositivo.\n\n" +
                    "Verifica que tengas la aplicación instalada o un navegador disponible.");
            }
        }
        catch (Exception ex)
        {
            await MostrarAlertaAsync("Error", $"Ocurrió una anomalía: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private static async Task MostrarAlertaAsync(string titulo, string mensaje)
    {
        if (Application.Current?.Windows.FirstOrDefault()?.Page is Page page)
        {
            await page.DisplayAlertAsync(titulo, mensaje, "Entendido");
        }
        else if (Shell.Current is not null)
        {
            await Shell.Current.DisplayAlertAsync(titulo, mensaje, "Entendido");
        }
    }
}

