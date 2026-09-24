using CrossfitWellnessApp.Models;
using CrossfitWellnessApp.Services;

namespace CrossfitWellnessApp;

public partial class RegistroDiarioPage : ContentPage
{
    private readonly DatabaseService _databaseService;
    private List<Rutina> _rutinas = new();

    public RegistroDiarioPage()
    {
        InitializeComponent();
        _databaseService = new DatabaseService();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Mostrar la fecha actual
        LblFecha.Text = $"📅 {DateTime.Now:dddd, d 'de' MMMM 'de' yyyy}";
        LblSaludo.Text = $"Hola, {SessionService.NombreUsuario ?? "Atleta"}";

        // Cargar rutinas para el picker
        await CargarRutinas();

        // Cargar registro del día (si ya existe) para precargar campos
        await CargarRegistroDelDia();
    }

    private async Task CargarRutinas()
    {
        _rutinas = await _databaseService.ObtenerTodasLasRutinas();

        var nombres = _rutinas
            .Select(r => $"{r.DiaSemana} · {r.NombreRutina}")
            .ToList();

        PkrRutina.ItemsSource = nombres;
    }

    private async Task CargarRegistroDelDia()
    {
        if (SessionService.UsuarioId is not Guid usuarioId) return;

        var registro = await _databaseService.ObtenerRegistroDelDia(usuarioId);

        if (registro is null) return;

        // Precargar peso
        if (registro.Value.peso.HasValue)
        {
            TxtPeso.Text = registro.Value.peso.Value.ToString("0.00");
        }

        // Precargar agua
        if (registro.Value.agua > 0)
        {
            TxtAgua.Text = registro.Value.agua.ToString();
        }

        // Precargar calorías
        if (registro.Value.calorias > 0)
        {
            TxtCalorias.Text = registro.Value.calorias.ToString();
        }

        // Precargar WOD si existe
        if (registro.Value.rutinaId.HasValue)
        {
            int idx = _rutinas.FindIndex(r => r.Id == registro.Value.rutinaId.Value);
            if (idx >= 0) PkrRutina.SelectedIndex = idx;
        }

        // Precargar notas
        if (!string.IsNullOrWhiteSpace(registro.Value.notas))
        {
            TxtNotas.Text = registro.Value.notas;
        }
    }

    private async void OnGuardarClicked(object? sender, EventArgs e)
    {
        LblError.IsVisible = false;
        LblError.Text = string.Empty;

        if (SessionService.UsuarioId is not Guid usuarioId)
        {
            await DisplayAlertAsync("Sesión expirada",
                                    "Vuelve a iniciar sesión para guardar tu registro.",
                                    "Aceptar");
            return;
        }

        // Validaciones (todas opcionales excepto coherencia)
        decimal? peso = null;
        if (!string.IsNullOrWhiteSpace(TxtPeso.Text))
        {
            if (!decimal.TryParse(TxtPeso.Text, out var p) || p <= 0 || p > 400)
            {
                LblError.Text = "El peso debe estar entre 1 y 400 kg.";
                LblError.IsVisible = true;
                return;
            }
            peso = p;
        }

        int agua = 0;
        if (!string.IsNullOrWhiteSpace(TxtAgua.Text))
        {
            if (!int.TryParse(TxtAgua.Text, out agua) || agua < 0 || agua > 10000)
            {
                LblError.Text = "El agua debe estar entre 0 y 10000 ml.";
                LblError.IsVisible = true;
                return;
            }
        }

        int calorias = 0;
        if (!string.IsNullOrWhiteSpace(TxtCalorias.Text))
        {
            if (!int.TryParse(TxtCalorias.Text, out calorias) || calorias < 0 || calorias > 20000)
            {
                LblError.Text = "Las calorías deben estar entre 0 y 20000.";
                LblError.IsVisible = true;
                return;
            }
        }

        Guid? rutinaId = null;
        if (PkrRutina.SelectedIndex >= 0 && PkrRutina.SelectedIndex < _rutinas.Count)
        {
            rutinaId = _rutinas[PkrRutina.SelectedIndex].Id;
        }

        string? notas = string.IsNullOrWhiteSpace(TxtNotas.Text) ? null : TxtNotas.Text.Trim();

        try
        {
            BtnGuardar.IsEnabled = false;
            BtnGuardar.Text = "GUARDANDO...";

            bool exito = await _databaseService.GuardarRegistroDiario(
                usuarioId, peso, agua, calorias, rutinaId, notas);

            if (!exito)
            {
                await DisplayAlertAsync("Error",
                                        "No se pudo guardar el registro. Intenta de nuevo.",
                                        "Aceptar");
                return;
            }

            await DisplayAlertAsync("¡Registro guardado!",
                                    "Tu progreso del día ha sido registrado correctamente.",
                                    "Continuar");
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error",
                                    $"Ocurrió un problema: {ex.Message}",
                                    "Aceptar");
        }
        finally
        {
            BtnGuardar.IsEnabled = true;
            BtnGuardar.Text = "GUARDAR REGISTRO";
        }
    }

    private async void OnVolverClicked(object? sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}