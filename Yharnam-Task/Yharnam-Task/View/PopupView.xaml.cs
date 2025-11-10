using CommunityToolkit.Maui.Views;
using Yharnam_Task.ViewModel;

namespace Yharnam_Task.View
{
    public partial class PopupView : Popup
    {
        public PopupView()
        {
            InitializeComponent();

            this.BindingContext = new TareaViewModel();
        }

        private async void CrearTarea_Clicked(object sender, System.EventArgs e)
        {
            if (BindingContext is TareaViewModel viewModel)
            {
                if (viewModel.AgregarTareaCommand.CanExecute(null))
                {
                    viewModel.AgregarTareaCommand.Execute(null);
                }
            }

            await this.CloseAsync();
            await Application.Current.MainPage.DisplayAlert("Tarea guardada", "La información se ha guardado correctamente.", "Aceptar");
        }

        private async void CerrarPopup_Clicked(object sender, System.EventArgs e)
        {
            await this.CloseAsync();
        }
    }
}