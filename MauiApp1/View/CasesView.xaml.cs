using AdvocaPro.ViewModel;

namespace AdvocaPro.View
{
    public partial class CasesView : ContentPage
    {
        public CasesView(CasesViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
