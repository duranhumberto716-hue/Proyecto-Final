using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using Proyecto_Final.Models;
using Proyecto_Final.Services;

namespace Proyecto_Final.ViewModels;

/// <summary>
/// Lógica común de todos los CRUD (Crear, Leer, Actualizar, Eliminar) en formato tabla.
/// Incluye búsqueda, filas alternadas, formulario desplegable y exportación a CSV.
/// </summary>
public abstract class CrudViewModel<T> : BaseViewModel where T : EntidadBase, new()
{
    protected readonly DatabaseService Db;
    private List<T> _todos = new();
    private T _actual = new();
    private string _busqueda = string.Empty;
    private bool _formularioVisible;

    protected CrudViewModel(DatabaseService db)
    {
        Db = db;
        CargarCommand = new AsyncRelayCommand(CargarAsync);
        GuardarCommand = new AsyncRelayCommand(GuardarAsync);
        NuevoCommand = new RelayCommand(AbrirNuevo);
        CancelarCommand = new RelayCommand(CancelarEdicion);
        EditarCommand = new AsyncRelayCommand<T>(EditarAsync);
        EliminarCommand = new AsyncRelayCommand<T>(EliminarAsync);
        ExportarCsvCommand = new AsyncRelayCommand(ExportarCsvAsync);
        VerReporteCommand = new AsyncRelayCommand(VerReporteAsync);
        _actual = CrearNuevo();
    }

    public ObservableCollection<T> Items { get; } = new();

    /// <summary>Registro que se está creando o editando en el formulario.</summary>
    public T Actual
    {
        get => _actual;
        set
        {
            if (SetProperty(ref _actual, value))
            {
                OnPropertyChanged(nameof(EsEdicion));
                OnPropertyChanged(nameof(TextoBotonGuardar));
                OnPropertyChanged(nameof(TituloFormulario));
                AlCambiarActual();
            }
        }
    }

    public string Busqueda
    {
        get => _busqueda;
        set
        {
            if (SetProperty(ref _busqueda, value))
                AplicarFiltro();
        }
    }

    /// <summary>Controla si el panel del formulario (Agregar / Editar) está visible.</summary>
    public bool FormularioVisible
    {
        get => _formularioVisible;
        set => SetProperty(ref _formularioVisible, value);
    }

    public bool EsEdicion => Actual.Id != 0;
    public string TextoBotonGuardar => EsEdicion ? "Actualizar" : "Guardar";
    public string TituloFormulario => EsEdicion ? $"Editando registro #{Actual.Id}" : "Agregar nuevo registro";
    public int TotalRegistros => Items.Count;

    public ICommand CargarCommand { get; }
    public ICommand GuardarCommand { get; }
    public ICommand NuevoCommand { get; }
    public ICommand CancelarCommand { get; }
    public ICommand EditarCommand { get; }
    public ICommand EliminarCommand { get; }
    public ICommand ExportarCsvCommand { get; }
    public ICommand VerReporteCommand { get; }

    protected abstract string? Validar(T entidad);
    protected abstract bool Coincide(T entidad, string filtro);
    protected abstract string Describir(T entidad);
    protected abstract string RutaReporte { get; }
    protected abstract string NombreArchivoCsv { get; }
    protected abstract string EncabezadoCsv { get; }
    protected abstract string FormatearFilaCsv(T entidad);
    protected virtual T CrearNuevo() => new();
    protected virtual void Normalizar(T entidad) { }
    protected virtual void AlCambiarActual() { }
    protected virtual Task AntesDeCargarAsync() => Task.CompletedTask;
    protected virtual IEnumerable<T> Ordenar(IEnumerable<T> datos) => datos.OrderBy(x => x.Id);

    protected static string EscaparCsv(string? valor)
    {
        var texto = valor ?? string.Empty;
        if (texto.Contains(',') || texto.Contains('"') || texto.Contains('\n') || texto.Contains('\r'))
            return $"\"{texto.Replace("\"", "\"\"")}\"";
        return texto;
    }

    /// <summary>READ: lee todos los registros de la tabla.</summary>
    public async Task CargarAsync()
    {
        if (IsBusy) return;
        try
        {
            IsBusy = true;
            await AntesDeCargarAsync();
            _todos = await Db.ListarAsync<T>();
            AplicarFiltro();
        }
        catch (Exception ex)
        {
            await Dialogos.AlertaAsync("Base de datos", $"No se pudieron cargar los datos: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void AplicarFiltro()
    {
        var filtro = Busqueda?.Trim().ToLowerInvariant() ?? string.Empty;
        var datos = string.IsNullOrEmpty(filtro) ? _todos : _todos.Where(x => Coincide(x, filtro));

        Items.Clear();
        int indice = 0;
        foreach (var item in Ordenar(datos))
        {
            item.EsFilaAlterna = indice % 2 == 1;
            Items.Add(item);
            indice++;
        }

        OnPropertyChanged(nameof(TotalRegistros));
    }

    private void AbrirNuevo()
    {
        Actual = CrearNuevo();
        FormularioVisible = true;
    }

    private void CancelarEdicion()
    {
        Actual = CrearNuevo();
        FormularioVisible = false;
    }

    /// <summary>CREATE / UPDATE: valida y guarda el registro del formulario.</summary>
    private async Task GuardarAsync()
    {
        Normalizar(Actual);
        var error = Validar(Actual);
        if (error is not null)
        {
            await Dialogos.AlertaAsync("Validación", error);
            return;
        }

        try
        {
            var esNuevo = !EsEdicion;
            await Db.GuardarAsync(Actual);
            Actual = CrearNuevo();
            FormularioVisible = false;
            await CargarAsync();
            await Dialogos.AlertaAsync("Éxito", esNuevo ? "Registro creado correctamente." : "Registro actualizado correctamente.");
        }
        catch (SQLite.SQLiteException ex) when (ex.Message.Contains("UNIQUE", StringComparison.OrdinalIgnoreCase))
        {
            await Dialogos.AlertaAsync("Dato duplicado", "Ya existe un registro con ese identificador único. Verifique los datos.");
        }
        catch (Exception ex)
        {
            await Dialogos.AlertaAsync("Error", $"No se pudo guardar: {ex.Message}");
        }
    }

    /// <summary>Carga una copia del registro en el formulario para editarlo.</summary>
    private Task EditarAsync(T? entidad)
    {
        if (entidad is not null)
        {
            Actual = (T)entidad.Clonar();
            FormularioVisible = true;
        }
        return Task.CompletedTask;
    }

    /// <summary>DELETE: elimina el registro previa confirmación.</summary>
    private async Task EliminarAsync(T? entidad)
    {
        if (entidad is null) return;

        var confirmar = await Dialogos.ConfirmarAsync("Eliminar registro", $"¿Desea eliminar \"{Describir(entidad)}\"? Esta acción no se puede deshacer.");
        if (!confirmar) return;

        try
        {
            await Db.EliminarAsync(entidad);
            if (Actual.Id == entidad.Id)
            {
                Actual = CrearNuevo();
                FormularioVisible = false;
            }
            await CargarAsync();
        }
        catch (Exception ex)
        {
            await Dialogos.AlertaAsync("Error", $"No se pudo eliminar: {ex.Message}");
        }
    }

    /// <summary>Exporta los registros visibles en la tabla a un archivo CSV compatible con Excel.</summary>
    private async Task ExportarCsvAsync()
    {
        if (Items.Count == 0)
        {
            await Dialogos.AlertaAsync("Exportar CSV", "No hay registros para exportar.");
            return;
        }

        try
        {
            var sb = new StringBuilder();
            sb.AppendLine(EncabezadoCsv);
            foreach (var item in Items)
                sb.AppendLine(FormatearFilaCsv(item));

            var carpeta = DeviceInfo.Current.Platform == DevicePlatform.WinUI
                ? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                : FileSystem.CacheDirectory;

            if (string.IsNullOrWhiteSpace(carpeta))
                carpeta = FileSystem.CacheDirectory;

            var archivo = $"{NombreArchivoCsv}_{DateTime.Now:yyyyMMdd_HHmm}.csv";
            var ruta = Path.Combine(carpeta, archivo);

            await File.WriteAllTextAsync(ruta, sb.ToString(), new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));

            if (DeviceInfo.Current.Platform == DevicePlatform.WinUI)
            {
                await Dialogos.AlertaAsync("CSV exportado", $"Archivo guardado correctamente en:\n{ruta}");
            }
            else
            {
                await Share.Default.RequestAsync(new ShareFileRequest
                {
                    Title = $"Exportar {Title}",
                    File = new ShareFile(ruta)
                });
            }
        }
        catch (Exception ex)
        {
            await Dialogos.AlertaAsync("Exportar CSV", $"No se pudo exportar el archivo: {ex.Message}");
        }
    }

    private async Task VerReporteAsync()
    {
        if (!string.IsNullOrWhiteSpace(RutaReporte))
            await Shell.Current.GoToAsync(RutaReporte);
    }
}
