using System.IO;

namespace HG.Engine.Controllers
{
    internal class EngineAudioController
    {
        private readonly Core _core;

        public AudioClip BackgroundMusicSound { get; private set; }
        public AudioClip RadarBlipsSound { get; private set; }
        public AudioClip DoorIsAjarSound { get; private set; }
        public AudioClip LockedOnBlip { get; private set; }

        public EngineAudioController(Core core)
        {
            _core = core;

            DoorIsAjarSound = Get(@"Sounds\Ship\Door Is Ajar.wav", 0.50f, false);
            RadarBlipsSound = Get(@"Sounds\Ship\Radar Blips.wav", 0.20f, false);
            LockedOnBlip = Get(@"Sounds\Ship\Locked On.wav", 0.20f, false);
            BackgroundMusicSound = Get(@"Sounds\Music\Background.wav", 0.25f, true);
        }

        public AudioClip Get(string path, float initialVolumne, bool loopForever = false)
        {
            return _core.Assets.GetAudio(path, initialVolumne, loopForever);
        }

        public void PlayRandomExplosion()
        {
            const string _assetExplosionSoundPath = @"Sounds\Explode\";
            int explosionSoundCount = 4;
            int selectedExplosionSoundIndex = HgRandom.Random.Next(0, 1000) % explosionSoundCount;
            var explodeSound = _core.Audio.Get(Path.Combine(_assetExplosionSoundPath, $"{selectedExplosionSoundIndex}.wav"), 1.0f);
            explodeSound?.Play();
        }
    }
}
