using SharpDX;
using Si.Library;
using System;

namespace Si.GameEngine.Core.GraphicsProcessing
{
    internal static class GraphicsUtility
    {
        /// <summary>
        /// Gets a random color that would be associated with fire.
        /// </summary>
        /// <returns></returns>
        static public Color4 GetRandomHotColor()
        {
            float hue = SiRandom.Between(0, 60);
            float saturation = (float)SiRandom.Between(0.8, 1.0);
            float lightness = (float)SiRandom.Between(0.5, 1);
            return RGBFromHSL(hue, saturation, lightness);
        }

        /// <summary>
        /// RGB Fom HSL (hue,saturationlightness).
        /// </summary>
        /// <param name="hue"></param>
        /// <param name="saturation"></param>
        /// <param name="lightness"></param>
        static public Color4 RGBFromHSL(float hue, float saturation, float lightness)
        {
            float c = (1 - Math.Abs(2 * lightness - 1)) * saturation;
            float x = c * (1 - Math.Abs((hue / 60) % 2 - 1));
            float m = lightness - c / 2;

            float r, g, b;

            if (0 <= hue && hue < 60)
            {
                r = c; g = x; b = 0;
            }
            else if (60 <= hue && hue < 120)
            {
                r = x; g = c; b = 0;
            }
            else if (120 <= hue && hue < 180)
            {
                r = 0; g = c; b = x;
            }
            else if (180 <= hue && hue < 240)
            {
                r = 0; g = x; b = c;
            }
            else if (240 <= hue && hue < 300)
            {
                r = x; g = 0; b = c;
            }
            else
            {
                r = c; g = 0; b = x;
            }

            return new Color4(r + m, g + m, b + m, 1);
        }
    }
}
