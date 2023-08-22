using AI2D.Engine;
using AI2D.Types;
using System.IO;


namespace AI2D.Actors.Objects.Weapons.Bullets
{
    internal class BulletHotPepper : BulletBase
    {
        private const string _assetPath = @"..\..\..\Assets\Graphics\Weapon\BulletHotPepper";
        private readonly int imageCount = 4;
        private readonly int selectedImageIndex = 0;


        public BulletHotPepper(Core core, WeaponBase weapon, ActorBase firedFrom,
             ActorBase lockedTarget = null, Point<double> xyOffset = null)
            : base(core, weapon, firedFrom, null, lockedTarget, xyOffset)
        {
            selectedImageIndex = Utility.Random.Next(0, 1000) % imageCount;
            SetImage(Path.Combine(_assetPath, $"{selectedImageIndex}.png"));

            Initialize();
        }
    }
}
