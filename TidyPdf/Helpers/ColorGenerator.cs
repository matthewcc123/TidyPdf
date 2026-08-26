using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI;


namespace TidyPdf.Helpers
{
    public static class ColorGenerator
    {

        private static readonly Random _random = new Random();

        public static Color GenerateDistinctColor(List<Color> currentColorPool)
        {
            int count = currentColorPool.Count;

            double chosenHue;

            if (count == 0)
            {
                chosenHue = _random.NextDouble() * 360.0;
            }
            else
            {
                // 1. Golden Ratio conjugate disperses hues evenly across 360°
                // instead of repeatedly halving gaps into similar color bands
                const double goldenRatioConjugate = 0.618033988749895;
                double startHue = GetHue(currentColorPool[0]);
                chosenHue = (startHue + (count * goldenRatioConjugate * 360.0)) % 360.0;
            }

            // 2. Vary Saturation and Lightness out of sync with Hue steps
            // using prime numbers (3 and 5) to avoid repeating combinations
            double saturation = 0.65 + (count % 3) * 0.12; // 0.65, 0.77, 0.89
            double lightness = 0.45 + (count % 5) * 0.08;  // 0.45, 0.53, 0.61, 0.69, 0.77

            return HslToWindowsUiColor(chosenHue, saturation, lightness);
        }

        private static Color HslToWindowsUiColor(double hue, double saturation, double lightness)
        {
            double c = (1 - Math.Abs(2 * lightness - 1)) * saturation;
            double x = c * (1 - Math.Abs((hue / 60.0) % 2 - 1));
            double m = lightness - c / 2.0;

            double r = 0, g = 0, b = 0;
            if (hue < 60) { r = c; g = x; b = 0; }
            else if (hue < 120) { r = x; g = c; b = 0; }
            else if (hue < 180) { r = 0; g = c; b = x; }
            else if (hue < 240) { r = 0; g = x; b = c; }
            else if (hue < 300) { r = x; g = 0; b = c; }
            else { r = c; g = 0; b = x; }

            byte red = (byte)Math.Round((r + m) * 255);
            byte green = (byte)Math.Round((g + m) * 255);
            byte blue = (byte)Math.Round((b + m) * 255);

            return Color.FromArgb(255, red, green, blue);
        }

        private static double GetHue(Color color)
        {
            if (color.R == color.G && color.G == color.B)
                return 0.0; // Grayscale has no hue

            double r = color.R / 255.0;
            double g = color.G / 255.0;
            double b = color.B / 255.0;

            double max = Math.Max(r, Math.Max(g, b));
            double min = Math.Min(r, Math.Min(g, b));
            double delta = max - min;

            double hue = 0.0;

            if (max == r)
                hue = (g - b) / delta + (g < b ? 6 : 0);
            else if (max == g)
                hue = (b - r) / delta + 2;
            else if (max == b)
                hue = (r - g) / delta + 4;

            return hue * 60.0;
        }

    }
}
