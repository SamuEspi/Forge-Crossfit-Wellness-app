using Npgsql;

namespace CrossfitWellnessApp.Services;

public class DatabaseService
{
    // ============================================================
    // CONEXIÓN A POSTGRESQL
    // ============================================================
private const string ConnectionString = DbConfig.ConnectionString;

    // ============================================================
    // 1. VALIDAR LOGIN
    // ============================================================
    public async Task<bool> ValidarLogin(string correo, string contrasena)
    {
        try
        {
            await using var conn = new NpgsqlConnection(ConnectionString);
            await conn.OpenAsync();

            const string sql = @"SELECT contrasena_encriptada 
                                 FROM usuarios 
                                 WHERE correo = @correo 
                                 LIMIT 1";

            await using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("correo", correo);

            await using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                string hashGuardado = reader.GetString(0);
                // 🔐 En producción: BCrypt.Verify(contrasena, hashGuardado)
                return hashGuardado == contrasena;
            }

            return false;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[FORGE] Error ValidarLogin: {ex.Message}");
            return false;
        }
    }

    // ============================================================
    // 2. REGISTRAR USUARIO
    // ============================================================
    public async Task<bool> RegistrarUsuario(string nombre, string correo, string contrasena)
    {
        try
        {
            await using var conn = new NpgsqlConnection(ConnectionString);
            await conn.OpenAsync();

            const string checkSql = "SELECT COUNT(1) FROM usuarios WHERE correo = @correo";
            await using (var checkCmd = new NpgsqlCommand(checkSql, conn))
            {
                checkCmd.Parameters.AddWithValue("correo", correo);
                var existe = Convert.ToInt32(await checkCmd.ExecuteScalarAsync());
                if (existe > 0) return false;
            }

            const string insertSql = @"INSERT INTO usuarios (nombre, correo, contrasena_encriptada)
                                       VALUES (@nombre, @correo, @contrasena)
                                       RETURNING id";

            await using var insertCmd = new NpgsqlCommand(insertSql, conn);
            insertCmd.Parameters.AddWithValue("nombre", nombre);
            insertCmd.Parameters.AddWithValue("correo", correo);
            insertCmd.Parameters.AddWithValue("contrasena", contrasena);

            var nuevoId = await insertCmd.ExecuteScalarAsync();
            return nuevoId is not null;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[FORGE] Error RegistrarUsuario: {ex.Message}");
            return false;
        }
    }

    // ============================================================
    // 3. OBTENER ID DE USUARIO POR CORREO
    // ============================================================
    public async Task<Guid?> ObtenerUsuarioIdPorCorreo(string correo)
    {
        try
        {
            await using var conn = new NpgsqlConnection(ConnectionString);
            await conn.OpenAsync();

            const string sql = "SELECT id FROM usuarios WHERE correo = @correo LIMIT 1";
            await using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("correo", correo);

            var resultado = await cmd.ExecuteScalarAsync();
            return resultado is Guid id ? id : null;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[FORGE] Error ObtenerUsuarioIdPorCorreo: {ex.Message}");
            return null;
        }
    }

    // ============================================================
    // 4. OBTENER NOMBRE DE USUARIO POR CORREO
    // ============================================================
    public async Task<string?> ObtenerNombrePorCorreo(string correo)
    {
        try
        {
            await using var conn = new NpgsqlConnection(ConnectionString);
            await conn.OpenAsync();

            const string sql = "SELECT nombre FROM usuarios WHERE correo = @correo LIMIT 1";
            await using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("correo", correo);

            var resultado = await cmd.ExecuteScalarAsync();
            return resultado as string;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[FORGE] Error ObtenerNombrePorCorreo: {ex.Message}");
            return null;
        }
    }

    // ============================================================
    // 5. OBTENER PERFIL CORPORAL DEL USUARIO
    // ============================================================
    public async Task<(decimal peso, decimal altura, int edad, string objetivo)?> ObtenerPerfilCorporal(Guid usuarioId)
    {
        try
        {
            await using var conn = new NpgsqlConnection(ConnectionString);
            await conn.OpenAsync();

            const string sql = @"SELECT peso, altura, edad, objetivo_fisico 
                                 FROM perfiles_corporales 
                                 WHERE usuario_id = @usuarioId 
                                 LIMIT 1";

            await using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("usuarioId", usuarioId);

            await using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return (
                    reader.GetDecimal(0),
                    reader.GetDecimal(1),
                    reader.GetInt32(2),
                    reader.GetString(3)
                );
            }

            return null;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[FORGE] Error ObtenerPerfilCorporal: {ex.Message}");
            return null;
        }
    }

    // ============================================================
    // 6. GUARDAR O ACTUALIZAR PERFIL CORPORAL (UPSERT)
    // ============================================================
    public async Task<bool> GuardarPerfilCorporal(Guid usuarioId, decimal peso, decimal altura, int edad, string objetivoFisico)
    {
        try
        {
            await using var conn = new NpgsqlConnection(ConnectionString);
            await conn.OpenAsync();

            const string sql = @"
                INSERT INTO perfiles_corporales (usuario_id, peso, altura, edad, objetivo_fisico, fecha_actualizacion)
                VALUES (@usuarioId, @peso, @altura, @edad, @objetivo, NOW())
                ON CONFLICT (usuario_id) 
                DO UPDATE SET 
                    peso = EXCLUDED.peso,
                    altura = EXCLUDED.altura,
                    edad = EXCLUDED.edad,
                    objetivo_fisico = EXCLUDED.objetivo_fisico,
                    fecha_actualizacion = NOW()";

            await using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("usuarioId", usuarioId);
            cmd.Parameters.AddWithValue("peso", peso);
            cmd.Parameters.AddWithValue("altura", altura);
            cmd.Parameters.AddWithValue("edad", edad);
            cmd.Parameters.AddWithValue("objetivo", objetivoFisico);

            int filas = await cmd.ExecuteNonQueryAsync();
            return filas > 0;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[FORGE] Error GuardarPerfilCorporal: {ex.Message}");
            return false;
        }
    }

    // ============================================================
    // 7. OBTENER TODOS LOS EJERCICIOS (catálogo)
    // ============================================================
    public async Task<List<Models.Ejercicio>> ObtenerTodosLosEjercicios()
    {
        var lista = new List<Models.Ejercicio>();

        try
        {
            await using var conn = new NpgsqlConnection(ConnectionString);
            await conn.OpenAsync();

            const string sql = @"SELECT id, nombre, descripcion, grupo_muscular, url_multimedia
                                 FROM ejercicios
                                 ORDER BY nombre ASC";

            await using var cmd = new NpgsqlCommand(sql, conn);
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                lista.Add(new Models.Ejercicio
                {
                    Id = reader.GetGuid(0),
                    Nombre = reader.GetString(1),
                    Descripcion = reader.GetString(2),
                    GrupoMuscular = reader.GetString(3),
                    UrlMultimedia = reader.IsDBNull(4) ? null : reader.GetString(4)
                });
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[FORGE] Error ObtenerTodosLosEjercicios: {ex.Message}");
        }

        return lista;
    }

    // ============================================================
    // 8. OBTENER EJERCICIO POR ID
    // ============================================================
    public async Task<Models.Ejercicio?> ObtenerEjercicioPorId(Guid ejercicioId)
    {
        try
        {
            await using var conn = new NpgsqlConnection(ConnectionString);
            await conn.OpenAsync();

            const string sql = @"SELECT id, nombre, descripcion, grupo_muscular, url_multimedia
                                 FROM ejercicios
                                 WHERE id = @id
                                 LIMIT 1";

            await using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("id", ejercicioId);

            await using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new Models.Ejercicio
                {
                    Id = reader.GetGuid(0),
                    Nombre = reader.GetString(1),
                    Descripcion = reader.GetString(2),
                    GrupoMuscular = reader.GetString(3),
                    UrlMultimedia = reader.IsDBNull(4) ? null : reader.GetString(4)
                };
            }

            return null;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[FORGE] Error ObtenerEjercicioPorId: {ex.Message}");
            return null;
        }
    }

    // ============================================================
    // 9. OBTENER RUTINA DEL DÍA SEGÚN DÍA Y NIVEL
    // ============================================================
    public async Task<Models.Rutina?> ObtenerRutinaDelDia(string diaSemana, string nivel)
    {
        try
        {
            await using var conn = new NpgsqlConnection(ConnectionString);
            await conn.OpenAsync();

            const string sql = @"SELECT id, nombre_rutina, nivel_dificultad, dia_semana
                                 FROM rutinas
                                 WHERE LOWER(dia_semana) = LOWER(@dia)
                                   AND nivel_dificultad = @nivel
                                 LIMIT 1";

            await using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("dia", diaSemana);
            cmd.Parameters.AddWithValue("nivel", nivel);

            await using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new Models.Rutina
                {
                    Id = reader.GetGuid(0),
                    NombreRutina = reader.GetString(1),
                    NivelDificultad = reader.GetString(2),
                    DiaSemana = reader.GetString(3)
                };
            }

            await reader.CloseAsync();

            const string fallbackSql = @"SELECT id, nombre_rutina, nivel_dificultad, dia_semana
                                         FROM rutinas
                                         WHERE LOWER(dia_semana) = LOWER(@dia)
                                         LIMIT 1";

            await using var fallbackCmd = new NpgsqlCommand(fallbackSql, conn);
            fallbackCmd.Parameters.AddWithValue("dia", diaSemana);

            await using var fallbackReader = await fallbackCmd.ExecuteReaderAsync();

            if (await fallbackReader.ReadAsync())
            {
                return new Models.Rutina
                {
                    Id = fallbackReader.GetGuid(0),
                    NombreRutina = fallbackReader.GetString(1),
                    NivelDificultad = fallbackReader.GetString(2),
                    DiaSemana = fallbackReader.GetString(3)
                };
            }

            return null;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[FORGE] Error ObtenerRutinaDelDia: {ex.Message}");
            return null;
        }
    }

    // ============================================================
    // 10. OBTENER EJERCICIOS DE UNA RUTINA
    // ============================================================
    public async Task<List<Models.EjercicioDeRutina>> ObtenerEjerciciosDeRutina(Guid rutinaId)
    {
        var lista = new List<Models.EjercicioDeRutina>();

        try
        {
            await using var conn = new NpgsqlConnection(ConnectionString);
            await conn.OpenAsync();

            const string sql = @"
                SELECT e.id, e.nombre, e.descripcion, e.grupo_muscular, e.url_multimedia,
                       re.series, re.repeticiones
                FROM rutinas_ejercicios re
                INNER JOIN ejercicios e ON e.id = re.ejercicio_id
                WHERE re.rutina_id = @rutinaId
                ORDER BY e.nombre ASC";

            await using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("rutinaId", rutinaId);

            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                lista.Add(new Models.EjercicioDeRutina
                {
                    EjercicioId = reader.GetGuid(0),
                    Nombre = reader.GetString(1),
                    Descripcion = reader.GetString(2),
                    GrupoMuscular = reader.GetString(3),
                    UrlMultimedia = reader.IsDBNull(4) ? null : reader.GetString(4),
                    Series = reader.GetInt32(5),
                    Repeticiones = reader.GetString(6)
                });
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[FORGE] Error ObtenerEjerciciosDeRutina: {ex.Message}");
        }

        return lista;
    }

    // ============================================================
    // 11. OBTENER TODAS LAS RUTINAS (ordenadas por día)
    // ============================================================
    public async Task<List<Models.Rutina>> ObtenerTodasLasRutinas()
    {
        var lista = new List<Models.Rutina>();

        try
        {
            await using var conn = new NpgsqlConnection(ConnectionString);
            await conn.OpenAsync();

            const string sql = @"
                SELECT id, nombre_rutina, nivel_dificultad, dia_semana
                FROM rutinas
                ORDER BY 
                    CASE dia_semana 
                        WHEN 'Lunes' THEN 1
                        WHEN 'Martes' THEN 2
                        WHEN 'Miércoles' THEN 3
                        WHEN 'Jueves' THEN 4
                        WHEN 'Viernes' THEN 5
                        WHEN 'Sábado' THEN 6
                        WHEN 'Domingo' THEN 7
                        ELSE 8
                    END ASC,
                    nivel_dificultad ASC";

            await using var cmd = new NpgsqlCommand(sql, conn);
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                lista.Add(new Models.Rutina
                {
                    Id = reader.GetGuid(0),
                    NombreRutina = reader.GetString(1),
                    NivelDificultad = reader.GetString(2),
                    DiaSemana = reader.GetString(3)
                });
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[FORGE] Error ObtenerTodasLasRutinas: {ex.Message}");
        }

        return lista;
    }

    // ============================================================
    // 12. GUARDAR O ACTUALIZAR REGISTRO DIARIO (UPSERT)
    // ============================================================
    public async Task<bool> GuardarRegistroDiario(
        Guid usuarioId,
        decimal? pesoActual,
        int consumoAgua,
        int caloriasIngeridas,
        Guid? rutinaCompletadaId,
        string? notas)
    {
        try
        {
            await using var conn = new NpgsqlConnection(ConnectionString);
            await conn.OpenAsync();

            const string sql = @"
                INSERT INTO registros_diarios 
                    (usuario_id, fecha, peso_actual, consumo_agua_ml, 
                     calorias_ingeridas, entrenamiento_completado_id, notas)
                VALUES 
                    (@usuarioId, CURRENT_DATE, @peso, @agua, 
                     @calorias, @rutinaId, @notas)
                ON CONFLICT (usuario_id, fecha) 
                DO UPDATE SET 
                    peso_actual = EXCLUDED.peso_actual,
                    consumo_agua_ml = EXCLUDED.consumo_agua_ml,
                    calorias_ingeridas = EXCLUDED.calorias_ingeridas,
                    entrenamiento_completado_id = EXCLUDED.entrenamiento_completado_id,
                    notas = EXCLUDED.notas";

            await using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("usuarioId", usuarioId);
            cmd.Parameters.AddWithValue("peso", pesoActual.HasValue ? (object)pesoActual.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("agua", consumoAgua);
            cmd.Parameters.AddWithValue("calorias", caloriasIngeridas);
            cmd.Parameters.AddWithValue("rutinaId", rutinaCompletadaId.HasValue ? (object)rutinaCompletadaId.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("notas", string.IsNullOrWhiteSpace(notas) ? (object)DBNull.Value : notas.Trim());

            int filas = await cmd.ExecuteNonQueryAsync();
            return filas > 0;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[FORGE] Error GuardarRegistroDiario: {ex.Message}");
            return false;
        }
    }

    // ============================================================
    // 13. OBTENER REGISTRO DIARIO DEL DÍA ACTUAL
    // ============================================================
    public async Task<(decimal? peso, int agua, int calorias, Guid? rutinaId, string? notas)?> ObtenerRegistroDelDia(Guid usuarioId)
    {
        try
        {
            await using var conn = new NpgsqlConnection(ConnectionString);
            await conn.OpenAsync();

            const string sql = @"
                SELECT peso_actual, consumo_agua_ml, calorias_ingeridas, 
                       entrenamiento_completado_id, notas
                FROM registros_diarios
                WHERE usuario_id = @usuarioId 
                  AND fecha = CURRENT_DATE
                LIMIT 1";

            await using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("usuarioId", usuarioId);

            await using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                decimal? peso = reader.IsDBNull(0) ? null : reader.GetDecimal(0);
                int agua = reader.IsDBNull(1) ? 0 : reader.GetInt32(1);
                int calorias = reader.IsDBNull(2) ? 0 : reader.GetInt32(2);
                Guid? rutinaId = reader.IsDBNull(3) ? null : reader.GetGuid(3);
                string? notas = reader.IsDBNull(4) ? null : reader.GetString(4);

                return (peso, agua, calorias, rutinaId, notas);
            }

            return null;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[FORGE] Error ObtenerRegistroDelDia: {ex.Message}");
            return null;
        }
    }

    // ============================================================
    // 14. OBTENER HISTORIAL DE REGISTROS (últimos N días)
    // ============================================================
    public async Task<List<(DateTime fecha, decimal? peso, int agua, int calorias)>> ObtenerHistorialRegistros(Guid usuarioId, int dias = 7)
    {
        var lista = new List<(DateTime, decimal?, int, int)>();

        try
        {
            await using var conn = new NpgsqlConnection(ConnectionString);
            await conn.OpenAsync();

            const string sql = @"
                SELECT fecha, peso_actual, consumo_agua_ml, calorias_ingeridas
                FROM registros_diarios
                WHERE usuario_id = @usuarioId
                ORDER BY fecha DESC
                LIMIT @dias";

            await using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("usuarioId", usuarioId);
            cmd.Parameters.AddWithValue("dias", dias);

            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                lista.Add((
                    reader.GetDateTime(0),
                    reader.IsDBNull(1) ? null : reader.GetDecimal(1),
                    reader.IsDBNull(2) ? 0 : reader.GetInt32(2),
                    reader.IsDBNull(3) ? 0 : reader.GetInt32(3)
                ));
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[FORGE] Error ObtenerHistorialRegistros: {ex.Message}");
        }

        return lista;
    }

    // ============================================================
    // 15. OBTENER TODOS LOS CONSEJOS DE BIENESTAR
    // ============================================================
    public async Task<List<Models.ConsejoBienestar>> ObtenerConsejosBienestar()
    {
        var lista = new List<Models.ConsejoBienestar>();

        try
        {
            await using var conn = new NpgsqlConnection(ConnectionString);
            await conn.OpenAsync();

            const string sql = @"
                SELECT id, titulo, contenido, categoria, fecha_publicacion
                FROM seccion_bienestar
                ORDER BY fecha_publicacion DESC, titulo ASC";

            await using var cmd = new NpgsqlCommand(sql, conn);
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                lista.Add(new Models.ConsejoBienestar
                {
                    Id = reader.GetGuid(0),
                    Titulo = reader.GetString(1),
                    Contenido = reader.GetString(2),
                    Categoria = reader.GetString(3),
                    FechaPublicacion = reader.GetDateTime(4)
                });
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[FORGE] Error ObtenerConsejosBienestar: {ex.Message}");
        }

        return lista;
    }

    // ============================================================
    // 16. OBTENER CONSEJOS POR CATEGORÍA
    // ============================================================
    public async Task<List<Models.ConsejoBienestar>> ObtenerConsejosPorCategoria(string categoria)
    {
        var lista = new List<Models.ConsejoBienestar>();

        try
        {
            await using var conn = new NpgsqlConnection(ConnectionString);
            await conn.OpenAsync();

            const string sql = @"
                SELECT id, titulo, contenido, categoria, fecha_publicacion
                FROM seccion_bienestar
                WHERE categoria = @categoria
                ORDER BY fecha_publicacion DESC, titulo ASC";

            await using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("categoria", categoria);

            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                lista.Add(new Models.ConsejoBienestar
                {
                    Id = reader.GetGuid(0),
                    Titulo = reader.GetString(1),
                    Contenido = reader.GetString(2),
                    Categoria = reader.GetString(3),
                    FechaPublicacion = reader.GetDateTime(4)
                });
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[FORGE] Error ObtenerConsejosPorCategoria: {ex.Message}");
        }

        return lista;
    }

    // ============================================================
    // 17. AGREGAR RUTINA A FAVORITOS
    // ============================================================
    public async Task<bool> AgregarRutinaFavorita(Guid usuarioId, Guid rutinaId)
    {
        try
        {
            await using var conn = new NpgsqlConnection(ConnectionString);
            await conn.OpenAsync();

            const string sql = @"
                INSERT INTO favoritos (usuario_id, rutina_id)
                VALUES (@usuarioId, @rutinaId)
                ON CONFLICT (usuario_id, rutina_id) DO NOTHING";

            await using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("usuarioId", usuarioId);
            cmd.Parameters.AddWithValue("rutinaId", rutinaId);

            int filas = await cmd.ExecuteNonQueryAsync();
            return filas > 0;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[FORGE] Error AgregarRutinaFavorita: {ex.Message}");
            return false;
        }
    }

    // ============================================================
    // 18. QUITAR RUTINA DE FAVORITOS
    // ============================================================
    public async Task<bool> QuitarRutinaFavorita(Guid usuarioId, Guid rutinaId)
    {
        try
        {
            await using var conn = new NpgsqlConnection(ConnectionString);
            await conn.OpenAsync();

            const string sql = @"
                DELETE FROM favoritos 
                WHERE usuario_id = @usuarioId AND rutina_id = @rutinaId";

            await using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("usuarioId", usuarioId);
            cmd.Parameters.AddWithValue("rutinaId", rutinaId);

            int filas = await cmd.ExecuteNonQueryAsync();
            return filas > 0;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[FORGE] Error QuitarRutinaFavorita: {ex.Message}");
            return false;
        }
    }

    // ============================================================
    // 19. VERIFICAR SI UNA RUTINA ES FAVORITA
    // ============================================================
    public async Task<bool> EsRutinaFavorita(Guid usuarioId, Guid rutinaId)
    {
        try
        {
            await using var conn = new NpgsqlConnection(ConnectionString);
            await conn.OpenAsync();

            const string sql = @"
                SELECT COUNT(1) FROM favoritos 
                WHERE usuario_id = @usuarioId AND rutina_id = @rutinaId";

            await using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("usuarioId", usuarioId);
            cmd.Parameters.AddWithValue("rutinaId", rutinaId);

            int count = Convert.ToInt32(await cmd.ExecuteScalarAsync());
            return count > 0;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[FORGE] Error EsRutinaFavorita: {ex.Message}");
            return false;
        }
    }

    // ============================================================
    // 20. OBTENER FAVORITOS DE RUTINAS DEL USUARIO
    // ============================================================
    public async Task<List<Models.FavoritoItem>> ObtenerRutinasFavoritas(Guid usuarioId)
    {
        var lista = new List<Models.FavoritoItem>();

        try
        {
            await using var conn = new NpgsqlConnection(ConnectionString);
            await conn.OpenAsync();

            const string sql = @"
                SELECT f.id, f.rutina_id, r.nombre_rutina, r.dia_semana, r.nivel_dificultad, f.fecha_agregado
                FROM favoritos f
                INNER JOIN rutinas r ON r.id = f.rutina_id
                WHERE f.usuario_id = @usuarioId AND f.rutina_id IS NOT NULL
                ORDER BY f.fecha_agregado DESC";

            await using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("usuarioId", usuarioId);

            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                lista.Add(new Models.FavoritoItem
                {
                    Id = reader.GetGuid(0),
                    RutinaId = reader.GetGuid(1),
                    Tipo = "Rutina",
                    Titulo = reader.GetString(2),
                    Subtitulo = $"{reader.GetString(3)} · {reader.GetString(4)}",
                    FechaAgregado = reader.GetDateTime(5)
                });
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[FORGE] Error ObtenerRutinasFavoritas: {ex.Message}");
        }

        return lista;
    }

    // ============================================================
    // 21. OBTENER FAVORITOS DE RECETAS DEL USUARIO
    // ============================================================
    public async Task<List<Models.FavoritoItem>> ObtenerRecetasFavoritas(Guid usuarioId)
    {
        var lista = new List<Models.FavoritoItem>();

        try
        {
            await using var conn = new NpgsqlConnection(ConnectionString);
            await conn.OpenAsync();

            const string sql = @"
                SELECT f.id, f.receta_id, rec.titulo, rec.calorias, f.fecha_agregado
                FROM favoritos f
                INNER JOIN recetas rec ON rec.id = f.receta_id
                WHERE f.usuario_id = @usuarioId AND f.receta_id IS NOT NULL
                ORDER BY f.fecha_agregado DESC";

            await using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("usuarioId", usuarioId);

            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                lista.Add(new Models.FavoritoItem
                {
                    Id = reader.GetGuid(0),
                    RecetaId = reader.GetGuid(1),
                    Tipo = "Receta",
                    Titulo = reader.GetString(2),
                    Subtitulo = $"{reader.GetInt32(3)} kcal",
                    FechaAgregado = reader.GetDateTime(4)
                });
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[FORGE] Error ObtenerRecetasFavoritas: {ex.Message}");
        }

        return lista;
    }
}
