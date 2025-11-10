using CommunityToolkit.Maui.Views;
using Yharnam_Task.ViewModel;

namespace Yharnam_Task.View
{
    public partial class PopupView : Popup
    {
        public PopupView()
        {
            InitializeComponent();

            // Establecer el BindingContext
            this.BindingContext = new TareaViewModel();
        }

        private async void CrearTarea_Clicked(object sender, System.EventArgs e)
        {
            if (BindingContext is TareaViewModel viewModel)
            {
                // Ejecutar el comando para agregar tarea
                if (viewModel.AgregarTareaCommand.CanExecute(null))
                {
                    viewModel.AgregarTareaCommand.Execute(null);
                }
            }

            await this.CloseAsync();
        }

        private async void CerrarPopup_Clicked(object sender, System.EventArgs e)
        {
            await this.CloseAsync();
        }
    }
}