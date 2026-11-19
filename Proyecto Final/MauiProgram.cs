using Microsoft.Extensions.Logging;
using Proyecto_Final.Services;
using Proyecto_Final.ViewModels;

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

        // REGISTRO DE VIEWMODELS
        builder.Services.AddTransient<MainViewModel>();
        builder.Services.AddTransient<CotizacionViewModel>();

        // REGISTRO DE PÁGINAS / VISTAS
        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<CotizacionPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
