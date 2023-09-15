using HG.Actors.BaseClasses;
using HG.Actors.Weapons.BaseClasses;
using HG.Actors.Weapons.Bullets.BaseClasses;
using HG.Engine;
using HG.Engine.Types.Geometry;
using HG.Utility;
using System.IO;

namespace HG.Actors.Weapons.Bullets
{
    internal class BulletScattershot : BulletBase
    {
        private const string _assetPath = @"Graphics\Weapon\BulletScattershot";
        private readonly int imageCount = 4;
        private readonly int selectedImageIndex = 0;

        public BulletScattershot(EngineCore core, WeaponBase weapon, ActorBase firedFrom,
             ActorBase lockedTarget = null, HgPoint xyOffset = null)
            : base(core, weapon, firedFrom, null, lockedTarget, xyOffset)
        {
            selectedImageIndex = HgRandom.Random.Next(0, 1000) % imageCount;
            SetImage(Path.Combine(_assetPath, $"{selectedImageIndex}.png"));

            Initialize();
        }
    }
}
