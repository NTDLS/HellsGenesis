using NebulaSiege.Engine;
using NebulaSiege.Engine.Types.Geometry;
using NebulaSiege.Sprites;
using NebulaSiege.Sprites.Player;
using NebulaSiege.Utility;
using System.Linq;
using System.Xml.Linq;

namespace NebulaSiege.Menus
{
    /// <summary>
    /// The menu that is displayed at game start to allow the player to select a loadout.
    /// </summary>
    internal class PlayerLoadoutMenu : _MenuBase
    {
        private readonly SpriteMenuItem _shipBlurb;

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

            var playerTypes = NsReflection.GetSubClassesOf<_SpritePlayerBase>();

            foreach (var playerType in playerTypes)
            {
                var playerTypeInstance = NsReflection.CreateInstanceFromType<_SpritePlayerBase>(playerType, new object[] { core });
                playerTypeInstance.SpriteTag = "MENU_SHIP_SELECT";
                playerTypeInstance.Velocity.Angle.Degrees = 45;
                playerTypeInstance.ThrustAnimation.Visable = true;
                playerTypeInstance.BoostAnimation.Visable = true;

                var menuItem = CreateAndAddMenuItem(new NsPoint(offsetX + 25, offsetY), playerTypeInstance.Loadout.Name, playerTypeInstance.Loadout.Name);
                menuItem.Y -= menuItem.Size.Height / 2;

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

            SelectableItems().First().Selected = true;
        }

        public override void SelectionChanged(SpriteMenuItem item)
        {
            var selectedSprite = _core.Sprites.Collection.OfType<_SpritePlayerBase>()
                .Where(o => o.SpriteTag == "MENU_SHIP_SELECT" && o.ShipClass.ToString() == item.Key).First();

            _shipBlurb.Text = selectedSprite.GetLoadoutHelpText();
        }

        public override void ExecuteSelection(SpriteMenuItem item)
        {
            var selectedSprite = _core.Sprites.Collection.OfType<_SpritePlayerBase>()
                .Where(o => o.SpriteTag == "MENU_SHIP_SELECT" && o.ShipClass.ToString() == item.Key).First();

            _core.Player.Sprite.ResetLoadout(selectedSprite.Loadout);
            _core.Sprites.DeleteAllSpriteByAssetTag("MENU_SHIP_SELECT");
            _core.Sprites.NewGame();
        }
    }
}
