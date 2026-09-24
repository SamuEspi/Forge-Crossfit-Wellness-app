using CrossfitWellnessApp.Services;

namespace CrossfitWellnessApp;

public partial class MainPage : ContentPage
{
    private readonly DatabaseService _databaseService;

    public MainPage()
    {
        InitializeComponent();
        _databaseService = new DatabaseService();
    }

    // ============================================================
    // LOGIN
    // ============================================================
    private async void OnLoginClicked(object? sender, EventArgs e)
    {
        string correo = TxtCorreo.Text?.Trim() ?? string.Empty;
        string contrasena = TxtContrasena.Text ?? string.Empty;

        LblError.IsVisible = false;
        LblError.Text = string.Empty;

        if (string.IsNullOrWhiteSpace(correo) || string.IsNullOrWhiteSpace(contrasena))
        {
            LblError.Text = "Por favor, completa todos los campos.";
            LblError.IsVisible = true;
            return;
        }

        if (!correo.Contains('@') || !correo.Contains('.'))
        {
            LblError.Text = "Ingresa un correo electrónico válido.";
            LblError.IsVisible = true;
            return;
        }

        try
        {
            BtnLogin.IsEnabled = false;
            BtnLogin.Text = "VALIDANDO...";

            bool esValido = await _databaseService.ValidarLogin(correo, contrasena);

            if (!esValido)
            {
                await DisplayAlertAsync("Acceso denegado",
                                        "Correo o contraseña incorrectos. Inténtalo de nuevo.",
                                        "Aceptar");
                return;
            }

            // 👇 GUARDAMOS LA SESIÓN ANTES DE NAVEGAR 👇
            var usuarioId = await _databaseService.ObtenerUsuarioIdPorCorreo(correo);
            var nombre = await _databaseService.ObtenerNombrePorCorreo(correo);

            SessionService.UsuarioId = usuarioId;
            SessionService.CorreoUsuario = correo;
            SessionService.NombreUsuario = nombre ?? "Atleta FORGE";

            // Si ya tiene perfil, cargamos su objetivo para futuras consultas
            if (usuarioId.HasValue)
            {
                var perfil = await _databaseService.ObtenerPerfilCorporal(usuarioId.Value);
                if (perfil.HasValue)
                {
                    SessionService.ObjetivoFisico = perfil.Value.objetivo;
                }
            }

            await DisplayAlertAsync("¡Bienvenido a FORGE!",
                                    $"Hola {SessionService.NombreUsuario}. ¡A entrenar!",
                                    "Continuar");

            // Navegación al Dashboard envuelto en NavigationPage
            NavegarA(new NavigationPage(new DashboardPage()));
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error",
                                    $"Ocurrió un problema al iniciar sesión: {ex.Message}",
                                    "Aceptar");
        }
        finally
        {
            BtnLogin.IsEnabled = true;
            BtnLogin.Text = "ENTRAR";
        }
    }

    // ============================================================
    // IR A REGISTRO
    // ============================================================
    private void OnRegisterClicked(object? sender, EventArgs e)
    {
        NavegarA(new RegisterPage());
    }

    // ============================================================
    // NAVEGACIÓN GENÉRICA
    // ============================================================
    private static void NavegarA(Page destino)
    {
        var ventanaActiva = Application.Current?.Windows.Count > 0
            ? Application.Current.Windows[0]
            : null;

        if (ventanaActiva is not null)
        {
            ventanaActiva.Page = destino;
        }
    }
}