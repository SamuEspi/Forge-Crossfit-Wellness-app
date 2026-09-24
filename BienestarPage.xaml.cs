using CrossfitWellnessApp.Models;
using CrossfitWellnessApp.Services;

namespace CrossfitWellnessApp;

public partial class BienestarPage : ContentPage
{
    private readonly DatabaseService _databaseService;

    public BienestarPage()
    {
        InitializeComponent();
        _databaseService = new DatabaseService();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await CargarConsejos();
    }

    // ============================================================
    // CARGAR TODOS LOS CONSEJOS
    // ============================================================
    private async Task CargarConsejos()
    {
        var consejos = await _databaseService.ObtenerConsejosBienestar();
        CvConsejos.ItemsSource = consejos;
        ActualizarBotonesFiltro("Todos");
    }

    // ============================================================
    // CARGAR CONSEJOS FILTRADOS
    // ============================================================
    private async Task CargarConsejosFiltrados(string categoria)
    {
        var consejos = await _databaseService.ObtenerConsejosPorCategoria(categoria);
        CvConsejos.ItemsSource = consejos;
        ActualizarBotonesFiltro(categoria);
    }

    // ============================================================
    // ACTUALIZAR ESTADO VISUAL DE LOS BOTONES DE FILTRO
    // ============================================================
    private void ActualizarBotonesFiltro(string categoriaActiva)
    {
        var activoBg = Color.FromArgb("#9FE870");
        var activoText = Color.FromArgb("#0A0A0A");
        var inactivoBg = Color.FromArgb("#141414");
        var inactivoText = Color.FromArgb("#9FE870");

        void SetEstado(Button btn, bool activo)
        {
            btn.BackgroundColor = activo ? activoBg : inactivoBg;
            btn.TextColor = activo ? activoText : inactivoText;
        }

        SetEstado(BtnTodos, categoriaActiva == "Todos");
        SetEstado(BtnSalud, categoriaActiva == "Salud");
        SetEstado(BtnEntrenamiento, categoriaActiva == "Entrenamiento");
        SetEstado(BtnNutricion, categoriaActiva == "Nutrición");
    }

    // ============================================================
    // FILTROS
    // ============================================================
    private async void OnFiltroTodosClicked(object? sender, EventArgs e)
    {
        await CargarConsejos();
    }

    private async void OnFiltroSaludClicked(object? sender, EventArgs e)
    {
        await CargarConsejosFiltrados("Salud");
    }

    private async void OnFiltroEntrenamientoClicked(object? sender, EventArgs e)
    {
        await CargarConsejosFiltrados("Entrenamiento");
    }

    private async void OnFiltroNutricionClicked(object? sender, EventArgs e)
    {
        await CargarConsejosFiltrados("Nutrición");
    }

    // ============================================================
    // VER DETALLE DEL CONSEJO
    // ============================================================
    private async void OnConsejoTapped(object? sender, TappedEventArgs e)
    {
        if (sender is Border border && border.BindingContext is ConsejoBienestar consejo)
        {
            await Navigation.PushAsync(new DetallePage(
                consejo.Titulo,
                $"📖 {consejo.Categoria}",
                consejo.Contenido));
        }
    }

    private async void OnVolverClicked(object? sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}