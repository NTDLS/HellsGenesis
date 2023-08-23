using System.Collections.Generic;

namespace AI2D.Engine.Managers
{
    internal class EngineAudioManager
    {
        private Dictionary<string, AudioClip> _collection { get; set; } = new();

        private readonly Core _core;

        public AudioClip BackgroundMusicSound { get; private set; }
        public AudioClip RadarBlipsSound { get; private set; }
        public AudioClip DoorIsAjarSound { get; private set; }
        public AudioClip LockedOnBlip { get; private set; }

        public EngineAudioManager(Core core)
        {
            _core = core;

            DoorIsAjarSound = Get(@"..\..\..\Assets\Sounds\Ship\Door Is Ajar.wav", 0.50f, false);
            RadarBlipsSound = Get(@"..\..\..\Assets\Sounds\Ship\Radar Blips.wav", 0.20f, false);
            LockedOnBlip = Get(@"..\..\..\Assets\Sounds\Ship\Locked On.wav", 0.20f, false);
            BackgroundMusicSound = Get(@"..\..\..\Assets\Sounds\Music\Background.wav", 0.25f, true);
        }

        public AudioClip Get(string wavFilePath, float initialVolumne, bool loopForever = false)
        {
            lock (_collection)
            {
                AudioClip result = null;

                wavFilePath = wavFilePath.ToLower();

                if (_collection.ContainsKey(wavFilePath))
                {
                    result = _collection[wavFilePath];
                }
                else
                {
                    result = new AudioClip(wavFilePath, initialVolumne, loopForever);
                    _collection.Add(wavFilePath, result);
                }

                return result;
            }
        }

    }
}
