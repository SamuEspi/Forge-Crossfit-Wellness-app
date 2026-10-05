# 📋 REQUISITOS DEL PROYECTO FORGE

Documento de especificación de requerimientos funcionales, no funcionales, actores, necesidades y requisitos de datos del sistema FORGE.

---

## 🎯 Alcance del proyecto

FORGE es una aplicación móvil Android que integra entrenamiento funcional tipo CrossFit, guía multimedia de ejercicios, planes nutricionales y seguimiento de progreso físico, con backend en PostgreSQL alojado en la nube.

---

## 🧩 Necesidades identificadas

Los atletas de CrossFit suelen usar múltiples aplicaciones separadas para entrenar, contar calorías, ver rutinas y registrar su progreso. Esto genera:

- **Fragmentación:** datos dispersos en varias apps.
- **Pérdida de información:** registros que no se centralizan.
- **Falta de seguimiento integral:** no hay una visión unificada del progreso.
- **Dificultad para consultar rutinas:** las rutinas no están organizadas por día.
- **Falta de guía visual:** muchos ejercicios no tienen imágenes o GIFs explicativos.

FORGE resuelve estas necesidades centralizando **entrenamiento, nutrición, bienestar y progreso** en una sola aplicación con backend en la nube.

---

## 👥 Actores del sistema

| Actor | Rol | Interacción con el sistema |
|-------|-----|----------------------------|
| **Usuario (Atleta)** | Persona que usa la app | Se registra, inicia sesión, consulta rutinas, registra progreso, marca favoritos |
| **Administrador** | Carga datos maestros | Carga rutinas, ejercicios, planes alimenticios y consejos en la base de datos (implícito, no hay interfaz de admin en la app) |
| **Base de Datos** | Almacena información | Recibe consultas y devuelve datos. Gestiona integridad referencial |
| **Aiven Cloud** | Hosting de la BD | Provee el servicio PostgreSQL con SSL y respaldos automáticos |
| **Wikimedia / Tenor** | Fuente multimedia | Provee las imágenes y GIFs de los ejercicios vía URLs |

---

## 🎯 Funciones principales por actor

### Usuario (Atleta)

- Registrarse en la app.
- Iniciar y cerrar sesión.
- Consultar el WOD del día (entrenamiento diario).
- Explorar el catálogo completo de ejercicios con imágenes.
- Ver el detalle de un ejercicio (imagen grande + descripción).
- Consultar el calendario semanal con todas las rutinas.
- Ver el detalle de una rutina (ejercicios, series y repeticiones).
- Registrar peso, agua, calorías y WOD completado.
- Editar el perfil corporal (peso, altura, edad, objetivo).
- Marcar rutinas como favoritas.
- Leer consejos de bienestar filtrados por categoría.

### Administrador

- Cargar rutinas, ejercicios, planes y consejos en la base de datos.
- Actualizar la información existente.
- Gestionar la base de datos PostgreSQL.

### Base de Datos

- Almacenar todos los datos de usuarios, rutinas, ejercicios, planes y progreso.
- Garantizar integridad referencial mediante FOREIGN KEY.
- Aplicar restricciones de validación (CHECK, UNIQUE).
- Optimizar consultas mediante índices.

---

## ✅ Requerimientos Funcionales

| ID | Requerimiento | Prioridad | Estado |
|----|---------------|-----------|--------|
| REQ-01 | Gestión de usuarios: registro e inicio de sesión con correo y contraseña | Alta | ✅ Completado |
| REQ-02 | Perfil corporal: peso, altura, edad y objetivo físico | Alta | ✅ Completado |
| REQ-03 | Generación de rutinas según día de la semana | Alta | ✅ Completado |
| REQ-04 | Guía multimedia: imágenes y GIFs de cada ejercicio | Media | ✅ Completado |
| REQ-05 | Registro de entrenamiento: guardar progreso diario | Alta | ✅ Completado |
| REQ-06 | Calendario semanal de rutinas | Media | ✅ Completado |
| REQ-07 | Notificaciones de recordatorio | Media | ❌ No implementado |
| REQ-08 | Recomendaciones nutricionales por objetivo | Alta | 🟡 Parcial | Se muestran planes pero no se filtran por objetivo del usuario y no hay mucha variedad para recomendar|
| REQ-09 | Dietas especiales: Low-Carb y Keto | Media | ✅ Completado |
| REQ-10 | Favoritos: marcar rutinas y recetas | Baja | ✅ Completado |
| REQ-11 | Consejos de bienestar: salud, entrenamiento y nutrición | Baja | ✅ Completado |
| REQ-12 | Registro de ingesta: agua y calorías diarias | Media | ✅ Completado |
| REQ-13 | Panel de estadísticas del progreso | Media | ❌ No implementado |

---

## ✅ Requerimientos No Funcionales

| ID | Requerimiento | Prioridad | Estado |
|----|---------------|-----------|--------|
| REQ-14 | Disponibilidad del servicio 99% | Alta | ✅ Completado |
| REQ-15 | Seguridad de datos: SSL, BCrypt, consultas parametrizadas | Alta | ✅ Completado |
| REQ-16 | Rendimiento: respuesta en menos de 3 segundos | Alta | ✅ Completado |
| REQ-17 | Compatibilidad con Android | Media | ✅ Completado |
| REQ-18 | Escalabilidad del sistema | Media | ✅ Completado |

---

## 🗄️ Requisitos de datos / Base de datos

### Modelo de datos

La base de datos está compuesta por **10 tablas normalizadas en Tercera Forma Normal (3FN)**:

| Tabla | Propósito | Relaciones |
|-------|-----------|------------|
| `usuarios` | Credenciales de acceso | 1:1 con `perfiles_corporales`, 1:N con `registros_diarios` y `favoritos` |
| `perfiles_corporales` | Datos físicos del usuario | 1:1 con `usuarios` (relación obligatoria) |
| `ejercicios` | Catálogo de ejercicios | M:N con `rutinas` vía `rutinas_ejercicios` |
| `rutinas` | Rutinas por día y nivel | M:N con `ejercicios`, 1:N con `registros_diarios` |
| `rutinas_ejercicios` | Tabla intermedia M:N | Contiene series y repeticiones |
| `planes_alimenticios` | Planes nutricionales | 1:N con `recetas` |
| `recetas` | Recetas por plan | N:1 con `planes_alimenticios`, N:M con `usuarios` vía `favoritos` |
| `favoritos` | Favoritos del usuario | N:M entre `usuarios` y `rutinas`/`recetas` |
| `seccion_bienestar` | Tips de bienestar | Independiente |
| `registros_diarios` | Progreso diario del usuario | N:1 con `usuarios` y `rutinas` |

### Diagrama Entidad-Relación (resumido)
usuarios (1) ──── (1) perfiles_corporales

│
├──── () favoritos () ──── (1) rutinas

│ │
│ └────── (1) recetas
│
└──── () registros_diarios () ──── (1) rutinas


rutinas () ──── () ejercicios
(vía rutinas_ejercicios)

planes_alimenticios (1) ──── (*) recetas
seccion_bienestar (tabla independiente)

text

### Características técnicas

- **Identificadores:** UUID generados automáticamente con `gen_random_uuid()`.
- **Restricciones de integridad:**
  - `CHECK` para validar valores (niveles, categorías, pesos positivos).
  - `FOREIGN KEY` con `ON DELETE CASCADE` y `ON DELETE SET NULL`.
  - `UNIQUE` compuesto para evitar duplicados (usuario + fecha, usuario + rutina).
- **Índices estratégicos:**
  - `idx_usuarios_correo`
  - `idx_registros_diarios_usuario_fecha`
  - `idx_seccion_bienestar_categoria`
- **Cifrado:** conexión SSL/TLS obligatoria hacia Aiven Cloud.
- **Normalización:** 3FN para evitar redundancia y dependencias innecesarias.

---

## 📊 Resumen de cumplimiento

| Categoría | Total | Completados | Parciales | Pendientes |
|-----------|-------|-------------|-----------|------------|
| Funcionales | 13 | 10 | 1 | 2 |
| No funcionales | 5 | 5 | 0 | 0 |
| **Total** | **18** | **15** | **1** | **2** |

**Cobertura global: 83%**

---

## 📅 Fecha

Octubre 2026

## 📄 Estado

Documento final — Requisitos del proyecto FORGE.
