using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.Windows.AppNotifications;
using System;
using TidyPdf.Services;
using TidyPdf.ViewModels;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace TidyPdf
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        public static MainWindow MainWindow { get; private set; } = null!;
        public static IServiceProvider ServiceProvider { get; private set; } = null!;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();

            var services = new ServiceCollection();

            services.AddTransient<OrganizerPageViewModel>();
            services.AddTransient<SettingPageViewModel>();
            services.AddSingleton<ISettingService, SettingService>();

            ServiceProvider = services.BuildServiceProvider();

        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override async void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            await ServiceProvider.GetRequiredService<ISettingService>().LoadAsync();

            MainWindow = new MainWindow();
            MainWindow.Init();

            AppNotificationManager.Default.NotificationInvoked += Default_NotificationInvoked;
            AppNotificationManager.Default.Register();
        }

        private void Default_NotificationInvoked(AppNotificationManager sender, AppNotificationActivatedEventArgs args)
        {
            if (MainWindow != null)
            {

                MainWindow.DispatcherQueue.TryEnqueue(() => {

                    MainWindow.ShowWindow();

                });

            }
        }
    }
}
