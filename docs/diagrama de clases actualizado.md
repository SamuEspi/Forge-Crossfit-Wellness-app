# 📊 DIAGRAMA DE CLASES — FORGE

Diagrama de clases UML del sistema FORGE, correspondiente al estado final del proyecto.

---

## 🎯 Diagrama general

```mermaid
classDiagram
    %% ============================================
    %% CLASES BASE DE MAUI
    %% ============================================
    class ContentPage {
        <<abstract>>
        +OnAppearing()
        +OnDisappearing()
        +DisplayAlertAsync()
        +Navigation
        +Title
        +BackgroundColor
        +InitializeComponent()
    }

    %% ============================================
    %% PANTALLAS (heredan de ContentPage)
    %% ============================================
    class MainPage {
        -DatabaseService _databaseService
        -OnLoginClicked()
        -OnRegisterClicked()
        -NavegarA(Page)
    }
    class RegisterPage {
        -DatabaseService _databaseService
        -OnRegisterSubmitClicked()
        -OnBackToLoginClicked()
    }
    class DashboardPage {
        -DatabaseService _databaseService
        -Rutina _rutinaActual
        -List~EjercicioDeRutina~ _ejerciciosActuales
        -CargarRutinaDelDia()
        -OnPerfilTapped()
        -OnWodTapped()
        -OnEjerciciosTapped()
        -OnCalendarioTapped()
        -OnRegistroTapped()
        -OnBienestarTapped()
        -OnFavoritosTapped()
    }
    class PerfilPage {
        -DatabaseService _databaseService
        -OnGuardarClicked()
        -OnVolverClicked()
        -ActualizarImc()
    }
    class EjerciciosPage {
        -DatabaseService _databaseService
        -OnEjercicioTapped()
        -OnVolverClicked()
    }
    class DetalleEjercicioPage {
        +DetalleEjercicioPage(Ejercicio)
        -OnVolverClicked()
    }
    class CalendarioPage {
        -DatabaseService _databaseService
        -OnRutinaTapped()
        -OnToggleFavoritoTapped()
        -OnVolverClicked()
    }
    class DetalleRutinaPage {
        +DetalleRutinaPage(Rutina, List~EjercicioDeRutina~)
        -OnVolverClicked()
    }
    class RegistroDiarioPage {
        -DatabaseService _databaseService
        -List~Rutina~ _rutinas
        -OnGuardarClicked()
        -OnVolverClicked()
        -CargarRutinas()
        -CargarRegistroDelDia()
    }
    class BienestarPage {
        -DatabaseService _databaseService
        -CargarConsejos()
        -OnFiltroTodosClicked()
        -OnFiltroSaludClicked()
        -OnConsejoTapped()
    }
    class FavoritosPage {
        -DatabaseService _databaseService
        -List~FavoritoItem~ _favoritos
        -CargarFavoritos()
        -OnFavoritoTapped()
        -OnEliminarTapped()
    }
    class DetallePage {
        +DetallePage(string, string, string)
        -OnVolverClicked()
    }

    %% ============================================
    %% SERVICIOS
    %% ============================================
    class DatabaseService {
        -string ConnectionString
        +ValidarLogin(string, string) Task
        +RegistrarUsuario(string, string, string) Task
        +ObtenerUsuarioIdPorCorreo(string) Task
        +ObtenerNombrePorCorreo(string) Task
        +ObtenerPerfilCorporal(Guid) Task
        +GuardarPerfilCorporal(Guid, decimal, decimal, int, string) Task
        +ObtenerTodosLosEjercicios() Task
        +ObtenerEjercicioPorId(Guid) Task
        +ObtenerRutinaDelDia(string, string) Task
        +ObtenerEjerciciosDeRutina(Guid) Task
        +ObtenerTodasLasRutinas() Task
        +GuardarRegistroDiario(Guid, decimal, int, int, Guid, string) Task
        +ObtenerRegistroDelDia(Guid) Task
        +ObtenerHistorialRegistros(Guid, int) Task
        +ObtenerConsejosBienestar() Task
        +ObtenerConsejosPorCategoria(string) Task
        +AgregarRutinaFavorita(Guid, Guid) Task
        +QuitarRutinaFavorita(Guid, Guid) Task
        +EsRutinaFavorita(Guid, Guid) Task
        +ObtenerRutinasFavoritas(Guid) Task
        +ObtenerRecetasFavoritas(Guid) Task
    }
    class SessionService {
        +Guid UsuarioId
        +string NombreUsuario
        +string CorreoUsuario
        +string ObjetivoFisico
        +bool EstaAutenticado
        +CerrarSesion()
    }

    %% ============================================
    %% MODELOS
    %% ============================================
    class Ejercicio {
        +Guid Id
        +string Nombre
        +string Descripcion
        +string GrupoMuscular
        +string UrlMultimedia
    }
    class Rutina {
        +Guid Id
        +string NombreRutina
        +string NivelDificultad
        +string DiaSemana
    }
    class EjercicioDeRutina {
        +Guid EjercicioId
        +string Nombre
        +string Descripcion
        +string GrupoMuscular
        +string UrlMultimedia
        +int Series
        +string Repeticiones
    }
    class ConsejoBienestar {
        +Guid Id
        +string Titulo
        +string Contenido
        +string Categoria
        +DateTime FechaPublicacion
    }
    class FavoritoItem {
        +Guid Id
        +Guid RutinaId
        +Guid RecetaId
        +string Tipo
        +string Titulo
        +string Subtitulo
        +DateTime FechaAgregado
    }

    %% ============================================
    %% RELACIONES DE HERENCIA
    %% ============================================
    ContentPage <|-- MainPage
    ContentPage <|-- RegisterPage
    ContentPage <|-- DashboardPage
    ContentPage <|-- PerfilPage
    ContentPage <|-- EjerciciosPage
    ContentPage <|-- DetalleEjercicioPage
    ContentPage <|-- CalendarioPage
    ContentPage <|-- DetalleRutinaPage
    ContentPage <|-- RegistroDiarioPage
    ContentPage <|-- BienestarPage
    ContentPage <|-- FavoritosPage
    ContentPage <|-- DetallePage

    %% ============================================
    %% RELACIONES DE USO
    %% ============================================
    MainPage ..> DatabaseService
    MainPage ..> SessionService
    RegisterPage ..> DatabaseService
    DashboardPage ..> DatabaseService
    DashboardPage ..> SessionService
    PerfilPage ..> DatabaseService
    PerfilPage ..> SessionService
    EjerciciosPage ..> DatabaseService
    CalendarioPage ..> DatabaseService
    CalendarioPage ..> SessionService
    RegistroDiarioPage ..> DatabaseService
    RegistroDiarioPage ..> SessionService
    BienestarPage ..> DatabaseService
    FavoritosPage ..> DatabaseService
    FavoritosPage ..> SessionService

    %% ============================================
    %% RELACIONES CON MODELOS
    %% ============================================
    DashboardPage ..> Rutina
    DashboardPage ..> EjercicioDeRutina
    EjerciciosPage ..> Ejercicio
    DetalleEjercicioPage ..> Ejercicio
    DetalleRutinaPage ..> Rutina
    DetalleRutinaPage ..> EjercicioDeRutina
    BienestarPage ..> ConsejoBienestar
    FavoritosPage ..> FavoritoItem
    RegistroDiarioPage ..> Rutina
