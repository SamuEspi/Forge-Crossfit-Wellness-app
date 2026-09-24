using CrossfitWellnessApp.Models;
using CrossfitWellnessApp.Services;

namespace CrossfitWellnessApp;

public partial class CalendarioPage : ContentPage
{
    private readonly DatabaseService _databaseService;

    public CalendarioPage()
    {
        InitializeComponent();
        _databaseService = new DatabaseService();
    }

    // ============================================================
    // AL APARECER LA PÁGINA → CARGAR TODAS LAS RUTINAS
    // ============================================================
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        var rutinas = await _databaseService.ObtenerTodasLasRutinas();
        CvRutinas.ItemsSource = rutinas;
    }

    // ============================================================
    // TOCAR UNA RUTINA → ABRIR SU DETALLE
    // ============================================================
    private async void OnRutinaTapped(object? sender, TappedEventArgs e)
    {
        if (sender is Border border && border.BindingContext is Rutina rutina)
        {
            var ejercicios = await _databaseService.ObtenerEjerciciosDeRutina(rutina.Id);
            await Navigation.PushAsync(new DetalleRutinaPage(rutina, ejercicios));
        }
    }

    // ============================================================
    // TOCAR EL CORAZÓN → AGREGAR/QUITAR DE FAVORITOS
    // ============================================================
    private async void OnToggleFavoritoTapped(object? sender, TappedEventArgs e)
    {
        if (sender is not Border border || border.BindingContext is not Rutina rutina)
            return;

        if (SessionService.UsuarioId is not Guid usuarioId)
        {
            await DisplayAlertAsync("Sesión expirada",
                                    "Inicia sesión para guardar favoritos.",
                                    "Aceptar");
            return;
        }

        bool esFavorita = await _databaseService.EsRutinaFavorita(usuarioId, rutina.Id);

        if (esFavorita)
        {
            await _databaseService.QuitarRutinaFavorita(usuarioId, rutina.Id);
            await DisplayAlertAsync("Quitado",
                                    $"\"{rutina.NombreRutina}\" ya no está en favoritos.",
                                    "Aceptar");
        }
        else
        {
            await _databaseService.AgregarRutinaFavorita(usuarioId, rutina.Id);
            await DisplayAlertAsync("¡Agregado!",
                                    $"\"{rutina.NombreRutina}\" se guardó en favoritos.",
                                    "Aceptar");
        }
    }

    // ============================================================
    // VOLVER AL DASHBOARD
    // ============================================================
    private async void OnVolverClicked(object? sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}