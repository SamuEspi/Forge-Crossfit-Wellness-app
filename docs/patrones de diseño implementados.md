# 🎨 PATRONES DE DISEÑO IMPLEMENTADOS EN FORGE

Documentación de los patrones de diseño aplicados en el proyecto FORGE, correspondientes al estado final del sistema.

---

## 📌 Introducción

En FORGE se aplicaron **4 patrones de diseño** de forma deliberada durante el desarrollo. No se documentan patrones adicionales porque no fueron implementados en el código. Los patrones aquí descritos son verificables directamente en el repositorio.

---

## 1️⃣ Patrón MVC (Model-View-Controller)

### ¿Qué es?

Separa la aplicación en tres componentes:
- **Model:** los datos y la lógica de negocio.
- **View:** la interfaz visual.
- **Controller:** la lógica que conecta el modelo con la vista.

### ¿Dónde se aplica en FORGE?

| Componente | Dónde está | Ejemplo |
|------------|------------|---------|
| **Model** | Carpeta `Models/` | `Ejercicio.cs`, `Rutina.cs`, `ConsejoBienestar.cs`, `FavoritoItem.cs` |
| **View** | Archivos `.xaml` | `DashboardPage.xaml`, `PerfilPage.xaml`, `EjerciciosPage.xaml` |
| **Controller** | Archivos `.xaml.cs` | `DashboardPage.xaml.cs`, `PerfilPage.xaml.cs`, `EjerciciosPage.xaml.cs` |

### Ejemplo en el código

- **View (`DashboardPage.xaml`):** define la interfaz con `Label`, `Border`, `CollectionView`.
- **Controller (`DashboardPage.xaml.cs`):** maneja la lógica (`OnAppearing`, `CargarRutinaDelDia`).
- **Model (`Rutina.cs`):** representa una rutina con sus propiedades.

### Beneficio

- Separación clara de responsabilidades.
- Fácil de mantener y extender.
- Cada capa se puede modificar sin afectar las otras.

---

## 2️⃣ Patrón Repository

### ¿Qué es?

Centraliza todo el acceso a datos en una única clase, ocultando la complejidad del motor de base de datos y proporcionando métodos específicos para cada operación.

### ¿Dónde se aplica en FORGE?

En la clase **`DatabaseService.cs`**, que contiene los **21 métodos** que acceden a PostgreSQL:

| Método | Propósito |
|--------|-----------|
| `ValidarLogin` | Autenticación |
| `RegistrarUsuario` | Crear usuario |
| `ObtenerRutinaDelDia` | Obtener rutina del día |
| `GuardarRegistroDiario` | Guardar progreso |
| `ObtenerTodosLosEjercicios` | Catálogo |
| `AgregarRutinaFavorita` | Favoritos |
| ... (21 en total) | ... |

### Ejemplo en el código

**Sin el patrón Repository**, cada pantalla tendría que abrir su propia conexión:

```csharp
// ❌ Sin Repository
using var conn = new NpgsqlConnection(ConnectionString);
await conn.OpenAsync();
const string sql = "SELECT ...";
// ... 10 líneas más
```

**Con el patrón Repository**, cada pantalla usa métodos simples:

```csharp
// ✅ Con Repository
var rutina = await _databaseService.ObtenerRutinaDelDia(dia, nivel);
```

### Beneficio

- Punto único de acceso a datos.
- Fácil de cambiar el motor de base de datos.
- Las pantallas no saben nada de SQL ni de PostgreSQL.

---

## 3️⃣ Patrón Singleton (clase estática)

### ¿Qué es?

Garantiza que exista **una sola instancia** de una clase en toda la aplicación y proporciona un punto de acceso global a ella.

### ¿Dónde se aplica en FORGE?

En la clase **`SessionService.cs`**, que se declara como `static`:

```csharp
public static class SessionService
{
    public static Guid? UsuarioId { get; set; }
    public static string? NombreUsuario { get; set; }
    public static string? CorreoUsuario { get; set; }
    public static string? ObjetivoFisico { get; set; }

    public static void CerrarSesion() { ... }
}
```

### ¿Por qué es Singleton?

- **`static`** → se carga una sola vez en memoria.
- **No se puede instanciar** con `new`.
- Todos los accesos pasan por `SessionService.UsuarioId`, etc.

### Uso en el código

Cualquier pantalla puede acceder a la sesión:

```csharp
if (SessionService.UsuarioId is Guid usuarioId)
{
    var perfil = await _databaseService.ObtenerPerfilCorporal(usuarioId);
}
```

### Beneficio

- Una única fuente de verdad para los datos de sesión.
- Fácil acceso desde cualquier parte de la app.
- Sin necesidad de pasar el usuario entre pantallas.

---

## 4️⃣ Patrón Dependency Injection

### ¿Qué es?

En lugar de que una clase cree sus dependencias internamente, se las **inyecta** desde fuera (generalmente en el constructor).

### ¿Dónde se aplica en FORGE?

En **todas las pantallas** que necesitan `DatabaseService`:

```csharp
public partial class DashboardPage : ContentPage
{
    private readonly DatabaseService _databaseService;

    public DashboardPage()
    {
        InitializeComponent();
        _databaseService = new DatabaseService();  // ← Inyección
    }
}
```

### Ejemplo en múltiples pantallas

| Pantalla | Inyecta |
|----------|---------|
| `DashboardPage` | `DatabaseService` |
| `PerfilPage` | `DatabaseService` |
| `EjerciciosPage` | `DatabaseService` |
| `RegistroDiarioPage` | `DatabaseService` |
| `FavoritosPage` | `DatabaseService` |

### Beneficio

- Las pantallas no dependen de cómo se construye `DatabaseService`.
- Fácil de reemplazar por un mock en pruebas unitarias (en el futuro).
- Separación clara de responsabilidades.

---

## 📊 Resumen de patrones aplicados

| # | Patrón | Archivo(s) donde se aplica |
|---|--------|----------------------------|
| 1 | **MVC** | `Models/`, `*.xaml`, `*.xaml.cs` |
| 2 | **Repository** | `Services/DatabaseService.cs` |
| 3 | **Singleton (static)** | `Services/SessionService.cs` |
| 4 | **Dependency Injection** | Constructor de cada pantalla |

---

## 🎯 Aplicación de POO junto con los patrones

Los patrones de diseño se apoyan en los principios de POO:

| Pilar | Cómo se refleja en los patrones |
|-------|--------------------------------|
| **Encapsulamiento** | `DatabaseService.ConnectionString` es `private` |
| **Herencia** | Las 12 pantallas heredan de `ContentPage` |
| **Polimorfismo** | `OnAppearing()` sobrescrito con `override` |
| **Abstracción** | `DatabaseService` oculta PostgreSQL |

---

## 📅 Fecha

Octubre 2026

## 📄 Estado

Documento final — Patrones de diseño implementados en FORGE.
