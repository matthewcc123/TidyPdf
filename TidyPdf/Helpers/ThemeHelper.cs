using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace TidyPdf.Helpers
{
    public static class ThemeHelper
    {
        public static ElementTheme GetCurrentTheme()
        {
            if (App.MainWindow == null) { throw new InvalidOperationException("MainWindow is not initialized"); ; }

            return ((FrameworkElement)App.MainWindow.Content).RequestedTheme;
        }
        public static void CycleTheme()
        {
            if (App.MainWindow == null) { return; }

            // Get current theme before cycle
            var currentTheme = GetCurrentTheme();
            int nextTheme = ((int)currentTheme + 1) % 3;
            var newTheme = (ElementTheme)nextTheme;

            // Apply the new theme
            ((FrameworkElement)App.MainWindow.Content).RequestedTheme = newTheme;
        }

        public static void UpdateTitleBar(ElementTheme requestedTheme)
        {
            AppWindowTitleBar titleBar = App.MainWindow.AppWindow.TitleBar;
            var isDark = requestedTheme == ElementTheme.Dark || requestedTheme == ElementTheme.Default && Application.Current.RequestedTheme == ApplicationTheme.Dark;

           titleBar.PreferredTheme = isDark ? TitleBarTheme.Dark : TitleBarTheme.Light;

            if (isDark)
            {
                titleBar.ButtonHoverBackgroundColor = Color.FromArgb(25, 255, 255, 255);
                titleBar.ButtonPressedBackgroundColor = Color.FromArgb(50, 255, 255, 255);
                titleBar.ButtonForegroundColor = Colors.White;

                titleBar.ForegroundColor = Colors.White;
                titleBar.InactiveForegroundColor = Colors.Gray;
            }
            else
            {
                titleBar.ButtonHoverBackgroundColor = Color.FromArgb(25, 0, 0, 0);
                titleBar.ButtonPressedBackgroundColor = Color.FromArgb(50, 0, 0, 0);
                titleBar.ButtonForegroundColor = Colors.Black;

                titleBar.ForegroundColor = Colors.Black;
                titleBar.InactiveForegroundColor = Colors.DarkGray;
            }

            titleBar.ButtonBackgroundColor = Colors.Transparent;
            titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
            titleBar.InactiveBackgroundColor = Colors.Transparent;
            titleBar.BackgroundColor = Colors.Transparent;
        }

        public static void ChangeTheme(ElementTheme newTheme)
        {
            if (App.MainWindow == null) { return; }

            // Apply the new theme
            ((FrameworkElement)App.MainWindow.Content).RequestedTheme = newTheme;
        }
    }
}
