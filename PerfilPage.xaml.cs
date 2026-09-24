using CrossfitWellnessApp.Services;

namespace CrossfitWellnessApp;

public partial class PerfilPage : ContentPage
{
    private readonly DatabaseService _databaseService;

    public PerfilPage()
    {
        InitializeComponent();
        _databaseService = new DatabaseService();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        LblSaludo.Text = $"Hola, {SessionService.NombreUsuario ?? "Atleta"}";

        // Si el usuario ya tiene perfil, precargarlo
        if (SessionService.UsuarioId is Guid usuarioId)
        {
            var perfil = await _databaseService.ObtenerPerfilCorporal(usuarioId);
            if (perfil.HasValue)
            {
                TxtPeso.Text = perfil.Value.peso.ToString("0.00");
                TxtAltura.Text = perfil.Value.altura.ToString("0.00");
                TxtEdad.Text = perfil.Value.edad.ToString();

                // Seleccionar el objetivo en el Picker
                int idx = -1;
                var items = PkrObjetivo.ItemsSource as IList<string>;
                if (items is not null)
                {
                    for (int i = 0; i < items.Count; i++)
                    {
                        if (items[i] == perfil.Value.objetivo) { idx = i; break; }
                    }
                }
                if (idx >= 0) PkrObjetivo.SelectedIndex = idx;

                ActualizarImc();
            }
        }
    }

    private void ActualizarImc()
    {
        if (decimal.TryParse(TxtPeso.Text, out var peso) &&
            decimal.TryParse(TxtAltura.Text, out var altura) &&
            altura > 0)
        {
            var imc = peso / (altura * altura);
            LblImc.Text = $"{imc:0.00} kg/m²";
        }
        else
        {
            LblImc.Text = "—";
        }
    }

    private async void OnGuardarClicked(object? sender, EventArgs e)
    {
        LblErrorPerfil.IsVisible = false;
        LblErrorPerfil.Text = string.Empty;

        if (SessionService.UsuarioId is not Guid usuarioId)
        {
            await DisplayAlertAsync("Sesión expirada",
                                    "Vuelve a iniciar sesión para guardar tu perfil.",
                                    "Aceptar");
            return;
        }

        // Validaciones
        if (!decimal.TryParse(TxtPeso.Text, out var peso) || peso <= 0 || peso > 400)
        {
            LblErrorPerfil.Text = "Ingresa un peso válido entre 1 y 400 kg.";
            LblErrorPerfil.IsVisible = true;
            return;
        }

        if (!decimal.TryParse(TxtAltura.Text, out var altura) || altura <= 0 || altura > 3)
        {
            LblErrorPerfil.Text = "Ingresa una altura válida entre 0.5 y 3 m.";
            LblErrorPerfil.IsVisible = true;
            return;
        }

        if (!int.TryParse(TxtEdad.Text, out var edad) || edad <= 0 || edad >= 120)
        {
            LblErrorPerfil.Text = "Ingresa una edad válida entre 1 y 119 años.";
            LblErrorPerfil.IsVisible = true;
            return;
        }

        if (PkrObjetivo.SelectedItem is not string objetivo)
        {
            LblErrorPerfil.Text = "Selecciona tu objetivo físico.";
            LblErrorPerfil.IsVisible = true;
            return;
        }

        try
        {
            BtnGuardar.IsEnabled = false;
            BtnGuardar.Text = "GUARDANDO...";

            bool exito = await _databaseService.GuardarPerfilCorporal(
                usuarioId, peso, altura, edad, objetivo);

            if (!exito)
            {
                await DisplayAlertAsync("Error",
                                        "No se pudo guardar el perfil. Intenta de nuevo.",
                                        "Aceptar");
                return;
            }

            // Actualizamos la sesión
            SessionService.ObjetivoFisico = objetivo;

            await DisplayAlertAsync("¡Perfil guardado!",
                                    "Tus datos han sido actualizados correctamente.",
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
            BtnGuardar.Text = "GUARDAR PERFIL";
        }
    }

    private async void OnVolverClicked(object? sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}