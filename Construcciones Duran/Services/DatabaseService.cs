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
                await conexion.CreateTableAsync<Obra>();
                await conexion.CreateTableAsync<Empleado>();
                await conexion.CreateTableAsync<Material>();
                await conexion.CreateTableAsync<Avance>();
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
            Rol = Roles.Cliente
        });
    }

    // ===================== DATOS INICIALES =====================

    private static async Task SembrarDatosAsync(SQLiteAsyncConnection db)
    {
        await AsegurarUsuarioAsync(db, "admin", "Administrador Construcciones Durán", "admin123", Roles.Administrador);
        await AsegurarUsuarioAsync(db, "trabajador", "Luis Gutiérrez (Obra)", "trab123", Roles.Trabajador);
        await AsegurarUsuarioAsync(db, "cliente", "María Rojas (Cliente)", "cli123", Roles.Cliente);

        if (await db.Table<Obra>().CountAsync() == 0)
        {
            await db.InsertAllAsync(new[]
            {
                new Obra
                {
                    Nombre = "Residencia Moderna Las Palmas",
                    Cliente = "María Fernanda Rojas",
                    Ubicacion = "Zona Norte - Residencial Las Palmas",
                    TipoObra = "Construcción residencial",
                    Presupuesto = 680000,
                    FechaInicio = DateTime.Today.AddDays(-90),
                    Estado = "En ejecución"
                },
                new Obra
                {
                    Nombre = "Edificio Corporativo Alianza",
                    Cliente = "Inmobiliaria Los Ceibos S.R.L.",
                    Ubicacion = "Av. Empresarial #450",
                    TipoObra = "Obra comercial",
                    Presupuesto = 1450000,
                    FechaInicio = DateTime.Today.AddDays(-180),
                    Estado = "En ejecución"
                },
                new Obra
                {
                    Nombre = "Remodelación Penthouse Mirador",
                    Cliente = "Jorge Antonio Suárez",
                    Ubicacion = "Condominio Mirador Real",
                    TipoObra = "Remodelación integral",
                    Presupuesto = 240000,
                    FechaInicio = DateTime.Today.AddDays(-120),
                    Estado = "Finalizada"
                },
                new Obra
                {
                    Nombre = "Nave Industrial Sector B",
                    Cliente = "Logística Oriente Ltda.",
                    Ubicacion = "Parque Industrial Sector B",
                    TipoObra = "Obra gruesa",
                    Presupuesto = 920000,
                    FechaInicio = DateTime.Today.AddDays(-15),
                    Estado = "Planificación"
                }
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

        if (await db.Table<Avance>().CountAsync() == 0)
        {
            await db.InsertAllAsync(new[]
            {
                new Avance
                {
                    NombreObra = "Residencia Moderna Las Palmas",
                    Etapa = "Obra gruesa / Estructura",
                    Porcentaje = 65,
                    Responsable = "Carlos Durán Méndez",
                    Fecha = DateTime.Today.AddDays(-5),
                    Observaciones = "Vaciado de losa del segundo nivel completado sin novedades."
                },
                new Avance
                {
                    NombreObra = "Edificio Corporativo Alianza",
                    Etapa = "Instalaciones eléctricas y sanitarias",
                    Porcentaje = 78,
                    Responsable = "Luis Fernando Gutiérrez",
                    Fecha = DateTime.Today.AddDays(-2),
                    Observaciones = "Cableado principal instalado hasta el tercer piso."
                },
                new Avance
                {
                    NombreObra = "Remodelación Penthouse Mirador",
                    Etapa = "Entrega final",
                    Porcentaje = 100,
                    Responsable = "Carlos Durán Méndez",
                    Fecha = DateTime.Today.AddDays(-14),
                    Observaciones = "Acta de recepción firmada por el cliente."
                },
                new Avance
                {
                    NombreObra = "Nave Industrial Sector B",
                    Etapa = "Replanteo y excavación",
                    Porcentaje = 20,
                    Responsable = "Pedro Vaca Justiniano",
                    Fecha = DateTime.Today.AddDays(-1),
                    Observaciones = "Nivelación de terreno y trazado de zapatas en curso."
                }
            });
        }
    }

    private static async Task AsegurarUsuarioAsync(SQLiteAsyncConnection db, string nombreUsuario, string nombreCompleto, string password, string rol)
    {
        var existe = await db.Table<Usuario>().Where(u => u.NombreUsuario == nombreUsuario).CountAsync() > 0;
        if (!existe)
        {
            await db.InsertAsync(new Usuario
            {
                NombreUsuario = nombreUsuario,
                NombreCompleto = nombreCompleto,
                PasswordHash = Seguridad.HashPassword(password),
                Rol = rol
            });
        }
    }
}
