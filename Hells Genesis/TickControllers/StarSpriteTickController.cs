using HG.Controller.Interfaces;
using HG.Engine;
using HG.Engine.Types.Geometry;
using HG.Managers;
using HG.Sprites;
using HG.Utility;
using System.Collections.Generic;

namespace HG.Controller
{
    internal class StarSpriteTickController : IVectoredTickController
    {
        private readonly EngineCore _core;
        private readonly EngineSpriteManager _controller;

        public List<subType> VisibleOfType<subType>() where subType : SpriteStar => _controller.VisibleOfType<subType>();
        public List<SpriteStar> Visible() => _controller.VisibleOfType<SpriteStar>();
        public List<subType> OfType<subType>() where subType : SpriteStar => _controller.OfType<subType>();


        public StarSpriteTickController(EngineCore core, EngineSpriteManager manager)
        {
            _core = core;
            _controller = manager;
        }

        public void ExecuteWorldClockTick(HgPoint displacementVector)
        {
            if (displacementVector.X != 0 || displacementVector.Y != 0)
            {
                #region Add new stars...

                if (_controller.VisibleOfType<SpriteStar>().Count < _core.Settings.DeltaFrameTargetStarCount) //Never wan't more than n stars.
                {
                    if (displacementVector.X > 0)
                    {
                        for (int i = 0; i < 100; i++) //n chances to create a star.
                        {
                            if (HgRandom.ChanceIn(1000)) //1 in n chance to create a star.
                            {
                                int x = HgRandom.Random.Next(_core.Display.TotalCanvasSize.Width - (int)displacementVector.X, _core.Display.TotalCanvasSize.Width);
                                int y = HgRandom.Random.Next(0, _core.Display.TotalCanvasSize.Height);
                                _controller.Stars.Create(x, y);
                            }
                        }
                    }
                    else if (displacementVector.X < 0)
                    {
                        for (int i = 0; i < 100; i++) //n chances to create a star.
                        {
                            if (HgRandom.ChanceIn(1000)) //1 in n chance to create a star.
                            {
                                int x = HgRandom.Random.Next(0, (int)-displacementVector.X);
                                int y = HgRandom.Random.Next(0, _core.Display.TotalCanvasSize.Height);
                                _controller.Stars.Create(x, y);
                            }
                        }
                    }
                    if (displacementVector.Y > 0)
                    {
                        for (int i = 0; i < 100; i++) //n chances to create a star.
                        {
                            if (HgRandom.ChanceIn(1000)) //1 in n chance to create a star.
                            {
                                int x = HgRandom.Random.Next(0, _core.Display.TotalCanvasSize.Width);
                                int y = HgRandom.Random.Next(_core.Display.TotalCanvasSize.Height - (int)displacementVector.Y, _core.Display.TotalCanvasSize.Height);
                                _controller.Stars.Create(x, y);
                            }
                        }
                    }
                    else if (displacementVector.Y < 0)
                    {
                        for (int i = 0; i < 100; i++) //n chances to create a star.
                        {
                            if (HgRandom.ChanceIn(1000)) //1 in n chance to create a star.
                            {
                                int x = HgRandom.Random.Next(0, _core.Display.TotalCanvasSize.Width);
                                int y = HgRandom.Random.Next(0, (int)-displacementVector.Y);
                                _controller.Stars.Create(x, y);
                            }
                        }
                    }
                }

                #endregion

                foreach (var star in Visible())
                {
                    star.ApplyMotion(displacementVector);
                }
            }
        }

        #region Factories.

        public SpriteStar Create(double x, double y)
        {
            lock (_controller.Collection)
            {
                var obj = new SpriteStar(_core)
                {
                    X = x,
                    Y = y
                };
                _controller.Collection.Add(obj);
                return obj;
            }
        }

        public SpriteStar Create()
        {
            lock (_controller.Collection)
            {
                var obj = new SpriteStar(_core);
                _controller.Collection.Add(obj);
                return obj;
            }
        }

        public void Delete(SpriteStar obj)
        {
            lock (_controller.Collection)
            {
                obj.Cleanup();
                _controller.Collection.Remove(obj);
            }
        }

        #endregion
    }
}
