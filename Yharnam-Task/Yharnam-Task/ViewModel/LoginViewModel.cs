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

            string dificultadRaw = await MostrarPopupAsync(
            "📊 Prefieres realizar las tareas con DIFICULTAD...",
            new[] { "🙂 Facil", "🥶 Normal", "😈 Dificil" });

            string prioridadRaw = await MostrarPopupAsync(
                "⏳ Te gusta hacer las tareas con PRIORIDAD de entrega...",
                new[] { "🔥 Alta", "⚖️ Media", "🧊 Baja" });

            string duracionRaw = await MostrarPopupAsync(
                "⏱️ Acostumbras realizar las tareas de DURACIÓN...",
                new[] { "🕐 Corta", "🕑 Media", "🕜 Larga" });

            prefs.PreferenciaDificultad = QuitarEmoji(dificultadRaw);
            prefs.PreferenciaPrioridad = QuitarEmoji(prioridadRaw);
            prefs.PreferenciaDuracion = QuitarEmoji(duracionRaw);

            await _usuarioService.SavePreferenciasAsync(prefs);

            var (primera, segunda, tercera) = await MostrarFlujoPrioridadesCohesivo(prefs, Nombre);

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

    private async Task<(string primera, string segunda, string tercera)>
    MostrarFlujoPrioridadesCohesivo(ConfiguracionUsuario prefs, string nombreUsuario)
    {
        string primera = string.Empty;
        string segunda = string.Empty;
        string tercera = string.Empty;

        const string DIF = "📊 Dificultad";
        const string TIE = "⏳ Tiempo de entrega";
        const string DUR = "⏱️ Duracion";

        primera = await Application.Current.MainPage.DisplayActionSheet(
            "             🎯 Prioridad Principal\n¿Qué factor es MÁS importante para ti?",
            null,
            null,
            new[] { DIF, TIE, DUR }
        );

        primera = primera.Replace("📊 ", "").Replace("⏳ ", "").Replace("⏱️ ", "");

        var opcionesRestantes = new List<string> { "Dificultad", "Tiempo de entrega", "Duracion" };
        opcionesRestantes.Remove(primera);

        var opcionesConEmojis = opcionesRestantes.Select(op =>
            op == "Dificultad" ? DIF :
            op == "Tiempo de entrega" ? TIE :
            DUR
        ).ToArray();

        segunda = await Application.Current.MainPage.DisplayActionSheet(
            $"🎯 Ahora elige la segunda prioridad:\n(Elegiste como primera: {primera})",
            null,
            null,
            opcionesConEmojis
        );

        segunda = segunda.Replace("📊 ", "").Replace("⏳ ", "").Replace("⏱️ ", "");

        opcionesRestantes.Remove(segunda);
        tercera = opcionesRestantes[0];

        await Application.Current.MainPage.DisplayAlert(
            $"¡Bienvenido {nombreUsuario}!",

            $"Preferencias iniciales\n" +
            $"📊Dificultad: {prefs.PreferenciaDificultad}\n" +
            $"⏳Prioridad de entrega: {prefs.PreferenciaPrioridad}\n" +
            $"⏱️Duración: {prefs.PreferenciaDuracion}\n\n" +

            $"Tus prioridades finales\n" +
            $"🥇 1. {primera}\n" +
            $"🥈 2. {segunda}\n" +
            $"🥉 3. {tercera}\n\n" +
            $"¡Perfecto! Ahora organizaremos tus tareas según estas preferencias.",
            "Continuar"
        );

        return (primera, segunda, tercera);
    }

    private string QuitarEmoji(string texto)
    {
        if (string.IsNullOrEmpty(texto) || texto.Length <= 2)
            return texto;

        return texto.Substring(2).Trim();
    }

    private async Task<string> MostrarPopupAsync(string pregunta, string[] opciones)
    {
        string respuesta = await Application.Current!.MainPage.DisplayActionSheet(
            pregunta,
            null,         
            null,
            opciones);

        return respuesta ?? "";
    }

}

