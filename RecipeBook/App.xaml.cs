using Microsoft.Maui.Controls;

namespace RecipeBook
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
        }
    }
}