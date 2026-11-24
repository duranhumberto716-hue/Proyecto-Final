using Proyecto_Final.Models;
using Proyecto_Final.Services;

namespace Proyecto_Final.ViewModels;

/// <summary>CRUD de Personal / Empleados de obra (Integrante 2).</summary>
public class EmpleadosViewModel : CrudViewModel<Empleado>
{
    public EmpleadosViewModel(DatabaseService db) : base(db)
    {
        Title = "Base de Datos - Personal";
    }

    public List<string> Cargos { get; } = new()
    {
        "Ingeniero civil", "Arquitecto", "Maestro de obra", "Albañil",
        "Ayudante", "Electricista", "Plomero", "Carpintero", "Administración"
    };

    public DateTime FechaMaxima => DateTime.Today;

    protected override string RutaReporte => "ReportePage?tipo=personal";
    protected override string NombreArchivoCsv => "personal_construcciones_duran";
    protected override string EncabezadoCsv => "ID,NombreCompleto,CI,Cargo,Telefono,SalarioBs,FechaIngreso,Estado";

    protected override string FormatearFilaCsv(Empleado e) =>
        $"{e.Id},{EscaparCsv(e.NombreCompleto)},{EscaparCsv(e.Ci)},{EscaparCsv(e.Cargo)},{EscaparCsv(e.Telefono)},{e.SalarioMensual:F2},{e.FechaIngreso:dd/MM/yyyy},{EscaparCsv(e.EstadoTexto)}";

    protected override Empleado CrearNuevo() => new() { FechaIngreso = DateTime.Today, Activo = true };

    protected override string? Validar(Empleado e) => Validador.ValidarEmpleado(e);

    protected override void Normalizar(Empleado e)
    {
        e.NombreCompleto = e.NombreCompleto?.Trim() ?? string.Empty;
        e.Ci = e.Ci?.Trim().ToUpperInvariant() ?? string.Empty;
        e.Telefono = e.Telefono?.Trim() ?? string.Empty;
    }

    protected override bool Coincide(Empleado e, string filtro) =>
        e.NombreCompleto.ToLowerInvariant().Contains(filtro) ||
        e.Ci.ToLowerInvariant().Contains(filtro) ||
        e.Cargo.ToLowerInvariant().Contains(filtro);

    protected override string Describir(Empleado e) => e.NombreCompleto;
}
