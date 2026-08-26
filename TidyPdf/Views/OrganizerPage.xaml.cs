using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TidyPdf.Controls;
using TidyPdf.Enums;
using TidyPdf.ViewModels;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.Storage.Pickers;
using WinRT.Interop;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace TidyPdf.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class OrganizerPage : Page
    {
        public OrganizerPageViewModel ViewModel { get; } = App.ServiceProvider.GetRequiredService<OrganizerPageViewModel>();
        public OrganizerPage()
        {
            InitializeComponent();
            this.DataContext = ViewModel;

        }


        #region OrganizeFile
        private async Task Organize(WindowId windowId)
        {
            ViewModel.IsBusy = true;

            var picker = new FileSavePicker();

            nint hwnd = Win32Interop.GetWindowFromWindowId(windowId);
            InitializeWithWindow.Initialize(picker, hwnd);

            picker.FileTypeChoices.Add("PDF Document", new List<string>() { ".pdf" });

            picker.CommitButtonText = "Save";
            picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            picker.SuggestedFileName = $"{ViewModel.Files.First().Name}_organized" ?? "organized";
            picker.DefaultFileExtension = ".pdf";

            var result = await picker.PickSaveFileAsync();

            if (result != null)
                await ViewModel.Organize(result.Path);

            ViewModel.IsBusy = false;
        }

        private async void Organize_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
                await Organize(button.XamlRoot.ContentIslandEnvironment.AppWindowId);
        }

        #endregion


        #region AddingFile
        private async Task AddFiles(List<StorageFile> files)
        {
            if (!files.Any()) return;

            var fileViewModels = files.Select(file => new FileViewModel(
                        file.Path,
                        Path.GetExtension(file.Path).ToLower() == ".pdf" ? FileItemType.Pdf : FileItemType.Image
                    )).ToList();

            await ViewModel.AddFiles(fileViewModels);
        }

        //Picker
        private async Task PickFiles(WindowId windowId)
        {
            ViewModel.IsBusy = true;

            var picker = new FileOpenPicker();

            nint hwnd = Win32Interop.GetWindowFromWindowId(windowId);
            InitializeWithWindow.Initialize(picker, hwnd);

            picker.CommitButtonText = "Pick Files";
            picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            picker.ViewMode = PickerViewMode.List;
            foreach (var format in ViewModel.AllowedFormats.Split(','))
            {
                picker.FileTypeFilter.Add(format);
            }

            var files = await picker.PickMultipleFilesAsync();

            if (files.Any())
                await AddFiles(files.ToList());

            ViewModel.IsBusy = false;
        }
        private async void AddFile_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
                await PickFiles(button.XamlRoot.ContentIslandEnvironment.AppWindowId);
        }
        private async void FileDropper_Tapped(object sender, TappedRoutedEventArgs e)
        {
            e.Handled = true;

            if (sender is FileDropper dropper)
                await PickFiles(dropper.XamlRoot.ContentIslandEnvironment.AppWindowId);
        }

        //Dropper
        private async void FileDropper_Drop(object sender, DragEventArgs e)
        {

            if (sender is FileDropper dropper && e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                dropper.IsEnabled = false;

                var files = await e.DataView.GetStorageItemsAsync();

                //Validate Format
                if (!string.IsNullOrEmpty(dropper.FileFormats))
                {
                    var formats = dropper.FileFormats.Split(',').Select(f => f.Trim().ToLower()).ToList();
                    files = files.Where(item => formats.Contains(Path.GetExtension(item.Path).ToLower())).OfType<StorageFile>().ToList();
                }

                if (files.Any())
                    await AddFiles(files.OfType<StorageFile>().ToList());

                ViewModel.IsDragging = false;
                dropper.IsEnabled = true;
            }

        }

        //To allow Drop on top of GridView
        private new void DragOver(object sender, DragEventArgs e)
        {
            if (sender is GridView)
            {
                ViewModel.IsDragging = true;
            }
        }
        private new void DragLeave(object sender, DragEventArgs e)
        {
            if (sender is not GridView)
            {
                ViewModel.IsDragging = false;
            }
        }
        #endregion

    }
}
