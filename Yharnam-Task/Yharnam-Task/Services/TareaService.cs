using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yharnam_Task.Models;

namespace Yharnam_Task.Services
{
    public class TareaService
    {
        private List<Tarea> _tareas = new List<Tarea>();
        private int _nextId = 1;

        public TareaService()
        {
            _tareas.Add(new Tarea
            {
                Id = _nextId++,
                Titulo = "Configurar entorno de desarrollo",
                Descripcion = "Instalar Visual Studio, configurar MAUI y probar el simulador.",
                FechaCreacion = DateTime.Parse("2025-11-05T12:00:00"),
                FechaEntrega = null,
                Dificultad = "Alta",
                TiempoEstimado = TimeSpan.FromHours(1.5),
                Completada = false,
                Prioridad = 1
            });

            _tareas.Add(new Tarea
            {
                Id = _nextId++,
                Titulo = "tarea 2",
                Descripcion = "esto es una prueba",
                FechaCreacion = DateTime.Parse("2025-11-05T13:00:00"),
                FechaEntrega = null,
                Dificultad = "Media",
                TiempoEstimado = null,
                Completada = false,
                Prioridad = 0
            });
        }

        public Task<Tarea> AddTareaAsync(Tarea tarea)
        {
            tarea.Id = _nextId++;
            tarea.FechaCreacion = DateTime.Now;
            _tareas.Add(tarea);
            return Task.FromResult(tarea);
        }
    }
}