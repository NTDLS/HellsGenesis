using Si.GameEngine.Core.Managers;
using Si.GameEngine.Core.TickControllers._Superclass;
using Si.GameEngine.Sprites;
using Si.Library;
using Si.Library.ExtensionMethods;
using Si.Library.Mathematics.Geometry;
using System;

namespace Si.GameEngine.Core.TickControllers
{
    public class StarsSpriteTickController : SpriteTickControllerBase<SpriteStar>
    {
        public StarsSpriteTickController(GameEngineCore gameEngine, EngineSpriteManager manager)
            : base(gameEngine, manager)
        {
        }

        public override void ExecuteWorldClockTick(float epoch, SiVector displacementVector)
        {
            if (Math.Abs(displacementVector.X) > 1 || Math.Abs(displacementVector.Y) > 1)
            {
                #region Add new stars...

                if (SpriteManager.VisibleOfType<SpriteStar>().Count < GameEngine.Settings.DeltaFrameTargetStarCount) //Never wan't more than n stars.
                {
                    if (displacementVector.X > 0)
                    {
                        if (SiRandom.PercentChance(20))
                        {
                            int x = SiRandom.Between(
                                (GameEngine.Display.TotalCanvasSize.Width) - (int)displacementVector.X,
                                (GameEngine.Display.TotalCanvasSize.Width));
                            int y = SiRandom.Between(0, GameEngine.Display.TotalCanvasSize.Height);

                            SpriteManager.Stars.Create(GameEngine.Display.RenderWindowPosition.X + x, GameEngine.Display.RenderWindowPosition.Y + y);
                        }

                    }
                    else if (displacementVector.X < 0)
                    {
                        if (SiRandom.PercentChance(20))
                        {
                            int x = SiRandom.Between(0, (int)-displacementVector.X);
                            int y = SiRandom.Between(0, GameEngine.Display.TotalCanvasSize.Height);
                            SpriteManager.Stars.Create(GameEngine.Display.RenderWindowPosition.X + x, GameEngine.Display.RenderWindowPosition.Y + y);
                        }

                    }
                    if (displacementVector.Y > 0)
                    {
                        if (SiRandom.PercentChance(20))
                        {
                            int x = SiRandom.Between(0, GameEngine.Display.TotalCanvasSize.Width);
                            int y = SiRandom.Between(GameEngine.Display.TotalCanvasSize.Height - (int)displacementVector.Y, GameEngine.Display.TotalCanvasSize.Height);
                            SpriteManager.Stars.Create(GameEngine.Display.RenderWindowPosition.X + x, GameEngine.Display.RenderWindowPosition.Y + y);
                        }
                    }
                    else if (displacementVector.Y < 0)
                    {
                        if (SiRandom.PercentChance(20))
                        {
                            int x = SiRandom.Between(0, GameEngine.Display.TotalCanvasSize.Width);
                            int y = SiRandom.Between(0, (int)-displacementVector.Y);
                            SpriteManager.Stars.Create(GameEngine.Display.RenderWindowPosition.X + x, GameEngine.Display.RenderWindowPosition.Y + y);
                        }
                    }
                }

                #endregion

                foreach (var star in All())
                {
                    star.ApplyMotion(epoch, displacementVector);

                    //Remove stars that are too far off-screen.
                    if (GameEngine.Display.TotalCanvasBounds.Balloon(1000).IntersectsWith(star.RenderBounds) == false)
                    {
                        star.QueueForDelete();
                    }
                }
            }
        }
    }
}
