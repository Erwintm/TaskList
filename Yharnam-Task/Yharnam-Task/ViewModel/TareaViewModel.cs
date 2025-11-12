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

        public async Task<bool> AgregarTareaAsync()
        {
            string error = ValidarCampos();
            if (error != "Good")
            {
                await Application.Current.MainPage.DisplayAlert("Error", error, "Aceptar");
                return false;
            }

            var nuevaTarea = new Tarea
            {
                Titulo = NuevoTitulo.Trim(),
                Descripcion = NuevaDescripcion?.Trim() ?? string.Empty,
                FechaEntrega = FechaEntrega,
                Dificultad = DificultadSeleccionada,
                TiempoEstimado = TimeSpan.FromMinutes(TiempoEstimadoMinutos),
                FechaCreacion = DateTime.Now,
                Completada = false
            };

            Tareas.Add(nuevaTarea);
            await tareaService.GuardarTareasAsync(Tareas);

            NuevoTitulo = string.Empty;
            NuevaDescripcion = string.Empty;
            FechaEntrega = DateTime.Today;
            DificultadSeleccionada = "Media";
            TiempoEstimadoMinutos = 60;

            return true;
        }

        private string ValidarCampos()
        {
            if (string.IsNullOrWhiteSpace(NuevoTitulo?.Trim()))
                return "Debes ingresar un título para la tarea.";

            if (FechaEntrega < DateTime.Today)
                return "La fecha de entrega no puede ser anterior a hoy.";

            if (TiempoEstimadoMinutos < 15)
                return "El tiempo estimado debe ser al menos de 15 minutos.";

            if (TiempoEstimadoMinutos < 15)
                return "El tiempo estimado debe ser como máximmo 8 horas.";

            if (TiempoEstimadoMinutos > 480)
                return "El tiempo estimado debe ser al menos de 15 minutos.";

            if (!string.IsNullOrEmpty(NuevaDescripcion) && NuevaDescripcion.Length > 100)
                return "La descripción no debe exceder los 50 caracteres.";

            return "Good";
        }
    }
}
