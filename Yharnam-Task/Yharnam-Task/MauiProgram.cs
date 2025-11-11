using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using Yharnam_Task.Services;
using Yharnam_Task.ViewModel;
using Yharnam_Task.View;

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

#if DEBUG
        builder.Logging.AddDebug();
#endif

        builder.Services.AddSingleton<UsuarioService>();
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddSingleton<TareaViewModel>();
        builder.Services.AddTransient<PopupView>();

        return builder.Build();
    }
}
