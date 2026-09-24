using CrossfitWellnessApp.Models;
using CrossfitWellnessApp.Services;

namespace CrossfitWellnessApp;

public partial class EjerciciosPage : ContentPage
{
    private readonly DatabaseService _databaseService;

    public EjerciciosPage()
    {
        InitializeComponent();
        _databaseService = new DatabaseService();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        var lista = await _databaseService.ObtenerTodosLosEjercicios();
        CvEjercicios.ItemsSource = lista;
    }

    private async void OnEjercicioTapped(object? sender, TappedEventArgs e)
    {
        if (sender is Border border && border.BindingContext is Ejercicio ejercicio)
        {
            await Navigation.PushAsync(new DetalleEjercicioPage(ejercicio));
        }
    }

    private async void OnVolverClicked(object? sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}