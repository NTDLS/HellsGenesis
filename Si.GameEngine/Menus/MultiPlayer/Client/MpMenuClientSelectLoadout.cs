using Si.GameEngine.Engine;
using Si.GameEngine.Menus.BasesAndInterfaces;
using Si.GameEngine.Sprites.MenuItems;
using Si.GameEngine.Sprites.Player.BasesAndInterfaces;
using Si.Menus.MultiPlayer.Client;
using Si.Shared;
using Si.Shared.Types.Geometry;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using static Si.Shared.SiConstants;

namespace Si.Menus.SinglePlayer
{
    /// <summary>
    /// The menu that is displayed at game start to allow the player to select a loadout.
    /// </summary>
    internal class MpMenuClientSelectLoadout : MenuBase
    {
        private readonly SpriteMenuItem _shipBlurb;
        private Timer _animationTimer;
        private SpritePlayerBase _selectedSprite;

        public MpMenuClientSelectLoadout(EngineCore gameCore)
            : base(gameCore)
        {
            _gameCore.Multiplay.SetPlayMode(SiPlayMode.MutiPlayerClient);

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
            var playerTypes = SiReflection.GetSubClassesOf<SpritePlayerBase>()
                .Where(o => o.Name.EndsWith("Drone") == false)
                .OrderBy(o => o.Name).ToList();

            //Move the debug player to the top of the list.
            var debugPlayer = playerTypes.Where(o => o.Name.Contains("Debug")).FirstOrDefault();
            if (debugPlayer != null)
            {
                playerTypes.Remove(debugPlayer);
                playerTypes.Insert(0, debugPlayer);
            }

            List<SpritePlayerBase> playerSprites = new();

            foreach (var playerType in playerTypes)
            {
                var playerSprite = SiReflection.CreateInstanceFromType<SpritePlayerBase>(playerType, new object[] { gameCore });
                playerSprite.SpriteTag = "MENU_SHIP_SELECT";
                playerSprite.Velocity.Angle.Degrees = 45;

                var menuItem = CreateAndAddSelectableItem(new SiPoint(offsetX + 25, offsetY), playerSprite.Loadout.Name, playerSprite.Loadout.Name);
                menuItem.Y -= menuItem.Size.Height / 2;

                menuItem.UserData = playerSprite;

                playerSprites.Add(playerSprite);

                playerSprite.X = offsetX;
                playerSprite.Y = offsetY;
                offsetY += 50;
            }

            playerSprites.ForEach(sprite =>
            {
                _gameCore.Sprites.AddPlayer(sprite);
                sprite.ThrustAnimation.Visable = true;
            });

            OnSelectionChanged += PlayerLoadoutMenu_OnSelectionChanged;
            OnExecuteSelection += PlayerLoadoutMenu_OnExecuteSelection;
            OnCleanup += PlayerLoadoutMenu_OnCleanup;
            OnEscape += MpMenuClientSelectLoadout_OnEscape;

            VisibleSelectableItems().First().Selected = true;

            _animationTimer = new Timer(PlayerLoadoutMenu_Tick, null, 10, 10);
        }

        private bool MpMenuClientSelectLoadout_OnEscape()
        {
            _gameCore.Menus.Add(new MpMenuClientJoinLobby(_gameCore));
            return true;
        }

        private void PlayerLoadoutMenu_Tick(object sender)
        {
            _selectedSprite?.Rotate(1);
        }

        private void PlayerLoadoutMenu_OnCleanup()
        {
            _gameCore.Sprites.DeleteAllSpritesByTag("MENU_SHIP_SELECT");
        }

        private bool PlayerLoadoutMenu_OnExecuteSelection(SpriteMenuItem item)
        {
            _animationTimer.Change(Timeout.Infinite, Timeout.Infinite);
            _animationTimer.Dispose();

            if (item.UserData is SpritePlayerBase selectedSprite)
            {
                _gameCore.Player.InstantiatePlayerClass(selectedSprite.GetType());
            }

            _gameCore.Menus.Add(new MpMenuClientLobbyWait(_gameCore));
            return true;
        }

        private void PlayerLoadoutMenu_OnSelectionChanged(SpriteMenuItem item)
        {
            if (item.UserData is SpritePlayerBase selectedSprite)
            {
                _shipBlurb.Text = selectedSprite.GetLoadoutHelpText();
                _selectedSprite = selectedSprite;
            }
        }
    }
}
