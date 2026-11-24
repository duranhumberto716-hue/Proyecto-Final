namespace Proyecto_Final.Services;

/// <summary>
/// Mensajes y confirmaciones reutilizables desde los ViewModels.
/// </summary>
public static class Dialogos
{
    private static Page? PaginaActual =>
        Shell.Current?.CurrentPage ?? Application.Current?.Windows.FirstOrDefault()?.Page;

    public static async Task AlertaAsync(string titulo, string mensaje)
    {
        if (PaginaActual is Page pagina)
            await pagina.DisplayAlertAsync(titulo, mensaje, "Entendido");
    }

    public static async Task<bool> ConfirmarAsync(string titulo, string mensaje)
    {
        if (PaginaActual is Page pagina)
            return await pagina.DisplayAlertAsync(titulo, mensaje, "Sí", "No");
        return false;
    }
}

