using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace RecipeBook.ViewModels
{
    // Base view model that implements INotifyPropertyChanged for data binding support
    public class BaseViewModel : INotifyPropertyChanged
    {
        // Backing fields for properties
        private bool _isBusy;
        private string _title;
        private string _errorMessage;

        // Indicates if a background operation is running
        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        // View model title
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        // Holds error messages for display in the UI
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        // Event raised when a property value changes
        public event PropertyChangedEventHandler PropertyChanged;

        // Sets the property and notifies listeners of the change
        protected bool SetProperty<T>(ref T backingStore, T value,
            [CallerMemberName] string propertyName = "",
            Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        // Raises the PropertyChanged event for the specified property
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Executes an asynchronous action while showing a busy indicator
        protected async Task ExecuteWithBusyIndicator(Func<Task> action)
        {
            if (IsBusy)
                return;

            try
            {
                ErrorMessage = string.Empty;
                IsBusy = true;
                await action();
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
