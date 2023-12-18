using NebulaSiege.Engine;
using NebulaSiege.Engine.Types.Geometry;
using NebulaSiege.Menus.BaseClasses;
using NebulaSiege.Menus.MenuItems;
using NebulaSiege.Situations.BaseClasses;
using NebulaSiege.Utility;
using System.Linq;

namespace NebulaSiege.Menus
{
    /// <summary>
    /// The menu that is displayed at game start to allow the player to select a situation.
    /// </summary>
    internal class SituationSelectMenu : MenuBase
    {
        private readonly SpriteMenuItem _situationBlurb;

        public SituationSelectMenu(EngineCore core)
            : base(core)
        {
            var currentScaledScreenBounds = _core.Display.GetCurrentScaledScreenBounds();

            double offsetX = currentScaledScreenBounds.X + 40;
            double offsetY = currentScaledScreenBounds.Y + 100;

            var itemTitle = CreateAndAddTitleItem(new NsPoint(offsetX, offsetY), "Select a Situation");
            itemTitle.X = offsetX + 200;
            itemTitle.Y = offsetY - itemTitle.Size.Height;

            offsetY += itemTitle.Height;

            _situationBlurb = CreateAndAddTextblock(new NsPoint(offsetX, offsetY), "");
            _situationBlurb.X = offsetX + 300;
            _situationBlurb.Y = offsetY - _situationBlurb.Size.Height;

            //Use reflection to get a list of possible player types.
            var situationTypes = NsReflection.GetSubClassesOf<SituationBase>().OrderBy(o => o.Name).ToList();

            //Move the debug player to the top of the list.
            var situations = situationTypes.Where(o => o.Name.Contains("Debug")).FirstOrDefault();
            if (situations != null)
            {
                situationTypes.Remove(situations);
                situationTypes.Insert(0, situations);
            }

            foreach (var situationType in situationTypes)
            {
                var situationInstance = NsReflection.CreateInstanceFromType<SituationBase>(situationType, new object[] { core, });

                var menuItem = CreateAndAddSelectableItem(new NsPoint(offsetX + 25, offsetY), situationInstance.Name, $"> {situationInstance.Name}");

                menuItem.UserData = situationInstance;

                menuItem.Y -= menuItem.Size.Height / 2;
                offsetY += 50;
            }

            OnSelectionChanged += SituationSelectMenu_OnSelectionChanged;
            OnExecuteSelection += SituationSelectMenu_OnExecuteSelection;

            VisibleSelectableItems().First().Selected = true;
        }

        private void SituationSelectMenu_OnExecuteSelection(SpriteMenuItem item)
        {
            if (item.UserData is SituationBase situation)
            {
                _core.ResetGame();
                _core.Situations.Select(situation.GetType().Name);
                _core.Menus.Insert(new PlayerLoadoutMenu(_core));
            }
        }

        private void SituationSelectMenu_OnSelectionChanged(SpriteMenuItem item)
        {
            if (item.UserData is SituationBase situation)
            {
                _situationBlurb.Text = situation.Description;
            }
        }
    }
}
