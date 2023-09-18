using HG.Engine;
using HG.Engine.Types;
using NTDLS.Determinet;
using System.Collections.Generic;
using System.IO;

namespace HG.Managers
{
    /// <summary>
    /// Allows for access to various types of assets (images, sounds, text, etc).
    /// </summary>
    internal class EngineAssetManager
    {
        private readonly EngineCore _core;
        private readonly Dictionary<string, object> _collection = new();

        private readonly string assetRawPath = @"..\..\..\Assets";

        public EngineAssetManager(EngineCore core)
        {
            _core = core;
        }

        /// <summary>
        /// Gets and caches a neural network instance from the asset path. Returns the clone of that network.
        /// </summary>
        /// <param name="assetRelativePath"></param>
        /// <returns>The clone of the cached network.</returns>
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

        /// <summary>
        /// Gets and caches a text files content from the asset path.
        /// </summary>
        /// <param name="assetRelativePath"></param>
        /// <param name="defaultText"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Saves and caches a text file into the asset path.
        /// </summary>
        /// <param name="assetRelativePath"></param>
        /// <param name="value"></param>
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

        /// <summary>
        /// Gets and caches an audio clip from the asset path.
        /// </summary>
        /// <param name="assetRelativePath"></param>
        /// <param name="initialVolumne"></param>
        /// <param name="loopForever"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Gets a image from the asset path and caches it.
        /// </summary>
        /// <param name="assetRelativePath"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Get a image from the asset path and resizes it before caching. Note that future calls to
        /// this function will get the originally cached image regardless of the specified size.
        /// </summary>
        /// <param name="assetRelativePath"></param>
        /// <param name="newWidth"></param>
        /// <param name="newHeight"></param>
        /// <returns></returns>
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
