using CrossfitWellnessApp.Models;
using CrossfitWellnessApp.Services;

namespace CrossfitWellnessApp;

public partial class DashboardPage : ContentPage
{
    private readonly DatabaseService _databaseService;

    // Guardamos la rutina cargada para cuando el usuario toque la tarjeta
    private Rutina? _rutinaActual;
    private List<EjercicioDeRutina> _ejerciciosActuales = new();

    public DashboardPage()
    {
        InitializeComponent();
        _databaseService = new DatabaseService();
    }

    // ============================================================
    // AL APARECER LA PÁGINA → CARGAR RUTINA DEL DÍA
    // ============================================================
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await CargarRutinaDelDia();
    }

    // ============================================================
    // CARGAR LA RUTINA DEL DÍA DESDE POSTGRESQL
    // ============================================================
    private async Task CargarRutinaDelDia()
    {
        try
        {
            // 1) Calcular el día de la semana en español
            string diaSemana = ObtenerDiaSemanaEnEspanol(DateTime.Now.DayOfWeek);

            LblEncabezadoWod.Text = $"🏋️ Entrenamiento del Día · {diaSemana}";

            // 2) Nivel del usuario (por defecto "Principiante")
            string nivel = "Principiante";

            // 3) Pedir la rutina a la BD
            var rutina = await _databaseService.ObtenerRutinaDelDia(diaSemana, nivel);

            if (rutina is null)
            {
                LblNombreRutina.Text = "Día de descanso";
                LblNivelDia.Text = "No hay rutina programada para hoy.";
                StackEjerciciosResumen.Children.Clear();
                _rutinaActual = null;
                _ejerciciosActuales = new List<EjercicioDeRutina>();
                return;
            }

            // 4) Obtener los ejercicios de la rutina
            var ejercicios = await _databaseService.ObtenerEjerciciosDeRutina(rutina.Id);

            _rutinaActual = rutina;
            _ejerciciosActuales = ejercicios;

            // 5) Poblar la UI
            LblNombreRutina.Text = rutina.NombreRutina;
            LblNivelDia.Text = $"Nivel: {rutina.NivelDificultad} · {rutina.DiaSemana}";

            StackEjerciciosResumen.Children.Clear();

            foreach (var ej in ejercicios)
            {
                var label = new Label
                {
                    Text = $"• {ej.Nombre}  —  {ej.Series} × {ej.Repeticiones}",
                    FontSize = 14,
                    TextColor = Color.FromArgb("#C9C9C9"),
                    LineBreakMode = LineBreakMode.TailTruncation,
                    MaxLines = 2
                };
                StackEjerciciosResumen.Children.Add(label);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[FORGE] Error CargarRutinaDelDia: {ex.Message}");
            LblNombreRutina.Text = "Error al cargar";
            LblNivelDia.Text = "No se pudo obtener la rutina del día.";
        }
    }

    // ============================================================
    // CONVERSOR DE DÍA A ESPAÑOL
    // ============================================================
    private static string ObtenerDiaSemanaEnEspanol(DayOfWeek dia)
    {
        return dia switch
        {
            DayOfWeek.Monday => "Lunes",
            DayOfWeek.Tuesday => "Martes",
            DayOfWeek.Wednesday => "Miércoles",
            DayOfWeek.Thursday => "Jueves",
            DayOfWeek.Friday => "Viernes",
            DayOfWeek.Saturday => "Sábado",
            DayOfWeek.Sunday => "Domingo",
            _ => "Lunes"
        };
    }

    // ============================================================
    // IR AL PERFIL CORPORAL
    // ============================================================
    private async void OnPerfilTapped(object? sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new PerfilPage());
    }

    // ============================================================
    // IR AL CATÁLOGO DE EJERCICIOS
    // ============================================================
    private async void OnEjerciciosTapped(object? sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new EjerciciosPage());
    }

    // ============================================================
    // IR AL CALENDARIO SEMANAL
    // ============================================================
    private async void OnCalendarioTapped(object? sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new CalendarioPage());
    }

    // ============================================================
    // IR AL REGISTRO DIARIO
    // ============================================================
    private async void OnRegistroTapped(object? sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new RegistroDiarioPage());
    }

    // ============================================================
    // IR A CONSEJOS DE BIENESTAR
    // ============================================================
    private async void OnBienestarTapped(object? sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new BienestarPage());
    }

    // ============================================================
    // IR A FAVORITOS
    // ============================================================
    private async void OnFavoritosTapped(object? sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new FavoritosPage());
    }

    // ============================================================
    // TARJETA: WOD DEL DÍA (abre el detalle de la rutina real)
    // ============================================================
    private async void OnWodTapped(object? sender, TappedEventArgs e)
    {
        if (_rutinaActual is null)
        {
            await DisplayAlertAsync("Sin rutina",
                                    "No hay una rutina programada para hoy.",
                                    "Aceptar");
            return;
        }

        await Navigation.PushAsync(new DetalleRutinaPage(_rutinaActual, _ejerciciosActuales));
    }

    // ============================================================
    // TARJETA: PLAN LOW-CARB
    // ============================================================
    private async void OnLowCarbTapped(object? sender, TappedEventArgs e)
    {
        const string titulo = "🥑 Plan Low-Carb Moderno";
        const string subtitulo = "Déficit controlado · energía estable";

        const string contenido =
            "El Plan Low-Carb Moderno de FORGE está diseñado para atletas que buscan " +
            "reducir grasa corporal sin sacrificar rendimiento ni energía diaria.\n\n" +
            "🔹 CARBOHIDRATOS: Menos de 100 g netos al día, priorizando vegetales de " +
            "hoja verde, brócoli, espárragos y calabacín.\n\n" +
            "🔹 PROTEÍNAS: 1.8 a 2.2 g por kg de peso corporal. Fuentes magras como " +
            "pechuga de pollo, pavo, pescado blanco y claras de huevo.\n\n" +
            "🔹 GRASAS SALUDABLES: Aguacate, aceite de oliva virgen extra, frutos secos " +
            "naturales (almendra, nuez, pistacho) y semillas de chía.\n\n" +
            "🍽️ EJEMPLO DE DÍA:\n" +
            "• Desayuno: Tortilla de 3 huevos con espinacas y queso feta (280 kcal).\n" +
            "• Media mañana: Puñado de almendras + café solo.\n" +
            "• Almuerzo: Ensalada de pollo a la plancha con aguacate y aceite de oliva.\n" +
            "• Merienda: Yogur griego natural sin azúcar.\n" +
            "• Cena: Salmón al horno con espárragos y ensalada verde.\n\n" +
            "💧 HIDRATACIÓN: Mínimo 2.5 litros de agua al día. Añade electrolitos los " +
            "días de WOD intenso para evitar calambres.\n\n" +
            "⚡ RECOMENDACIÓN FORGE: Combínalo con entrenamientos metabólicos (AMRAP, " +
            "EMOM) y sesiones de fuerza para preservar masa muscular durante la definición.";

        await Navigation.PushAsync(new DetallePage(titulo, subtitulo, contenido));
    }

    // ============================================================
    // TARJETA: KETO PROTOCOL
    // ============================================================
    private async void OnKetoTapped(object? sender, TappedEventArgs e)
    {
        const string titulo = "🥩 Keto Protocol";
        const string subtitulo = "Alta grasa · baja en carbohidratos";

        const string contenido =
            "El Keto Protocol de FORGE es una estrategia nutricional avanzada que " +
            "induce al cuerpo a un estado metabólico de cetosis, donde la grasa se " +
            "convierte en la principal fuente de energía.\n\n" +
            "🔹 PRINCIPIO BÁSICO: Menos de 20-25 g de carbohidratos netos al día para " +
            "entrar y mantener la cetosis.\n\n" +
            "🔹 DISTRIBUCIÓN DE MACROS:\n" +
            "• 70-75% de las calorías provienen de grasas saludables.\n" +
            "• 20-25% de proteínas de alta calidad biológica.\n" +
            "• 5% o menos de carbohidratos.\n\n" +
            "🍳 ALIMENTOS PERMITIDOS:\n" +
            "• Carnes rojas, cerdo, pollo con piel, pescados grasos (salmón, atún, sardina).\n" +
            "• Huevos enteros, mantequilla, quesos curados, nata.\n" +
            "• Aceite de coco, aceite de oliva, aguacate, frutos secos.\n" +
            "• Vegetales bajos en carbohidratos: espinaca, coliflor, brócoli, calabacín.\n\n" +
            "🚫 ALIMENTOS PROHIBIDOS:\n" +
            "Arroz, pasta, pan, papa, frutas dulces, azúcar, miel, legumbres.\n\n" +
            "⚠️ ADVERTENCIA FORGE: Durante los primeros 7-10 días puede aparecer la " +
            "llamada 'gripe keto' (fatiga, dolor de cabeza). Aumenta el consumo de " +
            "agua y electrolitos (sodio, potasio, magnesio) para minimizar los síntomas.\n\n" +
            "⚡ RECOMENDACIÓN FORGE: Este plan es ideal para fases cortas de 4 a 8 " +
            "semanas. Para atletas de CrossFit con alto volumen de entrenamiento, " +
            "considera ciclar carbohidratos (targeted keto) los días de WOD intenso.";

        await Navigation.PushAsync(new DetallePage(titulo, subtitulo, contenido));
    }
}