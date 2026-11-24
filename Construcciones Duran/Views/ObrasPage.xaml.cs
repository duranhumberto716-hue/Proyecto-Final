using Proyecto_Final.ViewModels;

namespace Proyecto_Final.Views;

/// <summary>CRUD de Obras - Integrante 1.</summary>
public partial class ObrasPage : ContentPage
{
    private readonly ObrasViewModel _viewModel;

    public ObrasPage(ObrasViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.CargarAsync();
    }
}

