using AI2D.Engine;
using System.Drawing;

namespace AI2D.GraphicObjects
{
    public class ObjPlayer : BaseGraphicObject
    {
        private const string _imagePath = @"..\..\Assets\Graphics\Player\Alien\1.png";

        public AudioClip AmmoLowSound { get; private set; }
        public AudioClip AmmoEmptySound { get; private set; }
        public AudioClip ShipEngineRoarSound { get; private set; }
        public AudioClip ShipEngineIdleSound { get; private set; }
        public AudioClip AllSystemsGoSound { get; private set; }
        public AudioClip ShieldFailSound { get; private set; }
        public AudioClip ShieldDownSound { get; private set; }
        public AudioClip ShieldMaxSound { get; private set; }
        public AudioClip ShieldNominalSound { get; private set; }
        public AudioClip SystemsFailingSound { get; private set; }
        public AudioClip IntegrityLowSound { get; private set; }

        public int Score { get; set; }
        public int MaxHitPoints { get; set; }
        public int MaxShieldPoints { get; set; }

        public ObjPlayer(Core core)
            : base(core)
        {
            Initialize(_imagePath, new Size(32, 32));

            AmmoLowSound = _core.Actors.GetSoundCached(@"..\..\Assets\Sounds\Ship\Ammo Low.wav", 0.75f);
            SystemsFailingSound = _core.Actors.GetSoundCached(@"..\..\Assets\Sounds\Ship\Systems Failing.wav", 0.75f);
            IntegrityLowSound = _core.Actors.GetSoundCached(@"..\..\Assets\Sounds\Ship\Integrity Low.wav", 0.75f);
            ShieldFailSound = _core.Actors.GetSoundCached(@"..\..\Assets\Sounds\Ship\Shield Fail.wav", 0.75f);
            ShieldDownSound = _core.Actors.GetSoundCached(@"..\..\Assets\Sounds\Ship\Shield Down.wav", 0.75f);
            ShieldMaxSound = _core.Actors.GetSoundCached(@"..\..\Assets\Sounds\Ship\Shield Max.wav", 0.75f);
            ShieldNominalSound = _core.Actors.GetSoundCached(@"..\..\Assets\Sounds\Ship\Shield Nominal.wav", 0.75f);

            AmmoLowSound = _core.Actors.GetSoundCached(@"..\..\Assets\Sounds\Ship\Ammo Low.wav", 0.75f);
            AmmoEmptySound = _core.Actors.GetSoundCached(@"..\..\Assets\Sounds\Ship\Ammo Empty.wav", 0.75f);
            ShipEngineRoarSound = _core.Actors.GetSoundCached(@"..\..\Assets\Sounds\Ship\Engine Roar.wav", 1.0f, true);
            ShipEngineIdleSound = _core.Actors.GetSoundCached(@"..\..\Assets\Sounds\Ship\Engine Idle.wav", 0.6f, true);
            AllSystemsGoSound = _core.Actors.GetSoundCached(@"..\..\Assets\Sounds\Ship\All Systems Go.wav", 0.75f, false);
        }
    }
}
