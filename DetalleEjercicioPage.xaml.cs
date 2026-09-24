using CrossfitWellnessApp.Models;

namespace CrossfitWellnessApp;

public partial class DetalleEjercicioPage : ContentPage
{
    public DetalleEjercicioPage(Ejercicio ejercicio)
    {
        InitializeComponent();

        LblNombre.Text = string.IsNullOrWhiteSpace(ejercicio.Nombre)
            ? "Ejercicio"
            : ejercicio.Nombre;

        LblGrupoMuscular.Text = string.IsNullOrWhiteSpace(ejercicio.GrupoMuscular)
            ? "Grupo no especificado"
            : ejercicio.GrupoMuscular;

        LblDescripcion.Text = string.IsNullOrWhiteSpace(ejercicio.Descripcion)
            ? "Sin descripción disponible."
            : ejercicio.Descripcion;

        // Cargar imagen (con protección contra URL nula)
        if (!string.IsNullOrWhiteSpace(ejercicio.UrlMultimedia))
        {
            ImgEjercicio.Source = ejercicio.UrlMultimedia;
        }
        else
        {
            ImgEjercicio.Source = "https://upload.wikimedia.org/wikipedia/commons/8/86/Crossfit_Gym.jpg";
        }
    }

    private async void OnVolverClicked(object? sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}