using SharpDX.Mathematics.Interop;
using Si.Engine.Sprite.Enemy._Superclass;
using Si.Engine.Sprite.Weapon.Munition._Superclass;
using Si.Library.Sprite;
using System.Drawing;
using static Si.Library.SiConstants;

namespace Si.Engine.Sprite._Superclass._Root
{
    /// <summary>
    /// Represents a single item that can be rendered to the screen. All on-screen objects are derived from this class.
    /// </summary>
    public partial class SpriteBase : ISprite
    {
        public virtual void Render(SharpDX.Direct2D1.RenderTarget renderTarget)
        {
            if (_isVisible && _image != null)
            {
                DrawImage(renderTarget, _image);

                if (IsHighlighted)
                {
                    _engine.Rendering.DrawRectangle(renderTarget, RawRenderBounds,
                        _engine.Rendering.Materials.Colors.Red, 0, 1, Orientation.RadiansSigned);
                }
            }
        }

        public virtual void Render(Graphics dc)
        {
        }

        public void RenderRadar(SharpDX.Direct2D1.RenderTarget renderTarget, int x, int y)
        {
            if (_isVisible && _image != null)
            {
                if (this is SpriteEnemyBase)
                {
                    _engine.Rendering.DrawTriangle(renderTarget, x, y, 3, 3, _engine.Rendering.Materials.Colors.OrangeRed);
                }
                else if (this is MunitionBase)
                {
                    float size;
                    RawColor4 color;

                    var munition = this as MunitionBase;
                    if (munition.FiredFromType == SiFiredFromType.Enemy)
                    {
                        color = _engine.Rendering.Materials.Colors.Red;
                    }
                    else
                    {
                        color = _engine.Rendering.Materials.Colors.Green;
                    }

                    if (munition.Weapon.Metadata.ExplodesOnImpact)
                    {
                        size = 2;
                    }
                    else
                    {
                        size = 1;
                    }

                    _engine.Rendering.DrawSolidEllipse(renderTarget, x, y, size, size, color);
                }
            }
        }

        public void DrawImage(SharpDX.Direct2D1.RenderTarget renderTarget, SharpDX.Direct2D1.Bitmap bitmap, float? angleRadians = null)
        {
            float angle = (float)(angleRadians == null ? Orientation.RadiansSigned : angleRadians);

            _engine.Rendering.DrawBitmap(renderTarget, bitmap,
                RenderLocation.X - bitmap.Size.Width / 2.0f,
                RenderLocation.Y - bitmap.Size.Height / 2.0f, angle);
        }
    }
}
