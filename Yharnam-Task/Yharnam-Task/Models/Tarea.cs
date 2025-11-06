using System;

namespace Yharnam_Task.Models
{
    public class Tarea
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public DateTime? FechaEntrega { get; set; }
        public string Dificultad { get; set; } = "Media";
        public TimeSpan? TiempoEstimado { get; set; }

        public bool Completada { get; set; }
        public int Prioridad { get; set; }
    }
}