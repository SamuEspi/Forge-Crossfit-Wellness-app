using CrossfitWellnessApp.Models;
using CrossfitWellnessApp.Services;

namespace CrossfitWellnessApp;

public partial class FavoritosPage : ContentPage
{
    private readonly DatabaseService _databaseService;
    private List<FavoritoItem> _favoritos = new();

    public FavoritosPage()
    {
        InitializeComponent();
        _databaseService = new DatabaseService();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await CargarFavoritos();
    }

    private async Task CargarFavoritos()
    {
        if (SessionService.UsuarioId is not Guid usuarioId)
        {
            _favoritos = new List<FavoritoItem>();
            CvFavoritos.ItemsSource = _favoritos;
            return;
        }

        var rutinas = await _databaseService.ObtenerRutinasFavoritas(usuarioId);
        var recetas = await _databaseService.ObtenerRecetasFavoritas(usuarioId);

        _favoritos = rutinas.Concat(recetas)
            .OrderByDescending(f => f.FechaAgregado)
            .ToList();

        CvFavoritos.ItemsSource = _favoritos;
    }

    private async void OnFavoritoTapped(object? sender, TappedEventArgs e)
    {
        if (sender is Border border && border.BindingContext is FavoritoItem fav)
        {
            if (fav.Tipo == "Rutina" && fav.RutinaId.HasValue)
            {
                var todasLasRutinas = await _databaseService.ObtenerTodasLasRutinas();
                var rutina = todasLasRutinas.FirstOrDefault(r => r.Id == fav.RutinaId.Value);
                if (rutina is null) return;

                var ejercicios = await _databaseService.ObtenerEjerciciosDeRutina(rutina.Id);
                await Navigation.PushAsync(new DetalleRutinaPage(rutina, ejercicios));
            }
            else if (fav.Tipo == "Receta")
            {
                await DisplayAlertAsync("Receta",
                                        $"Receta: {fav.Titulo}\n{fav.Subtitulo}",
                                        "Aceptar");
            }
        }
    }

    private async void OnEliminarTapped(object? sender, TappedEventArgs e)
    {
        if (sender is not Border border || border.BindingContext is not FavoritoItem fav)
            return;

        if (SessionService.UsuarioId is not Guid usuarioId) return;

        bool confirmar = await DisplayAlertAsync("Eliminar favorito",
                                            $"¿Quitar \"{fav.Titulo}\" de tus favoritos?",
                                            "Sí", "Cancelar");
        if (!confirmar) return;

        bool exito = false;

        if (fav.Tipo == "Rutina" && fav.RutinaId.HasValue)
        {
            exito = await _databaseService.QuitarRutinaFavorita(usuarioId, fav.RutinaId.Value);
        }

        if (exito)
        {
            await CargarFavoritos();
        }
    }

    private async void OnVolverClicked(object? sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}