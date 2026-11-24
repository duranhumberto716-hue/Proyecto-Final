using System.Collections.ObjectModel;
using System.Windows.Input;
using Proyecto_Final.Models;
using Proyecto_Final.Services;

namespace Proyecto_Final.ViewModels;

/// <summary>
/// Base de los formularios de información (reportes). Cada integrante
/// implementa su propio reporte heredando de esta clase.
/// </summary>
public abstract class ReporteViewModel : BaseViewModel
{
    protected readonly DatabaseService Db;
    private string _descripcion = string.Empty;
    private string _tituloLista = string.Empty;
    private string _notaPie = string.Empty;

    protected ReporteViewModel(DatabaseService db)
    {
        Db = db;
        ActualizarCommand = new AsyncRelayCommand(ActualizarAsync);
    }

    public ObservableCollection<Indicador> Indicadores { get; } = new();
    public ObservableCollection<FilaReporte> Filas { get; } = new();

    public string Descripcion { get => _descripcion; set => SetProperty(ref _descripcion, value); }
    public string TituloLista { get => _tituloLista; set => SetProperty(ref _tituloLista, value); }
    public string NotaPie { get => _notaPie; set => SetProperty(ref _notaPie, value); }
    public string FechaGeneracion => $"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}";

    public ICommand ActualizarCommand { get; }

    public async Task ActualizarAsync()
    {
        if (IsBusy) return;
        try
        {
            IsBusy = true;
            Indicadores.Clear();
            Filas.Clear();
            await GenerarAsync();
            OnPropertyChanged(nameof(FechaGeneracion));
        }
        catch (Exception ex)
        {
            await Dialogos.AlertaAsync("Reporte", $"No se pudo generar el reporte: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    protected abstract Task GenerarAsync();

    protected void AgregarIndicador(string etiqueta, string titulo, string valor) =>
        Indicadores.Add(new Indicador { Etiqueta = etiqueta, Titulo = titulo, Valor = valor });

    protected void AgregarFila(string titulo, string detalle, string valor, bool destacado = false) =>
        Filas.Add(new FilaReporte { Titulo = titulo, Detalle = detalle, Valor = valor, Destacado = destacado });
}

/// <summary>Información de obras (Integrante 1).</summary>
public class ReporteObrasViewModel : ReporteViewModel
{
    public ReporteObrasViewModel(DatabaseService db) : base(db)
    {
        Title = "Reporte de Obras";
        Descripcion = "Resumen general de obras registradas, estados y presupuesto comprometido.";
        TituloLista = "Obras por estado y presupuesto";
    }

    protected override async Task GenerarAsync()
    {
        var obras = await Db.ListarAsync<Obra>();
        var enEjecucion = obras.Count(o => o.EnEjecucion);
        var finalizadas = obras.Count(o => string.Equals(o.Estado, "Finalizada", StringComparison.OrdinalIgnoreCase));
        var presupuestoTotal = obras.Sum(o => o.Presupuesto);

        AgregarIndicador("OBRAS", "Total de obras", obras.Count.ToString());
        AgregarIndicador("EJEC", "En ejecución", enEjecucion.ToString());
        AgregarIndicador("FIN", "Finalizadas", finalizadas.ToString());
        AgregarIndicador("PRESUP", "Presupuesto total", $"Bs {presupuestoTotal:N2}");

        foreach (var grupo in obras.GroupBy(o => o.Estado).OrderByDescending(g => g.Sum(o => o.Presupuesto)))
        {
            var monto = grupo.Sum(o => o.Presupuesto);
            AgregarFila(grupo.Key,
                $"{grupo.Count()} obra(s)  •  Promedio Bs {grupo.Average(o => o.Presupuesto):N2}",
                $"Bs {monto:N2}",
                string.Equals(grupo.Key, "En ejecución", StringComparison.OrdinalIgnoreCase));
        }

        foreach (var obra in obras.OrderByDescending(o => o.Presupuesto).Take(5))
        {
            AgregarFila(obra.Nombre,
                $"{obra.Cliente}  •  {obra.Estado}",
                $"Bs {obra.Presupuesto:N2}");
        }

        NotaPie = "Las obras resaltadas se encuentran actualmente en ejecución.";
    }
}

/// <summary>Información del personal (Integrante 2).</summary>
public class ReportePersonalViewModel : ReporteViewModel
{
    public ReportePersonalViewModel(DatabaseService db) : base(db)
    {
        Title = "Reporte de Personal";
        Descripcion = "Planilla mensual y distribución del personal de obra por cargo.";
        TituloLista = "Personal activo por cargo";
    }

    protected override async Task GenerarAsync()
    {
        var empleados = await Db.ListarAsync<Empleado>();
        var activos = empleados.Where(e => e.Activo).ToList();
        var planilla = activos.Sum(e => e.SalarioMensual);
        var antiguedad = activos.Count == 0 ? 0 : activos.Average(e => (DateTime.Today - e.FechaIngreso).TotalDays / 365.25);

        AgregarIndicador("TOTAL", "Total personal", empleados.Count.ToString());
        AgregarIndicador("ACTIVO", "Activos", activos.Count.ToString());
        AgregarIndicador("PLANILLA", "Planilla mensual", $"Bs {planilla:N2}");
        AgregarIndicador("TIEMPO", "Antigüedad prom.", $"{antiguedad:N1} años");

        foreach (var grupo in activos.GroupBy(e => e.Cargo).OrderByDescending(g => g.Sum(e => e.SalarioMensual)))
        {
            AgregarFila(grupo.Key,
                $"{grupo.Count()} persona(s)  •  Promedio Bs {grupo.Average(e => e.SalarioMensual):N2}",
                $"Bs {grupo.Sum(e => e.SalarioMensual):N2}");
        }

        var inactivos = empleados.Count - activos.Count;
        NotaPie = inactivos > 0
            ? $"Hay {inactivos} empleado(s) inactivo(s) que no se incluyen en la planilla."
            : "Todo el personal registrado se encuentra activo.";
    }
}

/// <summary>
/// Información de materiales (Integrante 3). Usa la API externa de tipo de cambio
/// para mostrar el valor del inventario en dólares.
/// </summary>
public class ReporteInventarioViewModel : ReporteViewModel
{
    private readonly TipoCambioService _tipoCambio;

    public ReporteInventarioViewModel(DatabaseService db, TipoCambioService tipoCambio) : base(db)
    {
        _tipoCambio = tipoCambio;
        Title = "Reporte de Materiales";
        Descripcion = "Valorización del inventario de materiales en Bs y USD (tipo de cambio desde API externa).";
        TituloLista = "Valor por categoría y alertas de stock";
    }

    protected override async Task GenerarAsync()
    {
        var materiales = await Db.ListarAsync<Material>();
        var (bsPorDolar, enLinea) = await _tipoCambio.ObtenerBsPorDolarAsync();
        var valorBs = materiales.Sum(m => m.ValorTotal);
        var stockBajo = materiales.Where(m => m.StockBajo).ToList();

        AgregarIndicador("ITEMS", "Materiales", materiales.Count.ToString());
        AgregarIndicador("BS", "Valor total (Bs)", $"Bs {valorBs:N2}");
        AgregarIndicador("USD", "Valor total (USD)", $"$ {valorBs / bsPorDolar:N2}");
        AgregarIndicador("ALERTA", "Con stock bajo", stockBajo.Count.ToString());

        foreach (var grupo in materiales.GroupBy(m => m.Categoria).OrderByDescending(g => g.Sum(m => m.ValorTotal)))
        {
            var total = grupo.Sum(m => m.ValorTotal);
            AgregarFila(grupo.Key, $"{grupo.Count()} material(es)  •  $ {total / bsPorDolar:N2}", $"Bs {total:N2}");
        }

        foreach (var m in stockBajo)
            AgregarFila($"[Stock bajo] {m.Nombre}", $"Quedan {m.Cantidad:N2} {m.Unidad} (mínimo {m.StockMinimo:N0})", "Reponer", true);

        NotaPie = enLinea
            ? $"Tipo de cambio en línea (open.er-api.com): 1 USD = Bs {bsPorDolar:N2}"
            : $"Sin conexión a la API. Se usa el tipo de cambio oficial: 1 USD = Bs {bsPorDolar:N2}";
    }
}

/// <summary>Información de avances de obra (Integrante 4).</summary>
public class ReporteAvancesViewModel : ReporteViewModel
{
    public ReporteAvancesViewModel(DatabaseService db) : base(db)
    {
        Title = "Reporte de Avances";
        Descripcion = "Seguimiento del progreso físico acumulado por cada obra en ejecución.";
        TituloLista = "Último avance registrado por obra";
    }

    protected override async Task GenerarAsync()
    {
        var avances = await Db.ListarAsync<Avance>();
        var porObra = avances
            .GroupBy(a => a.NombreObra)
            .Select(g => g.OrderByDescending(a => a.Fecha).ThenByDescending(a => a.Porcentaje).First())
            .OrderByDescending(a => a.Porcentaje)
            .ToList();

        var promedio = porObra.Count == 0 ? 0 : porObra.Average(a => a.Porcentaje);
        var completadas = porObra.Count(a => a.Completado);
        var ultimaFecha = avances.Count == 0 ? "-" : avances.Max(a => a.Fecha).ToString("dd/MM/yyyy");

        AgregarIndicador("REG", "Registros totales", avances.Count.ToString());
        AgregarIndicador("PROM", "Avance promedio", $"{promedio:N0}%");
        AgregarIndicador("100%", "Obras completadas", completadas.ToString());
        AgregarIndicador("FECHA", "Último reporte", ultimaFecha);

        foreach (var a in porObra)
        {
            AgregarFila(a.NombreObra,
                $"{a.Etapa}  •  Resp: {a.Responsable}  •  {a.Fecha:dd/MM/yyyy}",
                a.PorcentajeTexto,
                a.Porcentaje < 30);
        }

        var rezagadas = porObra.Count(a => a.Porcentaje < 30);
        NotaPie = rezagadas > 0
            ? $"Hay {rezagadas} obra(s) resaltada(s) en etapa inicial (< 30% de avance)."
            : "Todas las obras con seguimiento superan el 30% de avance.";
    }
}
