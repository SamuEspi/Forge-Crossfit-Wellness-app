namespace CrossFitWellnessApp.Models
{
    public class Usuario
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string ContrasenaEncriptada { get; set; } = string.Empty;
    }
}
