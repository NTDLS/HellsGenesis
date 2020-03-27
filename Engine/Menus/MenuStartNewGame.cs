using AI2D.GraphicObjects;
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

            var itemTitle = NewTitleItem(new PointD(baseX, baseY), "Start a new game?", Brushes.OrangeRed);
            itemTitle.X -= itemTitle.Size.Width / 2;
            baseY += itemTitle.Size.Height + 10;

            var itemYes = NewMenuItem(new PointD(baseX, baseY), "YES", "Yes", Brushes.OrangeRed);
            var itemNo = NewMenuItem(new PointD(baseX + itemYes.Size.Width + 10, baseY), "NO", "No", Brushes.OrangeRed);
            baseY += itemNo.Size.Height + 20;
            itemYes.X -= (itemYes.Size.Width + itemNo.Size.Width) / 2;
            itemNo.X -= (itemYes.Size.Width + itemNo.Size.Width) / 2;

            var help1 = NewTextItem(new PointD(baseX, baseY), "Move with <W>, <A>, <S>, <D>.", Brushes.LawnGreen);
            help1.X -= help1.Size.Width / 2;
            baseY += itemTitle.Size.Height + 5;

            var help2 = NewTextItem(new PointD(baseX, baseY), "Boost Speed With <Ctrl>.", Brushes.LawnGreen);
            help2.X -= help2.Size.Width / 2;
            baseY += itemTitle.Size.Height + 5;

            var help3 = NewTextItem(new PointD(baseX, baseY), "Fire with <space> and change weapons with <Shift>.", Brushes.LawnGreen);
            help3.X -= help3.Size.Width / 2;
            baseY += itemTitle.Size.Height + 10;

            itemYes.Selected = true;
        }

        public override void ExecuteSelection(ObjMenuItem item)
        {
            if (item.Name == "NO")
            {
                _core.Stop();
            }
            else if (item.Name == "YES")
            {
                _core.Actors.NewGame();
            }

            this.ReadyForDeletion = true;
        }
    }
}
