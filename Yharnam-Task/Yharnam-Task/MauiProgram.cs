#if WINDOWS
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using WinRT.Interop;
#endif

using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using Yharnam_Task.Services;
using Yharnam_Task.ViewModel;
using Yharnam_Task.View;
using Microsoft.Maui.LifecycleEvents;

namespace Yharnam_Task;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if WINDOWS
        builder.ConfigureLifecycleEvents(events =>
        {
            events.AddWindows(wndLifeCycleBuilder =>
            {
                wndLifeCycleBuilder.OnWindowCreated(window =>
                {
                    // Obtener AppWindow
                    IntPtr hWnd = WindowNative.GetWindowHandle(window);
                    var windowId = Win32Interop.GetWindowIdFromWindow(hWnd);
                    var appWindow = AppWindow.GetFromWindowId(windowId);

                    // Mostrar barra de título y botones
                    appWindow.SetPresenter(AppWindowPresenterKind.Overlapped);

                    // Maximizar la ventana
                    if (appWindow.Presenter is OverlappedPresenter presenter)
                    {
                        presenter.Maximize();
                    }
                });
            });
        });
#endif

        builder.Services.AddSingleton<UsuarioService>();
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddSingleton<TareaViewModel>();
        builder.Services.AddTransient<PopupView>();

        return builder.Build();
    }
}
