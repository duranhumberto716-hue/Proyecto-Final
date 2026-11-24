using System.Windows.Input;
using Proyecto_Final.Services;

namespace Proyecto_Final.ViewModels;

/// <summary>Formulario de inicio de sesión amigable con validación inline y vista de contraseña.</summary>
public class LoginViewModel : BaseViewModel
{
    private readonly DatabaseService _db;
    private readonly SesionService _sesion;
    private string _usuario = string.Empty;
    private string _contrasena = string.Empty;
    private bool _ocultarContrasena = true;
    private string _mensajeError = string.Empty;

    public LoginViewModel(DatabaseService db, SesionService sesion)
    {
        _db = db;
        _sesion = sesion;
        Title = "Iniciar sesión";
        IniciarSesionCommand = new AsyncRelayCommand(IniciarSesionAsync);
        IrARegistroCommand = new AsyncRelayCommand(() => Shell.Current.GoToAsync(nameof(Views.RegistroPage)));
        AlternarContrasenaCommand = new RelayCommand(() => OcultarContrasena = !OcultarContrasena);
        AutocompletarDemoCommand = new RelayCommand(p => AutocompletarDemo(p?.ToString()));
    }

    public string Usuario
    {
        get => _usuario;
        set
        {
            if (SetProperty(ref _usuario, value))
                MensajeError = string.Empty;
        }
    }

    public string Contrasena
    {
        get => _contrasena;
        set
        {
            if (SetProperty(ref _contrasena, value))
                MensajeError = string.Empty;
        }
    }

    public bool OcultarContrasena
    {
        get => _ocultarContrasena;
        set
        {
            if (SetProperty(ref _ocultarContrasena, value))
                OnPropertyChanged(nameof(TextoVerContrasena));
        }
    }

    public string TextoVerContrasena => OcultarContrasena ? "Ver" : "Ocultar";

    public string MensajeError
    {
        get => _mensajeError;
        set
        {
            if (SetProperty(ref _mensajeError, value))
                OnPropertyChanged(nameof(HayError));
        }
    }

    public bool HayError => !string.IsNullOrWhiteSpace(MensajeError);

    public ICommand IniciarSesionCommand { get; }
    public ICommand IrARegistroCommand { get; }
    public ICommand AlternarContrasenaCommand { get; }
    public ICommand AutocompletarDemoCommand { get; }

    private void AutocompletarDemo(string? rol)
    {
        MensajeError = string.Empty;
        switch (rol)
        {
            case "admin":
                Usuario = "admin";
                Contrasena = "admin123";
                break;
            case "trabajador":
                Usuario = "trabajador";
                Contrasena = "trab123";
                break;
            case "cliente":
                Usuario = "cliente";
                Contrasena = "cli123";
                break;
        }
    }

    private async Task IniciarSesionAsync()
    {
        MensajeError = string.Empty;

        if (string.IsNullOrWhiteSpace(Usuario) || string.IsNullOrEmpty(Contrasena))
        {
            MensajeError = "Ingresa tu usuario y contraseña para continuar.";
            return;
        }

        try
        {
            IsBusy = true;
            var usuario = await _db.AutenticarAsync(Usuario, Contrasena);
            if (usuario is null)
            {
                MensajeError = "Usuario o contraseña incorrectos. Verifica tus datos e intenta nuevamente.";
                return;
            }

            _sesion.Iniciar(usuario);
            Contrasena = string.Empty;
            MensajeError = string.Empty;
            await Shell.Current.GoToAsync("//MenuPage");
        }
        catch (Exception ex)
        {
            MensajeError = $"No fue posible iniciar sesión: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }
}
