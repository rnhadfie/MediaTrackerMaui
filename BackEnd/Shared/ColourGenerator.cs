using System;
using System.Collections.Generic;
using System.Text;

namespace MauiApp1.BackEnd.Shared
{
    using System;

    public static class ColourHelper
    {
        private static readonly Random _rnd = new Random();

        public static string GenerateRandomColour()
        {
            // Choose hue while avoiding typical "brown" hue range (approx 20°-50°)
            double hue;
            do
            {
                hue = _rnd.NextDouble() * 360.0;
            } while (hue >= 20.0 && hue <= 50.0);

            // Avoid grays by ensuring decent saturation, avoid black/white by limiting lightness
            double saturation = 0.5 + _rnd.NextDouble() * 0.5; // 0.5 - 1.0
            double lightness = 0.35 + _rnd.NextDouble() * 0.25; // 0.35 - 0.60

            (int r, int g, int b) = HslToRgb(hue, saturation, lightness);
            return $"#{r:X2}{g:X2}{b:X2}";
        }

        private static (int r, int g, int b) HslToRgb(double h, double s, double l)
        {
            double c = (1.0 - Math.Abs(2.0 * l - 1.0)) * s;
            double hPrime = h / 60.0;
            double x = c * (1.0 - Math.Abs(hPrime % 2.0 - 1.0));

            double r1 = 0, g1 = 0, b1 = 0;
            if (0 <= hPrime && hPrime < 1) { r1 = c; g1 = x; b1 = 0; }
            else if (1 <= hPrime && hPrime < 2) { r1 = x; g1 = c; b1 = 0; }
            else if (2 <= hPrime && hPrime < 3) { r1 = 0; g1 = c; b1 = x; }
            else if (3 <= hPrime && hPrime < 4) { r1 = 0; g1 = x; b1 = c; }
            else if (4 <= hPrime && hPrime < 5) { r1 = x; g1 = 0; b1 = c; }
            else if (5 <= hPrime && hPrime < 6) { r1 = c; g1 = 0; b1 = x; }

            double m = l - c / 2.0;
            int r = (int)Math.Round((r1 + m) * 255.0);
            int g = (int)Math.Round((g1 + m) * 255.0);
            int b = (int)Math.Round((b1 + m) * 255.0);

            r = Math.Clamp(r, 0, 255);
            g = Math.Clamp(g, 0, 255);
            b = Math.Clamp(b, 0, 255);
            return (r, g, b);
        }
    }
}
