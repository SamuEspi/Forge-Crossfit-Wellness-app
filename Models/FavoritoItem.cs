namespace CrossfitWellnessApp.Models;

public class FavoritoItem
{
    public Guid Id { get; set; }              // ID del favorito
    public Guid? RutinaId { get; set; }
    public Guid? RecetaId { get; set; }
    public string Tipo { get; set; } = string.Empty;   // "Rutina" o "Receta"
    public string Titulo { get; set; } = string.Empty;
    public string Subtitulo { get; set; } = string.Empty;
    public DateTime FechaAgregado { get; set; }
}