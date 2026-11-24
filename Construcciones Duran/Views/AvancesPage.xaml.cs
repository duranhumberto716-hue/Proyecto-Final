using Proyecto_Final.ViewModels;

namespace Proyecto_Final.Views;

/// <summary>CRUD de Avances de obra - Integrante 4.</summary>
public partial class AvancesPage : ContentPage
{
    private readonly AvancesViewModel _viewModel;

    public AvancesPage(AvancesViewModel viewModel)
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

