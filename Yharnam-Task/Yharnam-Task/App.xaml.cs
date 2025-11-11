using Yharnam_Task.View;
using Yharnam_Task.ViewModel;

namespace Yharnam_Task;

public partial class App : Application
{
    public static TareaViewModel MainViewModel { get; } = new TareaViewModel();
    public App(LoginPage loginPage)
    {
        InitializeComponent();
        MainPage = new NavigationPage(loginPage);
    }
}
