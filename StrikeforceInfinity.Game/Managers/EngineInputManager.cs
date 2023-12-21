using SharpDX.DirectInput;
using StrikeforceInfinity.Game.Engine;
using StrikeforceInfinity.Game.Sprites.Enemies.BasesAndInterfaces;
using StrikeforceInfinity.Game.Utility;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace StrikeforceInfinity.Game.Managers
{
    /// <summary>
    /// Handles keyboard input and keeps track of key-press states.
    /// </summary>
    internal class EngineInputManager
    {
        private readonly EngineCore _gameCore;
        private readonly Dictionary<HgPlayerKey, bool> _playerKeyStates = new();
        private bool _collectDetailedKeyInformation = false;
        private readonly Dictionary<Key, bool> _allKeyStates = new();

        public DirectInput DirectInput { get; private set; }
        public Keyboard Keyboard { get; private set; }

        /// <summary>
        /// Any string that was typed by the user. Must enable via a call to CollectDetailedKeyInformation().
        /// </summary>
        public string TypedString { get; private set; }

        /// <summary>
        /// Contains a list of keys that have been pressed and then released (cycled).
        /// Must enable via a call to CollectDetailedKeyInformation().
        /// </summary>
        public List<Key> CycledKeys { get; private set; } = new();

        /// <summary>
        /// Contains a list of all keys that are currently pressed down.
        /// Must enable via a call to CollectDetailedKeyInformation().
        /// </summary>
        public List<Key> DepressedKeys { get; private set; } = new();

        public EngineInputManager(EngineCore gameCore)
        {
            _gameCore = gameCore;

            DirectInput = new();
            Keyboard = new Keyboard(DirectInput);
            Keyboard.Acquire();
        }

        ~EngineInputManager()
        {
            DirectInput.Dispose();
            Keyboard.Dispose();
        }

        public void CollectDetailedKeyInformation(bool state)
        {
            if (state != _collectDetailedKeyInformation) //Clear any residual state information.
            {
                TypedString = string.Empty;
                CycledKeys.Clear();
                DepressedKeys.Clear();
            }
            _collectDetailedKeyInformation = state;
        }

        public bool IsModifierKey(Key key)
        {
            return key == Key.LeftAlt || key == Key.RightAlt ||
                   key == Key.LeftControl || key == Key.RightControl ||
                   key == Key.LeftShift || key == Key.RightShift ||
                   key == Key.LeftWindowsKey || key == Key.RightWindowsKey;
        }

        public void Snapshot()
        {
            if (_gameCore.Display.IsDrawingSurfaceFocused == false)
            {
                //We do this so that I can have more than one instance open on the same computer 
                //  at a time without the keyboard commands to one affecting the other.
                _playerKeyStates.Clear();
                return;
            }

            var keyboardState = Keyboard.GetCurrentState();

            _gameCore.Input.KeyStateChanged(HgPlayerKey.SpeedBoost, keyboardState.IsPressed(Key.LeftShift));
            _gameCore.Input.KeyStateChanged(HgPlayerKey.Forward, keyboardState.IsPressed(Key.W));
            _gameCore.Input.KeyStateChanged(HgPlayerKey.RotateCounterClockwise, keyboardState.IsPressed(Key.A));
            _gameCore.Input.KeyStateChanged(HgPlayerKey.Reverse, keyboardState.IsPressed(Key.S));
            _gameCore.Input.KeyStateChanged(HgPlayerKey.RotateClockwise, keyboardState.IsPressed(Key.D));
            _gameCore.Input.KeyStateChanged(HgPlayerKey.PrimaryFire, keyboardState.IsPressed(Key.Space));
            _gameCore.Input.KeyStateChanged(HgPlayerKey.SecondaryFire, keyboardState.IsPressed(Key.RightControl));
            _gameCore.Input.KeyStateChanged(HgPlayerKey.Escape, keyboardState.IsPressed(Key.Escape));
            _gameCore.Input.KeyStateChanged(HgPlayerKey.Left, keyboardState.IsPressed(Key.Left));
            _gameCore.Input.KeyStateChanged(HgPlayerKey.Right, keyboardState.IsPressed(Key.Right));
            _gameCore.Input.KeyStateChanged(HgPlayerKey.Up, keyboardState.IsPressed(Key.Up));
            _gameCore.Input.KeyStateChanged(HgPlayerKey.Down, keyboardState.IsPressed(Key.Down));
            _gameCore.Input.KeyStateChanged(HgPlayerKey.Enter, keyboardState.IsPressed(Key.Return));

            //I beleive that this information may be taxing to gather.
            //Regardless we don't typically need is to require any code that uses it to enable it.
            if (_collectDetailedKeyInformation)
            {
                CycledKeys.Clear();

                DepressedKeys.Clear();
                DepressedKeys.AddRange(keyboardState.PressedKeys);

                foreach (var key in Enum.GetValues(typeof(Key)))
                {
                    var currentKey = (Key)key;
                    var isPressed = keyboardState.IsPressed(currentKey);

                    if (isPressed && !_allKeyStates.ContainsKey(currentKey))
                    {
                        _allKeyStates[currentKey] = true;

                        CycledKeys.Add(currentKey);
                    }
                    else if (!isPressed && _allKeyStates.ContainsKey(currentKey))
                    {
                        _allKeyStates.Remove(currentKey);
                    }
                }

                bool shouldBeCaps = Control.IsKeyLocked(Keys.CapsLock);
                bool shiftKeyDown = _gameCore.Input.DepressedKeys.Contains(Key.LeftShift) || _gameCore.Input.DepressedKeys.Contains(Key.RightShift);
                if (shiftKeyDown)
                {
                    shouldBeCaps = !shouldBeCaps;
                }

                TypedString = string.Empty;

                foreach (var key in _gameCore.Input.CycledKeys)
                {
                    if (key == Key.Space)
                    {
                        TypedString = " ";
                    }
                    else if (!_gameCore.Input.IsModifierKey(key))
                    {
                        char? singleChar = null;

                        #region Key mappings.... :(

                        switch (key)
                        {
                            case Key.Space: singleChar = ' '; break;

                            case Key.Minus: singleChar = shiftKeyDown ? '_' : '-'; break;
                            case Key.Equals: singleChar = shiftKeyDown ? '+' : '='; break;
                            case Key.Backslash: singleChar = shiftKeyDown ? '|' : '\\'; break;
                            case Key.Slash: singleChar = shiftKeyDown ? '?' : '/'; break;
                            case Key.Semicolon: singleChar = shiftKeyDown ? ':' : ';'; break;
                            case Key.Apostrophe: singleChar = shiftKeyDown ? '\"' : '\''; break;
                            case Key.LeftBracket: singleChar = shiftKeyDown ? '[' : '{'; break;
                            case Key.RightBracket: singleChar = shiftKeyDown ? ']' : '}'; break;
                            case Key.Comma: singleChar = shiftKeyDown ? '<' : ','; break;
                            case Key.Period: singleChar = shiftKeyDown ? '>' : '.'; break;

                            case Key.D0: singleChar = shiftKeyDown ? ')' : '0'; break;
                            case Key.D1: singleChar = shiftKeyDown ? '!' : '1'; break;
                            case Key.D2: singleChar = shiftKeyDown ? '@' : '2'; break;
                            case Key.D3: singleChar = shiftKeyDown ? '#' : '3'; break;
                            case Key.D4: singleChar = shiftKeyDown ? '$' : '4'; break;
                            case Key.D5: singleChar = shiftKeyDown ? '%' : '5'; break;
                            case Key.D6: singleChar = shiftKeyDown ? '^' : '6'; break;
                            case Key.D7: singleChar = shiftKeyDown ? '&' : '7'; break;
                            case Key.D8: singleChar = shiftKeyDown ? '*' : '8'; break;
                            case Key.D9: singleChar = shiftKeyDown ? '(' : '9'; break;

                            case Key.A: singleChar = 'A'; break;
                            case Key.B: singleChar = 'B'; break;
                            case Key.C: singleChar = 'C'; break;
                            case Key.D: singleChar = 'D'; break;
                            case Key.E: singleChar = 'E'; break;
                            case Key.F: singleChar = 'F'; break;
                            case Key.G: singleChar = 'G'; break;
                            case Key.H: singleChar = 'H'; break;
                            case Key.I: singleChar = 'I'; break;
                            case Key.J: singleChar = 'J'; break;
                            case Key.K: singleChar = 'K'; break;
                            case Key.L: singleChar = 'L'; break;
                            case Key.M: singleChar = 'M'; break;
                            case Key.N: singleChar = 'N'; break;
                            case Key.O: singleChar = 'O'; break;
                            case Key.P: singleChar = 'P'; break;
                            case Key.Q: singleChar = 'Q'; break;
                            case Key.R: singleChar = 'R'; break;
                            case Key.S: singleChar = 'S'; break;
                            case Key.T: singleChar = 'T'; break;
                            case Key.U: singleChar = 'U'; break;
                            case Key.V: singleChar = 'V'; break;
                            case Key.W: singleChar = 'W'; break;
                            case Key.X: singleChar = 'X'; break;
                            case Key.Y: singleChar = 'Y'; break;
                            case Key.Z: singleChar = 'Z'; break;
                            default: break;
                        }
                        #endregion

                        if (singleChar != null)
                        {
                            if (!shouldBeCaps)
                            {
                                singleChar = char.ToLowerInvariant((char)singleChar);
                            }
                        }

                        TypedString += singleChar;
                    }
                }
            }
        }

        public bool IsKeyPressed(HgPlayerKey key)
        {
            if (_playerKeyStates.ContainsKey(key))
            {
                return _playerKeyStates[key];
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
            if (_playerKeyStates.ContainsKey(key))
            {
                _playerKeyStates[key] = state;
            }
            else
            {
                _playerKeyStates.Add(key, state);
            }
        }

        public void HandleSingleKeyPress(Keys key)
        {
            if (key == Keys.Oem3)
            {
                _gameCore.Debug.ToggleVisibility();
            }

            else if (key == Keys.P)
            {
                _gameCore.TogglePause();
            }
            else if (key == Keys.F1)
            {
                if (_gameCore.Sprites.OfType<SpriteEnemyBase>().Count > 0)
                {
                    _gameCore.Sprites.OfType<SpriteEnemyBase>()[0].Explode();
                }
            }
            else if (key == Keys.F2)
            {
                SiDevelopmentTools.ParticleBlast(_gameCore, 50, _gameCore.Player.Sprite);
                //HgDevelopmentTools.CreateImageSizeVariants(@"..\..\..\Assets\Graphics\Fragments");
                //_gameCore.Sprites.NewGame();
                //_gameCore.Sprites.ResetAndShowPlayer();
            }
            else if (key == Keys.Left)
            {
                if (_gameCore.Player?.Sprite?.Visable == true)
                {
                    _gameCore.Player?.Sprite?.SelectPreviousAvailableUsableSecondaryWeapon();
                }
            }
            else if (key == Keys.Right)
            {
                if (_gameCore.Player?.Sprite?.Visable == true)
                {
                    _gameCore.Player?.Sprite?.SelectNextAvailableUsableSecondaryWeapon();
                }
            }
        }
    }
}
