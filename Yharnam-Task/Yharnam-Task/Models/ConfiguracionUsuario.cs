namespace Yharnam_Task.Models;

public class ConfiguracionUsuario
{
    public string Nombre { get; set; } = string.Empty;

    public string? PreferenciaDificultad { get; set; }

    public string? PreferenciaPrioridad { get; set; }

    public string? PreferenciaDuracion { get; set; }
    public List<string>? OrdenPrioridades { get; set; }
}
