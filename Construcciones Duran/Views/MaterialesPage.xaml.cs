using Proyecto_Final.ViewModels;

namespace Proyecto_Final.Views;

/// <summary>CRUD de Materiales - Integrante 2.</summary>
public partial class MaterialesPage : ContentPage
{
    private readonly MaterialesViewModel _viewModel;

    public MaterialesPage(MaterialesViewModel viewModel)
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

