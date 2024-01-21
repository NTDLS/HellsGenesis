using SharpCompress.Archives;
using SharpCompress.Common;
using Si.GameEngine.Core.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Si.GameEngine.Core.Managers
{
    public class EngineAssetManager
    {
        private readonly GameEngineCore _gameEngine;
        private readonly NTDLS.Semaphore.PessimisticSemaphore<Dictionary<string, object>> _collection = new();

        public EngineAssetManager(GameEngineCore gameEngine)
        {
            _gameEngine = gameEngine;
        }

        /// <summary>
        /// Gets and caches a text files content from the asset path.
        /// </summary>
        /// <param name="assetRelativePath"></param>
        /// <param name="defaultText"></param>
        /// <returns></returns>
        public static string GetUserText(string assetRelativePath, string defaultText = "")
        {
            var userDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Strikeforce Infinity");
            if (Directory.Exists(userDataPath) == false)
            {
                Directory.CreateDirectory(userDataPath);
            }
            string assetAbsolutePath = Path.Combine(userDataPath, assetRelativePath).Trim().Replace("\\", "/");
            if (File.Exists(assetAbsolutePath) == false)
            {
                return defaultText;
            }

            return File.ReadAllText(assetAbsolutePath);
        }

        /// <summary>
        /// Saves and caches a text file into the asset path.
        /// </summary>
        /// <param name="assetRelativePath"></param>
        /// <param name="value"></param>
        public static void PutUserText(string assetRelativePath, string value)
        {
            var userDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Strikeforce Infinity");
            if (Directory.Exists(userDataPath) == false)
            {
                Directory.CreateDirectory(userDataPath);
            }
            string assetAbsolutePath = Path.Combine(userDataPath, assetRelativePath).Trim().Replace("\\", "/");
            File.WriteAllText(assetAbsolutePath, value);
        }

        public string GetText(string path, string defaultText = "")
        {
            return _collection.Use(o =>
            {
                if (o.TryGetValue(path, out object value))
                {
                    return value as string;
                }
                else
                {
                    try
                    {
                        var text = GetCompressedText(path);
                        o.Add(path, text);
                        return text;
                    }
                    catch
                    {
                        return defaultText;
                    }
                }
            });
        }

        public SiAudioClip GetAudio(string path, float initialVolumne, bool loopForever = false)
        {
            return _collection.Use(o =>
            {
                path = path.Trim().ToLower();

                if (o.TryGetValue(path, out object value))
                {
                    return (SiAudioClip)value;
                }

                using (var stream = GetCompressedStream(path))
                {
                    var result = new SiAudioClip(_gameEngine, stream, initialVolumne, loopForever);
                    o.Add(path, result);
                    stream.Close();
                    return result;
                }
            });
        }

        public SharpDX.Direct2D1.Bitmap GetBitmap(string path)
        {
            return _collection.Use(o =>
            {
                if (o.TryGetValue(path, out object value))
                {
                    return (SharpDX.Direct2D1.Bitmap)value;
                }

                using (var stream = GetCompressedStream(path))
                {
                    var bitmap = _gameEngine.Rendering.GetBitmap(stream);
                    o.Add(path, bitmap);
                    return bitmap;
                }
            });
        }

        public SharpDX.Direct2D1.Bitmap GetBitmap(string path, int newWidth, int newHeight)
        {
            return _collection.Use(o =>
            {
                string cacheKey = $"{path}:{newWidth}:{newHeight}";

                if (o.TryGetValue(cacheKey, out object value))
                {
                    return (SharpDX.Direct2D1.Bitmap)value;
                }

                using (var stream = GetCompressedStream(path))
                {
                    var bitmap = _gameEngine.Rendering.GetBitmap(stream, newWidth, newHeight);

                    o.Add(cacheKey, bitmap);
                    return bitmap;
                }
            });
        }

        private string GetCompressedText(string path)
        {
            using (var stream = GetCompressedStream(path))
            {
                return System.Text.Encoding.UTF8.GetString(stream.ToArray());
            }
        }

        private MemoryStream GetCompressedStream(string path)
        {
#if DEBUG
            string zipFilePath = "C:\\NTDLS\\StrikeforceInfinity\\Installer\\Si.Assets.rez";
#else
            string zipFilePath = "Si.Assets.rez";
#endif

            using (var archive = ArchiveFactory.Open(zipFilePath, new SharpCompress.Readers.ReaderOptions() { ArchiveEncoding = new ArchiveEncoding() { Default = System.Text.Encoding.Default } }))
            {
                string desiredFilePath = path.Trim().Replace("\\", "/");

                var entry = archive.Entries.FirstOrDefault(e => e.Key.Equals(desiredFilePath, StringComparison.OrdinalIgnoreCase));
                if (entry != null)
                {
                    using (var stream = entry.OpenEntryStream())
                    {
                        var memoryStream = new MemoryStream();
                        stream.CopyTo(memoryStream);
                        memoryStream.Position = 0;
                        return memoryStream;
                    }
                }
            }

            throw new FileNotFoundException(path);
        }
    }
}
