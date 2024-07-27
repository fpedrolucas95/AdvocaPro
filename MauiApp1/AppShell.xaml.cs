using AdvocaPro.View;

namespace AdvocaPro
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute("LoginPage", typeof(View.LoginPageView));
            Routing.RegisterRoute("ClientView", typeof(View.ClientView));
            Routing.RegisterRoute(nameof(ClientProfileView), typeof(ClientProfileView));
            Routing.RegisterRoute("CasesView", typeof(View.CasesView));
            Routing.RegisterRoute("WelcomeView", typeof(View.WelcomeView));
            Routing.RegisterRoute("DeadlineManagementView", typeof(View.DeadlineManagementView));
        }
    }
}
