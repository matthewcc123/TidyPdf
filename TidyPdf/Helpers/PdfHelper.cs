using PDFiumSharp;
using System.Collections.Generic;
using System.IO;
using static TidyPdf.Helpers.ImageHelper;

namespace TidyPdf.Helpers
{
    public static class PdfHelper
    {

        public static List<(byte[], PageRotation)> GetPageImages(string path, int maxPreviewWidth = 300, int quality = 75)
        {

            using var pdfDoc = new PdfDocument(path);

            List<(byte[], PageRotation)> previewImages = new();

            foreach (var page in pdfDoc.Pages)
            {
                using (page)
                {
                    double scale = (double)maxPreviewWidth / page.Width;
                    int targetWidth = maxPreviewWidth;
                    int targetHeight = (int)(page.Height * scale);

                    using var bitmap = new PDFiumBitmap(targetWidth, targetHeight, true);
                    bitmap.FillRectangle(0, 0, targetWidth, targetHeight, 0xFFFFFFFF);

                    page.Render(bitmap, PDFiumSharp.Enums.PageOrientations.Normal, PDFiumSharp.Enums.RenderingFlags.Annotations);

                    using var memoryStream = new MemoryStream();
                    bitmap.Save(memoryStream, quality, quality);

                    previewImages.Add((memoryStream.ToArray(), (PageRotation)page.Orientation));
                }
            }

            return previewImages;
        }

        public static PdfDocument AddPage(PdfDocument pdfDoc, PdfDocument addedDocument, int pageNumber)
        {
            pdfDoc.Pages.Add(addedDocument, pageNumber);

            return pdfDoc;

        }

    }
}
