using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;

namespace Si.GameEngine.Core.GraphicsProcessing
{
    public class PrecreatedMaterials
    {
        public class RawColors
        {
            public RawColor4 Red { get; private set; } = new(1, 0, 0, 1);
            public RawColor4 Green { get; private set; } = new(0, 1, 0, 1);
            public RawColor4 Blue { get; private set; } = new(0, 0, 1, 1);
            public RawColor4 Black { get; private set; } = new(0, 0, 0, 1);
            public RawColor4 White { get; private set; } = new(1, 1, 1, 1);
            public RawColor4 Gray { get; private set; } = new(0.25f, 0.25f, 0.25f, 1);
            public RawColor4 WhiteSmoke { get; private set; } = new(0.9608f, 0.9608f, 0.9608f, 1);
            public RawColor4 Cyan { get; private set; } = new(0, 1f, 1f, 1f);
            public RawColor4 OrangeRed { get; private set; } = new(1f, 0.2706f, 0.0000f, 1);
            public RawColor4 Orange { get; private set; } = new(1.0f, 0.6471f, 0.0f, 1);
            public RawColor4 LawnGreen { get; private set; } = new(0.4863f, 0.9882f, 0f, 1);
        }

        public class ColorBrushes
        {
            public SolidColorBrush Red { get; internal set; }
            public SolidColorBrush Green { get; internal set; }
            public SolidColorBrush Blue { get; internal set; }
            public SolidColorBrush Black { get; internal set; }
            public SolidColorBrush White { get; internal set; }
            public SolidColorBrush Gray { get; internal set; }
            public SolidColorBrush WhiteSmoke { get; internal set; }
            public SolidColorBrush Cyan { get; internal set; }
            public SolidColorBrush OrangeRed { get; internal set; }
            public SolidColorBrush Orange { get; internal set; }
            public SolidColorBrush LawnGreen { get; internal set; }

            public ColorBrushes(RenderTarget renterTarget, RawColors raw)
            {
                Red = new SolidColorBrush(renterTarget, raw.Red);
                Green = new SolidColorBrush(renterTarget, raw.Green);
                Blue = new SolidColorBrush(renterTarget, raw.Blue);
                Black = new SolidColorBrush(renterTarget, raw.Black);
                White = new SolidColorBrush(renterTarget, raw.White);
                Gray = new SolidColorBrush(renterTarget, raw.Gray);
                WhiteSmoke = new SolidColorBrush(renterTarget, raw.WhiteSmoke);
                Cyan = new SolidColorBrush(renterTarget, raw.Cyan);
                OrangeRed = new SolidColorBrush(renterTarget, raw.OrangeRed);
                Orange = new SolidColorBrush(renterTarget, raw.Orange);
                LawnGreen = new SolidColorBrush(renterTarget, raw.LawnGreen);
            }
        }

        internal ColorBrushes Brushes { get; set; }
        internal RawColors Raw { get; set; } = new();

        internal PrecreatedMaterials(RenderTarget renterTarget)
        {
            Brushes = new ColorBrushes(renterTarget, Raw);
        }
    }
}
