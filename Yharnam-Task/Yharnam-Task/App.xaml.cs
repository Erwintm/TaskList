using Yharnam_Task.ViewModel;
using Yharnam_Task.View;

namespace Yharnam_Task;

public partial class App : Application
{
    public static TareaViewModel MainViewModel { get; } = new TareaViewModel();

    public App()
    {
        InitializeComponent();
        MainPage = new MenuPage();
    }
}
