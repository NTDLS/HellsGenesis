using NebulaSiege.Engine;
using NebulaSiege.Engine.Types.Geometry;
using NebulaSiege.Menus.BaseClasses;
using NebulaSiege.Menus.MenuItems;

namespace NebulaSiege.Menus
{
    /// <summary>
    /// Allows user to set parameters for a hosted game.
    /// </summary>
    internal class MenuMultiplayerHostGame : MenuBase
    {
        public MenuMultiplayerHostGame(EngineCore core)
            : base(core)
        {
            var currentScaledScreenBounds = _core.Display.GetCurrentScaledScreenBounds();

            double offsetX = _core.Display.TotalCanvasSize.Width / 2;
            double offsetY = currentScaledScreenBounds.Y + 100;

            var itemTitle = CreateAndAddTitleItem(new NsPoint(offsetX, offsetY), "Nebula Siege");
            itemTitle.X -= itemTitle.Size.Width / 2;
            offsetY += itemTitle.Size.Height + 60;
            itemTitle.Highlight = true;

            var helpItem = CreateAndAddTextblock(new NsPoint(offsetX, offsetY), "Name");
            helpItem.X -= helpItem.Size.Width / 2;
            offsetY += helpItem.Size.Height + 5;

            helpItem = CreateAndAddSelectableTextInput(new NsPoint(offsetX, offsetY), "Name", "My Game Name");
            helpItem.Selected = true;
            helpItem.X -= helpItem.Size.Width / 2;
            offsetY += helpItem.Size.Height + 5;

            OnExecuteSelection += MenuMultiplayerHostOrJoin_OnExecuteSelection;
        }

        private void MenuMultiplayerHostOrJoin_OnExecuteSelection(SpriteMenuItem item)
        {
            /*
            if (item.Key == "JOIN")
            {
                //_core.Menus.Insert(new SituationSelectMenu(_core));
            }
            else if (item.Key == "HOST")
            {
                //_core.Menus.Insert(new SituationSelectMenu(_core));
            }
            */
        }
    }
}
