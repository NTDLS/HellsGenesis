using NAudio.Wave;
using StrikeforceInfinity.Game.Engine;
using StrikeforceInfinity.Game.Sprites;
using System;
using System.Drawing;
using System.IO;

namespace StrikeforceInfinity.Game.Utility
{
    internal static class SiDevelopmentTools
    {
        public static void StripWavFiles(string path)
        {
            var files = Directory.EnumerateFiles(path, "*.wav", SearchOption.AllDirectories);

            // Display the names of all files in the folder
            foreach (string file in files)
            {
                Console.WriteLine(file);

                using (var reader = new WaveFileReader(file))
                {
                    // Create a WaveFileWriter to write the output WAV file
                    using (var writer = new WaveFileWriter($"{file}.tmp", reader.WaveFormat))
                    {
                        byte[] buffer = new byte[4096];
                        int bytesRead;

                        // Loop through the input WAV file, skipping metadata chunks
                        while ((bytesRead = reader.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            writer.Write(buffer, 0, bytesRead);
                        }
                    }
                }

                File.Delete(file);
                File.Move($"{file}.tmp", file);
            }
        }

        /// <summary>
        /// Tests dumping particles at a given position.
        /// </summary>
        /// <param name="core"></param>
        public static void ParticleBlast(EngineCore gameCore, int particleCount, SpriteBase at)
        {
            double X = at.X;
            double Y = at.Y;

            for (int i = 0; i < particleCount; i++)
            {
                var obj = gameCore.Sprites.Particles.CreateRandomShipPartParticleAt(X + HgRandom.Between(-20, 20), Y + HgRandom.Between(-20, 20));
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
