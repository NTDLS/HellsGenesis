using SharpCompress.Archives;
using SharpCompress.Common;
using SharpCompress.Readers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace HG.Engine.Managers
{
    internal class EngineAssetManager
    {
        private readonly Core _core;
        private readonly Dictionary<string, object> _collection = new();

        private readonly string _assetZipPath = @"..\..\..\";

        public EngineAssetManager(Core core)
        {
            _core = core;
        }

        public AudioClip GetAudio(string path, float initialVolumne, bool loopForever = false)
        {
            lock (_collection)
            {
                AudioClip result = null;

                path = path.Trim().ToLower();

                if (_collection.ContainsKey(path))
                {
                    result = (AudioClip)_collection[path];
                }
                else
                {
                    using (var stream = GetCompressedStream(path))
                    {
                        result = new AudioClip(stream, initialVolumne, loopForever);
                    }
                    _collection.Add(path, result);
                }

                return result;
            }
        }

        public Bitmap GetBitmap(string path)
        {
            Bitmap result;

            path = path.Trim().ToLower();

            lock (_collection)
            {
                if (_collection.ContainsKey(path))
                {
                    result = (Bitmap)((Bitmap)_collection[path]).Clone();
                }
                else
                {
                    using (var stream = GetCompressedStream(path))
                    {
                        using var image = Image.FromStream(stream);
                        var newbitmap = new Bitmap(image);
                        result = newbitmap.Clone() as Bitmap;
                        _collection.Add(path, newbitmap);
                    }
                }
            }

            return result;
        }

        private MemoryStream GetCompressedStream(string path)
        {
            string zipFilePath = Path.Combine(_assetZipPath, "Assets.zip");

            using (var archive = ArchiveFactory.Open(zipFilePath, new ReaderOptions() { ArchiveEncoding = new ArchiveEncoding() { Default = Encoding.Default } }))
            {
                string desiredFilePath = Path.Combine("Assets", path).Trim().Replace("\\", "/");

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
