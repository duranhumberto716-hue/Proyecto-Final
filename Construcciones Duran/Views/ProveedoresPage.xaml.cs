using Proyecto_Final.ViewModels;

namespace Proyecto_Final.Views;

/// <summary>CRUD de Proveedores - Integrante 4.</summary>
public partial class ProveedoresPage : ContentPage
{
    private readonly ProveedoresViewModel _viewModel;

    public ProveedoresPage(ProveedoresViewModel viewModel)
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

