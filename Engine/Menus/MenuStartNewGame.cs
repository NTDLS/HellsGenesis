using AI2D.GraphicObjects;
using AI2D.Types;

namespace AI2D.Engine.Menus
{
    public class MenuStartNewGame: BaseMenu
    {
        public MenuStartNewGame(Core core)
            : base(core)
        {
            double baseX = 100;
            double baseY = 100;

            var itemTitle = NewTitleItem(new PointD(baseX, baseY), "Start a new game?");
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
                _core.NewGame();
            }

            this.ReadyForDeletion = true;
        }
    }
}
