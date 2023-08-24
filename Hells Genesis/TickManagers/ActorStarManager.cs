using HG.Actors;
using HG.Engine;
using HG.Engine.Managers;
using HG.TickManagers.Interfaces;
using HG.Types;
using System.Collections.Generic;

namespace HG.TickManagers
{
    internal class ActorStarManager : IVectoredTickManager
    {
        private readonly Core _core;
        private readonly EngineActorManager _manager;

        public List<subType> VisibleOfType<subType>() where subType : ActorStar => _manager.VisibleOfType<subType>();
        public List<ActorStar> Visible() => _manager.VisibleOfType<ActorStar>();
        public List<subType> OfType<subType>() where subType : ActorStar => _manager.OfType<subType>();


        public ActorStarManager(Core core, EngineActorManager manager)
        {
            _core = core;
            _manager = manager;
        }

        public void ExecuteWorldClockTick(HgPoint<double> displacementVector)
        {
            if (displacementVector.X != 0 || displacementVector.Y != 0)
            {
                #region Add new stars...

                if (_manager.VisibleOfType<ActorStar>().Count < 100) //Never wan't more than n stars.
                {
                    if (displacementVector.X > 0)
                    {
                        for (int i = 0; i < 100; i++) //n chances to create a star.
                        {
                            if (HgRandom.ChanceIn(1000)) //1 in n chance to create a star.
                            {
                                int x = HgRandom.Random.Next(_core.Display.TotalCanvasSize.Width - (int)displacementVector.X, _core.Display.TotalCanvasSize.Width);
                                int y = HgRandom.Random.Next(0, _core.Display.TotalCanvasSize.Height);
                                _manager.Stars.Create(x, y);
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
                                _manager.Stars.Create(x, y);
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
                                _manager.Stars.Create(x, y);
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
                                _manager.Stars.Create(x, y);
                            }
                        }
                    }
                }

                #endregion

                foreach (var star in _manager.VisibleOfType<ActorStar>())
                {
                    star.ApplyMotion(displacementVector);
                }
            }
        }

        #region Factories.

        public ActorStar Create(double x, double y)
        {
            lock (_manager.Collection)
            {
                var obj = new ActorStar(_core)
                {
                    X = x,
                    Y = y
                };
                _manager.Collection.Add(obj);
                return obj;
            }
        }

        public ActorStar Create()
        {
            lock (_manager.Collection)
            {
                var obj = new ActorStar(_core);
                _manager.Collection.Add(obj);
                return obj;
            }
        }

        public void Delete(ActorStar obj)
        {
            lock (_manager.Collection)
            {
                obj.Cleanup();
                _manager.Collection.Remove(obj);
            }
        }

        #endregion
    }
}
