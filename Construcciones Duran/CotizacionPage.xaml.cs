using Microsoft.Extensions.DependencyInjection;
using Proyecto_Final.Services;
using Proyecto_Final.ViewModels;

namespace Proyecto_Final;

public partial class CotizacionPage : ContentPage
{
    public CotizacionPage(CotizacionViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    public CotizacionPage() : this(
        IPlatformApplication.Current?.Services?.GetService<CotizacionViewModel>()
        ?? new CotizacionViewModel(new WhatsAppService()))
    {
    }
}

