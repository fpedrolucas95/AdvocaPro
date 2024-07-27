using AdvocaPro.ViewModel;

namespace AdvocaPro.View
{
    public partial class DeadlineManagementView : ContentPage
    {
        public DeadlineManagementView(DeadlineManagementViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
