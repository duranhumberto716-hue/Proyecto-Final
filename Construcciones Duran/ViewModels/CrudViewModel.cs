using System.Collections.ObjectModel;
using System.Windows.Input;
using Proyecto_Final.Models;
using Proyecto_Final.Services;

namespace Proyecto_Final.ViewModels;

/// <summary>
/// Lógica común de todos los CRUD (Crear, Leer, Actualizar, Eliminar).
/// Cada CRUD concreto solo define cómo validar, buscar y describir su entidad.
/// </summary>
public abstract class CrudViewModel<T> : BaseViewModel where T : EntidadBase, new()
{
    protected readonly DatabaseService Db;
    private List<T> _todos = new();
    private T _actual = new();
    private string _busqueda = string.Empty;

    protected CrudViewModel(DatabaseService db)
    {
        Db = db;
        CargarCommand = new AsyncRelayCommand(CargarAsync);
        GuardarCommand = new AsyncRelayCommand(GuardarAsync);
        NuevoCommand = new RelayCommand(() => Actual = CrearNuevo());
        EditarCommand = new AsyncRelayCommand<T>(EditarAsync);
        EliminarCommand = new AsyncRelayCommand<T>(EliminarAsync);
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

    public bool EsEdicion => Actual.Id != 0;
    public string TextoBotonGuardar => EsEdicion ? "💾 Actualizar" : "➕ Registrar";
    public string TituloFormulario => EsEdicion ? $"✏️ Editando registro #{Actual.Id}" : "📝 Nuevo registro";
    public int TotalRegistros => Items.Count;

    public ICommand CargarCommand { get; }
    public ICommand GuardarCommand { get; }
    public ICommand NuevoCommand { get; }
    public ICommand EditarCommand { get; }
    public ICommand EliminarCommand { get; }

    protected abstract string? Validar(T entidad);
    protected abstract bool Coincide(T entidad, string filtro);
    protected abstract string Describir(T entidad);
    protected virtual T CrearNuevo() => new();
    protected virtual void Normalizar(T entidad) { }
    protected virtual void AlCambiarActual() { }
    protected virtual IEnumerable<T> Ordenar(IEnumerable<T> datos) => datos.OrderByDescending(x => x.Id);

    /// <summary>READ: lee todos los registros de la tabla.</summary>
    public async Task CargarAsync()
    {
        if (IsBusy) return;
        try
        {
            IsBusy = true;
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
        foreach (var item in Ordenar(datos))
            Items.Add(item);

        OnPropertyChanged(nameof(TotalRegistros));
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
            await Dialogos.AlertaAsync("Éxito", esNuevo ? "Registro creado correctamente." : "Registro actualizado correctamente.");
            Actual = CrearNuevo();
            await CargarAsync();
        }
        catch (SQLite.SQLiteException ex) when (ex.Message.Contains("UNIQUE", StringComparison.OrdinalIgnoreCase))
        {
            await Dialogos.AlertaAsync("Dato duplicado", "Ya existe un registro con ese CI. Verifique los datos.");
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
            Actual = (T)entidad.Clonar();
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
                Actual = CrearNuevo();
            await CargarAsync();
        }
        catch (Exception ex)
        {
            await Dialogos.AlertaAsync("Error", $"No se pudo eliminar: {ex.Message}");
        }
    }
}

