using System.Collections.ObjectModel;
using System.Windows.Input;
using Yharnam_Task.Models;
using Yharnam_Task.Services;

namespace Yharnam_Task.ViewModel
{
    public class TareaViewModel : BindableObject
    {
        private readonly TareaService _tareaService;
        private string _nuevoTitulo = string.Empty;
        private string _nuevaDescripcion = string.Empty;

        public TareaViewModel()
        {
            _tareaService = new TareaService();
            Tareas = new ObservableCollection<Tarea>();
            AgregarTareaCommand = new Command(async () => await AgregarTarea());
        }

        public ObservableCollection<Tarea> Tareas { get; set; }

        public string NuevoTitulo
        {
            get => _nuevoTitulo;
            set
            {
                _nuevoTitulo = value;
                OnPropertyChanged();
            }
        }

        public string NuevaDescripcion
        {
            get => _nuevaDescripcion;
            set
            {
                _nuevaDescripcion = value;
                OnPropertyChanged();
            }
        }

        public ICommand AgregarTareaCommand { get; }

        private async Task AgregarTarea()
        {
            if (string.IsNullOrWhiteSpace(NuevoTitulo))
                return;

            var nuevaTarea = new Tarea
            {
                Titulo = NuevoTitulo,
                Descripcion = NuevaDescripcion,
                Prioridad = 0,
                Dificultad = "Media"
            };

            await _tareaService.AddTareaAsync(nuevaTarea);
            Tareas.Add(nuevaTarea);

            // Limpiar campos
            NuevoTitulo = string.Empty;
            NuevaDescripcion = string.Empty;
        }
    }
}