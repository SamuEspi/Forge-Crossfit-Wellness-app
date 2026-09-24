namespace CrossfitWellnessApp.Services;

public static class SessionService
{
    public static Guid? UsuarioId { get; set; }
    public static string? NombreUsuario { get; set; }
    public static string? CorreoUsuario { get; set; }
    public static string? ObjetivoFisico { get; set; }

    public static bool EstaAutenticado => UsuarioId is not null;

    public static void CerrarSesion()
    {
        UsuarioId = null;
        NombreUsuario = null;
        CorreoUsuario = null;
        ObjetivoFisico = null;
    }
}