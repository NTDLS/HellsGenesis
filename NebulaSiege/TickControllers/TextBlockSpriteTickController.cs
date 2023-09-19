using NebulaSiege.Engine;
using NebulaSiege.Engine.Types.Geometry;
using NebulaSiege.Managers;
using NebulaSiege.Sprites;
using NebulaSiege.TickControllers;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using System.Linq;

namespace NebulaSiege.Controller
{
    internal class TextBlockSpriteTickController : _SpriteTickControllerBase<SpriteTextBlock>
    {
        public TextBlockSpriteTickController(EngineCore core, EngineSpriteManager manager)
            : base(core, manager)
        {
        }

        public override void ExecuteWorldClockTick(NsPoint displacementVector)
        {
            foreach (var textBlock in Visible().Where(o => o.IsFixedPosition == false))
            {
                textBlock.ApplyMotion(displacementVector);
            }
        }

        #region Factories.

        public SpriteRadarPositionTextBlock CreateRadarPosition(TextFormat format, SolidColorBrush color, NsPoint location)
        {
            lock (SpriteManager.Collection)
            {
                var obj = new SpriteRadarPositionTextBlock(Core, format, color, location);
                SpriteManager.Collection.Add(obj);
                return obj;
            }
        }

        public SpriteTextBlock Create(TextFormat format, SolidColorBrush color, NsPoint location, bool isPositionStatic)
        {
            lock (SpriteManager.Collection)
            {
                var obj = new SpriteTextBlock(Core, format, color, location, isPositionStatic);
                SpriteManager.Collection.Add(obj);
                return obj;
            }
        }

        public SpriteTextBlock Create(TextFormat format, SolidColorBrush color, NsPoint location, bool isPositionStatic, string name)
        {
            lock (SpriteManager.Collection)
            {
                var obj = new SpriteTextBlock(Core, format, color, location, isPositionStatic);
                obj.Name = name;
                SpriteManager.Collection.Add(obj);
                return obj;
            }
        }

        #endregion
    }
}
