using AdvocaPro.Services;
using AdvocaPro.View;
using AdvocaPro.ViewModel;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Windows.Graphics;

namespace AdvocaPro
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                })
                .ConfigureLifecycleEvents(events =>
                {
#if WINDOWS
                    events.AddWindows(windows =>
                    {
                        windows.OnWindowCreated(window =>
                        {
                            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
                            var windowId = Win32Interop.GetWindowIdFromWindow(hwnd);
                            var appWindow = AppWindow.GetFromWindowId(windowId);
                            var displayArea = DisplayArea.GetFromWindowId(windowId, DisplayAreaFallback.Primary);
                            var workArea = displayArea.WorkArea;

                            // Ajusta o tamanho e a posição da janela para não sobrepor a barra de tarefas
                            var newHeight = Math.Min(720, workArea.Height);
                            var newWidth = Math.Min(480, workArea.Width);
                            appWindow.Resize(new SizeInt32(newWidth, newHeight));
                            appWindow.Move(new PointInt32((workArea.Width - newWidth) / 2, (workArea.Height - newHeight) / 2));
                            appWindow.Title = "Meu Título Personalizado";
                        });
                    });
#endif
                });

            // Registro do Banco de Dados, Serviços, ViewModels e Views
            builder.Services.AddSingleton<DatabaseService>();

            var serviceProvider = builder.Services.BuildServiceProvider();
            var databaseService = serviceProvider.GetRequiredService<DatabaseService>();
            var databasePath = databaseService.GetDatabasePath();

            builder.Services.AddSingleton(new CaseService(databasePath));
            builder.Services.AddSingleton(new ClientService(databasePath));
            builder.Services.AddSingleton(new ContactListService(databasePath));
            builder.Services.AddSingleton(new ExpertiseService(databasePath));
            builder.Services.AddSingleton(new FinanceService(databasePath));
            builder.Services.AddSingleton(new HearingService(databasePath));
            builder.Services.AddSingleton(new DeadlineService(databasePath));
            builder.Services.AddSingleton(new SettingsService(databasePath));
            builder.Services.AddSingleton(new UserService(databasePath));
            builder.Services.AddSingleton(new LoginService(databasePath));
            builder.Services.AddSingleton(new CategoryEntryService(databasePath));
            builder.Services.AddSingleton(new CategoryExitService(databasePath));
            builder.Services.AddSingleton(new PaymentMethodService(databasePath));
            builder.Services.AddSingleton(new ProcessTypeService(databasePath));
            builder.Services.AddSingleton(new HolidayService(databasePath));

            builder.Services.AddTransient<ClientViewModel>();
            builder.Services.AddTransient<ClientProfileViewModel>();
            builder.Services.AddTransient<CasesViewModel>();
            builder.Services.AddTransient<DeadlineManagementViewModel>();
            builder.Services.AddTransient<LoginPageViewModel>();
            builder.Services.AddSingleton<MainPageViewModel>();

            builder.Services.AddTransient<LoginPageView>();
            builder.Services.AddTransient<WelcomeView>();
            builder.Services.AddTransient<ClientView>();
            builder.Services.AddTransient<ClientProfileView>();
            builder.Services.AddTransient<CasesView>();
            builder.Services.AddTransient<DeadlineManagementView>();

            // Registro do AppShell
            builder.Services.AddSingleton<AppShell>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
