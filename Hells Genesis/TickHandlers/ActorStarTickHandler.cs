using HG.Actors.Ordinary;
using HG.Engine;
using HG.Engine.Controllers;
using HG.TickHandlers.Interfaces;
using HG.Types;
using System.Collections.Generic;

namespace HG.TickHandlers
{
    internal class ActorStarTickHandler : IVectoredTickManager
    {
        private readonly Core _core;
        private readonly EngineActorController _controller;

        public List<subType> VisibleOfType<subType>() where subType : ActorStar => _controller.VisibleOfType<subType>();
        public List<ActorStar> Visible() => _controller.VisibleOfType<ActorStar>();
        public List<subType> OfType<subType>() where subType : ActorStar => _controller.OfType<subType>();


        public ActorStarTickHandler(Core core, EngineActorController manager)
        {
            _core = core;
            _controller = manager;
        }

        public void ExecuteWorldClockTick(HgPoint<double> displacementVector)
        {
            if (displacementVector.X != 0 || displacementVector.Y != 0)
            {
                #region Add new stars...

                if (_controller.VisibleOfType<ActorStar>().Count < 100) //Never wan't more than n stars.
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

        public ActorStar Create(double x, double y)
        {
            lock (_controller.Collection)
            {
                var obj = new ActorStar(_core)
                {
                    X = x,
                    Y = y
                };
                _controller.Collection.Add(obj);
                return obj;
            }
        }

        public ActorStar Create()
        {
            lock (_controller.Collection)
            {
                var obj = new ActorStar(_core);
                _controller.Collection.Add(obj);
                return obj;
            }
        }

        public void Delete(ActorStar obj)
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
