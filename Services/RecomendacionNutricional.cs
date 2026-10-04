namespace CrossfitWellnessApp.Services;

public static class RecomendacionNutricional
{
    public record Recomendacion(string Titulo, string Subtitulo, string Contenido);

    public static Recomendacion Obtener(string? objetivo) => objetivo switch
    {
        "Pérdida de peso" => new("🔥 Recomendación: Pérdida de peso",
            "Déficit calórico moderado · proteína alta",
            "Reduce grasa conservando músculo.\n\n" +
            "🔹 Calorías: déficit de 300 a 500 kcal sobre tu gasto diario.\n" +
            "🔹 Proteína: 1.8 a 2.2 g por kg de peso.\n" +
            "🔹 Carbohidratos: vegetales, avena y tubérculos; evita azúcares y harinas refinadas.\n" +
            "🔹 Agua: mínimo 2.5 litros al día.\n\n" +
            "Te puede servir el Plan Low-Carb de esta pantalla."),

        "Aumento muscular" => new("💪 Recomendación: Aumento muscular",
            "Superávit calórico controlado · proteína alta",
            "Gana masa muscular con poca grasa.\n\n" +
            "🔹 Calorías: superávit de 250 a 400 kcal.\n" +
            "🔹 Proteína: 1.6 a 2.2 g por kg de peso, repartida en 4 comidas.\n" +
            "🔹 Carbohidratos: arroz, avena, papa y fruta, sobre todo alrededor del entrenamiento.\n" +
            "🔹 Sueño: 7 a 9 horas para recuperarte."),

        "Mantenimiento" => new("⚖️ Recomendación: Mantenimiento",
            "Calorías de equilibrio · dieta variada",
            "Conserva tu peso y composición actual.\n\n" +
            "🔹 Calorías: aproximadamente tu gasto diario.\n" +
            "🔹 Proteína: 1.4 a 1.8 g por kg de peso.\n" +
            "🔹 Plato base: mitad vegetales, un cuarto proteína, un cuarto carbohidratos integrales.\n" +
            "🔹 Agua: 2 a 3 litros al día."),

        "Mejora de resistencia" => new("🏃 Recomendación: Mejora de resistencia",
            "Carbohidratos como combustible · buena hidratación",
            "Mejora tu capacidad aeróbica.\n\n" +
            "🔹 Carbohidratos: 5 a 7 g por kg de peso.\n" +
            "🔹 Proteína: 1.4 a 1.7 g por kg de peso.\n" +
            "🔹 Antes de entrenar: comida con carbohidratos 2 a 3 horas antes.\n" +
            "🔹 Sesiones largas: agua con electrolitos."),

        _ => new("🎯 Define tu objetivo",
            "Completa tu perfil para ver tu recomendación",
            "Aún no tienes un objetivo guardado. Entra a tu Perfil Corporal, elige uno y guárdalo; " +
            "así verás una recomendación nutricional adaptada a ti.")
    };
}