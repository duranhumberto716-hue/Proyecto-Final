using System.Windows.Input;
using Proyecto_Final.Services;

namespace Proyecto_Final.ViewModels;

/// <summary>Registro de nuevos usuarios del sistema.</summary>
public class RegistroViewModel : BaseViewModel
{
    private readonly DatabaseService _db;
    private string _nombreCompleto = string.Empty;
    private string _usuario = string.Empty;
    private string _contrasena = string.Empty;
    private string _confirmacion = string.Empty;

    public RegistroViewModel(DatabaseService db)
    {
        _db = db;
        Title = "Crear cuenta";
        RegistrarCommand = new AsyncRelayCommand(RegistrarAsync);
        VolverCommand = new AsyncRelayCommand(() => Shell.Current.GoToAsync(".."));
    }

    public string NombreCompleto { get => _nombreCompleto; set => SetProperty(ref _nombreCompleto, value); }
    public string Usuario { get => _usuario; set => SetProperty(ref _usuario, value); }
    public string Contrasena { get => _contrasena; set => SetProperty(ref _contrasena, value); }
    public string Confirmacion { get => _confirmacion; set => SetProperty(ref _confirmacion, value); }

    public ICommand RegistrarCommand { get; }
    public ICommand VolverCommand { get; }

    private async Task RegistrarAsync()
    {
        var error = Validador.ValidarRegistro(NombreCompleto, Usuario, Contrasena, Confirmacion);
        if (error is not null)
        {
            await Dialogos.AlertaAsync("Validación", error);
            return;
        }

        try
        {
            IsBusy = true;
            if (await _db.ExisteUsuarioAsync(Usuario))
            {
                await Dialogos.AlertaAsync("Registro", "Ese nombre de usuario ya está en uso.");
                return;
            }

            await _db.RegistrarUsuarioAsync(NombreCompleto, Usuario, Contrasena);
            await Dialogos.AlertaAsync("Registro", "Cuenta creada correctamente. Ya puede iniciar sesión.");
            await Shell.Current.GoToAsync("..");
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

