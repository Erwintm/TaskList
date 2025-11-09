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

        private DateTime fechaEntrega = DateTime.Today;
        public DateTime FechaEntrega
        {
            get => fechaEntrega;
            set { fechaEntrega = value; OnPropertyChanged(); }
        }

        private string dificultadSeleccionada = "Media";
        public string DificultadSeleccionada
        {
            get => dificultadSeleccionada;
            set { dificultadSeleccionada = value; OnPropertyChanged(); }
        }

        private double tiempoEstimadoHoras = 1;
        public double TiempoEstimadoHoras
        {
            get => tiempoEstimadoHoras;
            set
            {
                if (tiempoEstimadoHoras != value)
                {
                    tiempoEstimadoHoras = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(TiempoDisplay));
                }
            }
        }

        public string TiempoDisplay => $"{TiempoEstimadoHoras:0.##} h";

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
                TiempoEstimado = TimeSpan.FromHours(TiempoEstimadoHoras),
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
            TiempoEstimadoHoras = 1;
        }
    }
}
