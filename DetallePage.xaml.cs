namespace CrossfitWellnessApp;

public partial class DetallePage : ContentPage
{
    public DetallePage(string titulo, string subtitulo, string contenido)
    {
        InitializeComponent();

        // Asignación segura contra referencias nulas (CS8602 / IDE0031)
        LblTitulo.Text = string.IsNullOrWhiteSpace(titulo)
            ? "Sin título"
            : titulo;

        LblSubtitulo.Text = string.IsNullOrWhiteSpace(subtitulo)
            ? string.Empty
            : subtitulo;

        LblContenido.Text = string.IsNullOrWhiteSpace(contenido)
            ? "No hay contenido disponible para mostrar."
            : contenido;
    }

    private async void OnVolverClicked(object? sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}