using System.Numerics;
using System.Windows;
using System.Windows.Media;
using Vector = System.Windows.Vector;

namespace MouseInputApp
{
    struct RGB_ConversionDTO
    {
        public double R;
        public double G;
        public double B;

        public RGB_ConversionDTO(double r, double g, double b)
        {
            R = r;
            G = g;
            B = b;
        }
    }

    public static class HelpCalc
    {
        public static double CalcAngle2V(Vector v1, Vector v2)
        {
            v1.Normalize();
            v2.Normalize();
            double angle = Math.Atan2(v2.Y, v2.X) - Math.Atan2(v1.Y, v1.X);
            return angle;
        }

        public static SolidColorBrush ColorCalc(double Saturation, double Hue, double Value)
        {
            var c = Value / 100 * Saturation / 100;
            var x = c * (1 - Math.Abs(Hue / 60 % 2 - 1));
            var m = Value / 100 - c;

            RGB_ConversionDTO rgb = new RGB_ConversionDTO();

            if (0 <= Hue && Hue <= 60)
                rgb = new RGB_ConversionDTO(c, x, 0);
            else if (60 <= Hue && Hue <= 120)
                rgb = new RGB_ConversionDTO(x, c, 0);
            else if (120 <= Hue && Hue <= 180)
                rgb = new RGB_ConversionDTO(0, c, x);
            else if (180 <= Hue && Hue <= 240)
                rgb = new RGB_ConversionDTO(0, x, c);
            else if (240 <= Hue && Hue <= 300)
                rgb = new RGB_ConversionDTO(x, 0, c);
            else if (300 <= Hue && Hue <= 360)
                rgb = new RGB_ConversionDTO(c, 0, x);

            var clr = Color.FromArgb(255, (byte)(255 * (rgb.R + m)), (byte)(255 * (rgb.G + m)), (byte)(255 * (rgb.B + m)));
            return new SolidColorBrush(clr);
        }
    }

}
