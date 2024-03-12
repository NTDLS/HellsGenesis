using Si.Audio;
using Si.Library;
using System.IO;

namespace Si.Engine.Manager
{
    /// <summary>
    /// /// Contains global sounds and music.
    /// </summary>
    public class AudioManager
    {
        private readonly EngineCore _engine;

        public SiAudioClip BackgroundMusicSound { get; private set; }
        public SiAudioClip RadarBlipsSound { get; private set; }
        public SiAudioClip DoorIsAjarSound { get; private set; }
        public SiAudioClip LockedOnBlip { get; private set; }
        public SiAudioClip Click { get; private set; }

        public AudioManager(EngineCore engine)
        {
            _engine = engine;

            Click = _engine.Assets.GetAudio(@"Sounds\Other\Click.wav", 0.70f, false);
            DoorIsAjarSound = _engine.Assets.GetAudio(@"Sounds\Ship\Door Is Ajar.wav", 0.50f, false);
            RadarBlipsSound = _engine.Assets.GetAudio(@"Sounds\Ship\Radar Blips.wav", 0.20f, false);
            LockedOnBlip = _engine.Assets.GetAudio(@"Sounds\Ship\Locked On.wav", 0.20f, false);
            BackgroundMusicSound = _engine.Assets.GetAudio(@"Sounds\Music\Background.wav", 0.25f, true);
        }

        public void PlayRandomShieldHit()
        {
            var audioClip = _engine.Assets.GetAudio(@"Sounds\Ship\Shield Hit.wav", 1.0f);
            audioClip?.Play();
        }

        public void PlayRandomHullHit()
        {
            var audioClip = _engine.Assets.GetAudio(@"Sounds\Ship\Object Hit.wav", 1.0f);
            audioClip?.Play();
        }

        public void PlayRandomExplosion()
        {
            const string assetPath = @"Sounds\Explode\";
            int assetCount = 4;
            int selectedAssetIndex = SiRandom.Between(0, assetCount - 1);
            var audioClip = _engine.Assets.GetAudio(Path.Combine(assetPath, $"{selectedAssetIndex}.wav"), 1.0f);
            audioClip?.Play();
        }
    }
}
