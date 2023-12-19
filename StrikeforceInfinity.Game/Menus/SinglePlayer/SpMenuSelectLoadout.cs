using StrikeforceInfinity.Game.Engine;
using StrikeforceInfinity.Game.Engine.Types.Geometry;
using StrikeforceInfinity.Game.Menus.BaseClasses;
using StrikeforceInfinity.Game.Sprites.MenuItems;
using StrikeforceInfinity.Game.Sprites.Player.BaseClasses;
using StrikeforceInfinity.Game.Utility;
using System.Linq;
using System.Threading;

namespace StrikeforceInfinity.Menus.SinglePlayer
{
    /// <summary>
    /// The menu that is displayed at game start to allow the player to select a loadout.
    /// </summary>
    internal class SpMenuSelectLoadout : MenuBase
    {
        private readonly SpriteMenuItem _shipBlurb;
        private Timer _animationTimer;
        private SpritePlayerBase _selectedSprite;

        public SpMenuSelectLoadout(EngineCore gameCore)
            : base(gameCore)
        {
            var currentScaledScreenBounds = _gameCore.Display.GetCurrentScaledScreenBounds();

            double offsetX = currentScaledScreenBounds.X + 40;
            double offsetY = currentScaledScreenBounds.Y + 100;

            var itemTitle = CreateAndAddTitleItem(new SiPoint(offsetX, offsetY), "Select a Ship Class");
            itemTitle.X = offsetX + 200;
            itemTitle.Y = offsetY - itemTitle.Size.Height;

            offsetY += itemTitle.Height;

            _shipBlurb = CreateAndAddTextblock(new SiPoint(offsetX, offsetY), "");
            _shipBlurb.X = offsetX + 200;
            _shipBlurb.Y = offsetY - _shipBlurb.Size.Height;

            //Use reflection to get a list of possible player types.
            var playerTypes = SiReflection.GetSubClassesOf<SpritePlayerBase>().OrderBy(o => o.Name).ToList();

            //Move the debug player to the top of the list.
            var debugPlayer = playerTypes.Where(o => o.Name.Contains("Debug")).FirstOrDefault();
            if (debugPlayer != null)
            {
                playerTypes.Remove(debugPlayer);
                playerTypes.Insert(0, debugPlayer);
            }

            foreach (var playerType in playerTypes)
            {
                var playerTypeInstance = SiReflection.CreateInstanceFromType<SpritePlayerBase>(playerType, new object[] { gameCore });
                playerTypeInstance.SpriteTag = "MENU_SHIP_SELECT";
                playerTypeInstance.Velocity.Angle.Degrees = 45;
                playerTypeInstance.ThrustAnimation.Visable = true;
                playerTypeInstance.BoostAnimation.Visable = true;

                var menuItem = CreateAndAddSelectableItem(new SiPoint(offsetX + 25, offsetY), playerTypeInstance.Loadout.Name, playerTypeInstance.Loadout.Name);
                menuItem.Y -= menuItem.Size.Height / 2;

                menuItem.UserData = playerTypeInstance;

                var shipIcon = _gameCore.Sprites.InsertPlayer(playerTypeInstance);

                if (playerTypeInstance.Loadout.Name == "Debug")
                {
                    shipIcon.ThrustAnimation.Visable = true;
                }
                else
                {
                    shipIcon.BoostAnimation.Visable = true;
                }
                shipIcon.X = offsetX;
                shipIcon.Y = offsetY;
                offsetY += 50;
            }

            OnSelectionChanged += PlayerLoadoutMenu_OnSelectionChanged;
            OnExecuteSelection += PlayerLoadoutMenu_OnExecuteSelection;
            OnCleanup += PlayerLoadoutMenu_OnCleanup;

            VisibleSelectableItems().First().Selected = true;

            _animationTimer = new Timer(PlayerLoadoutMenu_Tick, null, 10, 10);
        }

        private void PlayerLoadoutMenu_OnCleanup()
        {
            _gameCore.Sprites.DeleteAllSpritesByTag("MENU_SHIP_SELECT");
        }

        private void PlayerLoadoutMenu_OnExecuteSelection(SpriteMenuItem item)
        {
            _animationTimer.Change(Timeout.Infinite, Timeout.Infinite);
            _animationTimer.Dispose();

            if (item.UserData is SpritePlayerBase selectedSprite)
            {
                _gameCore.Player.Sprite.ResetLoadout(selectedSprite.Loadout);
                _gameCore.StartGame();
            }
        }

        private void PlayerLoadoutMenu_OnSelectionChanged(SpriteMenuItem item)
        {
            if (item.UserData is SpritePlayerBase selectedSprite)
            {
                _shipBlurb.Text = selectedSprite.GetLoadoutHelpText();
                _selectedSprite = selectedSprite;
            }
        }

        private void PlayerLoadoutMenu_Tick(object sender)
        {
            _selectedSprite?.Rotate(1);
        }
    }
}
