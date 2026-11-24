using System.Windows.Input;
using Proyecto_Final.Services;

namespace Proyecto_Final.ViewModels;

/// <summary>Registro amigable de nuevos clientes del sistema.</summary>
public class RegistroViewModel : BaseViewModel
{
    private readonly DatabaseService _db;
    private string _nombreCompleto = string.Empty;
    private string _usuario = string.Empty;
    private string _contrasena = string.Empty;
    private string _confirmacion = string.Empty;
    private bool _ocultarContrasena = true;
    private string _mensajeError = string.Empty;

    public RegistroViewModel(DatabaseService db)
    {
        _db = db;
        Title = "Crear cuenta";
        RegistrarCommand = new AsyncRelayCommand(RegistrarAsync);
        VolverCommand = new AsyncRelayCommand(() => Shell.Current.GoToAsync(".."));
        AlternarContrasenaCommand = new RelayCommand(() => OcultarContrasena = !OcultarContrasena);
    }

    public string NombreCompleto { get => _nombreCompleto; set { if (SetProperty(ref _nombreCompleto, value)) MensajeError = string.Empty; } }
    public string Usuario { get => _usuario; set { if (SetProperty(ref _usuario, value)) MensajeError = string.Empty; } }
    public string Contrasena { get => _contrasena; set { if (SetProperty(ref _contrasena, value)) MensajeError = string.Empty; } }
    public string Confirmacion { get => _confirmacion; set { if (SetProperty(ref _confirmacion, value)) MensajeError = string.Empty; } }

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

    public ICommand RegistrarCommand { get; }
    public ICommand VolverCommand { get; }
    public ICommand AlternarContrasenaCommand { get; }

    private async Task RegistrarAsync()
    {
        MensajeError = string.Empty;

        var error = Validador.ValidarRegistro(NombreCompleto, Usuario, Contrasena, Confirmacion);
        if (error is not null)
        {
            MensajeError = error;
            return;
        }

        try
        {
            IsBusy = true;
            if (await _db.ExisteUsuarioAsync(Usuario))
            {
                MensajeError = "Ese nombre de usuario ya está registrado. Elige otro.";
                return;
            }

            await _db.RegistrarUsuarioAsync(NombreCompleto, Usuario, Contrasena);
            await Dialogos.AlertaAsync("Cuenta creada", "Tu cuenta de Cliente fue creada correctamente. Ya puedes iniciar sesión.");
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            MensajeError = $"No fue posible completar el registro: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }
}
