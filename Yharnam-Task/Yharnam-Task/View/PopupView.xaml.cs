using CommunityToolkit.Maui.Views;

namespace Yharnam_Task.View;

public partial class PopupView : Popup
{
    public PopupView()
    {
        InitializeComponent();
    }

    private async void CerrarPopup_Clicked(object sender, EventArgs e)
    {
        await this.CloseAsync();
    }
}