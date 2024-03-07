using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using Si.Engine.Manager;
using Si.Engine.Sprite;
using Si.Engine.TickController._Superclass;
using Si.Library.Mathematics.Geometry;
using System.Linq;

namespace Si.Engine.TickController.SpriteTickController
{
    public class TextBlocksSpriteTickController : SpriteTickControllerBase<SpriteTextBlock>
    {
        public TextBlocksSpriteTickController(EngineCore engine, EngineSpriteManager manager)
            : base(engine, manager)
        {
        }

        public override void ExecuteWorldClockTick(float epoch, SiPoint displacementVector)
        {
            foreach (var textBlock in Visible().Where(o => o.IsFixedPosition == false))
            {
                textBlock.ApplyMotion(epoch, displacementVector);
            }
        }

        #region Factories.

        public SpriteRadarPositionTextBlock CreateRadarPosition(TextFormat format, SolidColorBrush color, SiPoint location)
        {
            var obj = new SpriteRadarPositionTextBlock(Engine, format, color, location);
            SpriteManager.Add(obj);
            return obj;
        }

        public SpriteTextBlock Create(TextFormat format, SolidColorBrush color, SiPoint location, bool isPositionStatic)
        {
            var obj = new SpriteTextBlock(Engine, format, color, location, isPositionStatic);
            SpriteManager.Add(obj);
            return obj;
        }

        public SpriteTextBlock Create(TextFormat format, SolidColorBrush color, SiPoint location, bool isPositionStatic, string name)
        {
            var obj = new SpriteTextBlock(Engine, format, color, location, isPositionStatic);
            obj.SpriteTag = name;
            SpriteManager.Add(obj);
            return obj;
        }

        public SpriteTextBlock Create(TextFormat format, SolidColorBrush color, SiPoint location, bool isPositionStatic, string name, string text)
        {
            var obj = new SpriteTextBlock(Engine, format, color, location, isPositionStatic);
            obj.SpriteTag = name;
            obj.Text = text;
            SpriteManager.Add(obj);
            return obj;
        }

        #endregion
    }
}
