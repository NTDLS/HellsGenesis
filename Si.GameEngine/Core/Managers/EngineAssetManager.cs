using NTDLS.DelegateThreadPool;
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
#if DEBUG
        private const string _assetPackagePath = "../../../../Installer/Si.Assets.rez";
#else
        private const string _assetPackagePath = "Si.Assets.rez";
#endif

        private readonly GameEngineCore _gameEngine;
        private readonly NTDLS.Semaphore.OptimisticCriticalResource<Dictionary<string, object>> _collection = new();

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
            assetRelativePath = assetRelativePath.ToLower();

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
            assetRelativePath = assetRelativePath.ToLower();

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
            path = path.ToLower();

            var cached = _collection.Read(o =>
            {
                o.TryGetValue(path, out object value);
                return value as string;
            });
            if (cached != null)
            {
                return cached;
            }

            try
            {
                var text = GetCompressedText(path);
                _collection.Write(o => o.TryAdd(path, text));
                return text;
            }
            catch
            {
                return defaultText;
            }
        }

        public SiAudioClip GetAudio(string path)
        {
            path = path.ToLower();

            var cached = _collection.Read(o =>
            {
                o.TryGetValue(path, out object value);
                return (SiAudioClip)value;
            });
            if (cached != null)
            {
                return cached;
            }

            using var stream = GetCompressedStream(path);
            var result = new SiAudioClip(_gameEngine, stream, 1, false);
            _collection.Write(o => o.TryAdd(path, result));
            stream.Close();
            return result;
        }

        public SiAudioClip GetAudio(string path, float initialVolumne, bool loopForever = false)
        {
            path = path.ToLower();

            var cached = _collection.Read(o =>
            {
                if (o.TryGetValue(path, out object value))
                {
                    ((SiAudioClip)value).SetInitialVolumne(initialVolumne);
                    ((SiAudioClip)value).SetLoopForever(loopForever);
                    return (SiAudioClip)value;
                }
                return null;
            });
            if (cached != null)
            {
                return cached;
            }

            using var stream = GetCompressedStream(path);
            var result = new SiAudioClip(_gameEngine, stream, initialVolumne, loopForever);
            _collection.Write(o => o.TryAdd(path, result));
            stream.Close();
            return result;
        }

        public SharpDX.Direct2D1.Bitmap GetBitmap(string path)
        {
            path = path.ToLower();

            var cached = _collection.Read(o =>
            {
                o.TryGetValue(path, out object value);
                return (SharpDX.Direct2D1.Bitmap)value;
            });
            if (cached != null)
            {
                return cached;
            }

            using var stream = GetCompressedStream(path);
            var bitmap = _gameEngine.Rendering.GetBitmap(stream);
            _collection.Write(o => o.TryAdd(path, bitmap));
            return bitmap;
        }

        public SharpDX.Direct2D1.Bitmap GetBitmap(string path, int newWidth, int newHeight)
        {
            path = path.ToLower();

            return _collection.Write(o =>
            {
                string cacheKey = $"{path}:{newWidth}:{newHeight}";

                if (o.TryGetValue(cacheKey, out object value))
                {
                    return (SharpDX.Direct2D1.Bitmap)value;
                }

                using var stream = GetCompressedStream(path);
                var bitmap = _gameEngine.Rendering.GetBitmap(stream, newWidth, newHeight);
                o.Add(cacheKey, bitmap);
                return bitmap;
            });
        }

        public void PreCacheAllAssets()
        {
            using (var archive = ArchiveFactory.Open(_assetPackagePath))
            {
                using (DelegateThreadPool dtp = new(Environment.ProcessorCount * 4))
                {
                    var dtpQueue = dtp.CreateQueueStateCollection();

                    foreach (var entry in archive.Entries)
                    {
                        switch (Path.GetExtension(entry.Key).ToLower())
                        {
                            case ".png":
                                dtpQueue.Enqueue(() => GetBitmap(entry.Key));
                                break;
                            case ".wav":
                                dtpQueue.Enqueue(() => GetAudio(entry.Key));
                                break;
                        }
                    }

                    dtpQueue.WaitForCompletion();
                }
            }
        }

        private string GetCompressedText(string path)
        {
            using var stream = GetCompressedStream(path);
            return System.Text.Encoding.UTF8.GetString(stream.ToArray());
        }

        private MemoryStream GetCompressedStream(string path)
        {
            using (var archive = ArchiveFactory.Open(_assetPackagePath, new SharpCompress.Readers.ReaderOptions() { ArchiveEncoding = new ArchiveEncoding() { Default = System.Text.Encoding.Default } }))
            {
                string desiredFilePath = path.Trim().Replace("\\", "/");

                var entry = archive.Entries.FirstOrDefault(e => e.Key.Equals(desiredFilePath, StringComparison.OrdinalIgnoreCase));
                if (entry != null)
                {
                    using var stream = entry.OpenEntryStream();
                    var memoryStream = new MemoryStream();
                    stream.CopyTo(memoryStream);
                    memoryStream.Position = 0;
                    return memoryStream;
                }
            }

            throw new FileNotFoundException(path);
        }
    }
}
