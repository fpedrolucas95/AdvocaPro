using Microsoft.UI.Xaml;
using Microsoft.UI.Windowing;
using WinRT.Interop;
using Microsoft.UI;

namespace AdvocaPro.WinUI
{
    public partial class App : MauiWinUIApplication
    {
        public App()
        {
            this.InitializeComponent();
        }

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            base.OnLaunched(args);

            var currentWindow = Application.Windows[0].Handler?.PlatformView as Microsoft.UI.Xaml.Window;
            if (currentWindow != null)
            {
                IntPtr windowHandle = WindowNative.GetWindowHandle(currentWindow);
                WindowId windowId = Win32Interop.GetWindowIdFromWindow(windowHandle);
                AppWindow appWindow = AppWindow.GetFromWindowId(windowId);
                appWindow.Title = "Meu Título Personalizado";
            }
        }
    }
}
