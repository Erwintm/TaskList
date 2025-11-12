using CommunityToolkit.Maui.Extensions;
using System.Text.Json;
using Yharnam_Task.Services;

namespace Yharnam_Task.View;

public partial class MenuPage : ContentPage
{
    public MenuPage()
    {
        InitializeComponent();
        MostrarConfiguracionGuardada();
    }

    private void OnPopupButtonClicked(object sender, EventArgs e)
    {
        var popup = new PopupView
        {
            BindingContext = App.MainViewModel 
        };
        this.ShowPopup(popup);
    }

    private async void MostrarConfiguracionGuardada()
    {
        try
        {
            var usuarioService = new UsuarioService();
            var usuario = await usuarioService.GetUsuarioAsync();

            if (usuario == null)
            {
                await DisplayAlert("Archivo", "No hay archivo de configuración guardado.", "OK");
                return;
            }

            var json = JsonSerializer.Serialize(usuario, new JsonSerializerOptions { WriteIndented = true });

            System.Diagnostics.Debug.WriteLine("=== CONFIGURACIÓN GUARDADA ===");
            System.Diagnostics.Debug.WriteLine(json);
            System.Diagnostics.Debug.WriteLine("===============================");

            await DisplayAlert("Configuración guardada", json, "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No se pudo leer la configuración:\n{ex.Message}", "OK");
        }
    }
}
