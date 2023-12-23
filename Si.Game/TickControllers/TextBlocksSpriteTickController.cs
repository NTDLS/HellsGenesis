using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using Si.Game.Engine;
using Si.Game.Engine.Types.Geometry;
using Si.Game.Managers;
using Si.Game.Sprites;
using Si.Game.TickControllers.BasesAndInterfaces;
using System.Linq;

namespace Si.Game.Controller
{
    internal class TextBlocksSpriteTickController : SpriteTickControllerBase<SpriteTextBlock>
    {
        public TextBlocksSpriteTickController(EngineCore gameCore, EngineSpriteManager manager)
            : base(gameCore, manager)
        {
        }

        public override void ExecuteWorldClockTick(SiPoint displacementVector)
        {
            foreach (var textBlock in Visible().Where(o => o.IsFixedPosition == false))
            {
                textBlock.ApplyMotion(displacementVector);
            }
        }

        #region Factories.

        public SpriteRadarPositionTextBlock CreateRadarPosition(TextFormat format, SolidColorBrush color, SiPoint location)
        {
            lock (SpriteManager.Collection)
            {
                var obj = new SpriteRadarPositionTextBlock(GameCore, format, color, location);
                SpriteManager.Collection.Add(obj);
                return obj;
            }
        }

        public SpriteTextBlock Create(TextFormat format, SolidColorBrush color, SiPoint location, bool isPositionStatic)
        {
            lock (SpriteManager.Collection)
            {
                var obj = new SpriteTextBlock(GameCore, format, color, location, isPositionStatic);
                SpriteManager.Collection.Add(obj);
                return obj;
            }
        }

        public SpriteTextBlock Create(TextFormat format, SolidColorBrush color, SiPoint location, bool isPositionStatic, string name)
        {
            lock (SpriteManager.Collection)
            {
                var obj = new SpriteTextBlock(GameCore, format, color, location, isPositionStatic);
                obj.SpriteTag = name;
                SpriteManager.Collection.Add(obj);
                return obj;
            }
        }

        #endregion
    }
}
