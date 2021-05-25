using AI2D.Actors;
using AI2D.Types;
using System.Drawing;

namespace AI2D.Engine.Menus
{
    public class MenuStartNewGame: BaseMenu
    {
        public MenuStartNewGame(Core core)
            : base(core)
        {
            double baseX = _core.Display.VisibleSize.Width / 2;
            double baseY = _core.Display.VisibleSize.Height / 4;

            var itemTitle = NewTitleItem(new Point<double>(baseX, baseY), "Start new game?", Brushes.OrangeRed);
            itemTitle.X -= itemTitle.Size.Width / 2;
            baseY += itemTitle.Size.Height + 10;

            var itemYes = NewMenuItem(new Point<double>(baseX, baseY), "YES", "Yes", Brushes.OrangeRed);
            var itemNo = NewMenuItem(new Point<double>(baseX + itemYes.Size.Width + 10, baseY), "NO", "No", Brushes.OrangeRed);
            baseY += itemNo.Size.Height + 20;
            itemYes.X -= (itemYes.Size.Width + itemNo.Size.Width) / 2;
            itemNo.X -= (itemYes.Size.Width + itemNo.Size.Width) / 2;

            var help1 = NewTextItem(new Point<double>(baseX, baseY), "Move with <W>, <A>, <S>, <D>.", Brushes.LawnGreen);
            help1.X -= help1.Size.Width / 2;
            baseY += itemTitle.Size.Height + 5;

            var help2 = NewTextItem(new Point<double>(baseX, baseY), "Boost Speed With <SHIFT>.", Brushes.LawnGreen);
            help2.X -= help2.Size.Width / 2;
            baseY += itemTitle.Size.Height + 5;

            var help3 = NewTextItem(new Point<double>(baseX, baseY), "Fire primary with <SPACE>.", Brushes.LawnGreen);
            help3.X -= help3.Size.Width / 2;
            baseY += itemTitle.Size.Height + 10;

            var help4 = NewTextItem(new Point<double>(baseX, baseY), "Fire secondary with <CTRL>.", Brushes.LawnGreen);
            help4.X -= help4.Size.Width / 2;
            baseY += itemTitle.Size.Height + 10;

            var help5 = NewTextItem(new Point<double>(baseX, baseY), "Change weapons with <Q> and <E>.", Brushes.LawnGreen);
            help5.X -= help5.Size.Width / 2;
            baseY += itemTitle.Size.Height + 10;

            itemYes.Selected = true;
        }

        public override void ExecuteSelection(ActorMenuItem item)
        {
            if (item.Name == "NO")
            {
                _core.Stop();
            }
            else if (item.Name == "YES")
            {
                _core.Actors.InsertMenu(new PlayerShipMenu(_core));
            }

            QueueForDelete();
        }
    }
}
