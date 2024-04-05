using Si.Engine;
using Si.Engine.Manager;
using Si.Engine.Sprite;
using Si.Engine.TickController._Superclass;
using Si.Library;
using Si.Library.ExtensionMethods;
using Si.Library.Mathematics.Geometry;
using System;
using System.Linq;

namespace Si.GameEngine.TickController.VectoredTickController.Uncollidable
{
    public class StarSpriteTickController : VectoredTickControllerBase<SpriteStar>
    {
        public StarSpriteTickController(EngineCore engine, SpriteManager manager)
            : base(engine, manager)
        {
        }

        public override void ExecuteWorldClockTick(float epoch, SiVector displacementVector)
        {
            if (Math.Abs(displacementVector.X) > 1 || Math.Abs(displacementVector.Y) > 1)
            {
                #region Add new stars...

                if (SpriteManager.VisibleOfType<SpriteStar>().Count() < Engine.Settings.DeltaFrameTargetStarCount) //Never wan't more than n stars.
                {
                    if (displacementVector.X > 0)
                    {
                        if (SiRandom.PercentChance(20))
                        {
                            int x = SiRandom.Between(
                                Engine.Display.TotalCanvasSize.Width - (int)displacementVector.X,
                                Engine.Display.TotalCanvasSize.Width);
                            int y = SiRandom.Between(0, Engine.Display.TotalCanvasSize.Height);

                            SpriteManager.Stars.Add(Engine.Display.RenderWindowPosition.X + x, Engine.Display.RenderWindowPosition.Y + y);
                        }

                    }
                    else if (displacementVector.X < 0)
                    {
                        if (SiRandom.PercentChance(20))
                        {
                            int x = SiRandom.Between(0, (int)-displacementVector.X);
                            int y = SiRandom.Between(0, Engine.Display.TotalCanvasSize.Height);
                            SpriteManager.Stars.Add(Engine.Display.RenderWindowPosition.X + x, Engine.Display.RenderWindowPosition.Y + y);
                        }

                    }
                    if (displacementVector.Y > 0)
                    {
                        if (SiRandom.PercentChance(20))
                        {
                            int x = SiRandom.Between(0, Engine.Display.TotalCanvasSize.Width);
                            int y = SiRandom.Between(Engine.Display.TotalCanvasSize.Height - (int)displacementVector.Y, Engine.Display.TotalCanvasSize.Height);
                            SpriteManager.Stars.Add(Engine.Display.RenderWindowPosition.X + x, Engine.Display.RenderWindowPosition.Y + y);
                        }
                    }
                    else if (displacementVector.Y < 0)
                    {
                        if (SiRandom.PercentChance(20))
                        {
                            int x = SiRandom.Between(0, Engine.Display.TotalCanvasSize.Width);
                            int y = SiRandom.Between(0, (int)-displacementVector.Y);
                            SpriteManager.Stars.Add(Engine.Display.RenderWindowPosition.X + x, Engine.Display.RenderWindowPosition.Y + y);
                        }
                    }
                }

                #endregion

                foreach (var star in All())
                {
                    star.ApplyMotion(epoch, displacementVector);

                    //Remove stars that are too far off-screen.
                    if (Engine.Display.TotalCanvasBounds.Balloon(1000).IntersectsWith(star.RenderBounds) == false)
                    {
                        star.QueueForDelete();
                    }
                }
            }
        }
    }
}
