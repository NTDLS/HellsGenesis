using HG.Engine;
using HG.Engine.Types;
using HG.Utility;
using System.IO;

namespace HG.Managers
{
    /// <summary>
    /// /// Contains global sounds and music.
    /// </summary>
    internal class EngineAudioManager
    {
        private readonly EngineCore _core;

        public HgAudioClip BackgroundMusicSound { get; private set; }
        public HgAudioClip RadarBlipsSound { get; private set; }
        public HgAudioClip DoorIsAjarSound { get; private set; }
        public HgAudioClip LockedOnBlip { get; private set; }
        public HgAudioClip Click { get; private set; }

        public EngineAudioManager(EngineCore core)
        {
            _core = core;

            Click = _core.Assets.GetAudio(@"Sounds\Other\Click.wav", 0.70f, false);
            DoorIsAjarSound = _core.Assets.GetAudio(@"Sounds\Ship\Door Is Ajar.wav", 0.50f, false);
            RadarBlipsSound = _core.Assets.GetAudio(@"Sounds\Ship\Radar Blips.wav", 0.20f, false);
            LockedOnBlip = _core.Assets.GetAudio(@"Sounds\Ship\Locked On.wav", 0.20f, false);
            BackgroundMusicSound = _core.Assets.GetAudio(@"Sounds\Music\Background.wav", 0.25f, true);
        }

        public void PlayRandomExplosion()
        {
            const string _assetExplosionSoundPath = @"Sounds\Explode\";
            int explosionSoundCount = 4;
            int selectedExplosionSoundIndex = HgRandom.Generator.Next(0, 1000) % explosionSoundCount;
            var explodeSound = _core.Assets.GetAudio(Path.Combine(_assetExplosionSoundPath, $"{selectedExplosionSoundIndex}.wav"), 1.0f);
            explodeSound?.Play();
        }
    }
}
