using Si.GameEngine.Engine;
using Si.GameEngine.Engine.Types;
using Si.Shared;
using System.IO;

namespace Si.GameEngine.Managers
{
    /// <summary>
    /// /// Contains global sounds and music.
    /// </summary>
    public class EngineAudioManager
    {
        private readonly EngineCore _gameCore;

        public SiAudioClip BackgroundMusicSound { get; private set; }
        public SiAudioClip RadarBlipsSound { get; private set; }
        public SiAudioClip DoorIsAjarSound { get; private set; }
        public SiAudioClip LockedOnBlip { get; private set; }
        public SiAudioClip Click { get; private set; }

        public EngineAudioManager(EngineCore gameCore)
        {
            _gameCore = gameCore;

            Click = _gameCore.Assets.GetAudio(@"Sounds\Other\Click.wav", 0.70f, false);
            DoorIsAjarSound = _gameCore.Assets.GetAudio(@"Sounds\Ship\Door Is Ajar.wav", 0.50f, false);
            RadarBlipsSound = _gameCore.Assets.GetAudio(@"Sounds\Ship\Radar Blips.wav", 0.20f, false);
            LockedOnBlip = _gameCore.Assets.GetAudio(@"Sounds\Ship\Locked On.wav", 0.20f, false);
            BackgroundMusicSound = _gameCore.Assets.GetAudio(@"Sounds\Music\Background.wav", 0.25f, true);
        }

        public void PlayRandomExplosion()
        {
            const string _assetExplosionSoundPath = @"Sounds\Explode\";
            int explosionSoundCount = 4;
            int selectedExplosionSoundIndex = SiRandom.Generator.Next(0, 1000) % explosionSoundCount;
            var explodeSound = _gameCore.Assets.GetAudio(Path.Combine(_assetExplosionSoundPath, $"{selectedExplosionSoundIndex}.wav"), 1.0f);
            explodeSound?.Play();
        }
    }
}
