using SharpDX.DirectWrite;

namespace NebulaSiege.Engine.GraphicsProcessing
{
    internal class PrecreatedTextFormats
    {
        public TextFormat MenuGeneral { get; private set; }
        public TextFormat MenuTitle { get; private set; }
        public TextFormat MenuItem { get; private set; }
        public TextFormat LargeBlocker { get; private set; }
        public TextFormat RadarPositionIndicator { get; private set; }
        public TextFormat RealtimePlayerStats { get; private set; }

        public PrecreatedTextFormats(Factory factory)
        {
            //Digital-7 Mono

            LargeBlocker = new TextFormat(factory, "Orbitronio", 50) { WordWrapping = WordWrapping.NoWrap };
            MenuGeneral = new TextFormat(factory, "Consolas", 20) { WordWrapping = WordWrapping.NoWrap };
            MenuTitle = new TextFormat(factory, "Orbitronio", 72) { WordWrapping = WordWrapping.NoWrap };
            MenuItem = new TextFormat(factory, "Consolas", 20) { WordWrapping = WordWrapping.NoWrap };
            RadarPositionIndicator = new TextFormat(factory, "Digital-7 Mono", 16) { WordWrapping = WordWrapping.NoWrap };
            RealtimePlayerStats = new TextFormat(factory, "Consolas", 16) { WordWrapping = WordWrapping.NoWrap };
        }
    }
}
