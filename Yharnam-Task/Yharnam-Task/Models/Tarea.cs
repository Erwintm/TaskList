using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Yharnam_Task.Models
{
    public class Tarea : INotifyPropertyChanged
    {
        private string id = Guid.NewGuid().ToString();
        public string Id
        {
            get => id;
            set { id = value; OnPropertyChanged(); }
        }

        private string titulo = string.Empty;
        public string Titulo
        {
            get => titulo;
            set { titulo = value; OnPropertyChanged(); }
        }

        private string descripcion = string.Empty;
        public string Descripcion
        {
            get => descripcion;
            set { descripcion = value; OnPropertyChanged(); }
        }

        private DateTime fechaCreacion = DateTime.Now;
        public DateTime FechaCreacion
        {
            get => fechaCreacion;
            set { fechaCreacion = value; OnPropertyChanged(); }
        }

        private DateTime? fechaEntrega;
        public DateTime? FechaEntrega
        {
            get => fechaEntrega;
            set { fechaEntrega = value; OnPropertyChanged(); }
        }

        private string dificultad = "Media";
        public string Dificultad
        {
            get => dificultad;
            set { dificultad = value; OnPropertyChanged(); OnPropertyChanged(nameof(DificultadColor)); }
        }

        private TimeSpan? tiempoEstimado;
        public TimeSpan? TiempoEstimado
        {
            get => tiempoEstimado;
            set { tiempoEstimado = value; OnPropertyChanged(); OnPropertyChanged(nameof(TiempoEstimadoHoras)); }
        }

        private bool completada;
        public bool Completada
        {
            get => completada;
            set { completada = value; OnPropertyChanged(); }
        }


        [System.Text.Json.Serialization.JsonIgnore]
        public double PrioridadCalculada { get; set; }

        public double PrioridadDificultad { get; set; }
        public double PrioridadDuracion { get; set; }
        public double PrioridadFecha { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public string PrioridadResumen =>
            $"Dif: {PrioridadDificultad:F2}  Dur: {PrioridadDuracion:F2}  Ent: {PrioridadFecha:F2}";

        [System.Text.Json.Serialization.JsonIgnore]
        public string DificultadColor =>
            Dificultad switch
            {
                "Fácil"  => "#4CAF50",
                "Media"  => "#FF9800",
                "Difícil"=> "#F44336",
                _        => "#757575"
            };

        [System.Text.Json.Serialization.JsonIgnore]
        public double TiempoEstimadoHoras => TiempoEstimado?.TotalHours ?? 0;


        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
