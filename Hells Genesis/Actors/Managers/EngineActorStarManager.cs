using HG.Actors.Objects;
using HG.Engine;
using HG.Engine.Managers;
using HG.Types;

namespace HG.Actors.Factories
{
    internal class EngineActorStarManager
    {
        private readonly Core _core;
        private readonly EngineActorManager _manager;

        public EngineActorStarManager(Core core, EngineActorManager manager)
        {
            _core = core;
            _manager = manager;
        }

        public void ExecuteWorldClockTick(HGPoint<double> appliedOffset)
        {
            if (appliedOffset.X != 0 || appliedOffset.Y != 0)
            {
                #region Add new stars...

                if (_manager.VisibleOfType<ActorStar>().Count < 100) //Never wan't more than n stars.
                {
                    if (appliedOffset.X > 0)
                    {
                        for (int i = 0; i < 100; i++) //n chances to create a star.
                        {
                            if (HGRandom.ChanceIn(1000)) //1 in n chance to create a star.
                            {
                                int x = HGRandom.Random.Next(_core.Display.TotalCanvasSize.Width - (int)appliedOffset.X, _core.Display.TotalCanvasSize.Width);
                                int y = HGRandom.Random.Next(0, _core.Display.TotalCanvasSize.Height);
                                _manager.Stars.Create(x, y);
                            }
                        }
                    }
                    else if (appliedOffset.X < 0)
                    {
                        for (int i = 0; i < 100; i++) //n chances to create a star.
                        {
                            if (HGRandom.ChanceIn(1000)) //1 in n chance to create a star.
                            {
                                int x = HGRandom.Random.Next(0, (int)-appliedOffset.X);
                                int y = HGRandom.Random.Next(0, _core.Display.TotalCanvasSize.Height);
                                _manager.Stars.Create(x, y);
                            }
                        }
                    }
                    if (appliedOffset.Y > 0)
                    {
                        for (int i = 0; i < 100; i++) //n chances to create a star.
                        {
                            if (HGRandom.ChanceIn(1000)) //1 in n chance to create a star.
                            {
                                int x = HGRandom.Random.Next(0, _core.Display.TotalCanvasSize.Width);
                                int y = HGRandom.Random.Next(_core.Display.TotalCanvasSize.Height - (int)appliedOffset.Y, _core.Display.TotalCanvasSize.Height);
                                _manager.Stars.Create(x, y);
                            }
                        }
                    }
                    else if (appliedOffset.Y < 0)
                    {
                        for (int i = 0; i < 100; i++) //n chances to create a star.
                        {
                            if (HGRandom.ChanceIn(1000)) //1 in n chance to create a star.
                            {
                                int x = HGRandom.Random.Next(0, _core.Display.TotalCanvasSize.Width);
                                int y = HGRandom.Random.Next(0, (int)-appliedOffset.Y);
                                _manager.Stars.Create(x, y);
                            }
                        }
                    }
                }

                #endregion

                foreach (var star in _manager.VisibleOfType<ActorStar>())
                {
                    star.ApplyMotion(appliedOffset);
                }
            }
        }

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
    }
}
