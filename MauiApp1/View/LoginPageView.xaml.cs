using AdvocaPro.ViewModel;

namespace AdvocaPro.View
{
    public partial class LoginPageView : ContentPage
    {
        private readonly LoginPageViewModel _viewModel;

        public LoginPageView(LoginPageViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = _viewModel = viewModel;
        }
    }
}
