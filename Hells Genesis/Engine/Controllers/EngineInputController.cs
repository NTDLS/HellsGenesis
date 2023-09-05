using HG.Actors.Enemies.BaseClasses;
using HG.Actors.Ordinary;
using HG.Types;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace HG.Engine.Controllers
{
    internal class EngineInputController
    {
        private readonly Core _core;
        private readonly Dictionary<HgPlayerKey, HgKeyPressState> _keyStates = new Dictionary<HgPlayerKey, HgKeyPressState>();

        public EngineInputController(Core core)
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
                if (_core.Actors.OfType<EnemyBase>().Count > 0)
                {
                    _core.Actors.OfType<EnemyBase>()[0].Explode();
                }
            }
            else if (key == Keys.Oem3) //~
            {
                _core.ShowDebug = !_core.ShowDebug;
            }
            #endregion
            else if (key == Keys.F1)
            {
                _core.Actors.NewGame();
                //_core.Actors.ResetAndShowPlayer();
            }
            else if (key == Keys.P)
            {
                var textBlock = _core.Actors.GetActorByAssetTag<ActorTextBlock>("PausedText");
                if (textBlock == null)
                {
                    textBlock = _core.Actors.TextBlocks.Create("Consolas", Brushes.Red, 50, new HgPoint<double>(100, 100), true, "PausedText");
                    textBlock.Text = "Paused...";
                    textBlock.X = _core.Display.NatrualScreenSize.Width / 2 - textBlock.Size.Width / 2;
                    textBlock.Y = _core.Display.NatrualScreenSize.Height / 2 - textBlock.Size.Height / 2;
                }

                _core.TogglePause();
                textBlock.Visable = _core.IsPaused();
            }

            if (key == Keys.Q)
            {
                _core.Player.Actor.SelectPreviousAvailableUsableSecondaryWeapon();
            }
            else if (key == Keys.E)
            {
                _core.Player.Actor.SelectNextAvailableUsableSecondaryWeapon();
            }
        }
    }
}
