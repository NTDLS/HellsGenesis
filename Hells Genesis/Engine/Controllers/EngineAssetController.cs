using SharpCompress.Archives;
using SharpCompress.Common;
using SharpCompress.Readers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace HG.Engine.Controllers
{
    internal class EngineAssetController
    {
        private readonly Core _core;
        private readonly Dictionary<string, object> _collection = new();

        private readonly string _assetZipPath = @"..\..\..\";

        public EngineAssetController(Core core)
        {
            _core = core;
        }

        public string GetText(string path, string defaultText = "")
        {
            lock (_collection)
            {
                path = path.Trim().ToLower();

                if (_collection.ContainsKey(path))
                {
                    return _collection[path] as string;
                }
                else
                {
                    try
                    {
                        var text = GetCompressedText(path);
                        _collection.Add(path, text);
                        return text;
                    }
                    catch
                    {
                        return defaultText;
                    }
                }
            }
        }

        public AudioClip GetAudio(string path, float initialVolumne, bool loopForever = false)
        {
            lock (_collection)
            {
                path = path.Trim().ToLower();

                if (_collection.ContainsKey(path))
                {
                    return (AudioClip)_collection[path];
                }
                else
                {
                    using (var stream = GetCompressedStream(path))
                    {
                        var result = new AudioClip(stream, initialVolumne, loopForever);
                        _collection.Add(path, result);
                        return result;
                    }
                }
            }
        }

        public Bitmap GetBitmap(string path)
        {
            path = path.Trim().ToLower();

            lock (_collection)
            {
                if (_collection.ContainsKey(path))
                {
                    return (Bitmap)((Bitmap)_collection[path]).Clone();
                }
                else
                {
                    using (var stream = GetCompressedStream(path))
                    {
                        using var image = Image.FromStream(stream);
                        var newbitmap = new Bitmap(image);
                        var result = newbitmap.Clone() as Bitmap;
                        _collection.Add(path, newbitmap);
                        return result;
                    }
                }
            }
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
            string zipFilePath = Path.Combine(_assetZipPath, "Assets.zip");

            using (var archive = ArchiveFactory.Open(zipFilePath, new ReaderOptions() { ArchiveEncoding = new ArchiveEncoding() { Default = System.Text.Encoding.Default } }))
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
