using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
}
