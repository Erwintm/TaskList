using Yharnam_Task.ViewModel;

namespace Yharnam_Task.View;

public partial class LoginPage : ContentPage
{
    public LoginPage(LoginViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
