using Si.GameEngine.Core.Managers;
using Si.GameEngine.Core.TickControllers._Superclass;
using Si.GameEngine.Sprites;
using Si.Shared;
using Si.Shared.Types.Geometry;

namespace Si.GameEngine.Core.TickControllers
{
    public class StarsSpriteTickController : SpriteTickControllerBase<SpriteStar>
    {
        public StarsSpriteTickController(Engine gameEngine, EngineSpriteManager manager)
            : base(gameEngine, manager)
        {
        }

        public override void ExecuteWorldClockTick(SiPoint displacementVector)
        {
            if (displacementVector.X != 0 || displacementVector.Y != 0)
            {
                #region Add new stars...

                if (SpriteManager.VisibleOfType<SpriteStar>().Count < GameEngine.Settings.DeltaFrameTargetStarCount) //Never wan't more than n stars.
                {
                    if (displacementVector.X > 0)
                    {
                        if (SiRandom.PercentChance(20))
                        {
                            int x = SiRandom.Generator.Next(GameEngine.Display.TotalCanvasSize.Width - (int)displacementVector.X, GameEngine.Display.TotalCanvasSize.Width);
                            int y = SiRandom.Generator.Next(0, GameEngine.Display.TotalCanvasSize.Height);
                            SpriteManager.Stars.Create(x, y);
                        }

                    }
                    else if (displacementVector.X < 0)
                    {
                        if (SiRandom.PercentChance(20))
                        {
                            int x = SiRandom.Generator.Next(0, (int)-displacementVector.X);
                            int y = SiRandom.Generator.Next(0, GameEngine.Display.TotalCanvasSize.Height);
                            SpriteManager.Stars.Create(x, y);
                        }

                    }
                    if (displacementVector.Y > 0)
                    {
                        if (SiRandom.PercentChance(20))
                        {
                            int x = SiRandom.Generator.Next(0, GameEngine.Display.TotalCanvasSize.Width);
                            int y = SiRandom.Generator.Next(GameEngine.Display.TotalCanvasSize.Height - (int)displacementVector.Y, GameEngine.Display.TotalCanvasSize.Height);
                            SpriteManager.Stars.Create(x, y);
                        }

                    }
                    else if (displacementVector.Y < 0)
                    {

                        if (SiRandom.PercentChance(20))
                        {
                            int x = SiRandom.Generator.Next(0, GameEngine.Display.TotalCanvasSize.Width);
                            int y = SiRandom.Generator.Next(0, (int)-displacementVector.Y);
                            SpriteManager.Stars.Create(x, y);
                        }

                    }
                }

                #endregion

                foreach (var star in All())
                {
                    star.ApplyMotion(displacementVector);

                    if (GameEngine.Display.TotalCanvasBounds.IntersectsWith(star.Bounds) == false) //Remove off-screen stars.
                    {
                        star.QueueForDelete();
                    }
                }
            }
        }
    }
}
