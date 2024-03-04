using SharpDX.DirectInput;
using Si.GameEngine.Sprites.Enemies._Superclass;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using static Si.Library.SiConstants;

namespace Si.GameEngine.Managers
{
    /// <summary>
    /// Handles keyboard input and keeps track of key-press states.
    /// </summary>
    public class EngineInputManager
    {
        private readonly GameEngineCore _gameEngine;
        private readonly Dictionary<SiPlayerKey, bool> _playerKeyStates = new();
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

        public EngineInputManager(GameEngineCore gameEngine)
        {
            _gameEngine = gameEngine;

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
            if (_gameEngine.Display.IsDrawingSurfaceFocused == false)
            {
                //We do this so that I can have more than one instance open on the same computer 
                //  at a time without the keyboard commands to one affecting the other.
                _playerKeyStates.Clear();
                return;
            }

            var keyboardState = Keyboard.GetCurrentState();

            _gameEngine.Input.KeyStateChanged(SiPlayerKey.StrafeLeft, keyboardState.IsPressed(Key.Left));
            _gameEngine.Input.KeyStateChanged(SiPlayerKey.StrafeRight, keyboardState.IsPressed(Key.Right));
            _gameEngine.Input.KeyStateChanged(SiPlayerKey.SpeedBoost, keyboardState.IsPressed(Key.LeftShift));
            _gameEngine.Input.KeyStateChanged(SiPlayerKey.Forward, keyboardState.IsPressed(Key.W));
            _gameEngine.Input.KeyStateChanged(SiPlayerKey.RotateCounterClockwise, keyboardState.IsPressed(Key.A));
            _gameEngine.Input.KeyStateChanged(SiPlayerKey.Reverse, keyboardState.IsPressed(Key.S));
            _gameEngine.Input.KeyStateChanged(SiPlayerKey.RotateClockwise, keyboardState.IsPressed(Key.D));
            _gameEngine.Input.KeyStateChanged(SiPlayerKey.PrimaryFire, keyboardState.IsPressed(Key.Space));
            _gameEngine.Input.KeyStateChanged(SiPlayerKey.SecondaryFire, keyboardState.IsPressed(Key.RightControl));
            _gameEngine.Input.KeyStateChanged(SiPlayerKey.Escape, keyboardState.IsPressed(Key.Escape));
            _gameEngine.Input.KeyStateChanged(SiPlayerKey.Left, keyboardState.IsPressed(Key.Left));
            _gameEngine.Input.KeyStateChanged(SiPlayerKey.Right, keyboardState.IsPressed(Key.Left));
            _gameEngine.Input.KeyStateChanged(SiPlayerKey.Up, keyboardState.IsPressed(Key.Up));
            _gameEngine.Input.KeyStateChanged(SiPlayerKey.Down, keyboardState.IsPressed(Key.Down));
            _gameEngine.Input.KeyStateChanged(SiPlayerKey.Enter, keyboardState.IsPressed(Key.Return));

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
                bool shiftKeyDown = _gameEngine.Input.DepressedKeys.Contains(Key.LeftShift) || _gameEngine.Input.DepressedKeys.Contains(Key.RightShift);
                if (shiftKeyDown)
                {
                    shouldBeCaps = !shouldBeCaps;
                }

                TypedString = string.Empty;

                foreach (var key in _gameEngine.Input.CycledKeys)
                {
                    if (key == Key.Space)
                    {
                        TypedString = " ";
                    }
                    else if (!_gameEngine.Input.IsModifierKey(key))
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

        public bool IsKeyPressed(SiPlayerKey key)
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
        public void KeyStateChanged(SiPlayerKey key, bool state, Keys? other = null)
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
                _gameEngine.Debug.ToggleVisibility();
            }

            else if (key == Keys.P)
            {
                _gameEngine.TogglePause();
            }
            else if (key == Keys.F1)
            {
                if (_gameEngine.Sprites.OfType<SpriteEnemyBase>().Count > 0)
                {
                    _gameEngine.Sprites.OfType<SpriteEnemyBase>()[0].Explode();
                }
            }
            else if (key == Keys.F4)
            {
                _gameEngine.Rendering.AddScreenShake(4, 100);
            }
            else if (key == Keys.F2)
            {
                /*
                var bitmaps = _gameEngine.Rendering.GenerateIrregularFragments(_gameEngine.Player.Sprite.GetImage(), 10, 3);
                foreach (var bitmap in bitmaps)
                {
                    var frag = _gameEngine.Sprites.Debugs.CreateAtCenterScreen();
                    frag.X += SiRandom.Between(-200, 200);
                    frag.Y += SiRandom.Between(-200, 200);

                    frag.SetImage(bitmap);
                    frag.Visable = true;

                    //bitmaps.
                }
                */

                //_gameEngine.Sprites.GenericSprites.FragmentBlastOf(_gameEngine.Player.Sprite);
                _gameEngine.Sprites.Particles.ParticleBlast(100, _gameEngine.Player.Sprite);
                //_gameEngine.Sprites.NewGame();
                //_gameEngine.Sprites.ResetAndShowPlayer();
            }
            else if (key == Keys.F3)
            {
                _gameEngine.Sprites.Particles.ParticleCloud(500, _gameEngine.Player.Sprite);
            }
            else if (key == Keys.Q)
            {
                if (_gameEngine.Player?.Sprite?.Visable == true)
                {
                    _gameEngine.Player?.Sprite?.SelectPreviousAvailableUsableSecondaryWeapon();
                }
            }
            else if (key == Keys.E)
            {
                if (_gameEngine.Player?.Sprite?.Visable == true)
                {
                    _gameEngine.Player?.Sprite?.SelectNextAvailableUsableSecondaryWeapon();
                }
            }
        }
    }
}
