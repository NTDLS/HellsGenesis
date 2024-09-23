using Si.Engine.Menu._Superclass;
using Si.Engine.Sprite.MenuItem;
using Si.Engine.Sprite.Player._Superclass;
using Si.Library;
using Si.Library.Mathematics;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Si.Engine.Menu
{
    /// <summary>
    /// The menu that is displayed at game start to allow the player to select a loadout.
    /// </summary>
    internal class MenuSelectLoadout : MenuBase
    {
        private readonly SpriteMenuItem _shipBlurb;
        private Timer _animationTimer;
        private SpritePlayerBase _selectedSprite;

        public MenuSelectLoadout(EngineCore engine)
            : base(engine)
        {
            var currentScaledScreenBounds = _engine.Display.GetCurrentScaledScreenBounds();

            float offsetX = currentScaledScreenBounds.X + 40;
            float offsetY = currentScaledScreenBounds.Y + 100;

            var itemTitle = AddTitleItem(new SiVector(offsetX, offsetY), "Select a Ship Class");
            itemTitle.X = offsetX + 200;
            itemTitle.Y = offsetY - itemTitle.Size.Height;

            offsetY += itemTitle.Height;

            _shipBlurb = AddTextBlock(new SiVector(offsetX, offsetY), "");
            _shipBlurb.X = offsetX + 250;
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

            float previousSpriteSize = 0;

            foreach (var playerType in playerTypes)
            {
                var playerSprite = SiReflection.CreateInstanceFromType<SpritePlayerBase>(playerType, new object[] { engine });
                playerSprite.SpriteTag = "MENU_SHIP_SELECT";
                playerSprite.Orientation.Degrees = 45;

                offsetY += playerSprite.Size.Height / 2.0f + previousSpriteSize / 2.0f + 25;
                previousSpriteSize = playerSprite.Size.Height;

                var menuItem = AddSelectableItem(new SiVector(offsetX + 75, offsetY), playerSprite.Metadata.Name, playerSprite.Metadata.Name);
                menuItem.Y -= menuItem.Size.Height / 2;

                menuItem.UserData = playerSprite;

                playerSprites.Add(playerSprite);

                playerSprite.X = offsetX;
                playerSprite.Y = offsetY;
            }

            playerSprites.ForEach(sprite =>
            {
                _engine.Sprites.AddPlayer(sprite);
                sprite.ThrusterAnimation.Visible = true;
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
            _engine.Menus.Show(new MenuSituationSelect(_engine));
            return true;
        }

        private void PlayerLoadoutMenu_OnCleanup()
        {
            _engine.Sprites.QueueAllForDeletionByTag("MENU_SHIP_SELECT");
        }

        private bool PlayerLoadoutMenu_OnExecuteSelection(SpriteMenuItem item)
        {
            _animationTimer.Change(Timeout.Infinite, Timeout.Infinite);
            _animationTimer.Dispose();

            if (item.UserData is SpritePlayerBase selectedSprite)
            {
                _engine.Player.InstantiatePlayerClass(selectedSprite.GetType());
                _engine.StartGame();
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
            _selectedSprite?.RotatePointingDirection(0.01f);
        }
    }
}
