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

    protected void AgregarIndicador(string icono, string titulo, string valor) =>
        Indicadores.Add(new Indicador { Icono = icono, Titulo = titulo, Valor = valor });

    protected void AgregarFila(string titulo, string detalle, string valor, bool destacado = false) =>
        Filas.Add(new FilaReporte { Titulo = titulo, Detalle = detalle, Valor = valor, Destacado = destacado });
}

/// <summary>Información de clientes (Integrante 1).</summary>
public class ReporteClientesViewModel : ReporteViewModel
{
    public ReporteClientesViewModel(DatabaseService db) : base(db)
    {
        Title = "Reporte de Clientes";
        Descripcion = "Resumen de la cartera de clientes registrada en la base de datos.";
        TituloLista = "Clientes registrados recientemente";
    }

    protected override async Task GenerarAsync()
    {
        var clientes = await Db.ListarAsync<Cliente>();
        var inicioMes = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        var nuevosMes = clientes.Count(c => c.FechaRegistro >= inicioMes);
        var conCorreo = clientes.Count(c => !string.IsNullOrWhiteSpace(c.Correo));
        var porcentajeCorreo = clientes.Count == 0 ? 0 : conCorreo * 100.0 / clientes.Count;

        AgregarIndicador("👥", "Total clientes", clientes.Count.ToString());
        AgregarIndicador("🆕", "Nuevos este mes", nuevosMes.ToString());
        AgregarIndicador("✉️", "Con correo", $"{porcentajeCorreo:N0}%");
        AgregarIndicador("📅", "Último registro",
            clientes.Count == 0 ? "-" : clientes.Max(c => c.FechaRegistro).ToString("dd/MM/yyyy"));

        foreach (var c in clientes.OrderByDescending(c => c.FechaRegistro).Take(10))
            AgregarFila(c.Nombre, c.Resumen, c.FechaRegistro.ToString("dd/MM/yyyy"), c.FechaRegistro >= inicioMes);

        NotaPie = "Los clientes resaltados se registraron durante el mes actual.";
    }
}

/// <summary>
/// Información del inventario (Integrante 2). Usa la API externa de tipo de cambio
/// para mostrar el valor del inventario en dólares.
/// </summary>
public class ReporteInventarioViewModel : ReporteViewModel
{
    private readonly TipoCambioService _tipoCambio;

    public ReporteInventarioViewModel(DatabaseService db, TipoCambioService tipoCambio) : base(db)
    {
        _tipoCambio = tipoCambio;
        Title = "Reporte de Inventario";
        Descripcion = "Valorización del inventario de materiales en Bs y USD (tipo de cambio desde API externa).";
        TituloLista = "Valor por categoría y alertas de stock";
    }

    protected override async Task GenerarAsync()
    {
        var materiales = await Db.ListarAsync<Material>();
        var (bsPorDolar, enLinea) = await _tipoCambio.ObtenerBsPorDolarAsync();
        var valorBs = materiales.Sum(m => m.ValorTotal);
        var stockBajo = materiales.Where(m => m.StockBajo).ToList();

        AgregarIndicador("📦", "Materiales", materiales.Count.ToString());
        AgregarIndicador("💰", "Valor total (Bs)", $"Bs {valorBs:N2}");
        AgregarIndicador("💵", "Valor total (USD)", $"$ {valorBs / bsPorDolar:N2}");
        AgregarIndicador("⚠️", "Con stock bajo", stockBajo.Count.ToString());

        foreach (var grupo in materiales.GroupBy(m => m.Categoria).OrderByDescending(g => g.Sum(m => m.ValorTotal)))
        {
            var total = grupo.Sum(m => m.ValorTotal);
            AgregarFila($"📁 {grupo.Key}", $"{grupo.Count()} material(es)  •  $ {total / bsPorDolar:N2}", $"Bs {total:N2}");
        }

        foreach (var m in stockBajo)
            AgregarFila($"⚠️ {m.Nombre}", $"Quedan {m.Cantidad:N2} {m.Unidad} (mínimo {m.StockMinimo:N0})", "Reponer", true);

        NotaPie = enLinea
            ? $"Tipo de cambio en línea (open.er-api.com): 1 USD = Bs {bsPorDolar:N2}"
            : $"Sin conexión a la API. Se usa el tipo de cambio oficial: 1 USD = Bs {bsPorDolar:N2}";
    }
}

/// <summary>Información del personal (Integrante 3).</summary>
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

        AgregarIndicador("👷", "Total personal", empleados.Count.ToString());
        AgregarIndicador("🟢", "Activos", activos.Count.ToString());
        AgregarIndicador("💸", "Planilla mensual", $"Bs {planilla:N2}");
        AgregarIndicador("⏳", "Antigüedad prom.", $"{antiguedad:N1} años");

        foreach (var grupo in activos.GroupBy(e => e.Cargo).OrderByDescending(g => g.Sum(e => e.SalarioMensual)))
        {
            AgregarFila($"🦺 {grupo.Key}",
                $"{grupo.Count()} persona(s)  •  Promedio Bs {grupo.Average(e => e.SalarioMensual):N2}",
                $"Bs {grupo.Sum(e => e.SalarioMensual):N2}");
        }

        var inactivos = empleados.Count - activos.Count;
        NotaPie = inactivos > 0
            ? $"Hay {inactivos} empleado(s) inactivo(s) que no se incluyen en la planilla."
            : "Todo el personal registrado se encuentra activo.";
    }
}

/// <summary>Información de proveedores (Integrante 4).</summary>
public class ReporteProveedoresViewModel : ReporteViewModel
{
    public ReporteProveedoresViewModel(DatabaseService db) : base(db)
    {
        Title = "Reporte de Proveedores";
        Descripcion = "Cobertura de rubros y calificación de los proveedores de la empresa.";
        TituloLista = "Proveedores por rubro";
    }

    protected override async Task GenerarAsync()
    {
        var proveedores = await Db.ListarAsync<Proveedor>();
        var promedio = proveedores.Count == 0 ? 0 : proveedores.Average(p => p.Calificacion);
        var mejor = proveedores.OrderByDescending(p => p.Calificacion).FirstOrDefault();

        AgregarIndicador("🚚", "Total proveedores", proveedores.Count.ToString());
        AgregarIndicador("📁", "Rubros cubiertos", proveedores.Select(p => p.Rubro).Distinct().Count().ToString());
        AgregarIndicador("⭐", "Calificación prom.", $"{promedio:N1} / 5");
        AgregarIndicador("🏆", "Mejor calificado", mejor?.Empresa ?? "-");

        foreach (var grupo in proveedores.GroupBy(p => p.Rubro).OrderBy(g => g.Key))
        {
            var top = grupo.OrderByDescending(p => p.Calificacion).First();
            AgregarFila($"📁 {grupo.Key}",
                $"{grupo.Count()} proveedor(es)  •  Recomendado: {top.Empresa}",
                top.Estrellas);
        }

        var bajos = proveedores.Count(p => p.Calificacion <= 2);
        NotaPie = bajos > 0
            ? $"Hay {bajos} proveedor(es) con calificación baja (≤ 2). Se recomienda evaluarlos."
            : "Todos los proveedores tienen una calificación aceptable.";
    }
}

