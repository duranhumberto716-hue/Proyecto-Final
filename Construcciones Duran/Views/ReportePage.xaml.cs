using Proyecto_Final.ViewModels;

namespace Proyecto_Final.Views;

/// <summary>
/// Pantalla común para los 4 formularios de información.
/// Según el parámetro "tipo" de la ruta (ReportePage?tipo=clientes) carga el ViewModel
/// del integrante correspondiente.
/// </summary>
public partial class ReportePage : ContentPage, IQueryAttributable
{
    private readonly IServiceProvider _servicios;
    private ReporteViewModel? _viewModel;

    public ReportePage(IServiceProvider servicios)
    {
        InitializeComponent();
        _servicios = servicios;
    }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        var tipo = query.TryGetValue("tipo", out var valor) ? valor?.ToString() : null;

        _viewModel = tipo switch
        {
            "inventario" => _servicios.GetRequiredService<ReporteInventarioViewModel>(),
            "personal" => _servicios.GetRequiredService<ReportePersonalViewModel>(),
            "proveedores" => _servicios.GetRequiredService<ReporteProveedoresViewModel>(),
            _ => _servicios.GetRequiredService<ReporteClientesViewModel>()
        };

        BindingContext = _viewModel;
        await _viewModel.ActualizarAsync();
    }
}

