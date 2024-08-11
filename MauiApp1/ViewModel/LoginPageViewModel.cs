using AdvocaPro.Models;
using AdvocaPro.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace AdvocaPro.ViewModel
{
    public partial class LoginPageViewModel : ObservableObject
    {
        #region Fields
        private readonly LoginService _loginService;
        private readonly UserService _userService;
        #endregion

        #region Properties

        private string _username = string.Empty;
        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        private string _password = string.Empty;
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        private string _confirmPassword = string.Empty;
        public string ConfirmPassword
        {
            get => _confirmPassword;
            set => SetProperty(ref _confirmPassword, value);
        }

        private string _firstName = string.Empty;
        public string FirstName
        {
            get => _firstName;
            set => SetProperty(ref _firstName, value);
        }

        private string _lastName = string.Empty;
        public string LastName
        {
            get => _lastName;
            set => SetProperty(ref _lastName, value);
        }

        private string _cellPhone = string.Empty;
        public string CellPhone
        {
            get => _cellPhone;
            set => SetProperty(ref _cellPhone, value);
        }

        private DateTime _birthdayDate = DateTime.Today;
        public DateTime BirthdayDate
        {
            get => _birthdayDate;
            set
            {
                SetProperty(ref _birthdayDate, value);
                CalculateAge();
            }
        }

        private int? _age;
        public int? Age
        {
            get => _age;
            private set => SetProperty(ref _age, value);
        }

        private string _email = string.Empty;
        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        private bool _isFirstRun;
        public bool IsFirstRun
        {
            get => _isFirstRun;
            set => SetProperty(ref _isFirstRun, value);
        }

        private string _welcomeMessage = "Bem-vindo de volta";
        public string WelcomeMessage
        {
            get => _welcomeMessage;
            set => SetProperty(ref _welcomeMessage, value);
        }

        public ICommand LoginCommand { get; }
        public ICommand CreateUserCommand { get; }

        #endregion

        #region Constructor
        public LoginPageViewModel(LoginService loginService, UserService userService)
        {
            _loginService = loginService;
            _userService = userService;
            IsFirstRun = CheckIfFirstRun();

            WelcomeMessage = IsFirstRun ? "Seja bem-vindo" : "Bem-vindo de volta";

            LoginCommand = new AsyncRelayCommand(LoginAsync);
            CreateUserCommand = new AsyncRelayCommand(CreateUserAsync);
        }
        #endregion

        #region Private Methods
        private bool CheckIfFirstRun()
        {
            var users = _userService.GetUsers();
            return !users.Any();
        }

        private void CalculateAge()
        {
            if (BirthdayDate != DateTime.MinValue && BirthdayDate <= DateTime.Today)
            {
                var today = DateTime.Today;
                var age = today.Year - BirthdayDate.Year;

                if (BirthdayDate.Date > today.AddYears(-age))
                    age--;

                // Clamp the age to a reasonable range to avoid overflow
                Age = (age >= 0 && age <= 150) ? age : (int?)null; // Valid ages typically 0-150
            }
            else
            {
                Age = null; // Invalid date defaults to null
            }
        }

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

        private async Task CreateUserAsync()
        {
            try
            {
                if (!IsEmailValid(Email))
                {
                    await ShowAlert("Erro", "O e-mail inserido não é válido.");
                    return;
                }

                if (!ArePasswordsMatching(Password, ConfirmPassword))
                {
                    await ShowAlert("Erro", "As senhas não correspondem.");
                    return;
                }

                var adminUser = new User
                {
                    UserName = Username,
                    Password = Password,
                    FirstName = FirstName,
                    LastName = LastName,
                    CellPhone = CellPhone,
                    UserType = 0,
                    CreatedAt = DateTime.Now,
                    CreatedBy = "system",
                    BirthdayDate = BirthdayDate,
                    Age = Age,
                    Email = Email
                };

                _userService.AddUser(adminUser);
                await ShowAlert("Sucesso", "Usuário administrador criado com sucesso.");
                IsFirstRun = false;
                WelcomeMessage = "Bem-vindo de volta";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exception in CreateUserAsync: {ex.Message}");
                await ShowAlert("Erro", "Ocorreu um erro ao criar o usuário.");
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

        private bool IsEmailValid(string email)
        {
            var emailRegex = @"^[^@\s]+@[^@\s]+\.(com|pt|net|org|edu|gov|mil|biz|info|mobi|name|aero|jobs|museum|co|com\.[a-z]{2}|[a-z]{2}|br|com\.br|com\.pt)$";
            return Regex.IsMatch(email, emailRegex, RegexOptions.IgnoreCase);
        }

        private bool ArePasswordsMatching(string password, string confirmPassword)
        {
            return password == confirmPassword;
        }
        #endregion
    }
}
