using Microsoft.Maui.Controls;
using RecipeBook.ViewModels;

namespace RecipeBook.Views
{
    // RegistrPage is the registration page for new users.
    // It binds to RegistrationViewModel to handle user registration logic.
    public partial class RegistrPage : ContentPage
    {
        // Constructor that initializes the page and sets its BindingContext.
        public RegistrPage(RegistrationViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        // Event handler for the "Pick Image" button click.
        // Invokes the method in the RegistrationViewModel to pick a profile image.
        private async void OnPickProfileImageClicked(object sender, EventArgs e)
        {
            if (BindingContext is RegistrationViewModel vm)
                await vm.PickProfileImageAsync();
        }

        // Event handler for the "Take Photo" button click.
        // Invokes the method in the RegistrationViewModel to capture a profile photo.
        private async void OnTakeProfilePhotoClicked(object sender, EventArgs e)
        {
            if (BindingContext is RegistrationViewModel vm)
                await vm.TakeProfilePhotoAsync();
        }
    }
}
