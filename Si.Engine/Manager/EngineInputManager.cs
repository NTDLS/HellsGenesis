using SharpDX.DirectInput;
using SharpDX.XInput;
using Si.Engine.Sprite.Enemy._Superclass;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using static Si.Library.SiConstants;

namespace Si.Engine.Manager
{
    /// <summary>
    /// Handles keyboard input and keeps track of key-press states.
    /// </summary>
    public class EngineInputManager
    {
        private readonly EngineCore _engine;
        private readonly Dictionary<SiPlayerKey, float> _playerKeyStates = new();
        private bool _collectDetailedKeyInformation = false;
        private readonly Dictionary<Key, bool> _allKeyStates = new();

        public bool UseGamepad { get; private set; }
        public DirectInput DxInput { get; private set; }
        public Keyboard DxKeyboard { get; private set; }
        public Controller DxController { get; private set; }

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

        //Controller controller;
        //Gamepad gamepad;

        public EngineInputManager(EngineCore engine)
        {
            _engine = engine;

            DxInput = new();
            DxKeyboard = new Keyboard(DxInput);
            DxKeyboard.Acquire();

            #region Gamepad.

            DxController = new Controller(UserIndex.One);
            UseGamepad = DxController.IsConnected;

            #endregion

        }

        ~EngineInputManager()
        {
            DxInput.Dispose();
            DxKeyboard.Dispose();
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
            if (_engine.Display.IsDrawingSurfaceFocused == false)
            {
                //We do this so that I can have more than one instance open on the same computer 
                //  at a time without the keyboard commands to one affecting the other.
                //_playerKeyStates.Clear();
                //return;
            }

            var keyboardState = DxKeyboard.GetCurrentState();

            if (UseGamepad)
            {
                var gamepadState = DxController.GetState();

                // Processing the left thumbstick
                float leftThumbX = gamepadState.Gamepad.LeftThumbX;
                float leftThumbY = gamepadState.Gamepad.LeftThumbY;
                leftThumbX = Math.Abs(leftThumbX) > 4500 ? leftThumbX /= 32768f : 0;
                leftThumbY = Math.Abs(leftThumbY) > 4500 ? leftThumbY /= 32768f : 0;

                // Processing the right thumbstick
                float rightThumbX = gamepadState.Gamepad.RightThumbX;
                float rightThumbY = gamepadState.Gamepad.RightThumbY;
                rightThumbX = Math.Abs(rightThumbX) > 4500 ? rightThumbX /= 32768f : 0;
                rightThumbY = Math.Abs(rightThumbY) > 4500 ? rightThumbY /= 32768f : 0;

                // Processing triggers
                byte leftTrigger = gamepadState.Gamepad.LeftTrigger;
                byte rightTrigger = gamepadState.Gamepad.RightTrigger;

                _engine.Input.KeyStateChangedAmount(SiPlayerKey.StrafeLeft, keyboardState.IsPressed(Key.Left) ? 1 : rightThumbX < 0 ? rightThumbX : 0);
                _engine.Input.KeyStateChangedAmount(SiPlayerKey.StrafeRight, keyboardState.IsPressed(Key.Right) ? 1 : rightThumbX > 0 ? rightThumbX : 0);

                _engine.Input.KeyStateChangedAmount(SiPlayerKey.Forward, keyboardState.IsPressed(Key.W) ? 1 : rightThumbY > 0 ? rightThumbY : 0);
                _engine.Input.KeyStateChangedAmount(SiPlayerKey.Reverse, keyboardState.IsPressed(Key.S) ? 1 : rightThumbY < 0 ? rightThumbY : 0);

                _engine.Input.KeyStateChangedAmount(SiPlayerKey.RotateCounterClockwise, keyboardState.IsPressed(Key.A) ? 1 : leftThumbX < 0 ? leftThumbX : 0);
                _engine.Input.KeyStateChangedAmount(SiPlayerKey.RotateClockwise, keyboardState.IsPressed(Key.D) ? 1 : leftThumbX > 0 ? leftThumbX : 0);

                _engine.Input.KeyStateChangedHard(SiPlayerKey.SpeedBoost, keyboardState.IsPressed(Key.LeftShift) || gamepadState.Gamepad.Buttons.HasFlag(GamepadButtonFlags.LeftThumb));

                _engine.Input.KeyStateChangedHard(SiPlayerKey.SwitchWeaponLeft, keyboardState.IsPressed(Key.Q) || gamepadState.Gamepad.Buttons.HasFlag(GamepadButtonFlags.LeftShoulder));
                _engine.Input.KeyStateChangedHard(SiPlayerKey.SwitchWeaponRight, keyboardState.IsPressed(Key.E) || gamepadState.Gamepad.Buttons.HasFlag(GamepadButtonFlags.RightShoulder));

                _engine.Input.KeyStateChangedHard(SiPlayerKey.PrimaryFire, keyboardState.IsPressed(Key.Space) || rightTrigger > 10);
                _engine.Input.KeyStateChangedHard(SiPlayerKey.SecondaryFire, keyboardState.IsPressed(Key.RightControl) || leftTrigger > 10);

                _engine.Input.KeyStateChangedHard(SiPlayerKey.Left, keyboardState.IsPressed(Key.Left) || gamepadState.Gamepad.Buttons.HasFlag(GamepadButtonFlags.DPadLeft) || rightThumbX < 0);
                _engine.Input.KeyStateChangedHard(SiPlayerKey.Right, keyboardState.IsPressed(Key.Right) || gamepadState.Gamepad.Buttons.HasFlag(GamepadButtonFlags.DPadRight) || rightThumbX > 0);
                _engine.Input.KeyStateChangedHard(SiPlayerKey.Up, keyboardState.IsPressed(Key.Up) || gamepadState.Gamepad.Buttons.HasFlag(GamepadButtonFlags.DPadUp) || leftThumbY > 0);
                _engine.Input.KeyStateChangedHard(SiPlayerKey.Down, keyboardState.IsPressed(Key.Down) || gamepadState.Gamepad.Buttons.HasFlag(GamepadButtonFlags.DPadDown) || leftThumbY < 0);

                _engine.Input.KeyStateChangedHard(SiPlayerKey.Enter, keyboardState.IsPressed(Key.Return) || gamepadState.Gamepad.Buttons.HasFlag(GamepadButtonFlags.A));
                _engine.Input.KeyStateChangedHard(SiPlayerKey.Escape, keyboardState.IsPressed(Key.Escape) || gamepadState.Gamepad.Buttons.HasFlag(GamepadButtonFlags.B));
            }
            else
            {
                _engine.Input.KeyStateChangedHard(SiPlayerKey.StrafeLeft, keyboardState.IsPressed(Key.Left));
                _engine.Input.KeyStateChangedHard(SiPlayerKey.StrafeRight, keyboardState.IsPressed(Key.Right));

                _engine.Input.KeyStateChangedHard(SiPlayerKey.Forward, keyboardState.IsPressed(Key.W));
                _engine.Input.KeyStateChangedHard(SiPlayerKey.Reverse, keyboardState.IsPressed(Key.S));

                _engine.Input.KeyStateChangedHard(SiPlayerKey.RotateCounterClockwise, keyboardState.IsPressed(Key.A));
                _engine.Input.KeyStateChangedHard(SiPlayerKey.RotateClockwise, keyboardState.IsPressed(Key.D));

                _engine.Input.KeyStateChangedHard(SiPlayerKey.SpeedBoost, keyboardState.IsPressed(Key.LeftShift));

                _engine.Input.KeyStateChangedHard(SiPlayerKey.SwitchWeaponLeft, keyboardState.IsPressed(Key.Q));
                _engine.Input.KeyStateChangedHard(SiPlayerKey.SwitchWeaponRight, keyboardState.IsPressed(Key.E));

                _engine.Input.KeyStateChangedHard(SiPlayerKey.PrimaryFire, keyboardState.IsPressed(Key.Space));
                _engine.Input.KeyStateChangedHard(SiPlayerKey.SecondaryFire, keyboardState.IsPressed(Key.RightControl));

                _engine.Input.KeyStateChangedHard(SiPlayerKey.Left, keyboardState.IsPressed(Key.Left));
                _engine.Input.KeyStateChangedHard(SiPlayerKey.Right, keyboardState.IsPressed(Key.Right));
                _engine.Input.KeyStateChangedHard(SiPlayerKey.Up, keyboardState.IsPressed(Key.Up));
                _engine.Input.KeyStateChangedHard(SiPlayerKey.Down, keyboardState.IsPressed(Key.Down));

                _engine.Input.KeyStateChangedHard(SiPlayerKey.Enter, keyboardState.IsPressed(Key.Return));
                _engine.Input.KeyStateChangedHard(SiPlayerKey.Escape, keyboardState.IsPressed(Key.Escape));
            }

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
                bool shiftKeyDown = _engine.Input.DepressedKeys.Contains(Key.LeftShift) || _engine.Input.DepressedKeys.Contains(Key.RightShift);
                if (shiftKeyDown)
                {
                    shouldBeCaps = !shouldBeCaps;
                }

                TypedString = string.Empty;

                foreach (var key in _engine.Input.CycledKeys)
                {
                    if (key == Key.Space)
                    {
                        TypedString = " ";
                    }
                    else if (!_engine.Input.IsModifierKey(key))
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

        /// <summary>
        /// Returns the percentage of a key that is pressed. This is for gamepad analog and triggers.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public float GetAnalogValue(SiPlayerKey key)
        {
            if (_playerKeyStates.ContainsKey(key))
            {
                return _playerKeyStates[key];
            }

            return 0;
        }

        /// <summary>
        /// Returns the percentage of a key that is pressed and its opposite (e.g. left/right, forward/reverse). This is for gamepad analog and triggers.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public float GetAnalogAxisValue(SiPlayerKey negativeAxisKey, SiPlayerKey positiveAxisKey)
        {
            _playerKeyStates.TryGetValue(positiveAxisKey, out var value1);
            if (value1 != 0)
            {
                return value1;
            }
            _playerKeyStates.TryGetValue(negativeAxisKey, out var value2);
            return value2 < 0 ? value2 : -value2;
        }

        /// <summary>
        /// Returns true or false depending on whether the applied key amount is zero or non-zero.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>

        public bool IsKeyPressed(SiPlayerKey key)
        {
            if (_playerKeyStates.ContainsKey(key))
            {
                return (_playerKeyStates[key] != 0);
            }

            return false;
        }

        /// <summary>
        /// Allows the containing window to tell the engine about key press events.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="state"></param>
        public void KeyStateChangedAmount(SiPlayerKey key, float amount)
        {
            if (_playerKeyStates.ContainsKey(key))
            {
                _playerKeyStates[key] = amount;
            }
            else
            {
                _playerKeyStates.Add(key, amount);
            }
        }

        /// <summary>
        /// Allows the containing window to tell the engine about key press events.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="state"></param>
        public void KeyStateChangedHard(SiPlayerKey key, bool state)
        {
            if (_playerKeyStates.ContainsKey(key))
            {
                _playerKeyStates[key] = state ? 1 : 0;
            }
            else
            {
                _playerKeyStates.Add(key, state ? 1 : 0);
            }
        }

        public void HandleSingleKeyPress(Keys key)
        {
            if (key == Keys.Oem3)
            {
                _engine.Debug.ToggleVisibility();
            }

            else if (key == Keys.P)
            {
                _engine.TogglePause();
            }
            else if (key == Keys.F1)
            {
                if (_engine.Sprites.OfType<SpriteEnemyBase>().Count > 0)
                {
                    _engine.Sprites.OfType<SpriteEnemyBase>()[0].Explode();
                }
            }
            else if (key == Keys.F4)
            {
                _engine.Rendering.AddScreenShake(4, 100);
            }
            else if (key == Keys.F2)
            {
                /*
                var bitmaps = _engine.Rendering.GenerateIrregularFragments(_engine.Player.Sprite.GetImage(), 10, 3);
                foreach (var bitmap in bitmaps)
                {
                    var frag = _engine.Sprites.Debugs.CreateAtCenterScreen();
                    frag.X += SiRandom.Between(-200, 200);
                    frag.Y += SiRandom.Between(-200, 200);

                    frag.SetImage(bitmap);
                    frag.Visable = true;

                    //bitmaps.
                }
                */

                //_engine.Sprites.GenericSprites.FragmentBlastOf(_engine.Player.Sprite);
                _engine.Sprites.Particles.ParticleBlastAt(100, _engine.Player.Sprite);
                //_engine.Sprites.NewGame();
                //_engine.Sprites.ResetAndShowPlayer();
            }
            else if (key == Keys.F3)
            {
                _engine.Sprites.Particles.ParticleCloud(500, _engine.Player.Sprite);
            }
        }
    }
}
