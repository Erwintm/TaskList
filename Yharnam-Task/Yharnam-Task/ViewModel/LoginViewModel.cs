using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Yharnam_Task.Services;
using Yharnam_Task.View;
using Yharnam_Task.Models;

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

    private async Task<string> MostrarPopupAsync(string pregunta, string[] opciones)
    {
        string respuesta = await Application.Current!.MainPage.DisplayActionSheet(pregunta, "Cancelar", null, opciones);
        return respuesta ?? "";
    }

}
