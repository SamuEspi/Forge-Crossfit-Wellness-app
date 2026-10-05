-- ============================================================================
-- SCRIPT DE ESQUEMA DE BASE DE DATOS Y DATOS DE PRUEBA
-- PROYECTO: FORGE - Entrenamiento CrossFit y Nutrición Deportiva
-- ASIGNATURA: Ingeniería de Software
-- ============================================================================

-- Habilitar extensiones necesarias
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
CREATE EXTENSION IF NOT EXISTS "pgcrypto";

-- Limpieza de tablas existentes (orden inverso de dependencia)
DROP TABLE IF EXISTS registros_diarios CASCADE;
DROP TABLE IF EXISTS seccion_bienestar CASCADE;
DROP TABLE IF EXISTS favoritos CASCADE;
DROP TABLE IF EXISTS recetas CASCADE;
DROP TABLE IF EXISTS planes_alimenticios CASCADE;
DROP TABLE IF EXISTS rutinas_ejercicios CASCADE;
DROP TABLE IF EXISTS rutinas CASCADE;
DROP TABLE IF EXISTS ejercicios CASCADE;
DROP TABLE IF EXISTS perfiles_corporales CASCADE;
DROP TABLE IF EXISTS usuarios CASCADE;


-- ============================================================================
-- 1. TABLAS
-- ============================================================================

-- Tabla 'usuarios' (REQ-01)
CREATE TABLE usuarios (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    nombre VARCHAR(100) NOT NULL,
    correo VARCHAR(150) NOT NULL UNIQUE,
    contrasena_encriptada VARCHAR(255) NOT NULL,
    fecha_registro TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

-- Tabla 'perfiles_corporales' (REQ-02)
CREATE TABLE perfiles_corporales (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    usuario_id UUID NOT NULL UNIQUE REFERENCES usuarios(id) ON DELETE CASCADE,
    peso NUMERIC(5,2) NOT NULL CHECK (peso > 0.0),
    altura NUMERIC(3,2) NOT NULL CHECK (altura > 0.0),
    edad INT NOT NULL CHECK (edad >= 0 AND edad < 120),
    objetivo_fisico VARCHAR(50) NOT NULL CHECK (objetivo_fisico IN ('Pérdida de peso', 'Aumento muscular', 'Mantenimiento', 'Mejora de resistencia')),
    fecha_actualizacion TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

-- Tabla 'ejercicios' (REQ-04)
CREATE TABLE ejercicios (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    nombre VARCHAR(100) NOT NULL UNIQUE,
    descripcion TEXT NOT NULL,
    grupo_muscular VARCHAR(200) NOT NULL,
    url_multimedia VARCHAR(500) CHECK (url_multimedia ~* '^https?://[^\s/$.?#].[^\s]*$')
);

-- Tabla 'rutinas' (REQ-03, REQ-06)
CREATE TABLE rutinas (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    nombre_rutina VARCHAR(100) NOT NULL,
    nivel_dificultad VARCHAR(20) NOT NULL CHECK (nivel_dificultad IN ('Principiante', 'Intermedio', 'Avanzado')),
    dia_semana VARCHAR(15) NOT NULL CHECK (dia_semana IN ('Lunes', 'Martes', 'Miércoles', 'Jueves', 'Viernes', 'Sábado', 'Domingo'))
);

-- Tabla intermedia 'rutinas_ejercicios'
CREATE TABLE rutinas_ejercicios (
    rutina_id UUID NOT NULL REFERENCES rutinas(id) ON DELETE CASCADE,
    ejercicio_id UUID NOT NULL REFERENCES ejercicios(id) ON DELETE CASCADE,
    series INT NOT NULL DEFAULT 4 CHECK (series > 0),
    repeticiones VARCHAR(50) NOT NULL DEFAULT '10',
    PRIMARY KEY (rutina_id, ejercicio_id)
);

-- Tabla 'planes_alimenticios' (REQ-08, REQ-09)
CREATE TABLE planes_alimenticios (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    titulo_plan VARCHAR(150) NOT NULL,
    descripcion_nutricional TEXT NOT NULL,
    tipo_dieta VARCHAR(30) NOT NULL CHECK (tipo_dieta IN ('Low Carb', 'Keto', 'Estándar')),
    objetivo_asociado VARCHAR(50) NOT NULL CHECK (objetivo_asociado IN ('Pérdida de peso', 'Aumento muscular', 'Mantenimiento', 'Mejora de resistencia'))
);

-- Tabla 'recetas'
CREATE TABLE recetas (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    titulo VARCHAR(150) NOT NULL,
    descripcion TEXT NOT NULL,
    calorias INT NOT NULL CHECK (calorias >= 0),
    plan_alimenticio_id UUID REFERENCES planes_alimenticios(id) ON DELETE CASCADE
);

-- Tabla 'favoritos' (REQ-10)
CREATE TABLE favoritos (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    usuario_id UUID NOT NULL REFERENCES usuarios(id) ON DELETE CASCADE,
    rutina_id UUID REFERENCES rutinas(id) ON DELETE CASCADE,
    receta_id UUID REFERENCES recetas(id) ON DELETE CASCADE,
    fecha_agregado TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT uq_usuario_rutina UNIQUE (usuario_id, rutina_id),
    CONSTRAINT uq_usuario_receta UNIQUE (usuario_id, receta_id),
    CONSTRAINT chk_favorito_tipo CHECK (
        (rutina_id IS NOT NULL AND receta_id IS NULL) OR
        (rutina_id IS NULL AND receta_id IS NOT NULL)
    )
);

-- Tabla 'seccion_bienestar' (REQ-11)
CREATE TABLE seccion_bienestar (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    titulo VARCHAR(200) NOT NULL,
    contenido TEXT NOT NULL,
    categoria VARCHAR(50) NOT NULL CHECK (categoria IN ('Salud', 'Entrenamiento', 'Nutrición')),
    fecha_publicacion DATE DEFAULT CURRENT_DATE
);

-- Tabla 'registros_diarios' (REQ-05, REQ-12, REQ-13)
CREATE TABLE registros_diarios (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    usuario_id UUID NOT NULL REFERENCES usuarios(id) ON DELETE CASCADE,
    fecha DATE NOT NULL DEFAULT CURRENT_DATE,
    peso_actual NUMERIC(5,2) CHECK (peso_actual > 0.0),
    consumo_agua_ml INT DEFAULT 0 CHECK (consumo_agua_ml >= 0),
    calorias_ingeridas INT DEFAULT 0 CHECK (calorias_ingeridas >= 0),
    entrenamiento_completado_id UUID REFERENCES rutinas(id) ON DELETE SET NULL,
    notas TEXT,
    CONSTRAINT uq_usuario_fecha UNIQUE (usuario_id, fecha)
);


-- ============================================================================
-- 2. ÍNDICES
-- ============================================================================

CREATE INDEX IF NOT EXISTS idx_usuarios_correo ON usuarios (correo);
CREATE INDEX IF NOT EXISTS idx_perfiles_usuario_id ON perfiles_corporales (usuario_id);
CREATE INDEX IF NOT EXISTS idx_rutinas_ejercicios_rutina_id ON rutinas_ejercicios (rutina_id);
CREATE INDEX IF NOT EXISTS idx_rutinas_ejercicios_ejercicio_id ON rutinas_ejercicios (ejercicio_id);
CREATE INDEX IF NOT EXISTS idx_favoritos_usuario_id ON favoritos (usuario_id);
CREATE INDEX IF NOT EXISTS idx_registros_diarios_usuario_fecha ON registros_diarios (usuario_id, fecha DESC);
CREATE INDEX IF NOT EXISTS idx_seccion_bienestar_categoria ON seccion_bienestar (categoria);


-- ============================================================================
-- 3. DATOS DE PRUEBA
-- ============================================================================

DO $$
DECLARE
    u_carlos UUID;
    u_ana UUID;
    u_roberto UUID;
    e_thruster UUID;
    e_burpee UUID;
    e_clean_jerk UUID;
    e_pull_up UUID;
    e_box_jump UUID;
    e_double_under UUID;
    e_kettlebell UUID;
    e_wall_ball UUID;
    e_toes_to_bar UUID;
    e_rowing UUID;
    e_deadlift UUID;
    e_snatch UUID;
    r_angie UUID;
    r_fran UUID;
    r_grace UUID;
    r_principiante UUID;
    r_fuerza UUID;
    p_keto UUID;
    p_low_carb UUID;
    p_deficit UUID;
    p_volumen UUID;
    p_hipertrofia UUID;
    p_carga UUID;
    rec_aguacate UUID;
    rec_tortilla UUID;
    rec_bowl UUID;
    rec_batido UUID;
BEGIN
    -- ------------------------------------------------------------------------
    -- USUARIOS (contraseñas hasheadas con BCrypt)
    -- ------------------------------------------------------------------------
    -- ⚠️ NOTA: Los hashes de estos usuarios de prueba son ficticios.
    -- Para que funcionen, registrar los usuarios desde la app (que aplica BCrypt real).
    -- O reemplazar el hash por uno generado en https://bcrypt-generator.com/
  
    INSERT INTO usuarios (nombre, correo, contrasena_encriptada) 
    VALUES ('Carlos Mendoza', 'carlos.mendoza@email.com', '$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewY5YqLpO8YpX4e2')
    RETURNING id INTO u_carlos;

    INSERT INTO usuarios (nombre, correo, contrasena_encriptada) 
    VALUES ('Ana Gómez', 'ana.gomez@email.com', '$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewY5YqLpO8YpX4e2')
    RETURNING id INTO u_ana;

    INSERT INTO usuarios (nombre, correo, contrasena_encriptada) 
    VALUES ('Roberto Díaz', 'roberto.diaz@email.com', '$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewY5YqLpO8YpX4e2')
    RETURNING id INTO u_roberto;

    -- ------------------------------------------------------------------------
    -- PERFILES CORPORALES
    -- ------------------------------------------------------------------------
    INSERT INTO perfiles_corporales (usuario_id, peso, altura, edad, objetivo_fisico)
    VALUES (u_carlos, 82.50, 1.78, 29, 'Aumento muscular');

    INSERT INTO perfiles_corporales (usuario_id, peso, altura, edad, objetivo_fisico)
    VALUES (u_ana, 60.20, 1.65, 26, 'Pérdida de peso');

    INSERT INTO perfiles_corporales (usuario_id, peso, altura, edad, objetivo_fisico)
    VALUES (u_roberto, 75.00, 1.72, 34, 'Mejora de resistencia');

    -- ------------------------------------------------------------------------
    -- EJERCICIOS (12)
    -- ------------------------------------------------------------------------
    INSERT INTO ejercicios (nombre, descripcion, grupo_muscular, url_multimedia) VALUES
    ('Sentadilla con Press (Thruster)', 'Movimiento compuesto que combina sentadilla frontal con press de hombros.', 'Piernas, hombros, glúteos, core', 'https://upload.wikimedia.org/wikipedia/commons/c/cb/Overhead_squat.jpg')
    RETURNING id INTO e_thruster;

    INSERT INTO ejercicios (nombre, descripcion, grupo_muscular, url_multimedia) VALUES
    ('Burpees', 'Ejercicio cardiovascular completo: flexión, salto y palmada.', 'Pecho, tríceps, cuádriceps, core', 'https://media.giphy.com/media/3o7TKu8DcbTMlMCNIs/giphy.gif')
    RETURNING id INTO e_burpee;

    INSERT INTO ejercicios (nombre, descripcion, grupo_muscular, url_multimedia) VALUES
    ('Cargada y Envión (Clean & Jerk)', 'Levantamiento olímpico en dos fases: cargada y envión.', 'Hombros, piernas, espalda, trapecios', 'https://upload.wikimedia.org/wikipedia/commons/d/df/Clean_and_jerk.jpg')
    RETURNING id INTO e_clean_jerk;

    INSERT INTO ejercicios (nombre, descripcion, grupo_muscular, url_multimedia) VALUES
    ('Dominadas (Pull-ups)', 'Ejercicio de tracción colgado de barra elevando la barbilla.', 'Espalda, bíceps, hombros, agarre', 'https://media.giphy.com/media/l0HlvtIPzPdt2usKs/giphy.gif')
    RETURNING id INTO e_pull_up;

    INSERT INTO ejercicios (nombre, descripcion, grupo_muscular, url_multimedia) VALUES
    ('Saltos al Cajón (Box Jumps)', 'Saltos pliométricos hacia un cajón.', 'Cuádriceps, glúteos, gemelos, potencia', 'https://media.giphy.com/media/l1J9EdzfOSgfyueLm/giphy.gif')
    RETURNING id INTO e_box_jump;

    INSERT INTO ejercicios (nombre, descripcion, grupo_muscular, url_multimedia) VALUES
    ('Cuerda Doble (Double Unders)', 'Dos saltos de cuerda por cada salto físico.', 'Gemelos, hombros, antebrazos, cardiovascular', 'https://media.giphy.com/media/26tPplGWjN0xLybiU/giphy.gif')
    RETURNING id INTO e_double_under;

    INSERT INTO ejercicios (nombre, descripcion, grupo_muscular, url_multimedia) VALUES
    ('Balanceo con Pesa Rusa', 'Balanceo de kettlebell con bisagra de cadera.', 'Cadena posterior, glúteos, isquiotibiales, core', 'https://media.giphy.com/media/3o6Zt8Me9HF82HwJcc/giphy.gif')
    RETURNING id INTO e_kettlebell;

    INSERT INTO ejercicios (nombre, descripcion, grupo_muscular, url_multimedia) VALUES
    ('Lanzamiento de Balón al Muro', 'Sentadilla con balón medicinal y lanzamiento a diana.', 'Cuádriceps, hombros, glúteos, pecho', 'https://upload.wikimedia.org/wikipedia/commons/d/d7/Wall_ball_exercise.jpg')
    RETURNING id INTO e_wall_ball;

    INSERT INTO ejercicios (nombre, descripcion, grupo_muscular, url_multimedia) VALUES
    ('Pies a la Barra (Toes-to-Bar)', 'Colgado en barra, elevar ambos pies hasta tocarla.', 'Abdominales, flexores de cadera, dorsales', 'https://upload.wikimedia.org/wikipedia/commons/8/86/Crossfit_Gym.jpg')
    RETURNING id INTO e_toes_to_bar;

    INSERT INTO ejercicios (nombre, descripcion, grupo_muscular, url_multimedia) VALUES
    ('Remo en Máquina (Rowing)', 'Remo de alta intensidad en ergómetro.', 'Espalda, piernas, core, cardiovascular', 'https://upload.wikimedia.org/wikipedia/commons/2/2f/Rowing_Machine.jpg')
    RETURNING id INTO e_rowing;

    INSERT INTO ejercicios (nombre, descripcion, grupo_muscular, url_multimedia) VALUES
    ('Peso Muerto (Deadlift)', 'Levantamiento de barra desde el suelo hasta cadera extendida.', 'Isquiotibiales, glúteos, erectores espinales', 'https://media.giphy.com/media/3o7TKu8DcbTMlMCNIs/giphy.gif')
    RETURNING id INTO e_deadlift;

    INSERT INTO ejercicios (nombre, descripcion, grupo_muscular, url_multimedia) VALUES
    ('Arrancada (Snatch)', 'Levantamiento olímpico de barra desde suelo a sobre la cabeza.', 'Hombros, espalda, piernas, potencia olímpica', 'https://media.giphy.com/media/xT0xeJpnrWC4XWblEk/giphy.gif')
    RETURNING id INTO e_snatch;

    -- ------------------------------------------------------------------------
    -- RUTINAS (5)
    -- ------------------------------------------------------------------------
    INSERT INTO rutinas (nombre_rutina, nivel_dificultad, dia_semana) VALUES
    ('WOD Angie Modificado', 'Avanzado', 'Lunes') RETURNING id INTO r_angie;

    INSERT INTO rutinas (nombre_rutina, nivel_dificultad, dia_semana) VALUES
    ('WOD Fran Explosivo', 'Avanzado', 'Miércoles') RETURNING id INTO r_fran;

    INSERT INTO rutinas (nombre_rutina, nivel_dificultad, dia_semana) VALUES
    ('WOD Grace Halterofilia', 'Intermedio', 'Viernes') RETURNING id INTO r_grace;

    INSERT INTO rutinas (nombre_rutina, nivel_dificultad, dia_semana) VALUES
    ('Acondicionamiento Aeróbico Principiante', 'Principiante', 'Martes') RETURNING id INTO r_principiante;

    INSERT INTO rutinas (nombre_rutina, nivel_dificultad, dia_semana) VALUES
    ('Fuerza Máxima y Potencia', 'Intermedio', 'Jueves') RETURNING id INTO r_fuerza;

    -- ------------------------------------------------------------------------
    -- ASOCIACIÓN RUTINAS - EJERCICIOS
    -- ------------------------------------------------------------------------
    INSERT INTO rutinas_ejercicios (rutina_id, ejercicio_id, series, repeticiones) VALUES
    (r_angie, e_pull_up, 4, '15 a 20 reps'),
    (r_angie, e_burpee, 4, '25 reps continuas'),
    (r_angie, e_box_jump, 4, '20 saltos (24")');

    INSERT INTO rutinas_ejercicios (rutina_id, ejercicio_id, series, repeticiones) VALUES
    (r_fran, e_thruster, 3, '21-15-9 reps'),
    (r_fran, e_pull_up, 3, '21-15-9 reps');

    INSERT INTO rutinas_ejercicios (rutina_id, ejercicio_id, series, repeticiones) VALUES
    (r_grace, e_clean_jerk, 1, '30 reps por tiempo');

    INSERT INTO rutinas_ejercicios (rutina_id, ejercicio_id, series, repeticiones) VALUES
    (r_principiante, e_burpee, 3, '10 reps controladas'),
    (r_principiante, e_kettlebell, 3, '15 reps'),
    (r_principiante, e_rowing, 3, '500 metros');

    INSERT INTO rutinas_ejercicios (rutina_id, ejercicio_id, series, repeticiones) VALUES
    (r_fuerza, e_deadlift, 5, '5 reps al 80% 1RM'),
    (r_fuerza, e_snatch, 4, '3 reps técnicas'),
    (r_fuerza, e_wall_ball, 4, '20 reps constantes'),
    (r_fuerza, e_double_under, 4, '50 saltos seguidos');

    -- ------------------------------------------------------------------------
    -- PLANES ALIMENTICIOS (6)
    -- ------------------------------------------------------------------------
    INSERT INTO planes_alimenticios (titulo_plan, descripcion_nutricional, tipo_dieta, objetivo_asociado)
    VALUES ('Keto Estricto de 14 Días', 'Plan alto en grasas, bajo en carbohidratos (menos de 20g netos).', 'Keto', 'Pérdida de peso')
    RETURNING id INTO p_keto;

    INSERT INTO planes_alimenticios (titulo_plan, descripcion_nutricional, tipo_dieta, objetivo_asociado)
    VALUES ('Low Carb Moderno y Funcional', 'Menos de 100g de carbohidratos diarios, priorizando vegetales y proteínas magras.', 'Low Carb', 'Pérdida de peso')
    RETURNING id INTO p_low_carb;

    INSERT INTO planes_alimenticios (titulo_plan, descripcion_nutricional, tipo_dieta, objetivo_asociado)
    VALUES ('Déficit Calórico Estándar', 'Reducción del 15-20% respecto al gasto energético total.', 'Estándar', 'Pérdida de peso')
    RETURNING id INTO p_deficit;

    INSERT INTO planes_alimenticios (titulo_plan, descripcion_nutricional, tipo_dieta, objetivo_asociado)
    VALUES ('Volumen Limpio de Alta Calidad', 'Superávit calórico controlado con alimentos enteros.', 'Estándar', 'Aumento muscular')
    RETURNING id INTO p_volumen;

    INSERT INTO planes_alimenticios (titulo_plan, descripcion_nutricional, tipo_dieta, objetivo_asociado)
    VALUES ('Hipertrofia Limpia Low Carb', 'Alta proteína (2.2g/kg) con carbohidratos peri-entrenamiento.', 'Low Carb', 'Aumento muscular')
    RETURNING id INTO p_hipertrofia;

    INSERT INTO planes_alimenticios (titulo_plan, descripcion_nutricional, tipo_dieta, objetivo_asociado)
    VALUES ('Carga de Carbohidratos para Atletas', 'Alto en carbohidratos para máximo rendimiento y glucógeno.', 'Estándar', 'Aumento muscular')
    RETURNING id INTO p_carga;

    -- ------------------------------------------------------------------------
    -- RECETAS (4)
    -- ------------------------------------------------------------------------
    INSERT INTO recetas (titulo, descripcion, calorias, plan_alimenticio_id)
    VALUES ('Aguacates Rellenos de Atún y Huevo', 'Aguacate con atún, huevo duro y mayonesa casera.', 350, p_keto)
    RETURNING id INTO rec_aguacate;

    INSERT INTO recetas (titulo, descripcion, calorias, plan_alimenticio_id)
    VALUES ('Tortilla de Espinacas Baby y Queso Feta', 'Tres huevos con espinaca y queso feta.', 280, p_low_carb)
    RETURNING id INTO rec_tortilla;

    INSERT INTO recetas (titulo, descripcion, calorias, plan_alimenticio_id)
    VALUES ('Bowl de Pechuga y Arroz Integral', 'Pechuga a la plancha con arroz integral y brócoli.', 550, p_volumen)
    RETURNING id INTO rec_bowl;

    INSERT INTO recetas (titulo, descripcion, calorias, plan_alimenticio_id)
    VALUES ('Batido de Proteína, Plátano y Avena', 'Proteína de suero, avena, plátano y leche de almendras.', 450, p_carga)
    RETURNING id INTO rec_batido;

    -- ------------------------------------------------------------------------
    -- FAVORITOS DE PRUEBA
    -- ------------------------------------------------------------------------
    INSERT INTO favoritos (usuario_id, rutina_id, receta_id) VALUES
    (u_carlos, r_fuerza, NULL),
    (u_carlos, NULL, rec_bowl),
    (u_ana, r_angie, NULL),
    (u_ana, NULL, rec_aguacate),
    (u_roberto, r_fran, NULL),
    (u_roberto, NULL, rec_tortilla);

    -- ------------------------------------------------------------------------
    -- CONSEJOS DE BIENESTAR (3)
    -- ------------------------------------------------------------------------
    INSERT INTO seccion_bienestar (titulo, contenido, categoria) VALUES
    ('La Importancia de la Hidratación y Electrolitos', 'La deshidratación leve puede mermar el rendimiento hasta en 20%. Reponer electrolitos como sodio y potasio es crucial.', 'Salud'),
    ('Recuperación Activa y Sueño Profundo', 'Las adaptaciones musculares se consolidan durante el sueño profundo. Procura dormir entre 7 y 9 horas.', 'Entrenamiento'),
    ('Las Grasas Saludables: Tus Aliadas Hormonales', 'Los ácidos grasos esenciales son precursores de hormonas clave. Prioriza aguacate, semillas y pescados grasos.', 'Nutrición');

    -- ------------------------------------------------------------------------
    -- REGISTROS DIARIOS DE PRUEBA
    -- ------------------------------------------------------------------------
    INSERT INTO registros_diarios (usuario_id, fecha, peso_actual, consumo_agua_ml, calorias_ingeridas, entrenamiento_completado_id, notas) VALUES
    (u_carlos, CURRENT_DATE - INTERVAL '2 days', 82.50, 3200, 2800, r_fuerza, 'Me sentí fuerte en peso muerto.'),
    (u_carlos, CURRENT_DATE - INTERVAL '1 day', 82.60, 3500, 2950, r_angie, 'WOD con mucha fatiga aeróbica.'),
    (u_carlos, CURRENT_DATE, 82.55, 3000, 2900, NULL, 'Día de descanso activo.');

    INSERT INTO registros_diarios (usuario_id, fecha, peso_actual, consumo_agua_ml, calorias_ingeridas, entrenamiento_completado_id, notas) VALUES
    (u_ana, CURRENT_DATE - INTERVAL '2 days', 60.20, 2500, 1500, r_principiante, 'Primer día de regreso.'),
    (u_ana, CURRENT_DATE - INTERVAL '1 day', 60.00, 2800, 1400, NULL, 'Día de descanso con caminata.'),
    (u_ana, CURRENT_DATE, 59.80, 2700, 1450, r_grace, 'Buenas sensaciones metabólicas.');

    INSERT INTO registros_diarios (usuario_id, fecha, peso_actual, consumo_agua_ml, calorias_ingeridas, entrenamiento_completado_id, notas) VALUES
    (u_roberto, CURRENT_DATE - INTERVAL '2 days', 75.00, 3000, 2200, r_fran, 'Fran en menos de 6 min.'),
    (u_roberto, CURRENT_DATE - INTERVAL '1 day', 74.90, 3200, 2150, r_principiante, 'Remo y burpees moderados.'),
    (u_roberto, CURRENT_DATE, 74.85, 3100, 2300, NULL, 'Sesión de movilidad.');

END $$;
