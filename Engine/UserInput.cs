using AI2D.Types;
using System.Collections.Generic;
using System.Windows.Forms;

namespace AI2D.Engine
{
    public class UserInput
    {
        private Game _game;
        private Dictionary<PlayerKey, KeyPressState> _keyStates = new Dictionary<PlayerKey, KeyPressState>();

        public UserInput(Game game)
        {
            _game = game;
        }

        public bool IsKeyPressed(PlayerKey key)
        {
            if (_keyStates.ContainsKey(key))
            {
                return (_keyStates[key] == KeyPressState.Down);
            }

            return false;
        }

        /// <summary>
        /// Allows the containing window to tell the engine about key press events.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="state"></param>
        public void KeyStateChanged(PlayerKey key, KeyPressState state, Keys? other = null)
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

        public void DebugKeyPress(Keys key)
        {
            if (key == Keys.D0)
            {
                _game.Actors.Boulders[0].X = 400;
                _game.Actors.Boulders[0].Y = 400;
            }
            if (key == Keys.D1)
            {
                _game.Actors.Player.Velocity.Angle.Degree = 0;
                _game.Actors.Player.Invalidate();
            }
            if (key == Keys.D2)
            {
                _game.Actors.Player.Velocity.Angle.Degree = 45;
                _game.Actors.Player.Invalidate();
            }
            if (key == Keys.D3)
            {
                _game.Actors.Player.Velocity.Angle.Degree = 90;
                _game.Actors.Player.Invalidate();
            }
            if (key == Keys.D4)
            {
                _game.Actors.Player.Velocity.Angle.Degree = 135;
                _game.Actors.Player.Invalidate();
            }
            if (key == Keys.D5)
            {
                _game.Actors.Player.Velocity.Angle.Degree = 180;
                _game.Actors.Player.Invalidate();
            }
            if (key == Keys.D6)
            {
                _game.Actors.Player.Velocity.Angle.Degree = 225;
                _game.Actors.Player.Invalidate();
            }
            if (key == Keys.D7)
            {
                _game.Actors.Player.Velocity.Angle.Degree = 270;
                _game.Actors.Player.Invalidate();
            }
            if (key == Keys.D8)
            {
                _game.Actors.Player.Velocity.Angle.Degree = 315;
                _game.Actors.Player.Invalidate();
            }
            if (key == Keys.D9)
            {
                _game.Actors.Boulders[0].MoveInDirectionOf(_game.Actors.Player);
            }

        }
    }
}
