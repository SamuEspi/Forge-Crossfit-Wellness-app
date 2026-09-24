namespace CrossfitWellnessApp.Models;

public class Rutina
{
    public Guid Id { get; set; }
    public string NombreRutina { get; set; } = string.Empty;
    public string NivelDificultad { get; set; } = string.Empty;
    public string DiaSemana { get; set; } = string.Empty;
}

public class EjercicioDeRutina
{
    public Guid EjercicioId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string GrupoMuscular { get; set; } = string.Empty;
    public string? UrlMultimedia { get; set; }
    public int Series { get; set; }
    public string Repeticiones { get; set; } = string.Empty;
}