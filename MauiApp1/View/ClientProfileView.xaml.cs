using AdvocaPro.Services;
using AdvocaPro.ViewModel;
using System.Globalization;

namespace AdvocaPro.View
{
    public partial class ClientProfileView : ContentPage
    {
        public ClientProfileView(ClientService clientService, LoginService loginService)
        {
            InitializeComponent();
            BindingContext = new ClientProfileViewModel(clientService, loginService);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            var culture = new CultureInfo("pt-BR");
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
        }

        protected override void OnNavigatedTo(NavigatedToEventArgs args)
        {
            base.OnNavigatedTo(args);

            if (BindingContext is ClientProfileViewModel viewModel && Shell.Current.CurrentPage.BindingContext is ClientViewModel clientViewModel)
            {
                viewModel.Initialize(clientViewModel.SelectedClient);
            }
        }
    }
}
