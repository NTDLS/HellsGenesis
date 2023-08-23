using HG.Engine;
using HG.Types;
using System.IO;


namespace HG.Actors.Objects.Weapons.Bullets
{
    internal class BulletHotPepper : BulletBase
    {
        private const string _assetPath = @"..\..\..\Assets\Graphics\Weapon\BulletHotPepper";
        private readonly int imageCount = 4;
        private readonly int selectedImageIndex = 0;

        public BulletHotPepper(Core core, WeaponBase weapon, ActorBase firedFrom,
             ActorBase lockedTarget = null, HGPoint<double> xyOffset = null)
            : base(core, weapon, firedFrom, null, lockedTarget, xyOffset)
        {
            selectedImageIndex = HGRandom.Random.Next(0, 1000) % imageCount;
            SetImage(Path.Combine(_assetPath, $"{selectedImageIndex}.png"));

            Initialize();
        }
    }
}
