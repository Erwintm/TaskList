using Yharnam_Task.View;

namespace Yharnam_Task
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new MenuPage();
        }
    }
}