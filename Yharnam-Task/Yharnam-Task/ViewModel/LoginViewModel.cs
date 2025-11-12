using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Yharnam_Task.Models;
using Yharnam_Task.Services;
using Yharnam_Task.View;

namespace Yharnam_Task.ViewModel;

public partial class LoginViewModel : ObservableObject
{
    private readonly UsuarioService _usuarioService;

    [ObservableProperty]
    private string nombre = string.Empty;

    [ObservableProperty]
    private bool isBusy;

    public IAsyncRelayCommand AceptarCommand { get; }

    public LoginViewModel(UsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
        AceptarCommand = new AsyncRelayCommand(OnAceptarAsync, CanAceptar);

        PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(Nombre) || e.PropertyName == nameof(IsBusy))
                AceptarCommand.NotifyCanExecuteChanged();
        };
    }

    private bool CanAceptar() => !string.IsNullOrWhiteSpace(Nombre) && !IsBusy;
    private async Task OnAceptarAsync()
    {
        try
        {
            IsBusy = true;
            await _usuarioService.SaveUsuarioAsync(Nombre);

            var prefs = new ConfiguracionUsuario();

            prefs.PreferenciaDificultad = await MostrarPopupAsync("Prefieres realizar las tareas con DIFICULTAD...", new[] { "Facil", "Normal", "Dificil" });
            prefs.PreferenciaPrioridad = await MostrarPopupAsync("Te gusta hacer las tareas con PRIORIDAD...", new[] { "Alta", "Media", "Baja" });
            prefs.PreferenciaDuracion = await MostrarPopupAsync("Acostumbras realizar las tareas de DURACIÓN...", new[] { "Corta", "Media", "Larga" });

            await _usuarioService.SavePreferenciasAsync(prefs);

            var (primera, segunda, tercera) = await MostrarFlujoPrioridadesCohesivo();

            await _usuarioService.SavePrioridadesAsync(primera, segunda, tercera);

            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                Application.Current!.MainPage = new MenuPage();
            });
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task<(string primera, string segunda, string tercera)> MostrarFlujoPrioridadesCohesivo()
    {
        string primera = string.Empty;
        string segunda = string.Empty;
        string tercera = string.Empty;

        primera = await Application.Current.MainPage.DisplayActionSheet(
            "🎯 Prioridad Principal\n\n¿Qué factor es MÁS importante para ti?",
            null, 
            null,
            new[] { "📊 Dificultad", "⏰ Tiempo de entrega", "🕒 Duracion" }
        );

        primera = primera.Replace("📊 ", "").Replace("⏰ ", "").Replace("🕒 ", "");

        var opcionesRestantes = new List<string> { "Dificultad", "Tiempo de entrega", "Duracion" };
        opcionesRestantes.Remove(primera);

        var opcionesConEmojis = opcionesRestantes.Select(op =>
            op == "Dificultad" ? "📊 Dificultad" :
            op == "Tiempo de entrega" ? "⏰ Tiempo de entrega" :
            "🕒 Duracion"
        ).ToArray();

        segunda = await Application.Current.MainPage.DisplayActionSheet(
            $"🎯 Segunda Prioridad\n\nPrimera: {primera}\n\nAhora elige la segunda:",
            null, 
            null,
            opcionesConEmojis
        );

        segunda = segunda.Replace("📊 ", "").Replace("⏰ ", "").Replace("🕒 ", "");

        opcionesRestantes.Remove(segunda);
        tercera = opcionesRestantes[0];

        await Application.Current.MainPage.DisplayAlert(
            "✅ Prioridades Establecidas",
            $"Tu orden de prioridades:\n\n" +
            $"🥇 1. {primera}\n" +
            $"🥈 2. {segunda}\n" +
            $"🥉 3. {tercera}\n\n" +
            $"¡Perfecto! Ahora organizaremos tus tareas según estas preferencias.",
            "Continuar"
        );

        return (primera, segunda, tercera);
    }

    private async Task<string> MostrarPopupAsync(string pregunta, string[] opciones)
    {
        string respuesta = await Application.Current!.MainPage.DisplayActionSheet(pregunta, "Cancelar", null, opciones);
        return respuesta ?? "";
    }
}

