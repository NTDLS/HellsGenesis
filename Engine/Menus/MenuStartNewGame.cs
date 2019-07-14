using AI2D.GraphicObjects;
using AI2D.Types;

namespace AI2D.Engine.Menus
{
    public class MenuStartNewGame: BaseMenu
    {
        public MenuStartNewGame(Core core)
            : base(core)
        {
            double baseX = _core.Display.VisibleSize.Width / 2;
            double baseY = _core.Display.VisibleSize.Height / 2; ;

            var itemTitle = NewTitleItem(new PointD(baseX, baseY), "Start a new game?");

            baseX -= itemTitle.Size.Width / 2;
            baseY -= itemTitle.Size.Height;
            itemTitle.X = baseX;
            itemTitle.Y = baseX;

            var itemYes = NewMenuItem(new PointD(baseX, baseY + itemTitle.Size.Height + 10), "YES", "Yes.");
            var itemNo = NewMenuItem(new PointD(baseX + itemYes.Size.Width + 10, baseY + itemTitle.Size.Height + 10), "NO", "No.");

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
