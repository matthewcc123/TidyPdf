using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TidyPdf.Helpers;
using static TidyPdf.Helpers.ImageHelper;

namespace TidyPdf.Models
{
    public class AppSettings
    {
        public ElementTheme AppTheme { get; set; } = ElementTheme.Default;
        public PageSize PageSize { get; set; } = PageSize.A4;
        public PageOrientation PageOrientation { get; set; } = PageOrientation.Portrait;
        public PageMargin PageMargin { get; set; } = PageMargin.Normal;
        public bool MinimizeToTray { get; set; } = true;
        public bool CloseButtonMinimizesToTray { get; set; } = false;
    }
}
