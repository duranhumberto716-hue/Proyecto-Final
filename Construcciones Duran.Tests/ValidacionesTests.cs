using Proyecto_Final.Models;
using Proyecto_Final.Services;
using Xunit;

namespace Construcciones_Duran.Tests;

/// <summary>Pruebas del CRUD de Obras (Integrante 1).</summary>
public class ObraTests
{
    private static Obra ObraValida() => new()
    {
        Nombre = "Residencia Las Palmas",
        Cliente = "María Fernanda Rojas",
        Ubicacion = "Zona Norte - Las Palmas",
        TipoObra = "Construcción residencial",
        Presupuesto = 680000,
        FechaInicio = DateTime.Today,
        Estado = "En ejecución"
    };

    [Fact]
    public void ObraValida_NoTieneErrores() =>
        Assert.Null(Validador.ValidarObra(ObraValida()));

    [Theory]
    [InlineData("")]
    [InlineData("Ab")]
    public void Nombre_Corto_EsRechazado(string nombre)
    {
        var o = ObraValida();
        o.Nombre = nombre;
        Assert.NotNull(Validador.ValidarObra(o));
    }

    [Fact]
    public void Presupuesto_CeroONegativo_EsRechazado()
    {
        var o = ObraValida();
        o.Presupuesto = 0;
        Assert.Contains("presupuesto", Validador.ValidarObra(o));
    }

    [Fact]
    public void EnEjecucion_EsVerdaderoSoloCuandoEstadoCorresponde()
    {
        var o = ObraValida();
        Assert.True(o.EnEjecucion);
        o.Estado = "Finalizada";
        Assert.False(o.EnEjecucion);
    }
}

/// <summary>Pruebas del CRUD de Personal (Integrante 2).</summary>
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

    [Theory]
    [InlineData("1234567-1A", true)]
    [InlineData("1234", false)]
    [InlineData("", false)]
    public void Ci_Formato(string ci, bool esperado) =>
        Assert.Equal(esperado, Validador.EsCiValido(ci));
}

/// <summary>Pruebas del CRUD de Materiales (Integrante 3).</summary>
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

/// <summary>Pruebas del CRUD de Avances (Integrante 4).</summary>
public class AvanceTests
{
    private static Avance AvanceValido() => new()
    {
        NombreObra = "Residencia Las Palmas",
        Etapa = "Obra gruesa / Estructura",
        Porcentaje = 65,
        Responsable = "Carlos Durán Méndez",
        Fecha = DateTime.Today,
        Observaciones = "Losa vaciada correctamente."
    };

    [Fact]
    public void AvanceValido_NoTieneErrores() =>
        Assert.Null(Validador.ValidarAvance(AvanceValido()));

    [Theory]
    [InlineData(-5)]
    [InlineData(101)]
    public void Porcentaje_FueraDeRango_EsRechazado(double porcentaje)
    {
        var a = AvanceValido();
        a.Porcentaje = porcentaje;
        Assert.Contains("porcentaje", Validador.ValidarAvance(a));
    }

    [Fact]
    public void Fecha_Futura_EsRechazada()
    {
        var a = AvanceValido();
        a.Fecha = DateTime.Today.AddDays(3);
        Assert.NotNull(Validador.ValidarAvance(a));
    }

    [Fact]
    public void ProgresoNormalizado_EscalaEntreCeroYUno()
    {
        var a = AvanceValido();
        a.Porcentaje = 50;
        Assert.Equal(0.5, a.ProgresoNormalizado);
        Assert.False(a.Completado);

        a.Porcentaje = 100;
        Assert.Equal(1.0, a.ProgresoNormalizado);
        Assert.True(a.Completado);
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
        Assert.Equal(64, hash.Length);
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
        var original = new Obra { Id = 1, Nombre = "Obra Original" };
        var copia = (Obra)original.Clonar();
        copia.Nombre = "Obra Modificada";
        Assert.Equal("Obra Original", original.Nombre);
        Assert.Equal(1, copia.Id);
    }
}

/// <summary>Pruebas de control de acceso por roles (Administrador, Trabajador y Cliente).</summary>
public class RolesPermisosTests
{
    [Fact]
    public void Administrador_TieneAccesoATodosLosModulos()
    {
        var sesion = new SesionService();
        sesion.Iniciar(new Usuario { NombreUsuario = "admin", Rol = Roles.Administrador });

        Assert.True(sesion.EsAdministrador);
        Assert.False(sesion.EsTrabajador);
        Assert.False(sesion.EsCliente);
        Assert.True(sesion.PuedeAccederARuta("ObrasPage"));
        Assert.True(sesion.PuedeAccederARuta("EmpleadosPage"));
        Assert.True(sesion.PuedeAccederARuta("MaterialesPage"));
        Assert.True(sesion.PuedeAccederARuta("AvancesPage"));
        Assert.True(sesion.PuedeAccederARuta("CotizacionPage"));
        Assert.True(sesion.PuedeAccederARuta("MainPage"));
    }

    [Fact]
    public void Trabajador_AccedeAOperacionDeObraPeroNoAAdministracionDePersonalNiContratos()
    {
        var sesion = new SesionService();
        sesion.Iniciar(new Usuario { NombreUsuario = "trabajador", Rol = Roles.Trabajador });

        Assert.True(sesion.EsTrabajador);
        Assert.True(sesion.PuedeAccederARuta("MaterialesPage"));
        Assert.True(sesion.PuedeAccederARuta("AvancesPage"));
        Assert.True(sesion.PuedeAccederARuta("ReportePage?tipo=avances"));
        Assert.True(sesion.PuedeAccederARuta("ReportePage?tipo=materiales"));
        Assert.True(sesion.PuedeAccederARuta("MainPage"));
        Assert.False(sesion.PuedeAccederARuta("EmpleadosPage"));
        Assert.False(sesion.PuedeAccederARuta("ObrasPage"));
        Assert.False(sesion.PuedeAccederARuta("ReportePage?tipo=personal"));
    }

    [Theory]
    [InlineData(Roles.Cliente)]
    [InlineData("Usuario")]
    [InlineData("")]
    public void Cliente_SoloPuedeVerObrasRealizadasYCotizacionSinOds(string rol)
    {
        var sesion = new SesionService();
        sesion.Iniciar(new Usuario { NombreUsuario = "nuevo", Rol = rol });

        Assert.True(sesion.EsCliente);
        Assert.False(sesion.EsAdministrador);
        Assert.True(sesion.PuedeAccederARuta("MainPage"));
        Assert.True(sesion.PuedeAccederARuta("CotizacionPage"));

        Assert.False(sesion.PuedeAccederARuta("OdsPage"));
        Assert.False(sesion.PuedeAccederARuta("ObrasPage"));
        Assert.False(sesion.PuedeAccederARuta("EmpleadosPage"));
        Assert.False(sesion.PuedeAccederARuta("MaterialesPage"));
        Assert.False(sesion.PuedeAccederARuta("AvancesPage"));
        Assert.False(sesion.PuedeAccederARuta("ReportePage?tipo=obras"));
        Assert.False(sesion.PuedeAccederARuta("ReportePage?tipo=personal"));
    }

    [Fact]
    public void UsuarioNuevo_PorDefectoTieneRolCliente()
    {
        var usuario = new Usuario { NombreUsuario = "juanp" };
        Assert.Equal(Roles.Cliente, usuario.Rol);
    }
}
