using CommunityToolkit.Maui.Extensions;
using System.Text.Json;
using Yharnam_Task.Services;

namespace Yharnam_Task.View;

public partial class MenuPage : ContentPage
{
    public MenuPage()
    {
        InitializeComponent();
        _ = CargarPreferenciasAsync();
    }

    private void OnPopupButtonClicked(object sender, EventArgs e)
    {
        var popup = new PopupView
        {
            BindingContext = App.MainViewModel
        };
        this.ShowPopup(popup);
    }

    private async void OnConfigurarClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Configurar", "Aquí irá la configuración de preferencias", "OK");
    }

    private async Task CargarPreferenciasAsync()
    {
        try
        {
            var usuarioService = new UsuarioService();
            var usuario = await usuarioService.GetUsuarioAsync();

            if (usuario == null)
            {
                NoPreferencesView.IsVisible = true;
                PreferencesContainer.IsVisible = false;
                return;
            }

            NoPreferencesView.IsVisible = false;
            PreferencesContainer.IsVisible = true;

            var json = JsonSerializer.Serialize(usuario, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error al cargar preferencias: {ex.Message}");
            NoPreferencesView.IsVisible = true;
            PreferencesContainer.IsVisible = false;
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _ = CargarPreferenciasAsync();
    }
}