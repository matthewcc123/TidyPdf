using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TidyPdf.Enums;
using Windows.Storage.Streams;
using static TidyPdf.Helpers.ImageHelper;

namespace TidyPdf.ViewModels
{
    public partial class PageViewModel : ObservableObject
    {

        [ObservableProperty]
        public partial int PageNumber { get; set; }
        [ObservableProperty]
        public partial byte[]? PageImage { get; set; }
        [ObservableProperty]
        public partial BitmapImage? PreviewImage { get; set; }
        [ObservableProperty]
        public partial FileViewModel File { get; set; }
        [ObservableProperty]
        public partial int FileIndex { get; set; }
        [ObservableProperty]
        public partial PageRotation DefaultPageRotation { get; set; } = PageRotation.Normal;

        //Modifiers
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CurrentRotation))]
        public partial PageRotation PageRotation { get; set; }
        [ObservableProperty]
        public partial PageSize PageSize { get; set; } = PageSize.A4;
        [ObservableProperty]
        public partial PageOrientation PageOrientation { get; set; } = PageOrientation.Portrait;

        public PageRotation CurrentRotation => (PageRotation)(((int)DefaultPageRotation + (int)PageRotation) % 4);
        private readonly DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();

        public PageViewModel(int pageNumber, byte[]? pageImage, PageRotation rotation, FileViewModel fileItem, int fileItemOrder)
        {
            PageNumber = pageNumber;
            File = fileItem;
            DefaultPageRotation = rotation;
            PageImage = pageImage;
            FileIndex = fileItemOrder;

            _ = InitAsync();

        }

        private async Task InitAsync()
        {
            await UpdatePreviewAsync(PageImage);
        }

        public async Task UpdatePreviewAsync(byte[]? image)
        {
            var bitmapImage = new BitmapImage();
            PreviewImage = null;

            dispatcherQueue.TryEnqueue(async () =>
            {

                if (image == null || image.Length == 0) return;

                var stream = new InMemoryRandomAccessStream();
                using (var writer = new DataWriter(stream.GetOutputStreamAt(0)))
                {
                    writer.WriteBytes(image);
                    await writer.StoreAsync();
                    await writer.FlushAsync();
                    writer.DetachStream();
                }

                stream.Seek(0);
                await bitmapImage.SetSourceAsync(stream);

                PreviewImage = bitmapImage;

            });
        }
        public void Clear()
        {
            if (PreviewImage != null)
            {
                PreviewImage.UriSource = null;
                PreviewImage = null;
            }

            PageImage = null;
        }

    }
}
