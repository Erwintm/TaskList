using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;
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

            prefs.PreferenciaDificultad = await MostrarPopupAsync("¿Qué dificultad prefieres?", new[] { "Fácil", "Normal", "Difícil" });
            prefs.PreferenciaPrioridad = await MostrarPopupAsync("¿Qué prioridad te interesa?", new[] { "Alta", "Media", "Baja" });
            prefs.PreferenciaDuracion = await MostrarPopupAsync("¿Qué duración te gusta?", new[] { "Corta", "Media", "Larga" });

            await _usuarioService.SavePreferenciasAsync(prefs);

            Debug.WriteLine("🔸 ✅ Preferencias guardadas, mostrando prioridades...");

            // FLUJO COHESIVO DE PRIORIDADES
            var (primera, segunda, tercera) = await MostrarFlujoPrioridadesCohesivo();

            await _usuarioService.SavePrioridadesAsync(primera, segunda, tercera);

            Debug.WriteLine("🔸 ✅ Navegando a MenuPage...");

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
            new[] { "📊 Dificultad", "⏰ Tiempo de entrega", "🕒 Duración" }
        );

        primera = primera.Replace("📊 ", "").Replace("⏰ ", "").Replace("🕒 ", "");

        Debug.WriteLine($"🔸 Primera prioridad: {primera}");

        var opcionesRestantes = new List<string> { "Dificultad", "Tiempo de entrega", "Duración" };
        opcionesRestantes.Remove(primera);

        var opcionesConEmojis = opcionesRestantes.Select(op =>
            op == "Dificultad" ? "📊 Dificultad" :
            op == "Tiempo de entrega" ? "⏰ Tiempo de entrega" :
            "🕒 Duración"
        ).ToArray();

        segunda = await Application.Current.MainPage.DisplayActionSheet(
            $"🎯 Segunda Prioridad\n\nPrimera: {primera}\n\nAhora elige la segunda:",
            null, 
            null,
            opcionesConEmojis
        );

        segunda = segunda.Replace("📊 ", "").Replace("⏰ ", "").Replace("🕒 ", "");

        Debug.WriteLine($"🔸 Segunda prioridad: {segunda}");

        opcionesRestantes.Remove(segunda);
        tercera = opcionesRestantes[0];

        Debug.WriteLine($"🔸 Tercera prioridad: {tercera}");

        await Application.Current.MainPage.DisplayAlert(
            "✅ Prioridades Establecidas",
            $"🎯 Tu orden de prioridades:\n\n" +
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

