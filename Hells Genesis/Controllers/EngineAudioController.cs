using HG.Engine;
using HG.Engine.Types;
using HG.Utility;
using System.IO;

namespace HG.Controllers
{
    internal class EngineAudioController
    {
        private readonly EngineCore _core;

        public HgAudioClip BackgroundMusicSound { get; private set; }
        public HgAudioClip RadarBlipsSound { get; private set; }
        public HgAudioClip DoorIsAjarSound { get; private set; }
        public HgAudioClip LockedOnBlip { get; private set; }
        public HgAudioClip Click { get; private set; }

        public EngineAudioController(EngineCore core)
        {
            _core = core;

            Click = Get(@"Sounds\Other\Click.wav", 0.70f, false);
            DoorIsAjarSound = Get(@"Sounds\Ship\Door Is Ajar.wav", 0.50f, false);
            RadarBlipsSound = Get(@"Sounds\Ship\Radar Blips.wav", 0.20f, false);
            LockedOnBlip = Get(@"Sounds\Ship\Locked On.wav", 0.20f, false);
            BackgroundMusicSound = Get(@"Sounds\Music\Background.wav", 0.25f, true);
        }

        public HgAudioClip Get(string path, float initialVolumne, bool loopForever = false)
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
