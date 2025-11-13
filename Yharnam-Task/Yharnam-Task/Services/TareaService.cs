using System.Collections.ObjectModel;
using System.Text.Json;
using Yharnam_Task.Models;

namespace Yharnam_Task.Services
{
    public class TareaService
    {
        private readonly string filePath;

        public TareaService()
        {
            filePath = Path.Combine(FileSystem.AppDataDirectory, "tareas.json");
        }

        public async Task<ObservableCollection<Tarea>> CargarTareasAsync()
        {
            if (!File.Exists(filePath))
                return new ObservableCollection<Tarea>();

            using var stream = File.OpenRead(filePath);
            var tareas = await JsonSerializer.DeserializeAsync<ObservableCollection<Tarea>>(stream)
                          ?? new ObservableCollection<Tarea>();

            return tareas;
        }

        public async Task GuardarTareasAsync(ObservableCollection<Tarea> tareas)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            using var stream = File.Create(filePath);
            await JsonSerializer.SerializeAsync(stream, tareas, options);
        }

        public Task ClearAsync()
        {
            if (File.Exists(filePath)) File.Delete(filePath);

            return Task.CompletedTask;
        }

    }
}
