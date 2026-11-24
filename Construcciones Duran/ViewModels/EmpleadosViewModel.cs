using Proyecto_Final.Models;
using Proyecto_Final.Services;

namespace Proyecto_Final.ViewModels;

/// <summary>CRUD de Empleados / Personal de obra (Integrante 3).</summary>
public class EmpleadosViewModel : CrudViewModel<Empleado>
{
    public EmpleadosViewModel(DatabaseService db) : base(db)
    {
        Title = "Personal de Obra";
    }

    public List<string> Cargos { get; } = new()
    {
        "Ingeniero civil", "Arquitecto", "Maestro de obra", "Albañil",
        "Ayudante", "Electricista", "Plomero", "Carpintero", "Administración"
    };

    public DateTime FechaMaxima => DateTime.Today;

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

    protected override IEnumerable<Empleado> Ordenar(IEnumerable<Empleado> datos) =>
        datos.OrderByDescending(e => e.Activo).ThenBy(e => e.NombreCompleto);

    protected override string Describir(Empleado e) => e.NombreCompleto;
}

