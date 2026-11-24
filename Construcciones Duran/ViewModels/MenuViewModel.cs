using System.Collections.ObjectModel;
using System.Windows.Input;
using Proyecto_Final.Models;
using Proyecto_Final.Services;

namespace Proyecto_Final.ViewModels;

/// <summary>
/// Menú principal / Dashboard minimalista separado por rol (Administrador, Trabajador, Cliente).
/// - Administrador y Trabajador: indicadores en vivo desde SQLite y gestión estructurada.
/// - Cliente: acceso inmediato al portafolio de obras realizadas, información detallada,
///   consulta directa por obra (WhatsApp) y cotización, sin apartado ODS ni administración.
/// </summary>
public class MenuViewModel : BaseViewModel
{
    private readonly SesionService _sesion;
    private readonly DatabaseService _db;
    private readonly IWhatsAppService _whatsApp;
    private readonly IProyectoService _proyectoService;

    private string _kpiObrasActivas = "-";
    private string _kpiPersonalActivo = "-";
    private string _kpiStockBajo = "-";
    private string _kpiAvancePromedio = "-";
    private bool _tieneAlertaStock;

    private string _resumenObras = "Cargando...";
    private string _resumenPersonal = "Cargando...";
    private string _resumenMateriales = "Cargando...";
    private string _resumenAvances = "Cargando...";

    public MenuViewModel(
        SesionService sesion,
        DatabaseService db,
        IWhatsAppService whatsApp,
        IProyectoService proyectoService)
    {
        _sesion = sesion;
        _db = db;
        _whatsApp = whatsApp;
        _proyectoService = proyectoService;
        Title = "Inicio";

        NavegarCommand = new AsyncRelayCommand<string>(NavegarAsync);
        CerrarSesionCommand = new AsyncRelayCommand(CerrarSesionAsync);
        ContactarWhatsAppCommand = new AsyncRelayCommand(ContactarWhatsAppAsync);
        PedirInfoObraCommand = new AsyncRelayCommand<Proyecto>(PedirInfoObraAsync);
    }

    public ObservableCollection<Proyecto> ObrasRealizadas { get; } = new();

    public string Saludo => $"Hola, {ObtenerPrimerNombre(_sesion.UsuarioActual?.NombreCompleto)}";
    public string NombreCompletoUsuario => _sesion.UsuarioActual?.NombreCompleto ?? "Usuario";
    public string RolEtiqueta => _sesion.RolActual.ToUpperInvariant();

    public string SubtituloPanel => _sesion.EsAdministrador
        ? "Control general de obras, personal, materiales y avances"
        : _sesion.EsTrabajador
            ? "Gestión operativa de avances e inventario en obra"
            : "Obras realizadas, información y cotizaciones";

    public bool EsAdministrador => _sesion.EsAdministrador;
    public bool EsTrabajador => _sesion.EsTrabajador;
    public bool EsCliente => _sesion.EsCliente;

    // Indicadores KPI en vivo (Admin / Trabajador)
    public string KpiObrasActivas { get => _kpiObrasActivas; private set => SetProperty(ref _kpiObrasActivas, value); }
    public string KpiPersonalActivo { get => _kpiPersonalActivo; private set => SetProperty(ref _kpiPersonalActivo, value); }
    public string KpiStockBajo { get => _kpiStockBajo; private set => SetProperty(ref _kpiStockBajo, value); }
    public string KpiAvancePromedio { get => _kpiAvancePromedio; private set => SetProperty(ref _kpiAvancePromedio, value); }
    public bool TieneAlertaStock { get => _tieneAlertaStock; private set => SetProperty(ref _tieneAlertaStock, value); }

    // Subtítulos dinámicos por módulo
    public string ResumenObras { get => _resumenObras; private set => SetProperty(ref _resumenObras, value); }
    public string ResumenPersonal { get => _resumenPersonal; private set => SetProperty(ref _resumenPersonal, value); }
    public string ResumenMateriales { get => _resumenMateriales; private set => SetProperty(ref _resumenMateriales, value); }
    public string ResumenAvances { get => _resumenAvances; private set => SetProperty(ref _resumenAvances, value); }

    public ICommand NavegarCommand { get; }
    public ICommand CerrarSesionCommand { get; }
    public ICommand ContactarWhatsAppCommand { get; }
    public ICommand PedirInfoObraCommand { get; }

    /// <summary>Actualiza la identidad del usuario y carga el contenido correspondiente a su rol.</summary>
    public async Task RefrescarAsync()
    {
        OnPropertyChanged(nameof(Saludo));
        OnPropertyChanged(nameof(NombreCompletoUsuario));
        OnPropertyChanged(nameof(RolEtiqueta));
        OnPropertyChanged(nameof(SubtituloPanel));
        OnPropertyChanged(nameof(EsAdministrador));
        OnPropertyChanged(nameof(EsTrabajador));
        OnPropertyChanged(nameof(EsCliente));

        if (EsCliente)
        {
            await CargarPortafolioClienteAsync();
            return;
        }

        try
        {
            var obras = await _db.ListarAsync<Obra>();
            var materiales = await _db.ListarAsync<Material>();
            var avances = await _db.ListarAsync<Avance>();

            var activas = obras.Count(o => o.EnEjecucion);
            KpiObrasActivas = $"{activas}/{obras.Count}";
            ResumenObras = $"{obras.Count} registradas • {activas} en ejecución";

            var alertas = materiales.Count(m => m.StockBajo);
            TieneAlertaStock = alertas > 0;
            KpiStockBajo = alertas.ToString();
            ResumenMateriales = alertas > 0
                ? $"{materiales.Count} ítems • {alertas} con stock bajo"
                : $"{materiales.Count} ítems • Stock al día";

            var ultimosAvances = avances
                .GroupBy(a => a.NombreObra)
                .Select(g => g.OrderByDescending(a => a.Fecha).ThenByDescending(a => a.Porcentaje).First())
                .ToList();
            var promedio = ultimosAvances.Count == 0 ? 0 : ultimosAvances.Average(a => a.Porcentaje);
            KpiAvancePromedio = $"{promedio:N0}%";
            ResumenAvances = $"{avances.Count} registros • {promedio:N0}% promedio";

            if (EsAdministrador)
            {
                var empleados = await _db.ListarAsync<Empleado>();
                var activos = empleados.Count(e => e.Activo);
                KpiPersonalActivo = activos.ToString();
                ResumenPersonal = $"{activos} activos de {empleados.Count} registrados";
            }
        }
        catch
        {
            // No bloquear el menú si la consulta rápida falla
        }
    }

    private async Task CargarPortafolioClienteAsync()
    {
        if (ObrasRealizadas.Count > 0)
            return;

        try
        {
            IsBusy = true;
            var proyectos = await _proyectoService.ObtenerProyectosAsync();
            ObrasRealizadas.Clear();
            foreach (var p in proyectos)
                ObrasRealizadas.Add(p);
        }
        catch
        {
            // Si falla la carga, mantener la vista operativa
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task PedirInfoObraAsync(Proyecto? obra)
    {
        if (obra is null) return;

        var cliente = _sesion.UsuarioActual?.NombreCompleto ?? "Cliente";
        var mensaje =
            $"Hola Construcciones Durán, soy {cliente}. " +
            $"Quisiera solicitar más información y costos referenciales sobre la obra \"{obra.Titulo}\" " +
            $"({obra.Categoria}, {obra.MetrosCuadrados:N0} m², tiempo referencial: {obra.TiempoEjecucion}).";

        var exito = await _whatsApp.EnviarMensajeAsync(mensaje);
        if (!exito)
            await Dialogos.AlertaAsync("WhatsApp", "No fue posible abrir WhatsApp en este dispositivo.");
    }

    private static string ObtenerPrimerNombre(string? nombreCompleto)
    {
        if (string.IsNullOrWhiteSpace(nombreCompleto))
            return "Usuario";
        var partes = nombreCompleto.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return partes.Length > 0 ? partes[0] : nombreCompleto.Trim();
    }

    private async Task NavegarAsync(string? ruta)
    {
        if (string.IsNullOrWhiteSpace(ruta)) return;

        if (!_sesion.PuedeAccederARuta(ruta))
        {
            await Dialogos.AlertaAsync("Acceso restringido", "Su rol no tiene permisos para ingresar a este apartado.");
            return;
        }

        try
        {
            await Shell.Current.GoToAsync(ruta);
        }
        catch (Exception ex)
        {
            await Dialogos.AlertaAsync("Navegación", ex.Message);
        }
    }

    private async Task ContactarWhatsAppAsync()
    {
        var exito = await _whatsApp.AbrirChatDirectoAsync();
        if (!exito)
            await Dialogos.AlertaAsync("WhatsApp", "No fue posible abrir WhatsApp en este dispositivo.");
    }

    private async Task CerrarSesionAsync()
    {
        if (!await Dialogos.ConfirmarAsync("Cerrar sesión", "¿Desea salir de su cuenta?"))
            return;

        _sesion.Cerrar();
        await Shell.Current.GoToAsync("//LoginPage");
    }
}
