using Microsoft.Extensions.DependencyInjection;
using Proyecto_Final.Services;
using Proyecto_Final.ViewModels;

namespace Proyecto_Final;

public partial class MainPage : ContentPage
{
    private readonly MainViewModel _viewModel;

    /// <summary>
    /// Constructor principal utilizado por el contenedor de Inyección de Dependencias (.NET MAUI).
    /// </summary>
    public MainPage(MainViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    /// <summary>
    /// Constructor sin parámetros para Shell o inicialización directa.
    /// </summary>
    public MainPage() : this(
        IPlatformApplication.Current?.Services?.GetService<MainViewModel>()
        ?? new MainViewModel(new ProyectoService(), new WhatsAppService()))
    {
    }

    /// <summary>
    /// Al aparecer la página en pantalla, se inicia la carga de las obras en la nube.
    /// </summary>
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_viewModel.Proyectos.Count == 0)
        {
            await _viewModel.CargarProyectosAsync();
        }
    }
}
