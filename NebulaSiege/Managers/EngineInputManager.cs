using NebulaSiege.Engine;
using NebulaSiege.Engine.Types.Geometry;
using NebulaSiege.Sprites;
using NebulaSiege.Sprites.Enemies;
using System.Collections.Generic;
using System.Windows.Forms;

namespace NebulaSiege.Managers
{
    /// <summary>
    /// Handles keyboard input and keeps track of key-press states.
    /// </summary>
    internal class EngineInputManager
    {
        private readonly EngineCore _core;
        private readonly Dictionary<HgPlayerKey, HgKeyPressState> _keyStates = new();

        public EngineInputManager(EngineCore core)
        {
            _core = core;
        }

        public bool IsKeyPressed(HgPlayerKey key)
        {
            if (_keyStates.ContainsKey(key))
            {
                return _keyStates[key] == HgKeyPressState.Down;
            }

            return false;
        }

        /// <summary>
        /// Allows the containing window to tell the engine about key press events.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="state"></param>
        public void KeyStateChanged(HgPlayerKey key, HgKeyPressState state, Keys? other = null)
        {
            if (_keyStates.ContainsKey(key))
            {
                _keyStates[key] = state;
            }
            else
            {
                _keyStates.Add(key, state);
            }
        }

        public void HandleSingleKeyPress(Keys key)
        {
            #region Debug stuff.
            if (key == Keys.Delete)
            {
                if (_core.Sprites.OfType<_SpriteEnemyBase>().Count > 0)
                {
                    _core.Sprites.OfType<_SpriteEnemyBase>()[0].Explode();
                }
            }
            else if (key == Keys.Oem3) //~
            {
                _core.Debug.ToggleVisibility();
            }
            #endregion
            else if (key == Keys.F1)
            {
                //HgDevelopmentTools.ParticleBlast(_core, 50);
                //HgDevelopmentTools.CreateImageSizeVariants(@"..\..\..\Assets\Graphics\Fragments");
                //_core.Sprites.NewGame();
                //_core.Sprites.ResetAndShowPlayer();
            }
            else if (key == Keys.P)
            {
                var textBlock = _core.Sprites.GetSpriteByTag<SpriteTextBlock>("PausedText");
                if (textBlock == null)
                {
                    textBlock = _core.Sprites.TextBlocks.Create(_core.DirectX.TextFormats.LargeBlocker, _core.DirectX.Materials.Brushes.Red, new NsPoint(100, 100), true, "PausedText");
                    textBlock.Text = "Paused";
                    textBlock.X = _core.Display.NatrualScreenSize.Width / 2 - textBlock.Size.Width / 2;
                    textBlock.Y = _core.Display.NatrualScreenSize.Height / 2 - textBlock.Size.Height / 2;
                }

                _core.TogglePause();
                textBlock.Visable = _core.IsPaused();
            }

            if (key == Keys.Left)
            {
                if (_core.Player?.Sprite?.Visable == true)
                {
                    _core.Player?.Sprite?.SelectPreviousAvailableUsableSecondaryWeapon();
                }
            }
            else if (key == Keys.Right)
            {
                if (_core.Player?.Sprite?.Visable == true)
                {
                    _core.Player?.Sprite?.SelectNextAvailableUsableSecondaryWeapon();
                }
            }
        }
    }
}
