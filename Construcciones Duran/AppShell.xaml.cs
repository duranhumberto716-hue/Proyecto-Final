namespace Proyecto_Final;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Registro de ruta para navegación a la pantalla de cotización
        Routing.RegisterRoute(nameof(CotizacionPage), typeof(CotizacionPage));
    }
}
