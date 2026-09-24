using CrossfitWellnessApp.Models;

namespace CrossfitWellnessApp;

public partial class DetalleRutinaPage : ContentPage
{
    public DetalleRutinaPage(Rutina rutina, List<EjercicioDeRutina> ejercicios)
    {
        InitializeComponent();

        LblNombreRutina.Text = string.IsNullOrWhiteSpace(rutina.NombreRutina)
            ? "Rutina del día"
            : rutina.NombreRutina;

        LblNivel.Text = string.IsNullOrWhiteSpace(rutina.NivelDificultad)
            ? "—"
            : rutina.NivelDificultad;

        LblDia.Text = string.IsNullOrWhiteSpace(rutina.DiaSemana)
            ? "—"
            : rutina.DiaSemana;

        CvEjercicios.ItemsSource = ejercicios;
    }

    private async void OnVolverClicked(object? sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}