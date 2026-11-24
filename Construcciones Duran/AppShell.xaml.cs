using Proyecto_Final.Views;

namespace Proyecto_Final;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Rutas de las pantallas secundarias (se abren desde el menú)
        Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
        Routing.RegisterRoute(nameof(CotizacionPage), typeof(CotizacionPage));
        Routing.RegisterRoute(nameof(RegistroPage), typeof(RegistroPage));
        Routing.RegisterRoute(nameof(ObrasPage), typeof(ObrasPage));
        Routing.RegisterRoute(nameof(EmpleadosPage), typeof(EmpleadosPage));
        Routing.RegisterRoute(nameof(MaterialesPage), typeof(MaterialesPage));
        Routing.RegisterRoute(nameof(AvancesPage), typeof(AvancesPage));
        Routing.RegisterRoute(nameof(ReportePage), typeof(ReportePage));
        Routing.RegisterRoute(nameof(OdsPage), typeof(OdsPage));
    }
}
