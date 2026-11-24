using System.Windows.Input;
using Proyecto_Final.Services;

namespace Proyecto_Final.ViewModels;

/// <summary>Formulario de inicio de sesión.</summary>
public class LoginViewModel : BaseViewModel
{
    private readonly DatabaseService _db;
    private readonly SesionService _sesion;
    private string _usuario = string.Empty;
    private string _contrasena = string.Empty;

    public LoginViewModel(DatabaseService db, SesionService sesion)
    {
        _db = db;
        _sesion = sesion;
        Title = "Iniciar sesión";
        IniciarSesionCommand = new AsyncRelayCommand(IniciarSesionAsync);
        IrARegistroCommand = new AsyncRelayCommand(() => Shell.Current.GoToAsync(nameof(Views.RegistroPage)));
    }

    public string Usuario
    {
        get => _usuario;
        set => SetProperty(ref _usuario, value);
    }

    public string Contrasena
    {
        get => _contrasena;
        set => SetProperty(ref _contrasena, value);
    }

    public ICommand IniciarSesionCommand { get; }
    public ICommand IrARegistroCommand { get; }

    private async Task IniciarSesionAsync()
    {
        if (string.IsNullOrWhiteSpace(Usuario) || string.IsNullOrEmpty(Contrasena))
        {
            await Dialogos.AlertaAsync("Inicio de sesión", "Ingrese su usuario y contraseña.");
            return;
        }

        try
        {
            IsBusy = true;
            var usuario = await _db.AutenticarAsync(Usuario, Contrasena);
            if (usuario is null)
            {
                await Dialogos.AlertaAsync("Inicio de sesión", "Usuario o contraseña incorrectos.");
                return;
            }

            _sesion.Iniciar(usuario);
            Contrasena = string.Empty;
            await Shell.Current.GoToAsync("//MenuPage");
        }
        catch (Exception ex)
        {
            await Dialogos.AlertaAsync("Error", ex.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }
}

