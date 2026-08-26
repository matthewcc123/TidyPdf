using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Media;
using PDFiumSharp;
using PDFiumSharp.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TidyPdf.Enums;
using TidyPdf.Helpers;
using TidyPdf.Services;
using Windows.Storage;
using Windows.System;
using static TidyPdf.Helpers.ImageHelper;

namespace TidyPdf.ViewModels
{
    public partial class OrganizerPageViewModel : ObservableObject
    {
        public ObservableCollection<FileViewModel> Files { get; } = new ObservableCollection<FileViewModel>();
        public ObservableCollection<PageViewModel> Pages { get; } = new ObservableCollection<PageViewModel>();
        public ObservableCollection<PageViewModel> DeletedPages { get; } = new ObservableCollection<PageViewModel>();

        public string AllowedFormats { get; } = ".pdf,.jpg,.jpeg,.png,.webp";
        
        //Check Files & Pages
        public bool IsFileEmpty => Files.Count == 0;
        public bool HasFiles => Files.Count > 0;
        public bool IsPageEmpty => Pages.Count == 0;
        public bool HasPages => Pages.Count > 0;


        //Check File Dropper
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ShowDropZone))]
        public partial bool IsDragging { get; set; }
        public bool ShowDropZone => (IsDragging && HasFiles) || IsFileEmpty;

        
        //Check Can Executes
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CanProcess))]
        [NotifyPropertyChangedFor(nameof(CanEdit))]
        [NotifyPropertyChangedFor(nameof(CanOrganize))]
        public partial bool IsBusy { get; set; }

        public bool CanProcess => !IsBusy;
        public bool CanEdit => !IsBusy && HasFiles;
        public bool CanOrganize => !IsBusy && HasFiles && HasPages;

        //Progress
        [ObservableProperty]
        public partial string ProgressText { get; set; } = string.Empty;

        //Privates
        private FileViewModel? movedItem;
        private readonly ISettingService settingService = App.ServiceProvider.GetRequiredService<ISettingService>();

        public OrganizerPageViewModel()
        {
            Files.CollectionChanged += FileItems_CollectionChanged;
            Pages.CollectionChanged += Pages_CollectionChanged;
        }

        partial void OnIsBusyChanged(bool value)
        {
            if (value == false)
                ProgressText = string.Empty;

            RefreshCommandStates();
        }

        private void RefreshCommandStates()
        {
            OrganizeCommand.NotifyCanExecuteChanged();
            AddFilesCommand.NotifyCanExecuteChanged();
            MoveFileCommand.NotifyCanExecuteChanged();
            DeleteFileCommand.NotifyCanExecuteChanged();
            ClearFilesCommand.NotifyCanExecuteChanged();
            DeletePageCommand.NotifyCanExecuteChanged();
            RotatePageCommand.NotifyCanExecuteChanged();
            ResetPagesCommand.NotifyCanExecuteChanged();
        }

        private void Pages_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(IsPageEmpty));
            OnPropertyChanged(nameof(HasPages));
            OnPropertyChanged(nameof(CanEdit));
            OnPropertyChanged(nameof(CanOrganize));
            RefreshCommandStates();
        }

        private void FileItems_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(HasFiles));
            OnPropertyChanged(nameof(IsFileEmpty));
            OnPropertyChanged(nameof(ShowDropZone));
            OnPropertyChanged(nameof(CanEdit));
            OnPropertyChanged(nameof(CanOrganize));
            RefreshCommandStates();

            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                var removedItem = e.OldItems?[0] as FileViewModel;

                if (movedItem != removedItem)
                    movedItem = removedItem;

                return;
            }

            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                var addedItem = e.NewItems?[0] as FileViewModel;

                if (addedItem == null) return;
                if (addedItem != movedItem) return;

                if (MoveFileCommand.CanExecute(movedItem))
                    MoveFileCommand.Execute(movedItem);
            }

        }

        #region Loader

        //Loading Page from File
        private async Task LoadPagesFromFile(FileViewModel file)
        {
            var previewWidth = 256;
            var previewQuality = 25;
            var margin = 12 * (int)settingService.Settings.PageMargin;
            var size = settingService.Settings.PageSize;
            var orientation = settingService.Settings.PageOrientation;

            switch (file.Type)
            {
                case FileItemType.Pdf:

                    //Get Preview Pages
                    var pageImages = await Task.Run(() => PdfHelper.GetPageImages(file.Path, previewWidth, previewQuality));

                    //Add Pages to Collection
                    for (int i = 0; i < pageImages.Count; i++)
                    {
                        Pages.Add(new PageViewModel(i + 1, pageImages[i].Item1, pageImages[i].Item2, file, Files.IndexOf(file)));
                    }

                    break;

                case FileItemType.Image:

                    //Get Preview Pages
                    var pageImage = await Task.Run(() => ImageHelper.PreviewAsPdf(file.Path, previewWidth, previewQuality, margin, size, orientation));

                    //Add Pages to Collection
                    Pages.Add(new PageViewModel(1, pageImage, PageRotation.Normal, file, Files.IndexOf(file)));

                    break;
            }
        }

        #endregion

        #region RelayCommands

        //Organize
        [RelayCommand(CanExecute = nameof(CanOrganize))]
        public async Task Organize(string savePath)
        {
            IsBusy = true;
            int index = 0;

            using PdfDocument? organized = new PdfDocument();
            var margin = 12 * (int)settingService.Settings.PageMargin;
            var size = settingService.Settings.PageSize;
            var orientation = settingService.Settings.PageOrientation;

            var dispatcher = Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread();
            var saved = false;

            await Task.Run(() => {

                for (int i = 0; i < Pages.Count; i++)
                {
                    dispatcher.TryEnqueue(() =>
                    {
                        ProgressText = $"Organizing page {index} of {Pages.Count}...";
                    });

                    using var pdfDoc = Pages[i].File.Type == FileItemType.Pdf ? new PdfDocument(Pages[i].File.Path) : new PdfDocument(ImageHelper.ImageToPdf(Pages[i].File.Path, margin, size, orientation));
                    pdfDoc.Pages[Pages[i].PageNumber - 1].Orientation = (PageOrientations)Pages[i].CurrentRotation;
                    organized.Pages.Add(pdfDoc, Pages[i].PageNumber - 1);

                    index++;
                }

                dispatcher.TryEnqueue(() =>
                {
                    ProgressText = $"Organizing page {Pages.Count} of {Pages.Count}...";
                });

                //Save
                if (organized != null && organized.Save(savePath))
                {
                    saved = organized.Save(savePath);
                }

            });

            var folder = await StorageFolder.GetFolderFromPathAsync(Path.GetDirectoryName(savePath));
            var file = await StorageFile.GetFileFromPathAsync(savePath);

            var options = new FolderLauncherOptions();
            options.ItemsToSelect.Add(file);

            await Task.Delay(1000);
            await Launcher.LaunchFolderAsync(folder, options);

            IsBusy = false;
        }

        //Files
        [RelayCommand]
        public async Task AddFiles(List<FileViewModel> files)
        {
            IsBusy = true;
            int index = 0;

            foreach (var file in files)
            {
                ProgressText = $"Importing file {index} of {files.Count}...";

                var colors = Files.Select(f => f.ColorBrush.Color).ToList();
                var generatedColor = await Task.Run(() => ColorGenerator.GenerateDistinctColor(colors));
                file.ColorBrush = new SolidColorBrush(generatedColor);

                Files.Add(file);
                await LoadPagesFromFile(file);

                index++;
            }

            ProgressText = $"Organizing page {files.Count} of {files.Count}...";

            IsBusy = false;
        }


        [RelayCommand(CanExecute = nameof(CanEdit))]
        public void DeleteFile(FileViewModel file)
        {
            //Clear pages associated with the file
            var pages = Pages.Concat(DeletedPages).Where(p => p.File == file).ToList();
            foreach (var page in pages)
            {
                page.Clear();
                Pages.Remove(page);
            }

            Files.Remove(file);
        }

        [RelayCommand(CanExecute = nameof(CanEdit))]
        public void ClearFiles(FileViewModel file)
        {
            //Clear FilePage first then Clear the Items
            foreach (var page in Pages.Concat(DeletedPages))
            {
                page.Clear();
            }
            DeletedPages.Clear();
            Pages.Clear();

            Files.Clear();

            GC.Collect(1, GCCollectionMode.Optimized);
        }

        [RelayCommand(CanExecute = nameof(CanEdit))]
        public void MoveFile(FileViewModel movedFile)
        {
            IsBusy = true;

            //Update newest FileIndex
            foreach (var page in Pages)
            {
                page.FileIndex = Files.IndexOf(page.File);
            }

            //Take Pages to be moved
            var pagesToBeMoved = Pages.Where(p => p.File == movedItem).ToList();
            if (!pagesToBeMoved.Any()) return;

            //Find move starting index
            int targetIndex = 0;
            var currentItemIndex = Files.IndexOf(movedFile);

            if (currentItemIndex > 0)
            {
                //Find Previous File
                var previousItemIndex = currentItemIndex - 1;
                var lastPageOfPreviousItem = Pages.LastOrDefault(p => p.FileIndex == previousItemIndex);

                if (lastPageOfPreviousItem != null)
                {
                    targetIndex = Pages.IndexOf(lastPageOfPreviousItem) + 1;
                }
            }
            else
            {
                //Set first if moved to the first item
                targetIndex = 0;
            }

            //Moving the page!
            foreach (var page in pagesToBeMoved)
            {
                var currentIndex = Pages.IndexOf(page);

                if (currentIndex != -1 && currentIndex != targetIndex)
                {
                    if (currentIndex < targetIndex)
                    {
                        Pages.Move(currentIndex, targetIndex - 1);
                    }
                    else
                    {
                        Pages.Move(currentIndex, targetIndex);
                        targetIndex++;
                    }
                }
                else if (currentIndex == targetIndex)
                {
                    targetIndex++;
                }
            }

            IsBusy = false;
        }

        //Pages

        [RelayCommand(CanExecute = nameof(CanEdit))]
        public async Task DeletePage(PageViewModel page)
        {
            DeletedPages.Add(page);
            Pages.Remove(page);
        }

        [RelayCommand(CanExecute = nameof(CanEdit))]
        public async Task ResetPages()
        {
            IsBusy = true;

            if (Pages.Count + DeletedPages.Count == 0) return;

            var sortedPages = Pages.Concat(DeletedPages).OrderBy(p => p.FileIndex).ThenBy(p => p.PageNumber).ToList();

            DeletedPages.Clear();
            Pages.Clear();

            var modifiedPages = sortedPages.Where(p => p.PageRotation != 0).ToList();

            foreach (var page in sortedPages)
            {
                page.PageRotation = 0;
                page.PageSize = PageSize.A4;
                page.PageOrientation = PageOrientation.Portrait;
            }

            if (modifiedPages.Count > 0)
            {
                var tasks = modifiedPages.Select(p => p.UpdatePreviewAsync(p.PageImage));
                await Task.WhenAll(tasks);
            }

            foreach (var page in sortedPages)
            {
                Pages.Add(page);
            }

            IsBusy = false;
        }

        [RelayCommand(CanExecute = nameof(CanEdit))]
        public async Task RotatePage(PageViewModel page)
        {
            if (page.PageImage == null) return;

            page.PageRotation = (PageRotation)((int)(page.PageRotation + 1) % 4);

            await page.UpdatePreviewAsync(ImageHelper.RotateImage(page.PageImage, page.PageRotation));

        }
        #endregion



    }
}
