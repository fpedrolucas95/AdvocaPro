using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AdvocaPro.ViewModel
{
    public partial class MainPageViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _title = "Welcome";

        public MainPageViewModel()
        {
            Shell.Current.Navigated += Current_Navigated;
        }

        private void Current_Navigated(object sender, ShellNavigatedEventArgs e)
        {
            Title = Shell.Current.CurrentItem.Title;
        }

        [RelayCommand]
        private async Task NavigateToPage(string route)
        {
            await Shell.Current.GoToAsync(route);
        }
    }
}