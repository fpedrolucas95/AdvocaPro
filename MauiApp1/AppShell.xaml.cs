using AdvocaPro.View;

namespace AdvocaPro
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute("LoginPage", typeof(View.LoginPageView));
            Routing.RegisterRoute("WelcomeView", typeof(View.WelcomeView));
        }
    }
}
