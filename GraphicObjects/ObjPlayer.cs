using AI2D.Engine;

namespace AI2D.GraphicObjects
{
    public class ObjPlayer : BaseGraphicObject
    {
        private const string _imagePath = @"..\..\Assets\Graphics\Player\Player (1).png";

        public AudioClip AmmoLowSound { get; private set; }
        public AudioClip AmmoEmptySound { get; private set; }

        public ObjPlayer(Core core)
            : base(core)
        {
            LoadResources(_imagePath, new System.Drawing.Size(32, 32));

            AmmoLowSound = _core.Actors.GetSoundCached(@"..\..\Assets\Sounds\Ammo Low.wav", 0.75f);
            AmmoEmptySound = _core.Actors.GetSoundCached(@"..\..\Assets\Sounds\Ammo Empty.wav", 0.75f);
        }
    }
}
