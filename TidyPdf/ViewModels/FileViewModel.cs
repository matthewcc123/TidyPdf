using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TidyPdf.Enums;
using Windows.UI;

namespace TidyPdf.ViewModels
{
    public partial class FileViewModel : ObservableObject
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(FullName))]
        [NotifyPropertyChangedFor(nameof(Name))]
        public partial string Path { get; set; } = string.Empty;
        [ObservableProperty]
        public partial SolidColorBrush ColorBrush { get; set; } = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
        [ObservableProperty]
        public partial FileItemType Type { get; set; }

        public string FullName => System.IO.Path.GetFileName(Path) ?? string.Empty;
        public string Name => System.IO.Path.GetFileNameWithoutExtension(Path) ?? string.Empty;

        public FileViewModel(string path, FileItemType type)
        {
            Path = path;
            ColorBrush = new SolidColorBrush(Color.FromArgb(255, 125, 125, 125));
            Type = type;
        }
    }
}
