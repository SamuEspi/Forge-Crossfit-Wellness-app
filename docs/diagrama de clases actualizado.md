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
        +ValidarLogin(string, string) Task~bool~
        +RegistrarUsuario(string, string, string) Task~bool~
        +ObtenerUsuarioIdPorCorreo(string) Task~Guid?~
        +ObtenerNombrePorCorreo(string) Task~string?~
        +ObtenerPerfilCorporal(Guid) Task
        +GuardarPerfilCorporal(Guid, decimal, decimal, int, string) Task~bool~
        +ObtenerTodosLosEjercicios() Task~List~
        +ObtenerEjercicioPorId(Guid) Task
        +ObtenerRutinaDelDia(string, string) Task
        +ObtenerEjerciciosDeRutina(Guid) Task~List~
        +ObtenerTodasLasRutinas() Task~List~
        +GuardarRegistroDiario(Guid, decimal?, int, int, Guid?, string) Task~bool~
        +ObtenerRegistroDelDia(Guid) Task
        +ObtenerHistorialRegistros(Guid, int) Task~List~
        +ObtenerConsejosBienestar() Task~List~
        +ObtenerConsejosPorCategoria(string) Task~List~
        +AgregarRutinaFavorita(Guid, Guid) Task~bool~
        +QuitarRutinaFavorita(Guid, Guid) Task~bool~
        +EsRutinaFavorita(Guid, Guid) Task~bool~
        +ObtenerRutinasFavoritas(Guid) Task~List~
        +ObtenerRecetasFavoritas(Guid) Task~List~
    }
    class SessionService {
        <<static>>
        +Guid? UsuarioId
        +string? NombreUsuario
        +string? CorreoUsuario
        +string? ObjetivoFisico
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
        +string? UrlMultimedia
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
        +string? UrlMultimedia
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
        +Guid? RutinaId
        +Guid? RecetaId
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
    %% RELACIONES DE USO (las pantallas usan servicios)
    %% ============================================
    MainPage ..> DatabaseService : usa
    MainPage ..> SessionService : usa
    RegisterPage ..> DatabaseService : usa
    DashboardPage ..> DatabaseService : usa
    DashboardPage ..> SessionService : usa
    PerfilPage ..> DatabaseService : usa
    PerfilPage ..> SessionService : usa
    EjerciciosPage ..> DatabaseService : usa
    CalendarioPage ..> DatabaseService : usa
    CalendarioPage ..> SessionService : usa
    RegistroDiarioPage ..> DatabaseService : usa
    RegistroDiarioPage ..> SessionService : usa
    BienestarPage ..> DatabaseService : usa
    FavoritosPage ..> DatabaseService : usa
    FavoritosPage ..> SessionService : usa

    %% ============================================
    %% RELACIONES CON MODELOS
    %% ============================================
    DashboardPage ..> Rutina : contiene
    DashboardPage ..> EjercicioDeRutina : contiene
    EjerciciosPage ..> Ejercicio : contiene
    DetalleEjercicioPage ..> Ejercicio : contiene
    DetalleRutinaPage ..> Rutina : contiene
    DetalleRutinaPage ..> EjercicioDeRutina : contiene
    BienestarPage ..> ConsejoBienestar : contiene
    FavoritosPage ..> FavoritoItem : contiene
    RegistroDiarioPage ..> Rutina : contiene

    %% ============================================
    %% ESTILOS
    %% ============================================
    classDef base fill:#0A0A0A,stroke:#9FE870,stroke-width:2px,color:#9FE870
    classDef pantalla fill:#141414,stroke:#9FE870,stroke-width:1px,color:#FFFFFF
    classDef servicio fill:#1A2E1A,stroke:#9FE870,stroke-width:2px,color:#9FE870
    classDef modelo fill:#2E1A2E,stroke:#9FE870,stroke-width:1px,color:#FFFFFF

    class ContentPage base
    class MainPage,RegisterPage,DashboardPage,PerfilPage,EjerciciosPage,DetalleEjercicioPage,CalendarioPage,DetalleRutinaPage,RegistroDiarioPage,BienestarPage,FavoritosPage,DetallePage pantalla
    class DatabaseService,SessionService servicio
    class Ejercicio,Rutina,EjercicioDeRutina,ConsejoBienestar,FavoritoItem modelo
