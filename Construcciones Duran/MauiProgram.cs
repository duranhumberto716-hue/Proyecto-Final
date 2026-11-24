using Microsoft.Extensions.Logging;
using Proyecto_Final.Services;
using Proyecto_Final.ViewModels;
using Proyecto_Final.Views;

namespace Proyecto_Final;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // =========================================================================
        // REGISTRO DE SERVICIOS (INYECCIÓN DE DEPENDENCIAS)
        // =========================================================================
        builder.Services.AddSingleton<IProyectoService, ProyectoService>();
        builder.Services.AddSingleton<IWhatsAppService, WhatsAppService>();
        builder.Services.AddSingleton<DatabaseService>();
        builder.Services.AddSingleton<SesionService>();
        builder.Services.AddSingleton<TipoCambioService>();

        // REGISTRO DE VIEWMODELS
        builder.Services.AddTransient<MainViewModel>();
        builder.Services.AddTransient<CotizacionViewModel>();
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<RegistroViewModel>();
        builder.Services.AddTransient<MenuViewModel>();
        builder.Services.AddTransient<ObrasViewModel>();
        builder.Services.AddTransient<EmpleadosViewModel>();
        builder.Services.AddTransient<MaterialesViewModel>();
        builder.Services.AddTransient<AvancesViewModel>();
        builder.Services.AddTransient<ReporteObrasViewModel>();
        builder.Services.AddTransient<ReportePersonalViewModel>();
        builder.Services.AddTransient<ReporteInventarioViewModel>();
        builder.Services.AddTransient<ReporteAvancesViewModel>();

        // REGISTRO DE PÁGINAS / VISTAS
        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<CotizacionPage>();
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<RegistroPage>();
        builder.Services.AddTransient<MenuPage>();
        builder.Services.AddTransient<ObrasPage>();
        builder.Services.AddTransient<EmpleadosPage>();
        builder.Services.AddTransient<MaterialesPage>();
        builder.Services.AddTransient<AvancesPage>();
        builder.Services.AddTransient<ReportePage>();
        builder.Services.AddTransient<OdsPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
