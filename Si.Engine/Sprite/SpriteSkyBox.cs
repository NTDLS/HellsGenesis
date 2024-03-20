using Si.Engine.Sprite._Superclass;
using Si.Library;
using Si.Library.Mathematics.Geometry;
using System;
using System.IO;

namespace Si.Engine.Sprite
{
    public class SpriteSkyBox : SpriteBase
    {
        private const string _assetPath = @"Sprites\SkyBox\";
        private readonly int _imageCount = 5;
        private readonly int selectedImageIndex = 0;

        public SpriteSkyBox(EngineCore engine)
            : base(engine)
        {
            selectedImageIndex = SiRandom.Between(0, _imageCount - 1);
            SetImage(Path.Combine(_assetPath, $"{selectedImageIndex}.png"));

            X = SiRandom.Between(0, engine.Display.TotalCanvasSize.Width);
            Y = SiRandom.Between(0, engine.Display.TotalCanvasSize.Height);

            ZOrder = int.MinValue;

            Velocity.MaximumSpeed = 0.10f;

            if (selectedImageIndex >= 0 && selectedImageIndex <= 0)
            {
                Velocity.ForwardVelocity = SiRandom.Between(8, 10) / 10.0f;
            }
            else
            {
                Velocity.ForwardVelocity = SiRandom.Between(4, 8) / 10.0f;
            }
        }

        private SiPoint _currentOfset = new();
        private readonly float _maxOffset = 200;

        public override void ApplyMotion(float epoch, SiPoint displacementVector)
        {
            if (displacementVector.Sum() != 0)
            {
                var offsetIncrement = new SiPoint(displacementVector.Normalize());

                offsetIncrement.X *= (1 - (Math.Abs(_currentOfset.X) / _maxOffset));
                offsetIncrement.Y *= (1 - (Math.Abs(_currentOfset.Y) / _maxOffset));

                _currentOfset += offsetIncrement;

                Location = _engine.Display.CenterOfCurrentScreen - _currentOfset;
            }
        }
    }
}
