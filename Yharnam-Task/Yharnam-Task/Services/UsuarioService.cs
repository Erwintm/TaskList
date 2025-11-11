using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using Yharnam_Task.Models;

namespace Yharnam_Task.Services;

public class UsuarioService
{
    private const string FileName = "configuracion.json";
    private readonly string _filePath =
        Path.Combine(FileSystem.AppDataDirectory, FileName);

    private static readonly JsonSerializerOptions _jsonOpts = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public async Task<bool> HasUsuarioAsync()
    {
        if (!File.Exists(_filePath)) return false;

        try
        {
            using var fs = File.OpenRead(_filePath);
            var cfg = await JsonSerializer.DeserializeAsync<ConfiguracionUsuario>(fs, _jsonOpts);
            return cfg != null && !string.IsNullOrWhiteSpace(cfg.Nombre);
        }
        catch { return false; }
    }

    public async Task<ConfiguracionUsuario?> GetUsuarioAsync()
    {
        if (!File.Exists(_filePath)) return null;
        using var fs = File.OpenRead(_filePath);
        return await JsonSerializer.DeserializeAsync<ConfiguracionUsuario>(fs, _jsonOpts);
    }

    public async Task SaveUsuarioAsync(string nombre)
    {
        var cfg = new ConfiguracionUsuario { Nombre = nombre.Trim() };
        using var fs = File.Create(_filePath);
        await JsonSerializer.SerializeAsync(fs, cfg, _jsonOpts);
        await fs.FlushAsync();
    }

    public Task ClearAsync()
    {
        if (File.Exists(_filePath)) File.Delete(_filePath);
        return Task.CompletedTask;
    }
}
