using Yharnam_Task.Services;
using Yharnam_Task.View;
using Yharnam_Task.ViewModel;

namespace Yharnam_Task;

public partial class App : Application
{
    public static TareaViewModel MainViewModel { get; } = new TareaViewModel();
    private readonly UsuarioService _usuarioService = new();

    public App(LoginPage loginPage)
    {
        InitializeComponent();
        MainPage = new ContentPage
        {
            BackgroundColor = Colors.Black,
            Content = new Label
            {
                Text = "Cargando...",
                TextColor = Colors.White,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            }
        };

#if DEBUG
        var usuarioService = new Services.UsuarioService();
        //usuarioService.ClearAsync();

        var tareaService = new Services.TareaService();
        //tareaService.ClearAsync();
#endif
        VerificarUsuario(loginPage);
    }

    private async void VerificarUsuario(LoginPage loginPage)
    {
        try
        {
            await Task.Delay(3000); 

            bool hayUsuario = await _usuarioService.HasUsuarioAsync();

            if (hayUsuario)
            {
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    MainPage = new NavigationPage(new MenuPage());
                });
            }
            else
            {
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    MainPage = new NavigationPage(loginPage);
                });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al verificar usuario: {ex.Message}");
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                MainPage = new NavigationPage(loginPage);
            });
        }
    }

}
