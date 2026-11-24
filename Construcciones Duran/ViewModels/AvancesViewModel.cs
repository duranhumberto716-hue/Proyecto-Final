using System.Collections.ObjectModel;
using Proyecto_Final.Models;
using Proyecto_Final.Services;

namespace Proyecto_Final.ViewModels;

/// <summary>CRUD de Avances de obra (Integrante 4).</summary>
public class AvancesViewModel : CrudViewModel<Avance>
{
    public AvancesViewModel(DatabaseService db) : base(db)
    {
        Title = "Base de Datos - Avances";
    }

    public ObservableCollection<string> ObrasDisponibles { get; } = new();

    public List<string> Etapas { get; } = new()
    {
        "Replanteo y excavación",
        "Fundaciones y cimientos",
        "Obra gruesa / Estructura",
        "Muros y cubierta",
        "Instalaciones eléctricas y sanitarias",
        "Revoque y acabados",
        "Entrega final"
    };

    public DateTime FechaMaxima => DateTime.Today;

    protected override string RutaReporte => "ReportePage?tipo=avances";
    protected override string NombreArchivoCsv => "avances_construcciones_duran";
    protected override string EncabezadoCsv => "ID,Obra,Etapa,Porcentaje,Responsable,Fecha,Observaciones";

    protected override string FormatearFilaCsv(Avance a) =>
        $"{a.Id},{EscaparCsv(a.NombreObra)},{EscaparCsv(a.Etapa)},{a.Porcentaje:F0}%,{EscaparCsv(a.Responsable)},{a.Fecha:dd/MM/yyyy},{EscaparCsv(a.Observaciones)}";

    protected override Avance CrearNuevo() => new() { Fecha = DateTime.Today };

    protected override async Task AntesDeCargarAsync()
    {
        var obras = await Db.ListarAsync<Obra>();
        var seleccionada = Actual?.NombreObra;
        var nombres = obras.Select(o => o.Nombre).Where(n => !string.IsNullOrWhiteSpace(n)).Distinct().OrderBy(n => n).ToList();

        if (!string.IsNullOrWhiteSpace(seleccionada) && !nombres.Contains(seleccionada))
            nombres.Add(seleccionada);

        for (int i = ObrasDisponibles.Count - 1; i >= 0; i--)
        {
            if (!nombres.Contains(ObrasDisponibles[i]))
                ObrasDisponibles.RemoveAt(i);
        }

        foreach (var nombre in nombres)
        {
            if (!ObrasDisponibles.Contains(nombre))
                ObrasDisponibles.Add(nombre);
        }
    }

    protected override void AlCambiarActual()
    {
        if (Actual is not null && !string.IsNullOrWhiteSpace(Actual.NombreObra) && !ObrasDisponibles.Contains(Actual.NombreObra))
            ObrasDisponibles.Add(Actual.NombreObra);
    }

    protected override string? Validar(Avance a) => Validador.ValidarAvance(a);

    protected override void Normalizar(Avance a)
    {
        a.NombreObra = a.NombreObra?.Trim() ?? string.Empty;
        a.Responsable = a.Responsable?.Trim() ?? string.Empty;
        a.Observaciones = a.Observaciones?.Trim() ?? string.Empty;
    }

    protected override bool Coincide(Avance a, string filtro) =>
        a.NombreObra.ToLowerInvariant().Contains(filtro) ||
        a.Etapa.ToLowerInvariant().Contains(filtro) ||
        a.Responsable.ToLowerInvariant().Contains(filtro);

    protected override string Describir(Avance a) => $"{a.NombreObra} ({a.Etapa})";
}
