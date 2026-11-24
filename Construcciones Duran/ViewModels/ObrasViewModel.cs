using Proyecto_Final.Models;
using Proyecto_Final.Services;

namespace Proyecto_Final.ViewModels;

/// <summary>CRUD de Obras (Integrante 1).</summary>
public class ObrasViewModel : CrudViewModel<Obra>
{
    public ObrasViewModel(DatabaseService db) : base(db)
    {
        Title = "Base de Datos - Obras";
    }

    public List<string> TiposObra { get; } = new()
    {
        "Construcción residencial",
        "Obra comercial",
        "Remodelación integral",
        "Obra gruesa",
        "Acabados y fachadas",
        "Infraestructura industrial"
    };

    public List<string> Estados { get; } = new()
    {
        "Planificación",
        "En ejecución",
        "Pausada",
        "Finalizada"
    };

    protected override string RutaReporte => "ReportePage?tipo=obras";
    protected override string NombreArchivoCsv => "obras_construcciones_duran";
    protected override string EncabezadoCsv => "ID,Obra,Cliente,Tipo,Ubicacion,PresupuestoBs,FechaInicio,Estado";

    protected override string FormatearFilaCsv(Obra o) =>
        $"{o.Id},{EscaparCsv(o.Nombre)},{EscaparCsv(o.Cliente)},{EscaparCsv(o.TipoObra)},{EscaparCsv(o.Ubicacion)},{o.Presupuesto:F2},{o.FechaInicio:dd/MM/yyyy},{EscaparCsv(o.Estado)}";

    protected override Obra CrearNuevo() => new()
    {
        FechaInicio = DateTime.Today,
        Estado = "Planificación"
    };

    protected override string? Validar(Obra o) => Validador.ValidarObra(o);

    protected override void Normalizar(Obra o)
    {
        o.Nombre = o.Nombre?.Trim() ?? string.Empty;
        o.Cliente = o.Cliente?.Trim() ?? string.Empty;
        o.Ubicacion = o.Ubicacion?.Trim() ?? string.Empty;
    }

    protected override bool Coincide(Obra o, string filtro) =>
        o.Nombre.ToLowerInvariant().Contains(filtro) ||
        o.Cliente.ToLowerInvariant().Contains(filtro) ||
        o.Ubicacion.ToLowerInvariant().Contains(filtro) ||
        o.Estado.ToLowerInvariant().Contains(filtro);

    protected override string Describir(Obra o) => o.Nombre;
}
