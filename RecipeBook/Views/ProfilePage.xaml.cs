using Microsoft.Maui.Controls;
using RecipeBook.ViewModels;

namespace RecipeBook.Views
{
    public partial class ProfilePage : ContentPage
    {
        private readonly ProfileViewModel _viewModel;

        public ProfilePage(ProfileViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
            _viewModel = viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            // Refresh user data when page appears
            // Вместо LoadUserCommand используем метод, который есть в ViewModel
            _viewModel.IsBusy = true;
            // Здесь можно добавить логику обновления данных пользователя
            _viewModel.IsBusy = false;
        }
    }
}