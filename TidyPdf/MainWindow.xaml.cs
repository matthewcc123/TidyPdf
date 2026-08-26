using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.Windows.AppNotifications;
using Microsoft.Windows.AppNotifications.Builder;
using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using TidyPdf.Dialogs;
using TidyPdf.Helpers;
using TidyPdf.Services;
using TidyPdf.Views;
using Windows.UI.WindowManagement;
using WinRT.Interop;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace TidyPdf
{
    /// <summary>
    /// An empty mainWindow that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private WinuiTrayIcon.TrayIcon? systemTrayIcon;

        private readonly ISettingService settingService = App.ServiceProvider.GetRequiredService<ISettingService>();

        private readonly string iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/WindowIcon.ico");

        private readonly string title = "TidyPdf";
        private bool isExiting;

        public MainWindow()
        {
            InitializeComponent();
        }

        public void Init()
        {
            RegisterWindowEvents();

            InitializeWindow();
            InitializeTray();
            ApplySettings();
            InitializeNavigation();

            ShowWindow();
        }

        #region Initialization

        private void RegisterWindowEvents()
        {
            AppWindow.Changed += AppWindow_Changed;
            AppWindow.Closing += AppWindow_Closing;
        }

        private void InitializeWindow()
        {
            //Window size
            AppWindow.Resize(new Windows.Graphics.SizeInt32(1336, 800));

            if (AppWindow.Presenter is OverlappedPresenter presenter)
            {
                presenter.PreferredMinimumWidth = 900;
                presenter.PreferredMinimumHeight = 600;
            }

            //Window title
            AppWindow.Title = title;
            AppWindow.SetIcon(iconPath);

            //WinUI title bar
            ExtendsContentIntoTitleBar = true;
        }

        private void InitializeTray()
        {
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);

            systemTrayIcon = new WinuiTrayIcon.TrayIcon(hwnd, iconPath)
            {
                IsIconVisible = false,
                IconToolTip = title
            };

            systemTrayIcon.AddMenuItem("Organize");
            systemTrayIcon.AddMenuItem("Settings");
            systemTrayIcon.AddMenuSeparator();
            systemTrayIcon.AddMenuItem("Exit");

            systemTrayIcon.LeftClicked += SystemTrayIcon_LeftClicked;
            systemTrayIcon.MenuItemClicked += SystemTrayIcon_MenuItemClicked;
        }

        private void ApplySettings()
        {
            ThemeHelper.ChangeTheme(settingService.Settings.AppTheme);
            UpdateTrayIcon();
        }

        private void InitializeNavigation()
        {
            MainNavView.SelectedItem = MainNavView.MenuItems.First();
        }

        #endregion

        #region Window

        public void ShowWindow()
        {
            Activate();

            // Bring to front
            if (AppWindow.Presenter is OverlappedPresenter presenter)
            {
                presenter.IsAlwaysOnTop = true;
                presenter.IsAlwaysOnTop = false;
            }
        }

        private void AppWindow_Changed(Microsoft.UI.Windowing.AppWindow sender, Microsoft.UI.Windowing.AppWindowChangedEventArgs args)
        {
            if (!settingService.Settings.MinimizeToTray)
                return;

            if (!args.DidPresenterChange)
                return;

            if (sender.Presenter is not OverlappedPresenter presenter)
                return;

            if (presenter.State != OverlappedPresenterState.Minimized)
                return;

            sender.Hide();
            ShowMinimizeNotification();
        }

        private void AppWindow_Closing(Microsoft.UI.Windowing.AppWindow sender, Microsoft.UI.Windowing.AppWindowClosingEventArgs args)
        {
            if (isExiting)
                return;

            var minimizeToTray = settingService.Settings.MinimizeToTray;
            var closeButtonMinimizesToTray = settingService.Settings.CloseButtonMinimizesToTray;

            if (minimizeToTray && closeButtonMinimizesToTray)
            {
                args.Cancel = true;

                sender.Hide();
                ShowMinimizeNotification();

                return;
            }

            isExiting = true;

            Dispose();
            ExitApplication();
        }

        private void ShowMinimizeNotification()
        {
            var notification = new AppNotificationBuilder()
                .AddText("App minimized to system tray")
                .AddText(
                    "The application is still running in the background. " +
                    "Click the tray icon to restore it.")
                .SetAudioEvent(AppNotificationSoundEvent.IM)
                .SetTimeStamp(DateTime.Now)
                .BuildNotification();

            AppNotificationManager.Default.Show(notification);
        }

        #endregion

        #region System Tray

        private void SystemTrayIcon_LeftClicked(object? sender, EventArgs e)
        {
            ShowWindow();
            MainNavView.SelectedItem = MainNavView.MenuItems.First();
        }

        private void SystemTrayIcon_MenuItemClicked(object? sender, MenuFlyoutItem e)
        {
            switch (e.Text)
            {
                case "Organize":
                    ShowWindow();
                    MainNavView.SelectedItem = MainNavView.MenuItems.First();
                    break;

                case "Settings":
                    ShowWindow();
                    MainNavView.SelectedItem = MainNavView.SettingsItem;
                    break;

                case "Exit":
                    Dispose();
                    ExitApplication();
                    break;
            }
        }

        public void UpdateTrayIcon()
        {
            if (systemTrayIcon != null)
            {
                systemTrayIcon.IsIconVisible =
                    settingService.Settings.MinimizeToTray;
            }
        }

        #endregion

        #region Cleanup

        private void Dispose()
        {
            AppWindow.Changed -= AppWindow_Changed;
            AppWindow.Closing -= AppWindow_Closing;

            if (systemTrayIcon == null)
                return;

            systemTrayIcon.LeftClicked -= SystemTrayIcon_LeftClicked;
            systemTrayIcon.MenuItemClicked -= SystemTrayIcon_MenuItemClicked;
            systemTrayIcon.Dispose();

            systemTrayIcon = null;
        }

        private void ExitApplication()
        {
            Application.Current.Exit();
        }

        #endregion

        #region Navigation

        private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItem is not NavigationViewItem selectedItem) return;

            switch (selectedItem.Tag)
            {
                case "Organize":
                    MainFrame.Navigate(
                        typeof(OrganizerPage),
                        null,
                        new DrillInNavigationTransitionInfo());
                    break;

                case "Donate":
                    break;

                case "Settings":
                    MainFrame.Navigate(
                        typeof(SettingPage),
                        null,
                        new DrillInNavigationTransitionInfo());
                    break;
            }
        }

        private async void MainNavView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.InvokedItemContainer?.Tag is not string tag)
                return;

            if (tag != "Donate")
                return;

            var dialog = new DonateDialog{ XamlRoot = Content.XamlRoot };

            await dialog.ShowAsync();
        }

        #endregion
    }
}
