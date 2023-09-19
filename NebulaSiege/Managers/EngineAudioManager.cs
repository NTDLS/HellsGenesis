using NebulaSiege.Engine;
using NebulaSiege.Engine.Types;
using NebulaSiege.Utility;
using System.IO;

namespace NebulaSiege.Managers
{
    /// <summary>
    /// /// Contains global sounds and music.
    /// </summary>
    internal class EngineAudioManager
    {
        private readonly EngineCore _core;

        public NsAudioClip BackgroundMusicSound { get; private set; }
        public NsAudioClip RadarBlipsSound { get; private set; }
        public NsAudioClip DoorIsAjarSound { get; private set; }
        public NsAudioClip LockedOnBlip { get; private set; }
        public NsAudioClip Click { get; private set; }

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
