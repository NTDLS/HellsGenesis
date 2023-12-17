using NebulaSiege.Engine;
using NebulaSiege.Sprites.Enemies.BaseClasses;
using NebulaSiege.Utility;
using SharpDX.DirectInput;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace NebulaSiege.Managers
{
    /// <summary>
    /// Handles keyboard input and keeps track of key-press states.
    /// </summary>
    internal class EngineInputManager
    {
        private readonly EngineCore _core;
        private readonly Dictionary<HgPlayerKey, bool> _keyStates = new();

        public DirectInput DirectInput { get; private set; }
        public Keyboard Keyboard { get; private set; }

        public EngineInputManager(EngineCore core)
        {
            _core = core;

            DirectInput = new();
            Keyboard = new Keyboard(DirectInput);
            Keyboard.Acquire();
        }

        ~EngineInputManager()
        {
            DirectInput.Dispose();
            Keyboard.Dispose();
        }

        public void Snapshot()
        {
            var keys = Keyboard.GetCurrentState();

            _core.Input.KeyStateChanged(HgPlayerKey.AltForward, keys.IsPressed(Key.Home));
            _core.Input.KeyStateChanged(HgPlayerKey.AltSpeedBoost, keys.IsPressed(Key.LeftControl));
            _core.Input.KeyStateChanged(HgPlayerKey.AltRotateCounterClockwise, keys.IsPressed(Key.Delete));
            _core.Input.KeyStateChanged(HgPlayerKey.AltRotateClockwise, keys.IsPressed(Key.PageDown));
            _core.Input.KeyStateChanged(HgPlayerKey.AltPrimaryFire, keys.IsPressed(Key.LeftAlt));

            _core.Input.KeyStateChanged(HgPlayerKey.SpeedBoost, keys.IsPressed(Key.LeftShift));
            _core.Input.KeyStateChanged(HgPlayerKey.Forward, keys.IsPressed(Key.W));
            _core.Input.KeyStateChanged(HgPlayerKey.RotateCounterClockwise, keys.IsPressed(Key.A));
            _core.Input.KeyStateChanged(HgPlayerKey.Reverse, keys.IsPressed(Key.S));
            _core.Input.KeyStateChanged(HgPlayerKey.RotateClockwise, keys.IsPressed(Key.D));
            _core.Input.KeyStateChanged(HgPlayerKey.PrimaryFire, keys.IsPressed(Key.Space));
            _core.Input.KeyStateChanged(HgPlayerKey.SecondaryFire, keys.IsPressed(Key.RightControl));
            _core.Input.KeyStateChanged(HgPlayerKey.Escape, keys.IsPressed(Key.Escape));
            _core.Input.KeyStateChanged(HgPlayerKey.Left, keys.IsPressed(Key.Left));
            _core.Input.KeyStateChanged(HgPlayerKey.Right, keys.IsPressed(Key.Right));
            _core.Input.KeyStateChanged(HgPlayerKey.Up, keys.IsPressed(Key.Up));
            _core.Input.KeyStateChanged(HgPlayerKey.Down, keys.IsPressed(Key.Down));
            _core.Input.KeyStateChanged(HgPlayerKey.Enter, keys.IsPressed(Key.Return));
        }

        public bool IsKeyPressed(HgPlayerKey key)
        {
            if (_keyStates.ContainsKey(key))
            {
                return _keyStates[key];
            }

            return false;
        }

        /// <summary>
        /// Allows the containing window to tell the engine about key press events.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="state"></param>
        public void KeyStateChanged(HgPlayerKey key, bool state, Keys? other = null)
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
            if (key == Keys.Oem3)
            {
                _core.Debug.ToggleVisibility();
            }

            else if (key == Keys.P)
            {
                _core.TogglePause();
            }
            else if (key == Keys.F1)
            {
                if (_core.Sprites.OfType<SpriteEnemyBase>().Count > 0)
                {
                    _core.Sprites.OfType<SpriteEnemyBase>()[0].Explode();
                }
            }
            else if (key == Keys.F2)
            {
                NsDevelopmentTools.ParticleBlast(_core, 50, _core.Player.Sprite);
                //HgDevelopmentTools.CreateImageSizeVariants(@"..\..\..\Assets\Graphics\Fragments");
                //_core.Sprites.NewGame();
                //_core.Sprites.ResetAndShowPlayer();
            }
            else if (key == Keys.Left)
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
