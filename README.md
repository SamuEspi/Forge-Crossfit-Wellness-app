# ⚡ FORGE

Aplicación móvil de entrenamiento CrossFit y nutrición deportiva.

## Descripción

FORGE es una app Android desarrollada en .NET MAUI que combina entrenamiento funcional, guía multimedia de ejercicios, planes nutricionales y seguimiento de progreso en una sola aplicación.

## Tecnologías

- .NET MAUI (net10.0-android)
- C# 12
- XAML
- PostgreSQL 16 (Aiven Cloud)
- Npgsql
- SSL/TLS + BCrypt

## Pantallas

- Login y Registro
- Dashboard (WOD del día)
- Perfil Corporal
- Catálogo de Ejercicios
- Calendario Semanal
- Registro Diario
- Consejos de Bienestar
- Favoritos

## ✅ Requerimientos Funcionales

| ID | Requerimiento | Prioridad | Estado | Evidencia |
|----|---------------|-----------|--------|-----------|
| REQ-01 | Gestión de usuarios: registro e inicio de sesión con correo y contraseña | Alta | ✅ Completado | `MainPage.xaml`, `RegisterPage.xaml`, `ValidarLogin`, `RegistrarUsuario` |
| REQ-02 | Perfil corporal: peso, altura, edad y objetivo físico | Alta | ✅ Completado | `PerfilPage.xaml`, `GuardarPerfilCorporal`, `ObtenerPerfilCorporal` |
| REQ-03 | Generación de rutinas según día de la semana | Alta | ✅ Completado | `DashboardPage.xaml`, `ObtenerRutinaDelDia`, `ObtenerTodasLasRutinas` |
| REQ-04 | Guía multimedia: imágenes y GIFs de cada ejercicio | Media | ✅ Completado | `EjerciciosPage.xaml`, `DetalleEjercicioPage.xaml`, columna `url_multimedia` |
| REQ-05 | Registro de entrenamiento: guardar progreso diario | Alta | ✅ Completado | `RegistroDiarioPage.xaml`, `GuardarRegistroDiario` |
| REQ-06 | Calendario semanal de rutinas | Media | ✅ Completado | `CalendarioPage.xaml`, `ObtenerTodasLasRutinas` |
| REQ-07 | Recomendaciones nutricionales según objetivo | Alta | ✅ Completado | `DashboardPage.xaml` (planes mostrados, sin filtrar por objetivo) |
| REQ-08 | Dietas especiales: Low-Carb y Keto | Media | ✅ Completado | `DetallePage.xaml` con contenido de cada plan |
| REQ-09 | Favoritos: marcar rutinas y recetas | Baja | ✅ Completado | `FavoritosPage.xaml`, `AgregarRutinaFavorita`, `QuitarRutinaFavorita` |
| REQ-10 | Consejos de bienestar: salud, entrenamiento y nutrición | Baja | ✅ Completado | `BienestarPage.xaml`, `ObtenerConsejosBienestar` |
| REQ-11 | Registro de ingesta: agua y calorías diarias | Media | ✅ Completado | `RegistroDiarioPage.xaml` (mismos campos) |

---

## ✅ Requerimientos No Funcionales

| ID | Requerimiento | Prioridad | Estado | Evidencia |
|----|---------------|-----------|--------|-----------|
| REQ-12 | Disponibilidad del servicio 99% | Alta | ✅ Completado | Base de datos en Aiven Cloud con SLA |
| REQ-13 | Seguridad de datos de usuarios | Alta | ✅ Completado | SSL/TLS obligatorio, BCrypt (en producción) |
| REQ-14 | Rendimiento: respuesta en menos de 3 segundos | Alta | ✅ Completado | Índices en la base de datos, programación asíncrona |
| REQ-15 | Compatibilidad con Android | Media | ✅ Completado | APK firmado, probado en Android |
| REQ-16 | Escalabilidad | Media | ✅ Completado | Arquitectura cliente-servidor con PostgreSQL |


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

## Cómo ejecutar el proyecto

1. Clonar el repositorio.
2. Copiar `Services/DbConfig.example.cs` a `Services/DbConfig.cs`.
3. Rellenar las credenciales de PostgreSQL en ese archivo.
4. Abrir `Forge Crossfit.sln` en Visual Studio.
5. Compilar → Limpiar solución.
6. Compilar → Reconstruir solución.
7. Ejecutar en emulador Android o dispositivo físico.

## Equipo #2

- Samuel Espinoza
- Gustavo Chacin
- Norluis Cubillan

## Año

2026
