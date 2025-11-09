using CommunityToolkit.Maui.Extensions;

namespace Yharnam_Task.View;

public partial class MenuPage : ContentPage
{
    public MenuPage()
    {
        InitializeComponent();
    }

    private void OnPopupButtonClicked(object sender, EventArgs e)
    {
        var popup = new PopupView();
        this.ShowPopup(popup);
    }
}