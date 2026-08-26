using Microsoft.UI.Xaml.Controls;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TidyPdf.Enums;
using static TidyPdf.Helpers.ImageHelper;

namespace TidyPdf.Helpers
{
    public static class ImageHelper
    {
        public enum PageSize
        {
            Fit,
            A4,
            Letter
        }

        public enum PageMargin
        {
            None,
            Narrow,
            Normal
        }

        public enum PageRotation
        {
            Normal,
            Rotate90,
            Rotate180,
            Rotate270
        }

        public enum PageOrientation
        {
            Auto,
            Portrait,
            Landscape
        }

        private readonly record struct PageLayout(
            float PageWidth,
            float PageHeight,
            SKRect DestinationRect
        );

        private static PageLayout CalculateLayout(int imgWidth, int imgHeight, int margin, PageSize pageSize, PageOrientation pageOrientation)
        {
            float pageWidth = 595f;
            float pageHeight = 842f;

            switch (pageSize)
            {
                case PageSize.Fit:
                    pageWidth = imgWidth;
                    pageHeight = imgHeight;
                    break;
                case PageSize.A4:
                    pageWidth = 595f;
                    pageHeight = 842f;
                    break;
                case PageSize.Letter:
                    pageWidth = 612f;
                    pageHeight = 792f;
                    break;
            }

            if (pageOrientation == PageOrientation.Auto)
            {
                double imageAspect = (double)imgWidth / imgHeight;
                double pageAspect = (double)pageWidth / pageHeight;

                bool imageIsLandscape = imgWidth > imgHeight;

                pageOrientation = imageIsLandscape && imageAspect > pageAspect ? PageOrientation.Landscape : PageOrientation.Portrait;
            }

            if (pageOrientation == PageOrientation.Landscape && pageSize != PageSize.Fit)
            {
                (pageWidth, pageHeight) = (pageHeight, pageWidth);
            }

            //Margin
            float printableWidth = Math.Max(1, pageWidth - (margin * 2));
            float printableHeight = Math.Max(1, pageHeight - (margin * 2));

            //Image Scaling
            float drawWidth = imgWidth;
            float drawHeight = imgHeight;

            if (imgWidth > printableWidth || imgHeight > printableHeight)
            {
                float scale = Math.Min(
                    printableWidth / imgWidth,
                    printableHeight / imgHeight);

                drawWidth = imgWidth * scale;
                drawHeight = imgHeight * scale;
            }

            //Center Image
            float x = margin + (printableWidth - drawWidth) / 2f;
            float y = margin + (printableHeight - drawHeight) / 2f;

            //Image Draw Rect
            var destRect = new SKRect(x, y, x + drawWidth, y + drawHeight);

            return new PageLayout(pageWidth, pageHeight, destRect);

        }

        public static byte[] PreviewAsPdf(string path, int maxPreviewWidth = 300, int quality = 75, int margin = 24, PageSize pageSize = PageSize.A4, PageOrientation pageOrientation = PageOrientation.Portrait)
        {
            using var codec = SKCodec.Create(path);
            using var bitmap = SKBitmap.Decode(codec);

            var layout = CalculateLayout(bitmap.Width, bitmap.Height, margin, pageSize, pageOrientation);

            //Scale Page
            float pageScale = (float)maxPreviewWidth / layout.PageWidth;
            var previewWidth = maxPreviewWidth;
            var previewHeight = (int)(layout.PageHeight * pageScale);

            //Draw Page
            using var page = new SKBitmap(previewWidth, previewHeight);
            using var canvas = new SKCanvas(page);

            //Draw Image on Page
            canvas.Scale(pageScale);
            canvas.Clear(SKColors.White);
            canvas.DrawBitmap(bitmap, layout.DestinationRect , SKSamplingOptions.Default);

            using var data = page.Encode(SKEncodedImageFormat.Png, quality);
            return data.ToArray();
        }

        public static byte[] ImageToPdf(string path, int margin = 24, PageSize pageSize = PageSize.A4, PageOrientation pageOrientation = PageOrientation.Portrait)
        {
            using var codec = SKCodec.Create(path);
            using var bitmap = SKBitmap.Decode(codec);

            var layout = CalculateLayout(bitmap.Width, bitmap.Height, margin, pageSize, pageOrientation);

            //Generate PDF Stream using SKDocument
            using var ms = new MemoryStream();
            using var pdfDocument = SKDocument.CreatePdf(ms);

            using var canvas = pdfDocument.BeginPage(layout.PageWidth, layout.PageHeight);

            canvas.Clear(SKColors.White);
            canvas.DrawBitmap(bitmap, layout.DestinationRect, SKSamplingOptions.Default);
            pdfDocument.EndPage();
            pdfDocument.Close();

            return ms.ToArray();
        }

        public static byte[] RotateImage(byte[] imageBytes, PageRotation rotateAngle)
        {

            var degrees = 90 * (int)rotateAngle;
            if (degrees % 360 == 0) return imageBytes;

            using var codec = SKCodec.Create(new SKMemoryStream(imageBytes));
            using var original = SKBitmap.Decode(codec);

            bool is90or270 = degrees == 90 || degrees == 270;
            int newWidth = is90or270 ? original.Height : original.Width;
            int newHeight = is90or270 ? original.Width : original.Height;

            using var rotated = new SKBitmap(newWidth, newHeight);
            using var canvas = new SKCanvas(rotated);
            
            canvas.Clear(SKColors.Transparent);

            canvas.Translate(newWidth / 2f, newHeight / 2f);
            canvas.RotateDegrees(degrees);
            canvas.Translate(-original.Width / 2f, -original.Height / 2f);

            canvas.DrawBitmap(original, 0, 0, SKSamplingOptions.Default);

            using var data = rotated.Encode(SKEncodedImageFormat.Png, 100);

            return data.ToArray();
        }


    }
}
