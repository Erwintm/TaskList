using System.Collections.ObjectModel;
using System.Windows.Input;
using Yharnam_Task.Models;
using Yharnam_Task.Services;

namespace Yharnam_Task.ViewModel
{
    public class TareaViewModel : BindableObject
    {
        private readonly TareaService tareaService;
        private ObservableCollection<Tarea> tareas;

        public ObservableCollection<Tarea> Tareas
        {
            get => tareas;
            set
            {
                tareas = value;
                OnPropertyChanged();
            }
        }

        // Campos de entrada
        private string nuevoTitulo;
        public string NuevoTitulo
        {
            get => nuevoTitulo;
            set { nuevoTitulo = value; OnPropertyChanged(); }
        }

        private string nuevaDescripcion;
        public string NuevaDescripcion
        {
            get => nuevaDescripcion;
            set { nuevaDescripcion = value; OnPropertyChanged(); }
        }

        public DateTime fechaEntrega = DateTime.Today;
        public DateTime FechaEntrega
        {
            get => fechaEntrega;
            set { fechaEntrega = value; OnPropertyChanged(); }
        }

        public DateTime Today => DateTime.Today;

        private string dificultadSeleccionada = "Media";
        public string DificultadSeleccionada
        {
            get => dificultadSeleccionada;
            set { dificultadSeleccionada = value; OnPropertyChanged(); }
        }

        private double tiempoEstimadoMinutos = 60;
        public double TiempoEstimadoMinutos
        {
            get => tiempoEstimadoMinutos;
            set
            {
                double stepped = Math.Round(value / 15.0) * 15.0;
                if (Math.Abs(tiempoEstimadoMinutos - stepped) > 0.001)
                {
                    tiempoEstimadoMinutos = stepped;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(TiempoDisplay));
                }
            }
        }


        public string TiempoDisplay
        {
            get
            {
                int horas = (int)(tiempoEstimadoMinutos / 60);
                int minutos = (int)(tiempoEstimadoMinutos % 60);

                if (horas > 0 && minutos > 0)
                    return $"{horas}h {minutos}m";
                else if (horas > 0)
                    return $"{horas}h";
                else
                    return $"{minutos}m";
            }
        }


        public ICommand AgregarTareaCommand { get; }

        public TareaViewModel()
        {
            tareaService = new TareaService();
            Tareas = new ObservableCollection<Tarea>();

            AgregarTareaCommand = new Command(async () => await AgregarTareaAsync());

            _ = CargarTareasAsync();
        }

        private async Task CargarTareasAsync()
        {
            Tareas = await tareaService.CargarTareasAsync();
        }

        private async Task AgregarTareaAsync()
        {
            if (string.IsNullOrWhiteSpace(NuevoTitulo))
                return;

            var nuevaTarea = new Tarea
            {
                Titulo = NuevoTitulo,
                Descripcion = NuevaDescripcion,
                FechaEntrega = FechaEntrega,
                Dificultad = DificultadSeleccionada,
                TiempoEstimado = TimeSpan.FromMinutes(TiempoEstimadoMinutos),
                FechaCreacion = DateTime.Now,
                Completada = false
            };

            Tareas.Add(nuevaTarea);
            await tareaService.GuardarTareasAsync(Tareas);

            // Limpiar campos
            NuevoTitulo = string.Empty;
            NuevaDescripcion = string.Empty;
            FechaEntrega = DateTime.Today;
            DificultadSeleccionada = "Media";
            TiempoEstimadoMinutos = 60;
        }
    }
}
