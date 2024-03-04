using Si.Audio;
using Si.Library;
using System.IO;

namespace Si.GameEngine.Managers
{
    /// <summary>
    /// /// Contains global sounds and music.
    /// </summary>
    public class EngineAudioManager
    {
        private readonly GameEngineCore _gameEngine;

        public SiAudioClip BackgroundMusicSound { get; private set; }
        public SiAudioClip RadarBlipsSound { get; private set; }
        public SiAudioClip DoorIsAjarSound { get; private set; }
        public SiAudioClip LockedOnBlip { get; private set; }
        public SiAudioClip Click { get; private set; }

        public EngineAudioManager(GameEngineCore gameEngine)
        {
            _gameEngine = gameEngine;

            Click = _gameEngine.Assets.GetAudio(@"Sounds\Other\Click.wav", 0.70f, false);
            DoorIsAjarSound = _gameEngine.Assets.GetAudio(@"Sounds\Ship\Door Is Ajar.wav", 0.50f, false);
            RadarBlipsSound = _gameEngine.Assets.GetAudio(@"Sounds\Ship\Radar Blips.wav", 0.20f, false);
            LockedOnBlip = _gameEngine.Assets.GetAudio(@"Sounds\Ship\Locked On.wav", 0.20f, false);
            BackgroundMusicSound = _gameEngine.Assets.GetAudio(@"Sounds\Music\Background.wav", 0.25f, true);
        }

        public void PlayRandomShieldHit()
        {
            var audioClip = _gameEngine.Assets.GetAudio(@"Sounds\Ship\Shield Hit.wav", 1.0f);
            audioClip?.Play();
        }

        public void PlayRandomHullHit()
        {
            var audioClip = _gameEngine.Assets.GetAudio(@"Sounds\Ship\Object Hit.wav", 1.0f);
            audioClip?.Play();
        }

        public void PlayRandomExplosion()
        {
            const string assetPath = @"Sounds\Explode\";
            int assetCount = 4;
            int selectedAssetIndex = SiRandom.Between(0, assetCount - 1);
            var audioClip = _gameEngine.Assets.GetAudio(Path.Combine(assetPath, $"{selectedAssetIndex}.wav"), 1.0f);
            audioClip?.Play();
        }
    }
}
