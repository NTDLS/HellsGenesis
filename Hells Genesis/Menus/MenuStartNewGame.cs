using HG.Actors.Ordinary;
using HG.Engine;
using HG.Menus.BaseClasses;
using HG.Types.Geometry;

namespace HG.Menus
{
    internal class MenuStartNewGame : MenuBase
    {
        public MenuStartNewGame(Core core)
            : base(core)
        {
            double baseX = _core.Display.TotalCanvasSize.Width / 2;
            double baseY = _core.Display.TotalCanvasSize.Height / 4 + 100;

            var itemTitle = CreateAndAddTitleItem(new HgPoint(baseX, baseY), "Start new game?");
            itemTitle.X -= itemTitle.Size.Width / 2;
            baseY += itemTitle.Size.Height + 10;

            var itemYes = CreateAndAddMenuItem(new HgPoint(baseX, baseY), "YES", "Yes");
            var itemNo = CreateAndAddMenuItem(new HgPoint(baseX + itemYes.Size.Width + 10, baseY), "NO", "No");
            baseY += itemNo.Size.Height + 20;
            itemYes.X -= (itemYes.Size.Width + itemNo.Size.Width) / 2;
            itemNo.X -= (itemYes.Size.Width + itemNo.Size.Width) / 2;

            var helpItem = CreateAndAddTextItem(new HgPoint(baseX, baseY), "Move with <W>, <A>, <S>.");
            helpItem.X -= helpItem.Size.Width / 2;
            baseY += itemTitle.Size.Height + 5;

            helpItem = CreateAndAddTextItem(new HgPoint(baseX, baseY), "Warp Drive with <SHIFT>.");
            helpItem.X -= helpItem.Size.Width / 2;
            baseY += itemTitle.Size.Height + 5;

            helpItem = CreateAndAddTextItem(new HgPoint(baseX, baseY), "Fire primary with <SPACE>.");
            helpItem.X -= helpItem.Size.Width / 2;
            baseY += itemTitle.Size.Height + 10;

            helpItem = CreateAndAddTextItem(new HgPoint(baseX, baseY), "Fire secondary with <CTRL>.");
            helpItem.X -= helpItem.Size.Width / 2;
            baseY += itemTitle.Size.Height + 10;

            helpItem = CreateAndAddTextItem(new HgPoint(baseX, baseY), "Change weapons with <left> and <right> arrows.");
            helpItem.X -= helpItem.Size.Width / 2;
            baseY += itemTitle.Size.Height + 10;

            helpItem = CreateAndAddTextItem(new HgPoint(baseX, baseY), "Change speed with <up> and <down> arrows.");
            helpItem.X -= helpItem.Size.Width / 2;
            baseY += itemTitle.Size.Height + 10;

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
