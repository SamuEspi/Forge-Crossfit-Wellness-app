# ⚡ FORGE

Aplicación móvil de entrenamiento CrossFit y nutrición deportiva.

## Descripción

FORGE es una app Android desarrollada en .NET MAUI que combina entrenamiento funcional, guía multimedia de ejercicios, planes nutricionales y seguimiento de progreso en una sola aplicación.

## Problema que aborda

Los atletas de CrossFit suelen usar múltiples aplicaciones para entrenar, contar calorías, ver rutinas y registrar su progreso. FORGE soluciona esto centralizando todas esas funciones en una sola app, con una experiencia visual premium y datos siempre disponibles desde la nube.

## Área de wellness

**Entrenamiento CrossFit y Nutrición Deportiva.**

## Área de wellness

**Entrenamiento CrossFit y Nutrición Deportiva.**

## Objetivo

Ofrecer al atleta una herramienta integral que le permita:
- Consultar su entrenamiento del día según el día de la semana.
- Aprender la técnica correcta de cada ejercicio con guías visuales.
- Seguir planes nutricionales adaptados a su objetivo.
- Registrar su progreso diario (peso, agua, calorías, WOD completado).
- Acceder a consejos de bienestar filtrados por categoría.

## Funcionalidades principales

- **Autenticación:** registro e inicio de sesión con correo y contraseña.
- **Dashboard dinámico:** muestra el WOD del día según el día actual.
- **Catálogo de ejercicios:** 12 ejercicios de CrossFit con imágenes/GIFs animados.
- **Calendario semanal:** rutinas organizadas de lunes a viernes.
- **Registro diario:** peso, agua, calorías y WOD completado.
- **Perfil corporal:** peso, altura, edad, objetivo físico y cálculo automático de IMC.
- **Planes alimenticios:** Low-Carb y Keto Protocol.
- **Consejos de bienestar:** tips filtrables por Salud, Entrenamiento y Nutrición.
- **Favoritos:** marcar rutinas como favoritas.

## Tecnologías

- .NET MAUI (net10.0-android)
- C# 12
- XAML
- PostgreSQL 16 (Aiven Cloud)
- Npgsql
- SSL/TLS + BCrypt

## Arquitectura / diseño

Cliente-servidor en 3 capas:

1. **Presentación:** 12 pantallas XAML en .NET MAUI.
2. **Lógica:** `DatabaseService` (21 métodos asíncronos) + `SessionService`.
3. **Datos:** PostgreSQL 16 con 10 tablas normalizadas en 3FN.
## Pantallas

- Login y Registro
- Dashboard (WOD del día)
- Perfil Corporal
- Catálogo de Ejercicios
- Calendario Semanal
- Registro Diario
- Consejos de Bienestar
- Favoritos

## 📋 Requerimientos del proyecto

### Requerimientos Funcionales

| ID | Requerimiento | Estado | Detalle |
|----|---------------|--------|---------|
| REQ-01 | Gestión de usuarios | ✅ Completado | Login, registro y sesión persistente |
| REQ-02 | Perfil corporal | ✅ Completado | Formulario con peso, altura, edad, objetivo + cálculo de IMC |
| REQ-03 | Generación de rutinas | ✅ Completado | Dashboard dinámico según día de la semana |
| REQ-04 | Guía multimedia | ✅ Completado | Catálogo con imágenes y GIFs animados |
| REQ-05 | Registro de entrenamiento | ✅ Completado | Guarda peso, agua, calorías y WOD completado |
| REQ-06 | Calendario semanal | ✅ Completado | Lista de rutinas de Lunes a Viernes |
| REQ-07 | Notificaciones | ❌ No implementado | Fuera del alcance del MVP |
| REQ-08 | Recomendaciones nutricionales | 🟡 Parcial | Se muestran 2 planes (Low-Carb y Keto) con su detalle, pero **NO se filtran automáticamente según el objetivo del usuario**. La tabla `planes_alimenticios` ya contiene el campo `objetivo_asociado`, por lo que la implementación sería directa. |
| REQ-09 | Dietas especiales | ✅ Completado | Low-Carb y Keto con detalle completo |
| REQ-10 | Favoritos | ✅ Completado | Agregar, quitar y listar rutinas favoritas |
| REQ-11 | Consejos de bienestar | ✅ Completado | Tips con filtro por categoría (Salud, Entrenamiento, Nutrición) |
| REQ-12 | Registro de ingesta | ✅ Completado | Agua y calorías en el registro diario |
| REQ-13 | Panel de estadísticas | ❌ No implementado | Los datos se guardan en `registros_diarios` pero no se grafican |

### Requerimientos No Funcionales

| ID | Requerimiento | Estado |
|----|---------------|--------|
| REQ-14 | Disponibilidad 99% | ✅ Completado (Aiven Cloud con SLA) |
| REQ-15 | Seguridad de datos | ✅ Completado (SSL + BCrypt + parametrización) |
| REQ-16 | Rendimiento < 3 seg | ✅ Completado (índices + async/await) |
| REQ-17 | Compatibilidad Android | ✅ Completado (APK firmado) |
| REQ-18 | Escalabilidad | ✅ Completado (arquitectura cliente-servidor) |

**Cobertura funcional: 10 de 13 completados (77%)**
**Cobertura total: 15 de 18 completados (83%)**

---

### ⚠️ Nota sobre REQ-08 (Recomendaciones nutricionales)

El requerimiento REQ-08 **no está completamente implementado**. Lo que sí existe:

- ✅ Se muestran 2 planes alimenticios (Low-Carb y Keto) en el Dashboard.
- ✅ Cada plan tiene su pantalla de detalle con información completa.
- ✅ La tabla `planes_alimenticios` incluye el campo `objetivo_asociado` con valores como "Pérdida de peso" o "Aumento muscular".

- ❌ El Dashboard **no filtra automáticamente** los planes según el `objetivo_fisico` del usuario.
- ❌ Un usuario con objetivo "Aumento muscular" ve los mismos planes que uno con "Pérdida de peso".
- ❌ No hay suficiente variedad para cubrir todos los casos
  
# PRUEBAS DEL PROYECTO FORGE

Documentación de las pruebas realizadas durante el desarrollo.

## Pruebas funcionales

| # | Prueba | Resultado esperado | Resultado obtenido |
|---|--------|-------------------|---------------------|
| 1 | Login con credenciales válidas | Redirigir al Dashboard | ✅ Exitoso |
| 2 | Login con credenciales inválidas | Mostrar alerta de error | ✅ Exitoso |
| 3 | Registro de nuevo usuario | Crear cuenta y volver al login | ✅ Exitoso |
| 4 | Registro con correo existente | Mostrar error de duplicado | ✅ Exitoso |
| 5 | Guardar perfil corporal | UPSERT en la BD | ✅ Exitoso |
| 6 | Ver WOD del día | Cargar rutina según el día actual | ✅ Exitoso |
| 7 | Guardar registro diario | UPSERT por usuario/fecha | ✅ Exitoso |
| 8 | Marcar rutina como favorita | Insertar en tabla favoritos | ✅ Exitoso |
| 9 | Quitar rutina de favoritos | DELETE de favoritos | ✅ Exitoso |
| 10 | Filtrar consejos por categoría | Mostrar solo la categoría | ✅ Exitoso |
| 11 | Cargar imágenes de ejercicios | Mostrar imágenes/GIFs | ✅ Exitoso |
| 12 | Compilar e instalar APK en Android | App funcional en dispositivo | ✅ Exitoso |

## Pruebas de conexión a la nube

| # | Prueba | Resultado |
|---|--------|-----------|
| 1 | Conexión SSL a Aiven | ✅ Exitosa |
| 2 | Login desde red externa | ✅ Exitoso |
| 3 | Consultas con parámetros | ✅ Sin inyección SQL |
| 4 | UPSERT de perfil y registro diario | ✅ Sin duplicados |

## Pruebas de manejo de errores

| # | Prueba | Resultado |
|---|--------|-----------|
| 1 | Login con campos vacíos | ✅ Muestra error inline |
| 2 | Sin conexión a internet | ✅ Muestra try/catch |
| 3 | Cierre inesperado de la app | ✅ No pierde sesión |

## Conclusión

Todas las pruebas fueron ejecutadas manualmente durante el desarrollo.
La aplicación cumple con los requerimientos funcionales establecidos.

## Decisiones técnicas

**¿Por qué .NET MAUI?**
- Usa C#, un lenguaje maduro y fuertemente tipado.
- Integración con Visual Studio.
- Conexión natural con PostgreSQL mediante Npgsql.
- Una sola base de código para Android, iOS, Windows y macOS.

**¿Por qué PostgreSQL?**
- Mejor soporte para UUID, restricciones complejas y tipos avanzados.
- PostgreSQL 16 es robusto y gratuito.
- Aiven Cloud ofrece un servicio administrado con SSL.

**¿Por qué Aiven Cloud?**
- Base de datos accesible desde cualquier lugar (no depende de servidor local).
- Backups automáticos y alta disponibilidad.
- SLA del 99.9%.

## UML de caso de usos de Forge

<img width="400" height="597" alt="image" src="https://github.com/user-attachments/assets/b01b99d2-f72b-4107-97a5-db5eec812d62" />

## IA utilizada

Se utilizó asistencia de IA durante el desarrollo para:
- Consultas técnicas sobre .NET MAUI y PostgreSQL.
- Revisión de buenas prácticas de seguridad.

## Cómo ejecutar el proyecto

1. Clonar el repositorio.
2. Copiar `Services/DbConfig.example.cs` a `Services/DbConfig.cs`.
3. Rellenar las credenciales de PostgreSQL en ese archivo.
4. Abrir `CrossfitWellnessApp.slnx` en Visual Studio.
5. Compilar → Limpiar solución.
6. Compilar → Reconstruir solución.
7. Ejecutar en emulador Android o dispositivo físico.

# 🏁 CIERRE DEL PROYECTO FORGE

Documentación final del proyecto FORGE — Entrenamiento CrossFit y Nutrición Deportiva.

---

## 🏆 Logros alcanzados

### Logros técnicos

- **Aplicación móvil funcional en Android** desarrollada en .NET MAUI con C# 12.
- **12 pantallas** completamente operativas con diseño premium consistente.
- **Backend robusto** con `DatabaseService` (21 métodos asíncronos) y `SessionService`.
- **Base de datos PostgreSQL 16** con **10 tablas normalizadas en 3FN**, alojada en Aiven Cloud.
- **Conexión cifrada con SSL/TLS** entre la app y la base de datos.
- **APK firmado** listo para instalación en dispositivos Android.

### Logros funcionales

- **15 de 18 requerimientos completados**.
- **Dashboard dinámico** que adapta el WOD según el día de la semana.
- **Catálogo de 12 ejercicios** con imágenes y GIFs animados.
- **Sistema de autenticación** con validaciones completas.
- **Registro diario** con operación UPSERT (sin duplicados por usuario y fecha).
- **Sistema de favoritos** para rutinas y recetas.
- **Consejos de bienestar** filtrables por categoría.

### Logros de proceso

- **Documentación completa** en el repositorio (README, requisitos, pruebas).
- **Historial Git** con commits descriptivos y trazabilidad del desarrollo.
- **Buenas prácticas** de seguridad (SSL, BCrypt, consultas parametrizadas).
- **Uso de IA** como apoyo durante el desarrollo y la documentación.

---

## ⚠️ Dificultades encontradas y soluciones adoptadas

| # | Dificultad | Solución adoptada |
|---|------------|-------------------|
| 1 | **Errores de compilación XAML** (`XLS0414`, `CS0103`) | Se corrigió la vinculación entre archivos `.xaml` y `.xaml.cs`, y se limpiaron cachés de compilación. |
| 2 | **Imágenes y GIFs no cargaban** en el catálogo | Se verificaron las URLs y se reemplazaron por enlaces directos a archivos `.gif` (no páginas HTML). |
| 3 | **Migración de PostgreSQL local a Aiven Cloud** | Se configuró SSL obligatorio con `SslMode=Require` y se resolvieron problemas de firewall. |
| 4 | **Bloqueo de puerto por VPN de Opera** | Se desactivó la VPN y se verificó la conectividad con `Test-NetConnection`. |
| 5 | **APK no se instalaba en BlueStacks** | Se firmó el APK con un keystore propio en modo Release. |
| 6 | **Programación asíncrona** al inicio resultaba confusa | Se migró todo el acceso a BD a `async/await` y se eliminaron los `.Result` bloqueantes. |
| 7 | **Métodos obsoletos** (`DisplayAlert`) | Se migró a `DisplayAlertAsync` (versión moderna de .NET MAUI). |
| 8 | **Credenciales expuestas** en el código | Se movieron a un archivo `DbConfig.cs` ignorado por `.gitignore`. |
| 9 | **Complejidad de la POO aplicada** a servicios asíncronos | Se estudiaron los 4 pilares y se aplicaron deliberadamente en el código. |
| 10 | **Gestión del tiempo** con múltiples requerimientos | Se priorizaron los de Alta prioridad y se documentaron los pendientes. |

---

## 🧱 Aplicación de principios de POO

### 1. Encapsulamiento

- La cadena de conexión está declarada como `private const` en `DatabaseService`.
- Las propiedades de los modelos usan `{ get; set; }` (acceso controlado).
- Los métodos de `SessionService` protegen los datos del usuario.

**Archivo:** `Services/DatabaseService.cs`, `Models/*.cs`

### 2. Herencia

- Las **12 pantallas** heredan de `ContentPage` (`: ContentPage`).
- Cada pantalla obtiene automáticamente: `OnAppearing()`, `DisplayAlertAsync()`, `Navigation`, `Title`, etc.

**Archivo:** Todos los `.xaml.cs`

### 3. Polimorfismo

- Cada pantalla sobrescribe `OnAppearing()` con `override` para hacer algo diferente.
- El método `NavegarA(Page destino)` acepta cualquier tipo de página (subclase de `Page`).

**Archivo:** `DashboardPage.xaml.cs`, `MainPage.xaml.cs`, etc.

### 4. Abstracción

- `DatabaseService` oculta toda la complejidad de PostgreSQL en 21 métodos simples.
- Las pantallas solo llaman a `_databaseService.ObtenerX()` sin saber cómo funciona por dentro.

**Archivo:** `Services/DatabaseService.cs`

### Principios SOLID aplicados

- **S (Single Responsibility):** cada clase hace una sola cosa.
- **O (Open/Closed):** `DatabaseService` se puede extender sin modificar.
- **L (Liskov):** cualquier subclase de `ContentPage` puede usarse donde se espera `Page`.
- **I (Interface Segregation):** los métodos del servicio son específicos.
- **D (Dependency Inversion):** las páginas instancian `DatabaseService` (inyección).

### Patrones de diseño aplicados

FORGE implementa **4 patrones de diseño** reales y verificables en el código:

| # | Patrón | Dónde se aplica |
|---|--------|-----------------|
| 1 | **MVC** (Model-View-Controller) | Carpeta `Models/`, archivos `.xaml` (View) y `.xaml.cs` (Controller) |
| 2 | **Repository** | `Services/DatabaseService.cs` — centraliza el acceso a datos |
| 3 | **Singleton (static)** | `Services/SessionService.cs` — clase estática con una única instancia |
| 4 | **Dependency Injection** | Constructor de cada pantalla (`_databaseService = new DatabaseService()`) |

---

## 🚀 Recomendaciones para futuras versiones

### Corto plazo (semanas)

- Implementar **rate limiting** en el login para prevenir ataques de fuerza bruta.
- Añadir **logs persistentes** de acciones en la base de datos.

### Mediano plazo (meses)

- Implementar el filtrado automático de planes alimenticios según el `objetivo_fisico` del usuario. La tabla `planes_alimenticios` ya tiene el campo `objetivo_asociado`, así que solo hace falta la consulta filtrada y la lógica en el Dashboard.
- Implementar **panel de estadísticas** con gráficos (`Microcharts.Maui`) para cubrir el REQ-13.
- Añadir **notificaciones push** con `Plugin.LocalNotification` para cubrir el REQ-07.
- Implementar **autenticación de dos factores (2FA)**.
### Largo plazo (producción)

- **Certificación SSL con dominio propio.**
- **Cifrado en reposo** de la base de datos.
- **Caché offline** con SQLite para uso sin conexión.
- **Pruebas de penetración** externas.
- **Certificación ISO 27001** (estándar de seguridad de la información).

---

## 🚢 Despliegue realizado

### Backend

- **Base de datos:** PostgreSQL 16 alojada en **Aiven Cloud**.
- **Conexión:** SSL/TLS obligatorio (`SslMode=Require`).
- **Backups:** automáticos por parte de Aiven.
- **SLA:** 99.9% de disponibilidad.

### Frontend

- **Framework:** .NET MAUI (net10.0-android).
- **Empaquetado:** APK firmado con keystore propio.
- **Distribución:** instalación manual en dispositivos Android (no publicado en Play Store).

---

## 🛠️ Mantenimiento

### Recomendaciones de mantenimiento

- **Actualizar dependencias NuGet** periódicamente (especialmente Npgsql y MAUI).
- **Revisar logs de errores** en Aiven Console cada 2 semanas.
- **Rotar credenciales** de la base de datos cada 6 meses.
- **Renovar el keystore** solo si es necesario (una vez perdido, no se recupera).
- **Backups manuales** mensuales de la base de datos.

### Monitorización

- **Uptime de Aiven:** verificar mensualmente en la consola.
- **Errores en la app:** revisar los `Debug.WriteLine` en logs.
- **Rendimiento:** el sistema responde en menos de 3 segundos.

---

## 🎓 Conclusiones

### Conclusión técnica

FORGE demuestra que es posible construir una **aplicación móvil real, funcional y desplegada** usando tecnologías modernas como **.NET MAUI**, **C# 12** y **PostgreSQL**. La arquitectura cliente-servidor con separación en 3 capas facilita el mantenimiento y la escalabilidad.

### Conclusión de proceso

El proyecto permitió aplicar **buenas prácticas de Ingeniería de Software**: análisis de requerimientos, diseño UML, arquitectura por capas, control de versiones con Git, pruebas y documentación.

### Conclusión sobre POO

Se aplicaron los **4 pilares de la programación orientada a objetos** (encapsulamiento, herencia, polimorfismo y abstracción) junto con los **principios SOLID**, resultando en un código limpio, mantenible y extensible.

### Conclusión personal

FORGE **no es un prototipo académico**: es una aplicación real, desplegada en Android, con backend en la nube, con diseño premium y con controles de seguridad implementados. **Estamos orgullosos del resultado y conscientes de las mejoras pendientes.**

### Cierre

El proyecto culmina con una **app funcional, documentada y desplegada**. Las recomendaciones planteadas sirven como hoja de ruta para futuras versiones, y el código queda disponible para evolucionar en otras asignaturas, proyectos personales o portafolio profesional.

---

## ⚠️ Nota de seguridad

Las credenciales que puedan aparecer en commits históricos ya fueron 
**rotadas o revocadas en Aiven**, por lo que **no representan un riesgo 
funcional**. Las credenciales actuales se cargan desde `DbConfig.cs`, 
que está incluido en `.gitignore`.

## 📅 Fecha

2026

## 📄 Licencia

Proyecto académico — Ingeniería de Software

## Equipo #2

- Samuel Espinoza C.I. 31.458.819
- Gustavo Chacin C.I. 28.406.955
- Norluis Cubillan C.I. 28.497.772

## Año

2026

##Evidencias / capturas
<img width="397" height="694" alt="01-login" src="https://github.com/user-attachments/assets/ce330e88-11f5-4437-ac8f-5f907ca27b5c" />
<img width="399" height="691" alt="02-dashboard" src="https://github.com/user-attachments/assets/ca6a86fd-8070-4bad-8214-1ff0ff35683c" />
<img width="394" height="702" alt="04-calendario" src="https://github.com/user-attachments/assets/b806cca7-09e6-4467-9467-6298a9d00f33" />
<img width="391" height="699" alt="03-catalogo" src="https://github.com/user-attachments/assets/50c077fb-d608-40ac-b7cb-75e523f1c484" />
<img width="401" height="690" alt="06-bienestar" src="https://github.com/user-attachments/assets/14e3bdc3-9b83-479e-91ad-6cf6d5b7453a" />
<img width="399" height="704" alt="05-registro-diario" src="https://github.com/user-attachments/assets/d11fdc67-141f-45b3-baa7-6ae2dd4030c1" />

