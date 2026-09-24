using CrossfitWellnessApp.Services;

namespace CrossfitWellnessApp;

public partial class RegisterPage : ContentPage
{
    private readonly DatabaseService _databaseService;

    public RegisterPage()
    {
        InitializeComponent();
        _databaseService = new DatabaseService();
    }

    private async void OnRegisterSubmitClicked(object? sender, EventArgs e)
    {
        string nombre = TxtNombre.Text?.Trim() ?? string.Empty;
        string correo = TxtNuevoCorreo.Text?.Trim() ?? string.Empty;
        string contrasena = TxtNuevaContrasena.Text ?? string.Empty;
        string confirmar = TxtConfirmarContrasena.Text ?? string.Empty;

        LblErrorRegistro.IsVisible = false;
        LblErrorRegistro.Text = string.Empty;

        if (string.IsNullOrWhiteSpace(nombre) ||
            string.IsNullOrWhiteSpace(correo) ||
            string.IsNullOrWhiteSpace(contrasena))
        {
            LblErrorRegistro.Text = "Por favor, completa todos los campos.";
            LblErrorRegistro.IsVisible = true;
            return;
        }

        if (!correo.Contains('@') || !correo.Contains('.'))
        {
            LblErrorRegistro.Text = "Ingresa un correo electrónico válido.";
            LblErrorRegistro.IsVisible = true;
            return;
        }

        if (contrasena.Length < 6)
        {
            LblErrorRegistro.Text = "La contraseña debe tener al menos 6 caracteres.";
            LblErrorRegistro.IsVisible = true;
            return;
        }

        if (contrasena != confirmar)
        {
            LblErrorRegistro.Text = "Las contraseñas no coinciden.";
            LblErrorRegistro.IsVisible = true;
            return;
        }

        try
        {
            BtnCrearCuenta.IsEnabled = false;
            BtnCrearCuenta.Text = "CREANDO...";

            bool exito = await _databaseService.RegistrarUsuario(nombre, correo, contrasena);

            if (!exito)
            {
                await DisplayAlertAsync("Registro fallido",
                                        "No se pudo crear la cuenta. Puede que el correo ya esté registrado.",
                                        "Aceptar");
                return;
            }

            await DisplayAlertAsync("¡Cuenta creada!",
                                    "Bienvenido a FORGE. Ahora puedes iniciar sesión.",
                                    "Continuar");

            VolverAlLogin();
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error",
                                    $"Ocurrió un problema al registrarte: {ex.Message}",
                                    "Aceptar");
        }
        finally
        {
            BtnCrearCuenta.IsEnabled = true;
            BtnCrearCuenta.Text = "REGISTRARME";
        }
    }

    private void OnBackToLoginClicked(object? sender, EventArgs e)
    {
        VolverAlLogin();
    }

    private static void VolverAlLogin()
    {
        var ventanaActiva = Application.Current?.Windows.Count > 0
            ? Application.Current.Windows[0]
            : null;

        if (ventanaActiva is not null)
        {
            ventanaActiva.Page = new MainPage();
        }
    }
}