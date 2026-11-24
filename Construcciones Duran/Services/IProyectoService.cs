using Proyecto_Final.Models;

namespace Proyecto_Final.Services;

public interface IProyectoService
{
    Task<IEnumerable<Proyecto>> ObtenerProyectosAsync();
}

