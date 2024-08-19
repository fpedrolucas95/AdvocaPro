using AdvocaPro.ViewModel;

namespace AdvocaPro.View
{
    public partial class DriverView : ContentPage
    {
        public DriverView(DriverViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
