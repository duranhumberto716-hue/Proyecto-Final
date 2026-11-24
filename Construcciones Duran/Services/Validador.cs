using System.Text.RegularExpressions;
using Proyecto_Final.Models;

namespace Proyecto_Final.Services;

/// <summary>
/// Reglas de negocio y validaciones de los formularios.
/// Cada método devuelve null si el dato es válido, o el mensaje de error a mostrar.
/// No depende de MAUI, por eso se puede probar con pruebas unitarias.
/// </summary>
public static partial class Validador
{
    [GeneratedRegex(@"^\d{7,8}$")]
    private static partial Regex TelefonoRegex();

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    private static partial Regex CorreoRegex();

    [GeneratedRegex(@"^\d{5,10}(-?[A-Za-z0-9]{1,3})?$")]
    private static partial Regex CiRegex();

    [GeneratedRegex(@"^[a-zA-Z0-9_.]{4,30}$")]
    private static partial Regex UsuarioRegex();

    public static bool EsTelefonoValido(string? telefono) =>
        !string.IsNullOrWhiteSpace(telefono) && TelefonoRegex().IsMatch(telefono.Trim());

    /// <summary>El correo es opcional: vacío se considera válido.</summary>
    public static bool EsCorreoValido(string? correo) =>
        string.IsNullOrWhiteSpace(correo) || CorreoRegex().IsMatch(correo.Trim());

    public static bool EsCiValido(string? ci) =>
        !string.IsNullOrWhiteSpace(ci) && CiRegex().IsMatch(ci.Trim());

    public static string? ValidarCliente(Cliente c)
    {
        if (string.IsNullOrWhiteSpace(c.Nombre) || c.Nombre.Trim().Length < 3)
            return "El nombre del cliente es obligatorio (mínimo 3 caracteres).";
        if (!EsCiValido(c.Ci))
            return "El CI debe tener entre 5 y 10 dígitos (puede incluir complemento, ej: 1234567-1A).";
        if (!EsTelefonoValido(c.Telefono))
            return "El teléfono debe tener 7 u 8 dígitos (ej: 77661522).";
        if (!EsCorreoValido(c.Correo))
            return "El correo electrónico no tiene un formato válido.";
        if (string.IsNullOrWhiteSpace(c.Direccion))
            return "La dirección es obligatoria.";
        return null;
    }

    public static string? ValidarMaterial(Material m)
    {
        if (string.IsNullOrWhiteSpace(m.Nombre) || m.Nombre.Trim().Length < 2)
            return "El nombre del material es obligatorio.";
        if (string.IsNullOrWhiteSpace(m.Categoria))
            return "Seleccione una categoría.";
        if (string.IsNullOrWhiteSpace(m.Unidad))
            return "Seleccione la unidad de medida.";
        if (m.Cantidad < 0)
            return "La cantidad no puede ser negativa.";
        if (m.PrecioUnitario <= 0)
            return "El precio unitario debe ser mayor a 0.";
        if (m.StockMinimo < 0)
            return "El stock mínimo no puede ser negativo.";
        return null;
    }

    public static string? ValidarEmpleado(Empleado e)
    {
        if (string.IsNullOrWhiteSpace(e.NombreCompleto) || e.NombreCompleto.Trim().Length < 3)
            return "El nombre del empleado es obligatorio (mínimo 3 caracteres).";
        if (!EsCiValido(e.Ci))
            return "El CI debe tener entre 5 y 10 dígitos.";
        if (string.IsNullOrWhiteSpace(e.Cargo))
            return "Seleccione el cargo del empleado.";
        if (!string.IsNullOrWhiteSpace(e.Telefono) && !EsTelefonoValido(e.Telefono))
            return "El teléfono debe tener 7 u 8 dígitos.";
        if (e.SalarioMensual < 2750)
            return "El salario no puede ser menor al salario mínimo nacional (Bs 2.750).";
        if (e.FechaIngreso.Date > DateTime.Today)
            return "La fecha de ingreso no puede ser futura.";
        return null;
    }

    public static string? ValidarProveedor(Proveedor p)
    {
        if (string.IsNullOrWhiteSpace(p.Empresa) || p.Empresa.Trim().Length < 3)
            return "El nombre de la empresa es obligatorio (mínimo 3 caracteres).";
        if (string.IsNullOrWhiteSpace(p.Nit) || !NitRegex().IsMatch(p.Nit.Trim()))
            return "El NIT debe tener entre 6 y 15 dígitos.";
        if (!EsTelefonoValido(p.Telefono))
            return "El teléfono debe tener 7 u 8 dígitos.";
        if (string.IsNullOrWhiteSpace(p.Rubro))
            return "Seleccione el rubro del proveedor.";
        if (!EsCorreoValido(p.Correo))
            return "El correo electrónico no tiene un formato válido.";
        if (p.Calificacion < 1 || p.Calificacion > 5)
            return "La calificación debe estar entre 1 y 5.";
        return null;
    }

    [GeneratedRegex(@"^\d{6,15}$")]
    private static partial Regex NitRegex();

    public static string? ValidarRegistro(string? nombreCompleto, string? usuario, string? password, string? confirmacion)
    {
        if (string.IsNullOrWhiteSpace(nombreCompleto) || nombreCompleto.Trim().Length < 3)
            return "Ingrese su nombre completo.";
        if (string.IsNullOrWhiteSpace(usuario) || !UsuarioRegex().IsMatch(usuario.Trim()))
            return "El usuario debe tener de 4 a 30 caracteres (letras, números, punto o guion bajo).";
        if (string.IsNullOrEmpty(password) || password.Length < 6)
            return "La contraseña debe tener al menos 6 caracteres.";
        if (password != confirmacion)
            return "Las contraseñas no coinciden.";
        return null;
    }
}

