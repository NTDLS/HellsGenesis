using Si.GameEngine.Menus._Superclass;
using Si.GameEngine.Sprites.MenuItems;
using Si.GameEngine.Sprites.Player._Superclass;
using Si.Library;
using Si.Library.Mathematics.Geometry;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Si.Menus.SinglePlayer
{
    /// <summary>
    /// The menu that is displayed at game start to allow the player to select a loadout.
    /// </summary>
    internal class SpMenuSelectLoadout : MenuBase
    {
        private readonly SpriteMenuItem _shipBlurb;
        private Timer _animationTimer;
        private SpritePlayerBase _selectedSprite;

        public SpMenuSelectLoadout(GameEngine.GameEngineCore gameEngine)
            : base(gameEngine)
        {
            var currentScaledScreenBounds = _gameEngine.Display.GetCurrentScaledScreenBounds();

            float offsetX = currentScaledScreenBounds.X + 40;
            float offsetY = currentScaledScreenBounds.Y + 100;

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
                var playerSprite = SiReflection.CreateInstanceFromType<SpritePlayerBase>(playerType, new object[] { gameEngine });
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
                _gameEngine.Sprites.AddPlayer(sprite);
                sprite.ThrustAnimation.Visable = true;
            });

            OnSelectionChanged += PlayerLoadoutMenu_OnSelectionChanged;
            OnExecuteSelection += PlayerLoadoutMenu_OnExecuteSelection;
            OnCleanup += PlayerLoadoutMenu_OnCleanup;
            OnEscape += SpMenuSelectLoadout_OnEscape;

            VisibleSelectableItems().First().Selected = true;

            _animationTimer = new Timer(PlayerLoadoutMenu_Tick, null, 10, 10);
        }

        private bool SpMenuSelectLoadout_OnEscape()
        {
            _gameEngine.Menus.Show(new SpMenuSituationSelect(_gameEngine));
            return true;
        }

        private void PlayerLoadoutMenu_OnCleanup()
        {
            _gameEngine.Sprites.DeleteAllSpritesByTag("MENU_SHIP_SELECT");
        }

        private bool PlayerLoadoutMenu_OnExecuteSelection(SpriteMenuItem item)
        {
            _animationTimer.Change(Timeout.Infinite, Timeout.Infinite);
            _animationTimer.Dispose();

            if (item.UserData is SpritePlayerBase selectedSprite)
            {
                _gameEngine.Player.InstantiatePlayerClass(selectedSprite.GetType());
                _gameEngine.StartGame();
            }

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

        private void PlayerLoadoutMenu_Tick(object sender)
        {
            _selectedSprite?.Rotate(1);
        }
    }
}
