using HG.Engine;
using HG.Types;
using System.Drawing;
using System.IO;

namespace HG.Actors
{
    internal class ActorAttachment : ActorBase
    {
        public bool TakesDamage { get; set; }

        private const string _assetPath = @"..\..\..\Assets\Graphics\Animation\Explode\Hit Explosion 66x66\";
        private readonly int _imageCount = 2;
        private readonly int _selectedImageIndex = 0;

        public ActorAttachment(Core core, string imagePath, Size? size = null)
            : base(core)
        {
            _selectedImageIndex = HgRandom.Random.Next(0, 1000) % _imageCount;
            Initialize(Path.Combine(_assetPath, $"{_selectedImageIndex}.png"));

            X = 0;
            Y = 0;
            Velocity = new HgVelocity<double>();
        }
    }
}
