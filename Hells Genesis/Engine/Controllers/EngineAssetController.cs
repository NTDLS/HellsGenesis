using Determinet;
using HG.Types;
using System.Collections.Generic;
using System.IO;

namespace HG.Engine.Controllers
{
    internal class EngineAssetController
    {
        private readonly Core _core;
        private readonly Dictionary<string, object> _collection = new();

        private readonly string assetRawPath = @"..\..\..\Assets";

        public EngineAssetController(Core core)
        {
            _core = core;
        }

        public DniNeuralNetwork GetNeuralNetwork(string assetRelativePath)
        {
            string assetAbsolutePath = Path.Combine(assetRawPath, assetRelativePath).Trim().Replace("\\", "/");
            string key = $"DniNeuralNetwork({assetAbsolutePath})";

            lock (_collection)
            {
                if (_collection.TryGetValue(key, out var value))
                {
                    return (value as DniNeuralNetwork).Clone();
                }

                var json = _core.Assets.GetText(assetRelativePath);
                if (string.IsNullOrEmpty(json) == false)
                {
                    var network = DniNeuralNetwork.LoadFromText(json);
                    _collection.Add(key, network);
                    return network.Clone();
                }
            }

            return null;
        }

        public string GetText(string assetRelativePath, string defaultText = "")
        {
            string assetAbsolutePath = Path.Combine(assetRawPath, assetRelativePath).Trim().Replace("\\", "/");
            string key = $"Text({assetRelativePath})";

            lock (_collection)
            {
                if (_collection.TryGetValue(key, out var value))
                {
                    return value as string;
                }

                if (File.Exists(assetAbsolutePath) == false)
                {
                    return defaultText;
                }

                return File.ReadAllText(assetAbsolutePath);
            }
        }

        public void PutText(string assetRelativePath, string value)
        {
            string assetAbsolutePath = Path.Combine(assetRawPath, assetRelativePath).Trim().Replace("\\", "/");
            string key = $"Text({assetRelativePath})";

            lock (_collection)
            {
                File.WriteAllText(assetAbsolutePath, value);

                if (_collection.ContainsKey(key))
                {
                    _collection[key] = value;
                }
                else
                {
                    _collection.Add(key, value);
                }
            }
        }

        public HgAudioClip GetAudio(string assetRelativePath, float initialVolumne, bool loopForever = false)
        {
            string assetAbsolutePath = Path.Combine(assetRawPath, assetRelativePath).Trim().Replace("\\", "/");
            string key = $"Audio({assetRelativePath})";

            lock (_collection)
            {
                if (_collection.TryGetValue(key, out var value))
                {
                    return value as HgAudioClip;
                }

                using (var stream = new FileStream(assetAbsolutePath, FileMode.Open, FileAccess.Read))
                {
                    var result = new HgAudioClip(stream, initialVolumne, loopForever);
                    _collection.Add(key, result);
                    stream.Close();
                    return result;
                }
            }
        }

        public SharpDX.Direct2D1.Bitmap GetBitmap(string assetRelativePath)
        {
            string assetAbsolutePath = Path.Combine(assetRawPath, assetRelativePath).Trim().Replace("\\", "/");
            string key = $"Bitmap({assetRelativePath})";

            lock (_collection)
            {
                if (_collection.TryGetValue(key, out var value))
                {
                    return value as SharpDX.Direct2D1.Bitmap;
                }

                var result = _core.DirectX.GetBitmap(assetAbsolutePath);
                _collection.Add(key, result);
                return result;
            }
        }

        public SharpDX.Direct2D1.Bitmap GetBitmap(string assetRelativePath, int newWidth, int newHeight)
        {
            string assetAbsolutePath = Path.Combine(assetRawPath, assetRelativePath).Trim().Replace("\\", "/");
            string key = $"Bitmap({assetRelativePath})";

            lock (_collection)
            {
                if (_collection.TryGetValue(key, out var value))
                {
                    return value as SharpDX.Direct2D1.Bitmap;
                }

                var result = _core.DirectX.GetBitmap(assetAbsolutePath, newWidth, newHeight);
                _collection.Add(key, result);
                return result;
            }
        }
    }
}
