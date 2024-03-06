using Si.Engine.Menus._Superclass;
using Si.Engine.Sprites.MenuItems;
using Si.Library.Mathematics.Geometry;

namespace Si.Engine.Menus
{
    /// <summary>
    /// The menu that is shows when the game is first started.
    /// </summary>
    internal class MenuStartNewGame : MenuBase
    {
        public MenuStartNewGame(EngineCore engine)
            : base(engine)
        {
            var currentScaledScreenBounds = _engine.Display.GetCurrentScaledScreenBounds();

            float offsetX = _engine.Display.TotalCanvasSize.Width / 2;
            float offsetY = currentScaledScreenBounds.Y + 100;

            var itemTitle = CreateAndAddTitleItem(new SiPoint(offsetX, offsetY), "Strikeforce Infinite");
            itemTitle.X -= itemTitle.Size.Width / 2;
            offsetY += itemTitle.Size.Height + 60;
            itemTitle.IsHighlighted = true;

            var helpItem = CreateAndAddSelectableItem(new SiPoint(offsetX, offsetY), "START", " - Start - ");
            helpItem.Selected = true;
            helpItem.X -= helpItem.Size.Width / 2;
            offsetY += helpItem.Size.Height + 5;

            offsetY += 50;

            helpItem = CreateAndAddTextblock(new SiPoint(offsetX, offsetY), "Forward and Rotate with <W>, <A> and <S>.");
            helpItem.X -= helpItem.Size.Width / 2;
            offsetY += helpItem.Size.Height + 5;

            helpItem = CreateAndAddTextblock(new SiPoint(offsetX, offsetY), "Strafe with <LEFT> and <RIGHT> arrows.");
            helpItem.X -= helpItem.Size.Width / 2;
            offsetY += helpItem.Size.Height + 5;

            helpItem = CreateAndAddTextblock(new SiPoint(offsetX, offsetY), "Surge Drive with <SHIFT>.");
            helpItem.X -= helpItem.Size.Width / 2;
            offsetY += helpItem.Size.Height + 5;

            helpItem = CreateAndAddTextblock(new SiPoint(offsetX, offsetY), "Fire primary with <SPACE>.");
            helpItem.X -= helpItem.Size.Width / 2;
            offsetY += helpItem.Size.Height + 10;

            helpItem = CreateAndAddTextblock(new SiPoint(offsetX, offsetY), "Fire secondary with <CTRL>.");
            helpItem.X -= helpItem.Size.Width / 2;
            offsetY += helpItem.Size.Height + 10;

            helpItem = CreateAndAddTextblock(new SiPoint(offsetX, offsetY), "Change weapons with <Q> and <E>.");
            helpItem.X -= helpItem.Size.Width / 2;
            offsetY += helpItem.Size.Height + 10;

            //helpItem = CreateAndAddTextblock(new SiPoint(offsetX, offsetY), "Change speed with <UP> and <DOWN> arrows.");
            //helpItem.X -= helpItem.Size.Width / 2;
            //offsetY += helpItem.Size.Height + 10;

            //itemYes.Selected = true;

            OnExecuteSelection += MenuStartNewGame_OnExecuteSelection;
        }

        private bool MenuStartNewGame_OnExecuteSelection(SpriteMenuItem item)
        {
            _engine.Menus.Show(new MenuSituationSelect(_engine));
            return true;
        }
    }
}
