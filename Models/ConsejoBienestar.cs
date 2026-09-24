namespace CrossfitWellnessApp.Models;

public class ConsejoBienestar
{
    public Guid Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Contenido { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
    public DateTime FechaPublicacion { get; set; }
}