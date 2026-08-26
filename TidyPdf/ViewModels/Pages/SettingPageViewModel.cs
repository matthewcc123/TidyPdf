using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using TidyPdf.Helpers;
using TidyPdf.Services;
using Windows.ApplicationModel;

namespace TidyPdf.ViewModels
{

    public partial class SettingPageViewModel : ObservableObject
    {
        [ObservableProperty]
        public partial int AppTheme { get; set; } = (int)ElementTheme.Default;
        [ObservableProperty]
        public partial int PageSize { get; set; } = (int)ImageHelper.PageSize.A4;
        [ObservableProperty]
        public partial int PageOrientation { get; set; } = (int)ImageHelper.PageOrientation.Portrait;
        [ObservableProperty]
        public partial int PageMargin { get; set; } = (int)ImageHelper.PageMargin.Normal;
        [ObservableProperty]
        public partial bool MinimizeToTray { get; set; } = true;
        [ObservableProperty]
        public partial bool CloseButtonMinimizesToTray { get; set; } = true;

        public string VersionText
        {
            get
            {
                try
                {
                    var version = Package.Current.Id.Version;
                    return $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
                }
                catch
                {
                    return "Unknown";
                }
            }
        }

        private readonly ISettingService settingService = App.ServiceProvider.GetRequiredService<ISettingService>();
        private CancellationTokenSource? cts;

        public SettingPageViewModel()
        {
            var settings = settingService.Settings;

            AppTheme = (int)settings.AppTheme;
            PageSize = (int)settings.PageSize;
            PageOrientation = (int)settings.PageOrientation;
            PageMargin = (int)settings.PageMargin;
            MinimizeToTray = settings.MinimizeToTray;
            CloseButtonMinimizesToTray = settings.CloseButtonMinimizesToTray;

        }

        private async void Save()
        {
            cts?.Cancel();
            cts = new CancellationTokenSource();
            var token = cts.Token;

            try
            {
                await Task.Delay(300, token);

                await settingService.SaveAsync(new Models.AppSettings
                {
                    AppTheme = (ElementTheme)this.AppTheme,
                    PageSize = (ImageHelper.PageSize)this.PageSize,
                    PageOrientation = (ImageHelper.PageOrientation)this.PageOrientation,
                    PageMargin = (ImageHelper.PageMargin)this.PageMargin,
                    MinimizeToTray = this.MinimizeToTray,
                    CloseButtonMinimizesToTray = this.CloseButtonMinimizesToTray,
                });

                App.MainWindow.UpdateTrayIcon();
            }
            catch
            {
                //Ignore
            }
        }

        partial void OnAppThemeChanged(int value)
        {
            ThemeHelper.ChangeTheme((ElementTheme)AppTheme);
            Save();
        }

        partial void OnPageSizeChanged(int value)
        {
            Save();
        }

        partial void OnPageOrientationChanged(int value)
        {
            Save();
        }

        partial void OnPageMarginChanged(int value)
        {
            Save();
        }

        partial void OnMinimizeToTrayChanged(bool value)
        {
            Save();
        }

        partial void OnCloseButtonMinimizesToTrayChanged(bool value)
        {
            Save();
        }

    }
}
