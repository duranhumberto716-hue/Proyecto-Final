using Proyecto_Final.ViewModels;

namespace Proyecto_Final.Views;

/// <summary>CRUD de Empleados - Integrante 3.</summary>
public partial class EmpleadosPage : ContentPage
{
    private readonly EmpleadosViewModel _viewModel;

    public EmpleadosPage(EmpleadosViewModel viewModel)
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

