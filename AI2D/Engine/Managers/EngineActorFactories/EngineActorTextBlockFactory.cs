using AI2D.Actors;
using AI2D.Types;
using System.Drawing;

namespace AI2D.Engine.Managers.EngineActorFactories
{
    internal class EngineActorTextBlockFactory
    {
        private readonly Core _core;
        private readonly EngineActorManager _manager;

        public EngineActorTextBlockFactory(Core core, EngineActorManager manager)
        {
            _core = core;
            _manager = manager;
        }

        public ActorRadarPositionTextBlock CreateRadarPosition(string font, Brush color, double size, Point<double> location)
        {
            lock (_manager.Collection)
            {
                var obj = new ActorRadarPositionTextBlock(_core, font, color, size, location);
                _manager.Collection.Add(obj);
                return obj;
            }
        }

        public ActorTextBlock Create(string font, Brush color, double size, Point<double> location, bool isPositionStatic)
        {
            lock (_manager.Collection)
            {
                var obj = new ActorTextBlock(_core, font, color, size, location, isPositionStatic);
                _manager.Collection.Add(obj);
                return obj;
            }
        }

        public ActorTextBlock Create(string font, Brush color, double size, Point<double> location, bool isPositionStatic, string tag)
        {
            lock (_manager.Collection)
            {
                var obj = new ActorTextBlock(_core, font, color, size, location, isPositionStatic);
                obj.Tag = tag;
                _manager.Collection.Add(obj);
                return obj;
            }
        }

        public void Delete(ActorTextBlock obj)
        {
            lock (_manager.Collection)
            {
                obj.Cleanup();
                _manager.Collection.Remove(obj);
            }
        }
    }
}
