using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AdvocaPro.Services;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using System.Windows.Input;

namespace AdvocaPro.ViewModel
{
    public partial class LoginPageViewModel : ObservableObject
    {
        #region Fields
        private readonly LoginService _loginService;
        #endregion

        #region Properties
        [ObservableProperty]
        private string username = string.Empty;

        [ObservableProperty]
        private string password = string.Empty;

        public ICommand LoginCommand { get; }
        #endregion

        #region Constructor
        public LoginPageViewModel(LoginService loginService)
        {
            _loginService = loginService;
            LoginCommand = new AsyncRelayCommand(LoginAsync);
        }
        #endregion

        #region Private Methods
        private async Task LoginAsync()
        {
            try
            {
                if (await IsValidLoginAsync())
                {
                    await OpenMainPageAndCloseLogin();
                }
                else
                {
                    await ShowAlert("Erro", "Nome de usuário ou senha inválidos.");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exception in LoginAsync: {ex.Message}");
                await ShowAlert("Erro", "Ocorreu um erro ao processar o login.");
            }
        }

        private async Task<bool> IsValidLoginAsync()
        {
            var user = await _loginService.GetUserAsync(Username, Password);
            return user != null;
        }

        private async Task OpenMainPageAndCloseLogin()
        {
            var mainPage = CreateMainPageWindow();
            if (mainPage != null)
            {
                ConfigureMainPageWindow(mainPage);
                await CloseAllWindowsExcept(mainPage);
                await Shell.Current.GoToAsync("//WelcomeView");
            }
        }

        private Microsoft.Maui.Controls.Window? CreateMainPageWindow()
        {
            try
            {
                var mainPage = new Microsoft.Maui.Controls.Window(new AdvocaPro.View.MainPage());
                Application.Current?.OpenWindow(mainPage);
                return mainPage;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exception in CreateMainPageWindow: {ex.Message}");
                return null;
            }
        }

        private void ConfigureMainPageWindow(Microsoft.Maui.Controls.Window mainPage)
        {
            try
            {
                var handler = mainPage.Handler;
                if (handler?.PlatformView is not Microsoft.UI.Xaml.Window window) return;

                var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
                var windowId = Win32Interop.GetWindowIdFromWindow(hwnd);
                var appWindow = AppWindow.GetFromWindowId(windowId);

                if (appWindow.Presenter is OverlappedPresenter presenter)
                {
                    presenter.IsResizable = true;
                    presenter.Maximize();
                }

                window.Activate();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exception in ConfigureMainPageWindow: {ex.Message}");
            }
        }

        private async Task CloseAllWindowsExcept(Microsoft.Maui.Controls.Window exceptWindow)
        {
            try
            {
                if (Application.Current == null) return;

                var windowsToClose = Application.Current.Windows.Where(w => w != exceptWindow).ToList();
                foreach (var window in windowsToClose)
                {
                    Application.Current.CloseWindow(window);
                }

                await Task.Delay(100);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exception in CloseAllWindowsExcept: {ex.Message}");
            }
        }

        private async Task ShowAlert(string title, string message)
        {
            try
            {
                var mainPage = Application.Current?.MainPage;
                if (mainPage != null)
                {
                    await mainPage.DisplayAlert(title, message, "OK");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Unable to display alert: MainPage is null.");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exception in ShowAlert: {ex.Message}");
            }
        }
        #endregion
    }
}
