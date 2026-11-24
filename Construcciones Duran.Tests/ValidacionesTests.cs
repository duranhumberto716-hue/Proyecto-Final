using Proyecto_Final.Models;
using Proyecto_Final.Services;
using Xunit;

namespace Construcciones_Duran.Tests;

/// <summary>Pruebas del CRUD de Clientes (Integrante 1).</summary>
public class ClienteTests
{
    private static Cliente ClienteValido() => new()
    {
        Nombre = "María Rojas",
        Ci = "6543210",
        Telefono = "71234567",
        Correo = "maria@gmail.com",
        Direccion = "Equipetrol, Calle 4"
    };

    [Fact]
    public void ClienteValido_NoTieneErrores() =>
        Assert.Null(Validador.ValidarCliente(ClienteValido()));

    [Theory]
    [InlineData("")]
    [InlineData("Al")]
    public void Nombre_Corto_EsRechazado(string nombre)
    {
        var c = ClienteValido();
        c.Nombre = nombre;
        Assert.NotNull(Validador.ValidarCliente(c));
    }

    [Theory]
    [InlineData("123")]
    [InlineData("71234")]
    [InlineData("7123456789")]
    [InlineData("abcdefgh")]
    public void Telefono_Invalido_EsRechazado(string telefono)
    {
        var c = ClienteValido();
        c.Telefono = telefono;
        Assert.Contains("teléfono", Validador.ValidarCliente(c));
    }

    [Fact]
    public void Correo_Vacio_EsPermitido()
    {
        var c = ClienteValido();
        c.Correo = "";
        Assert.Null(Validador.ValidarCliente(c));
    }

    [Fact]
    public void Correo_MalFormado_EsRechazado()
    {
        var c = ClienteValido();
        c.Correo = "maria@gmail";
        Assert.Contains("correo", Validador.ValidarCliente(c));
    }

    [Theory]
    [InlineData("1234567-1A", true)]
    [InlineData("1234", false)]
    [InlineData("", false)]
    public void Ci_Formato(string ci, bool esperado) =>
        Assert.Equal(esperado, Validador.EsCiValido(ci));
}

/// <summary>Pruebas del CRUD de Materiales (Integrante 2).</summary>
public class MaterialTests
{
    private static Material MaterialValido() => new()
    {
        Nombre = "Cemento IP-30",
        Categoria = "Cemento y agregados",
        Unidad = "Bolsa",
        Cantidad = 100,
        PrecioUnitario = 58,
        StockMinimo = 50
    };

    [Fact]
    public void MaterialValido_NoTieneErrores() =>
        Assert.Null(Validador.ValidarMaterial(MaterialValido()));

    [Fact]
    public void Precio_Cero_EsRechazado()
    {
        var m = MaterialValido();
        m.PrecioUnitario = 0;
        Assert.NotNull(Validador.ValidarMaterial(m));
    }

    [Fact]
    public void Cantidad_Negativa_EsRechazada()
    {
        var m = MaterialValido();
        m.Cantidad = -1;
        Assert.NotNull(Validador.ValidarMaterial(m));
    }

    [Fact]
    public void ValorTotal_EsCantidadPorPrecio() =>
        Assert.Equal(5800, MaterialValido().ValorTotal);

    [Theory]
    [InlineData(50, true)]
    [InlineData(10, true)]
    [InlineData(51, false)]
    public void StockBajo_CuandoCantidadMenorOIgualAlMinimo(double cantidad, bool esperado)
    {
        var m = MaterialValido();
        m.Cantidad = cantidad;
        Assert.Equal(esperado, m.StockBajo);
    }
}

/// <summary>Pruebas del CRUD de Personal (Integrante 3).</summary>
public class EmpleadoTests
{
    private static Empleado EmpleadoValido() => new()
    {
        NombreCompleto = "Luis Gutiérrez",
        Ci = "5678912",
        Cargo = "Albañil",
        Telefono = "70011223",
        SalarioMensual = 4200,
        FechaIngreso = DateTime.Today.AddYears(-1),
        Activo = true
    };

    [Fact]
    public void EmpleadoValido_NoTieneErrores() =>
        Assert.Null(Validador.ValidarEmpleado(EmpleadoValido()));

    [Fact]
    public void Salario_MenorAlMinimoNacional_EsRechazado()
    {
        var e = EmpleadoValido();
        e.SalarioMensual = 2000;
        Assert.Contains("salario mínimo", Validador.ValidarEmpleado(e));
    }

    [Fact]
    public void FechaIngreso_Futura_EsRechazada()
    {
        var e = EmpleadoValido();
        e.FechaIngreso = DateTime.Today.AddDays(5);
        Assert.NotNull(Validador.ValidarEmpleado(e));
    }

    [Fact]
    public void Telefono_Vacio_EsPermitido()
    {
        var e = EmpleadoValido();
        e.Telefono = "";
        Assert.Null(Validador.ValidarEmpleado(e));
    }
}

/// <summary>Pruebas del CRUD de Proveedores (Integrante 4).</summary>
public class ProveedorTests
{
    private static Proveedor ProveedorValido() => new()
    {
        Empresa = "Cementos del Oriente",
        Nit = "1020304050",
        Telefono = "33521100",
        Rubro = "Cemento y agregados",
        Calificacion = 5
    };

    [Fact]
    public void ProveedorValido_NoTieneErrores() =>
        Assert.Null(Validador.ValidarProveedor(ProveedorValido()));

    [Theory]
    [InlineData("123")]
    [InlineData("ABC123456")]
    public void Nit_Invalido_EsRechazado(string nit)
    {
        var p = ProveedorValido();
        p.Nit = nit;
        Assert.Contains("NIT", Validador.ValidarProveedor(p));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    public void Calificacion_FueraDeRango_EsRechazada(int calificacion)
    {
        var p = ProveedorValido();
        p.Calificacion = calificacion;
        Assert.NotNull(Validador.ValidarProveedor(p));
    }

    [Fact]
    public void Estrellas_MuestraLaCalificacion()
    {
        var p = ProveedorValido();
        p.Calificacion = 3;
        Assert.Equal("★★★☆☆", p.Estrellas);
    }
}

/// <summary>Pruebas del login y registro de usuarios.</summary>
public class SeguridadTests
{
    [Fact]
    public void Hash_EsSiempreIgualParaLaMismaContrasena() =>
        Assert.Equal(Seguridad.HashPassword("admin123"), Seguridad.HashPassword("admin123"));

    [Fact]
    public void Hash_NoGuardaLaContrasenaEnTextoPlano()
    {
        var hash = Seguridad.HashPassword("admin123");
        Assert.DoesNotContain("admin123", hash);
        Assert.Equal(64, hash.Length); // SHA-256 = 64 caracteres hexadecimales
    }

    [Fact]
    public void Registro_ContrasenasDistintas_EsRechazado() =>
        Assert.Contains("no coinciden", Validador.ValidarRegistro("Juan Pérez", "juanp", "123456", "654321"));

    [Fact]
    public void Registro_ContrasenaCorta_EsRechazado() =>
        Assert.NotNull(Validador.ValidarRegistro("Juan Pérez", "juanp", "123", "123"));

    [Fact]
    public void Registro_Valido_NoTieneErrores() =>
        Assert.Null(Validador.ValidarRegistro("Juan Pérez", "juanp", "secreto1", "secreto1"));

    [Fact]
    public void Clonar_CreaUnaCopiaIndependiente()
    {
        var original = new Cliente { Id = 1, Nombre = "Original" };
        var copia = (Cliente)original.Clonar();
        copia.Nombre = "Modificado";
        Assert.Equal("Original", original.Nombre);
        Assert.Equal(1, copia.Id);
    }
}

