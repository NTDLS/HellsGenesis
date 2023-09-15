using HG.Actors.Ordinary;
using HG.Engine;
using HG.Engine.Types.Geometry;
using HG.Menus.BaseClasses;

namespace HG.Menus
{
    internal class MenuStartNewGame : MenuBase
    {
        public MenuStartNewGame(EngineCore core)
            : base(core)
        {
            double offsetX = _core.Display.TotalCanvasSize.Width / 2;
            double offsetY = _core.Display.CurrentScaledScreenBounds.Y + 100;

            var itemTitle = CreateAndAddTitleItem(new HgPoint(offsetX, offsetY), "Ready to start?");
            itemTitle.X -= itemTitle.Size.Width / 2;
            offsetY += itemTitle.Size.Height + 10;

            var itemYes = CreateAndAddMenuItem(new HgPoint(offsetX, offsetY), "YES", "Yes");
            var itemNo = CreateAndAddMenuItem(new HgPoint(offsetX + itemYes.Size.Width + 10, offsetY), "NO", "No");
            offsetY += itemNo.Size.Height + 20;
            itemYes.X -= (itemYes.Size.Width + itemNo.Size.Width) / 2;
            itemNo.X -= (itemYes.Size.Width + itemNo.Size.Width) / 2;

            var helpItem = CreateAndAddTextItem(new HgPoint(offsetX, offsetY), "Move with <W>, <A>, <S>.");
            helpItem.X -= helpItem.Size.Width / 2;
            offsetY += itemTitle.Size.Height + 5;

            helpItem = CreateAndAddTextItem(new HgPoint(offsetX, offsetY), "Warp Drive with <SHIFT>.");
            helpItem.X -= helpItem.Size.Width / 2;
            offsetY += itemTitle.Size.Height + 5;

            helpItem = CreateAndAddTextItem(new HgPoint(offsetX, offsetY), "Fire primary with <SPACE>.");
            helpItem.X -= helpItem.Size.Width / 2;
            offsetY += itemTitle.Size.Height + 10;

            helpItem = CreateAndAddTextItem(new HgPoint(offsetX, offsetY), "Fire secondary with <CTRL>.");
            helpItem.X -= helpItem.Size.Width / 2;
            offsetY += itemTitle.Size.Height + 10;

            helpItem = CreateAndAddTextItem(new HgPoint(offsetX, offsetY), "Change weapons with <left> and <right> arrows.");
            helpItem.X -= helpItem.Size.Width / 2;
            offsetY += itemTitle.Size.Height + 10;

            helpItem = CreateAndAddTextItem(new HgPoint(offsetX, offsetY), "Change speed with <up> and <down> arrows.");
            helpItem.X -= helpItem.Size.Width / 2;
            offsetY += itemTitle.Size.Height + 10;

            itemYes.Selected = true;
        }

        public override void ExecuteSelection(ActorMenuItem item)
        {
            if (item.Key == "NO")
            {
                _core.Stop();
            }
            else if (item.Key == "YES")
            {
                _core.Menus.Insert(new PlayerShipMenu(_core));
            }
        }
    }
}
