using HG.Actors.Ordinary;
using HG.Engine;
using HG.Menus.BaseClasses;
using HG.Types;
using System.Drawing;

namespace HG.Menus
{
    internal class MenuStartNewGame : MenuBase
    {
        public MenuStartNewGame(Core core)
            : base(core)
        {
            double baseX = _core.Display.TotalCanvasSize.Width / 2;
            double baseY = _core.Display.TotalCanvasSize.Height / 4 + 100;

            var itemTitle = CreateAndAddTitleItem(new HgPoint<double>(baseX, baseY), "Start new game?", Brushes.OrangeRed);
            itemTitle.X -= itemTitle.Size.Width / 2;
            baseY += itemTitle.Size.Height + 10;

            var itemYes = CreateAndAddMenuItem(new HgPoint<double>(baseX, baseY), "YES", "Yes", Brushes.OrangeRed);
            var itemNo = CreateAndAddMenuItem(new HgPoint<double>(baseX + itemYes.Size.Width + 10, baseY), "NO", "No", Brushes.OrangeRed);
            baseY += itemNo.Size.Height + 20;
            itemYes.X -= (itemYes.Size.Width + itemNo.Size.Width) / 2;
            itemNo.X -= (itemYes.Size.Width + itemNo.Size.Width) / 2;

            var help1 = CreateAndAddTextItem(new HgPoint<double>(baseX, baseY), "Move with <W>, <A>, <S>, <D>.", Brushes.LawnGreen);
            help1.X -= help1.Size.Width / 2;
            baseY += itemTitle.Size.Height + 5;

            var help2 = CreateAndAddTextItem(new HgPoint<double>(baseX, baseY), "Boost Speed With <SHIFT>.", Brushes.LawnGreen);
            help2.X -= help2.Size.Width / 2;
            baseY += itemTitle.Size.Height + 5;

            var help3 = CreateAndAddTextItem(new HgPoint<double>(baseX, baseY), "Fire primary with <SPACE>.", Brushes.LawnGreen);
            help3.X -= help3.Size.Width / 2;
            baseY += itemTitle.Size.Height + 10;

            var help4 = CreateAndAddTextItem(new HgPoint<double>(baseX, baseY), "Fire secondary with <CTRL>.", Brushes.LawnGreen);
            help4.X -= help4.Size.Width / 2;
            baseY += itemTitle.Size.Height + 10;

            var help5 = CreateAndAddTextItem(new HgPoint<double>(baseX, baseY), "Change weapons with <Q> and <E>.", Brushes.LawnGreen);
            help5.X -= help5.Size.Width / 2;
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

            QueueForDelete();
        }
    }
}
