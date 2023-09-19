using HG.Engine;
using HG.Engine.Types.Geometry;
using HG.Sprites;


namespace HG.Menus
{
    /// <summary>
    /// The menu that is shows when the game is first started.
    /// </summary>
    internal class MenuStartNewGame : _MenuBase
    {
        public MenuStartNewGame(EngineCore core)
            : base(core)
        {
            var currentScaledScreenBounds = _core.Display.GetCurrentScaledScreenBounds();

            double offsetX = _core.Display.TotalCanvasSize.Width / 2;
            double offsetY = currentScaledScreenBounds.Y + 100;

            var itemTitle = CreateAndAddTitleItem(new HgPoint(offsetX, offsetY), "Hells Genesis");
            itemTitle.X -= itemTitle.Size.Width / 2;
            offsetY += itemTitle.Size.Height + 60;
            itemTitle.Highlight = true;

            var helpItem = CreateAndAddMenuItem(new HgPoint(offsetX, offsetY), "ENTER", " Press -ENTER- to start ");
            helpItem.Selected = true;
            helpItem.X -= helpItem.Size.Width / 2;
            offsetY += itemTitle.Size.Height + 5;

            helpItem = CreateAndAddTextItem(new HgPoint(offsetX, offsetY), "Move with <W>, <A>, <S>.");
            helpItem.X -= helpItem.Size.Width / 2;
            offsetY += helpItem.Size.Height + 5;

            helpItem = CreateAndAddTextItem(new HgPoint(offsetX, offsetY), "Warp Drive with <SHIFT>.");
            helpItem.X -= helpItem.Size.Width / 2;
            offsetY += helpItem.Size.Height + 5;

            helpItem = CreateAndAddTextItem(new HgPoint(offsetX, offsetY), "Fire primary with <SPACE>.");
            helpItem.X -= helpItem.Size.Width / 2;
            offsetY += helpItem.Size.Height + 10;

            helpItem = CreateAndAddTextItem(new HgPoint(offsetX, offsetY), "Fire secondary with <CTRL>.");
            helpItem.X -= helpItem.Size.Width / 2;
            offsetY += helpItem.Size.Height + 10;

            helpItem = CreateAndAddTextItem(new HgPoint(offsetX, offsetY), "Change weapons with <left> and <right> arrows.");
            helpItem.X -= helpItem.Size.Width / 2;
            offsetY += helpItem.Size.Height + 10;

            helpItem = CreateAndAddTextItem(new HgPoint(offsetX, offsetY), "Change speed with <up> and <down> arrows.");
            helpItem.X -= helpItem.Size.Width / 2;
            offsetY += helpItem.Size.Height + 10;

            //itemYes.Selected = true;
        }

        public override void ExecuteSelection(SpriteMenuItem item)
        {
            _core.Menus.Insert(new PlayerLoadoutMenu(_core));
        }
    }
}
