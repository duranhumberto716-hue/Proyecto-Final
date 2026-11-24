using System.Windows.Input;
using Proyecto_Final.Services;

namespace Proyecto_Final.ViewModels;

/// <summary>Menú principal del sistema.</summary>
public class MenuViewModel : BaseViewModel
{
    private readonly SesionService _sesion;

    public MenuViewModel(SesionService sesion)
    {
        _sesion = sesion;
        Title = "Menú principal";
        NavegarCommand = new AsyncRelayCommand<string>(NavegarAsync);
        CerrarSesionCommand = new AsyncRelayCommand(CerrarSesionAsync);
    }

    public string Bienvenida => $"¡Hola, {_sesion.UsuarioActual?.NombreCompleto ?? "usuario"}!";
    public string RolUsuario => $"Rol: {_sesion.UsuarioActual?.Rol ?? "-"}";

    public ICommand NavegarCommand { get; }
    public ICommand CerrarSesionCommand { get; }

    /// <summary>Se llama al mostrar la página para refrescar el nombre del usuario.</summary>
    public void Refrescar()
    {
        OnPropertyChanged(nameof(Bienvenida));
        OnPropertyChanged(nameof(RolUsuario));
    }

    private async Task NavegarAsync(string? ruta)
    {
        if (string.IsNullOrWhiteSpace(ruta)) return;
        try
        {
            await Shell.Current.GoToAsync(ruta);
        }
        catch (Exception ex)
        {
            await Dialogos.AlertaAsync("Navegación", ex.Message);
        }
    }

    private async Task CerrarSesionAsync()
    {
        if (!await Dialogos.ConfirmarAsync("Cerrar sesión", "¿Desea salir del sistema?"))
            return;

        _sesion.Cerrar();
        await Shell.Current.GoToAsync("//LoginPage");
    }
}

