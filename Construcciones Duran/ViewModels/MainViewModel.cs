using System.Collections.ObjectModel;
using System.Windows.Input;
using Proyecto_Final.Models;
using Proyecto_Final.Services;

namespace Proyecto_Final.ViewModels;

public class MainViewModel : BaseViewModel
{
    private readonly IProyectoService _proyectoService;
    private readonly IWhatsAppService _whatsAppService;

    public ObservableCollection<Proyecto> Proyectos { get; } = new();

    #region Comandos

    public ICommand CargarProyectosCommand { get; }
    public ICommand NavegarACotizarCommand { get; }
    public ICommand ContactarWhatsAppDirectoCommand { get; }

    #endregion

    public MainViewModel(IProyectoService proyectoService, IWhatsAppService whatsAppService)
    {
        _proyectoService = proyectoService ?? throw new ArgumentNullException(nameof(proyectoService));
        _whatsAppService = whatsAppService ?? throw new ArgumentNullException(nameof(whatsAppService));

        Title = "Inicio - Construcciones Durán";

        CargarProyectosCommand = new AsyncRelayCommand(CargarProyectosAsync);
        NavegarACotizarCommand = new AsyncRelayCommand(NavegarACotizarAsync);
        ContactarWhatsAppDirectoCommand = new AsyncRelayCommand(ContactarDirectoAsync);
    }

    /// <summary>
    /// Carga las obras desde el servicio en la nube (CDN).
    /// </summary>
    public async Task CargarProyectosAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            var obras = await _proyectoService.ObtenerProyectosAsync();

            Proyectos.Clear();
            foreach (var obra in obras)
            {
                Proyectos.Add(obra);
            }
        }
        catch (Exception ex)
        {
            await MostrarAlertaAsync("Catálogo", $"No fue posible sincronizar el catálogo de obras: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Navega a la pantalla dedicada de cotización de obra.
    /// </summary>
    private async Task NavegarACotizarAsync()
    {
        try
        {
            await Shell.Current.GoToAsync(nameof(CotizacionPage));
        }
        catch (Exception ex)
        {
            await MostrarAlertaAsync("Navegación", $"Error al abrir la pantalla de cotización: {ex.Message}");
        }
    }

    /// <summary>
    /// Abre el chat directo con el contratista por WhatsApp (77661522).
    /// </summary>
    private async Task ContactarDirectoAsync()
    {
        try
        {
            var exito = await _whatsAppService.AbrirChatDirectoAsync();
            if (!exito)
            {
                await MostrarAlertaAsync("WhatsApp", "No fue posible abrir WhatsApp. Verifica que la aplicación esté instalada.");
            }
        }
        catch (Exception ex)
        {
            await MostrarAlertaAsync("Error", ex.Message);
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
