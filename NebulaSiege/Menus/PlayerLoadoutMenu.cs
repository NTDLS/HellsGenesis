using NebulaSiege.Engine;
using NebulaSiege.Engine.Types.Geometry;
using NebulaSiege.Menus.BaseClasses;
using NebulaSiege.Sprites;
using NebulaSiege.Sprites.Player.BaseClasses;
using NebulaSiege.Utility;
using System.Linq;
using System.Threading;

namespace NebulaSiege.Menus
{
    /// <summary>
    /// The menu that is displayed at game start to allow the player to select a loadout.
    /// </summary>
    internal class PlayerLoadoutMenu : MenuBase
    {
        private readonly SpriteMenuItem _shipBlurb;
        private Timer _animationTimer;
        private SpritePlayerBase _selectedSprite;

        public PlayerLoadoutMenu(EngineCore core)
            : base(core)
        {
            var currentScaledScreenBounds = _core.Display.GetCurrentScaledScreenBounds();

            double offsetX = currentScaledScreenBounds.X + 40;
            double offsetY = currentScaledScreenBounds.Y + 100;

            var itemTitle = CreateAndAddTitleItem(new NsPoint(offsetX, offsetY), "Select a Ship Class");
            itemTitle.X = offsetX + 200;
            itemTitle.Y = offsetY - itemTitle.Size.Height;

            offsetY += itemTitle.Height;

            _shipBlurb = CreateAndAddTextItem(new NsPoint(offsetX, offsetY), "");
            _shipBlurb.X = offsetX + 200;
            _shipBlurb.Y = offsetY - _shipBlurb.Size.Height;

            //Use reflection to get a list of possible player types.
            var playerTypes = NsReflection.GetSubClassesOf<SpritePlayerBase>().OrderBy(o => o.Name).ToList();

            //Move the debug player to the top of the list.
            var debugPlayer = playerTypes.Where(o => o.Name.Contains("Debug")).FirstOrDefault();
            if (debugPlayer != null)
            {
                playerTypes.Remove(debugPlayer);
                playerTypes.Insert(0, debugPlayer);
            }

            foreach (var playerType in playerTypes)
            {
                var playerTypeInstance = NsReflection.CreateInstanceFromType<SpritePlayerBase>(playerType, new object[] { core });
                playerTypeInstance.SpriteTag = "MENU_SHIP_SELECT";
                playerTypeInstance.Velocity.Angle.Degrees = 45;
                playerTypeInstance.ThrustAnimation.Visable = true;
                playerTypeInstance.BoostAnimation.Visable = true;

                var menuItem = CreateAndAddMenuItem(new NsPoint(offsetX + 25, offsetY), playerTypeInstance.Loadout.Name, playerTypeInstance.Loadout.Name);
                menuItem.Y -= menuItem.Size.Height / 2;

                menuItem.UserData = playerTypeInstance;

                var shipIcon = _core.Sprites.InsertPlayer(playerTypeInstance);

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

            SelectableItems().First().Selected = true;

            _animationTimer = new Timer(PlayerLoadoutMenu_Tick, null, 10, 10);
        }

        private void PlayerLoadoutMenu_OnCleanup()
        {
            _core.Sprites.DeleteAllSpritesByTag("MENU_SHIP_SELECT");
        }

        private void PlayerLoadoutMenu_OnExecuteSelection(SpriteMenuItem item)
        {
            _animationTimer.Change(Timeout.Infinite, Timeout.Infinite);
            _animationTimer.Dispose();

            if (item.UserData is SpritePlayerBase selectedSprite)
            {
                _core.Player.Sprite.ResetLoadout(selectedSprite.Loadout);
                _core.StartGame();
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
