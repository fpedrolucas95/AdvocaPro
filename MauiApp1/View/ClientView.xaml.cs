using AdvocaPro.ViewModel;

namespace AdvocaPro.View
{
    public partial class ClientView : ContentPage
    {
        public ClientView(ClientViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        private void DatePicker_DateSelected(object sender, DateChangedEventArgs e)
        {
            var viewModel = BindingContext as ClientViewModel;
            if (viewModel?.SelectedClient != null)
            {
                viewModel.SelectedClient.Date = e.NewDate;
                viewModel.UpdateClientCounts();
            }
        }
    }
}
