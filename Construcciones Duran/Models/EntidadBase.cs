using SQLite;

namespace Proyecto_Final.Models;

/// <summary>
/// Clase base de todas las tablas de la base de datos SQLite.
/// Define la llave primaria autoincremental y un método para clonar el registro
/// (se usa al editar, para no modificar la lista hasta que el usuario guarde).
/// </summary>
public abstract class EntidadBase
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public EntidadBase Clonar() => (EntidadBase)MemberwiseClone();
}

