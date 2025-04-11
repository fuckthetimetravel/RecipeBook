using Microsoft.Maui.Controls;
using RecipeBook.ViewModels;

namespace RecipeBook.Views
{
    // ProfilePage binds to ProfileViewModel to display and manage user profile information.
    public partial class ProfilePage : ContentPage
    {
        private readonly ProfileViewModel _viewModel;

        // Constructor that initializes the page with the provided ProfileViewModel.
        public ProfilePage(ProfileViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
            _viewModel = viewModel;
        }

        // When the page appears, refresh the user data.
        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.LoadUserData();
        }
    }
}
