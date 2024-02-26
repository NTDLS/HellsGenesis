using SharpDX.Mathematics.Interop;

namespace Si.GameEngine.Core.GraphicsProcessing.Types
{
    public class SiPolygon
    {
        public RawVector2[] Vertices { get; }

        public SiPolygon(RawVector2[] vertices)
        {
            Vertices = vertices;
        }
    }
}
