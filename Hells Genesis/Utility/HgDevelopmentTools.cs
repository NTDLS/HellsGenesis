using HG.Engine;
using System.Drawing;
using System.IO;

namespace HG.Utility
{
    internal static class HgDevelopmentTools
    {
        /// <summary>
        /// Tests dumping particles at a given position.
        /// </summary>
        /// <param name="core"></param>
        public static void ParticleBlast(Core core, int particleCount)
        {
            double X = core.Player.Actor.X;
            double Y = core.Player.Actor.Y;

            for (int i = 0; i < particleCount; i++)
            {
                var obj = core.Actors.Particles.CreateRandomShipPartParticleAt(X + HgRandom.RandomNumber(-20, 20), Y + HgRandom.RandomNumber(-20, 20));
                obj.Visable = true;
            }
        }

        /// <summary>
        /// Development utility for creating resized variants of images. This is used to pre-render variant particles.
        /// </summary>
        /// <param name="path"></param>
        public static void CreateImageSizeVariants(string path)
        {
            int newImageIndex = 0;

            for (var imageIndex = 0; imageIndex < 31; imageIndex++)
            {
                for (int sizeX = 1; sizeX < 4; sizeX += 2)
                {
                    for (int sizeY = 1; sizeY < 24; sizeY += 2)
                    {
                        string imagePath = Path.Combine(path, $"{imageIndex}.png");
                        using (var image = Image.FromFile(imagePath))
                        {
                            var resizedImage = new Bitmap(sizeX, sizeY);

                            using (var g = Graphics.FromImage(resizedImage))
                            {
                                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bicubic;
                                g.DrawImage(image, 0, 0, sizeX, sizeY);
                            }

                            string newImagePath = Path.Combine(path, "Variants", $"{newImageIndex++}.png");
                            resizedImage.Save(newImagePath);
                        }
                    }
                }

                for (int sizeX = 1; sizeX < 24; sizeX += 2)
                {
                    for (int sizeY = 1; sizeY < 4; sizeY += 2)
                    {
                        string imagePath = Path.Combine(path, $"{imageIndex}.png");
                        using (var image = Image.FromFile(imagePath))
                        {
                            var resizedImage = new Bitmap(sizeX, sizeY);

                            using (var g = Graphics.FromImage(resizedImage))
                            {
                                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bicubic;
                                g.DrawImage(image, 0, 0, sizeX, sizeY);
                            }

                            string newImagePath = Path.Combine(path, "Variants", $"{newImageIndex++}.png");
                            resizedImage.Save(newImagePath);
                        }
                    }
                }
            }
        }
    }
}
