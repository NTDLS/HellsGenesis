using AI2D.Engine;
using AI2D.Types;
using System.Drawing;
using System.Linq;

namespace AI2D.Actors
{
    public class ActorAttachment : ActorBase
    {
        public bool TakesDamage { get; set; }

        private const string _assetExplosionAnimationPath = @"..\..\..\Assets\Graphics\Animation\Explode\";
        private readonly string[] _assetExplosionAnimationPathFiles = {
            #region Image Paths.
            "Hit Explosion 66 (1).png",
            "Hit Explosion 66 (2).png"
            #endregion
        };

        public ActorAttachment(Core core, string imagePath, Size? size = null)
            : base(core)
        {
            int _hitExplosionImageIndex = Utility.RandomNumber(0, _assetExplosionAnimationPathFiles.Count());

            Initialize(imagePath, size, _assetExplosionAnimationPath + _assetExplosionAnimationPathFiles[_hitExplosionImageIndex], new Size(66, 66));

            X = 0;
            Y = 0;
            Velocity = new Velocity<double>();
        }
    }
}
