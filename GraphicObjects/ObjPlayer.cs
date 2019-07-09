using AI2D.Engine;
using System.Drawing;

namespace AI2D.GraphicObjects
{
    public class ObjPlayer : BaseGraphicObject
    {
        private const string _imagePath = @"..\..\Assets\Graphics\Player\Player (1).png";

        public AudioClip AmmoLowSound { get; private set; }
        public AudioClip AmmoEmptySound { get; private set; }
        public AudioClip ShipEngineRoarSound { get; private set; }
        public AudioClip ShipEngineIdleSound { get; private set; }
        public AudioClip AllSystemsGoSound { get; private set; }

        public ObjPlayer(Core core)
            : base(core)
        {
            Initialize(_imagePath, new Size(32, 32));

            AmmoLowSound = _core.Actors.GetSoundCached(@"..\..\Assets\Sounds\Ship\Ammo Low.wav", 0.75f);
            AmmoEmptySound = _core.Actors.GetSoundCached(@"..\..\Assets\Sounds\Ship\Ammo Empty.wav", 0.75f);
            ShipEngineRoarSound = _core.Actors.GetSoundCached(@"..\..\Assets\Sounds\Ship\Engine Roar.wav", 1.0f, true);
            ShipEngineIdleSound = _core.Actors.GetSoundCached(@"..\..\Assets\Sounds\Ship\Engine Idle.wav", 0.6f, true);
            AllSystemsGoSound = _core.Actors.GetSoundCached(@"..\..\Assets\Sounds\Ship\All Systems Go.wav", 0.75f, false);
        }
    }
}
