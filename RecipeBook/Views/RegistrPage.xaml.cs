using Microsoft.Maui.Controls;
using RecipeBook.ViewModels;

namespace RecipeBook.Views
{
    public partial class RegistrPage : ContentPage
    {
        public RegistrPage(RegistrationViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        private async void OnPickProfileImageClicked(object sender, EventArgs e)
        {
            if (BindingContext is RegistrationViewModel vm)
                await vm.PickProfileImageAsync();
        }

        private async void OnTakeProfilePhotoClicked(object sender, EventArgs e)
        {
            if (BindingContext is RegistrationViewModel vm)
                await vm.TakeProfilePhotoAsync();
        }

    }
}