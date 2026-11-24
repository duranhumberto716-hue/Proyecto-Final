using Proyecto_Final.Models;
using SQLite;

namespace Proyecto_Final.Services;

/// <summary>
/// Acceso a la base de datos local SQLite (patrón Repositorio genérico).
/// El archivo se crea en la carpeta privada de la app (FileSystem.AppDataDirectory).
/// </summary>
public class DatabaseService
{
    private const string NombreArchivo = "construcciones_duran.db3";

    private const SQLiteOpenFlags Flags =
        SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache;

    private readonly SemaphoreSlim _candado = new(1, 1);
    private SQLiteAsyncConnection? _conexion;

    public static string RutaBaseDatos => Path.Combine(FileSystem.AppDataDirectory, NombreArchivo);

    /// <summary>
    /// Abre la conexión una sola vez, crea las tablas y carga datos iniciales.
    /// </summary>
    private async Task<SQLiteAsyncConnection> ObtenerConexionAsync()
    {
        if (_conexion is not null)
            return _conexion;

        await _candado.WaitAsync();
        try
        {
            if (_conexion is null)
            {
                var conexion = new SQLiteAsyncConnection(RutaBaseDatos, Flags);
                await conexion.CreateTableAsync<Usuario>();
                await conexion.CreateTableAsync<Cliente>();
                await conexion.CreateTableAsync<Material>();
                await conexion.CreateTableAsync<Empleado>();
                await conexion.CreateTableAsync<Proveedor>();
                await SembrarDatosAsync(conexion);
                _conexion = conexion;
            }
        }
        finally
        {
            _candado.Release();
        }

        return _conexion;
    }

    // ===================== CRUD GENÉRICO =====================

    public async Task<List<T>> ListarAsync<T>() where T : EntidadBase, new()
    {
        var db = await ObtenerConexionAsync();
        return await db.Table<T>().ToListAsync();
    }

    /// <summary>Inserta si Id = 0, si no actualiza.</summary>
    public async Task<int> GuardarAsync<T>(T entidad) where T : EntidadBase, new()
    {
        var db = await ObtenerConexionAsync();
        return entidad.Id == 0
            ? await db.InsertAsync(entidad)
            : await db.UpdateAsync(entidad);
    }

    public async Task<int> EliminarAsync<T>(T entidad) where T : EntidadBase, new()
    {
        var db = await ObtenerConexionAsync();
        return await db.DeleteAsync(entidad);
    }

    // ===================== USUARIOS / LOGIN =====================

    public async Task<Usuario?> AutenticarAsync(string usuario, string password)
    {
        var db = await ObtenerConexionAsync();
        var nombre = usuario.Trim().ToLowerInvariant();
        var hash = Seguridad.HashPassword(password);
        return await db.Table<Usuario>()
            .Where(u => u.NombreUsuario == nombre && u.PasswordHash == hash)
            .FirstOrDefaultAsync();
    }

    public async Task<bool> ExisteUsuarioAsync(string usuario)
    {
        var db = await ObtenerConexionAsync();
        var nombre = usuario.Trim().ToLowerInvariant();
        return await db.Table<Usuario>().Where(u => u.NombreUsuario == nombre).CountAsync() > 0;
    }

    public async Task RegistrarUsuarioAsync(string nombreCompleto, string usuario, string password)
    {
        var db = await ObtenerConexionAsync();
        await db.InsertAsync(new Usuario
        {
            NombreCompleto = nombreCompleto.Trim(),
            NombreUsuario = usuario.Trim().ToLowerInvariant(),
            PasswordHash = Seguridad.HashPassword(password),
            Rol = "Usuario"
        });
    }

    // ===================== DATOS INICIALES =====================

    private static async Task SembrarDatosAsync(SQLiteAsyncConnection db)
    {
        if (await db.Table<Usuario>().CountAsync() == 0)
        {
            await db.InsertAsync(new Usuario
            {
                NombreUsuario = "admin",
                NombreCompleto = "Administrador Construcciones Durán",
                PasswordHash = Seguridad.HashPassword("admin123"),
                Rol = "Administrador"
            });
        }

        if (await db.Table<Cliente>().CountAsync() == 0)
        {
            await db.InsertAllAsync(new[]
            {
                new Cliente { Nombre = "María Fernanda Rojas", Ci = "6543210", Telefono = "71234567", Correo = "maria.rojas@gmail.com", Direccion = "Barrio Equipetrol, Calle 4 #120", FechaRegistro = DateTime.Now.AddDays(-40) },
                new Cliente { Nombre = "Jorge Antonio Suárez", Ci = "7894561", Telefono = "76543210", Correo = "", Direccion = "Av. Banzer 6to anillo", FechaRegistro = DateTime.Now.AddDays(-12) },
                new Cliente { Nombre = "Inmobiliaria Los Ceibos S.R.L.", Ci = "1029384", Telefono = "33445566", Correo = "contacto@losceibos.bo", Direccion = "Urb. Los Ceibos, Mz 12", FechaRegistro = DateTime.Now.AddDays(-3) }
            });
        }

        if (await db.Table<Material>().CountAsync() == 0)
        {
            await db.InsertAllAsync(new[]
            {
                new Material { Nombre = "Cemento Portland IP-30", Categoria = "Cemento y agregados", Unidad = "Bolsa", Cantidad = 120, PrecioUnitario = 58, StockMinimo = 50 },
                new Material { Nombre = "Fierro corrugado 12 mm", Categoria = "Acero y fierro", Unidad = "Barra", Cantidad = 35, PrecioUnitario = 95, StockMinimo = 40 },
                new Material { Nombre = "Ladrillo 6 huecos", Categoria = "Ladrillos y bloques", Unidad = "Unidad", Cantidad = 4500, PrecioUnitario = 1.2, StockMinimo = 1000 },
                new Material { Nombre = "Arena fina", Categoria = "Cemento y agregados", Unidad = "m³", Cantidad = 8, PrecioUnitario = 150, StockMinimo = 10 },
                new Material { Nombre = "Pintura látex blanca", Categoria = "Pinturas y acabados", Unidad = "Galón", Cantidad = 25, PrecioUnitario = 85, StockMinimo = 10 }
            });
        }

        if (await db.Table<Empleado>().CountAsync() == 0)
        {
            await db.InsertAllAsync(new[]
            {
                new Empleado { NombreCompleto = "Carlos Durán Méndez", Ci = "4567891", Cargo = "Ingeniero civil", Telefono = "77661522", SalarioMensual = 9500, FechaIngreso = new DateTime(2018, 3, 1), Activo = true },
                new Empleado { NombreCompleto = "Luis Fernando Gutiérrez", Ci = "5678912", Cargo = "Maestro de obra", Telefono = "70011223", SalarioMensual = 6000, FechaIngreso = new DateTime(2020, 6, 15), Activo = true },
                new Empleado { NombreCompleto = "Pedro Vaca Justiniano", Ci = "6789123", Cargo = "Albañil", Telefono = "75544332", SalarioMensual = 4200, FechaIngreso = new DateTime(2022, 1, 10), Activo = true },
                new Empleado { NombreCompleto = "Ana Lucía Pérez", Ci = "8912345", Cargo = "Administración", Telefono = "72233445", SalarioMensual = 5000, FechaIngreso = new DateTime(2021, 9, 1), Activo = false }
            });
        }

        if (await db.Table<Proveedor>().CountAsync() == 0)
        {
            await db.InsertAllAsync(new[]
            {
                new Proveedor { Empresa = "Cementos del Oriente", Nit = "1020304050", Contacto = "Roberto Áñez", Telefono = "33521100", Rubro = "Cemento y agregados", Correo = "ventas@cemoriente.bo", Calificacion = 5 },
                new Proveedor { Empresa = "Ferretería El Constructor", Nit = "2030405060", Contacto = "Silvia Justiniano", Telefono = "71112233", Rubro = "Acero y fierro", Correo = "", Calificacion = 4 },
                new Proveedor { Empresa = "Maquinarias Santa Cruz", Nit = "3040506070", Contacto = "Diego Rivero", Telefono = "77889900", Rubro = "Alquiler de maquinaria", Correo = "alquiler@maqscz.bo", Calificacion = 3 }
            });
        }
    }
}

