using HG.Engine;
using HG.Types;
using System.IO;


namespace HG.Actors.Weapons.Bullets
{
    internal class BulletHotPepper : BulletBase
    {
        private const string _assetPath = @"Graphics\Weapon\BulletHotPepper";
        private readonly int imageCount = 4;
        private readonly int selectedImageIndex = 0;

        public BulletHotPepper(Core core, WeaponBase weapon, ActorBase firedFrom,
             ActorBase lockedTarget = null, HgPoint<double> xyOffset = null)
            : base(core, weapon, firedFrom, null, lockedTarget, xyOffset)
        {
            selectedImageIndex = HgRandom.Random.Next(0, 1000) % imageCount;
            SetImage(Path.Combine(_assetPath, $"{selectedImageIndex}.png"));

            InitializeGenericBasic();
        }
    }
}
