using Si.Engine.Menu._Superclass;
using Si.Engine.Situation._Superclass;
using Si.Engine.Sprite.MenuItem;
using Si.Library;
using Si.Library.Mathematics.Geometry;
using System.Linq;

namespace Si.Engine.Menu
{
    /// <summary>
    /// The menu that is displayed at game start to allow the player to select a situation.
    /// </summary>
    internal class MenuSituationSelect : MenuBase
    {
        private readonly SpriteMenuItem _situationBlurb;

        public MenuSituationSelect(EngineCore engine)
            : base(engine)
        {
            var currentScaledScreenBounds = _engine.Display.GetCurrentScaledScreenBounds();

            float offsetX = currentScaledScreenBounds.X + 40;
            float offsetY = currentScaledScreenBounds.Y + 100;

            var itemTitle = CreateAndAddTitleItem(new SiPoint(offsetX, offsetY), "Whats the Situation?");
            itemTitle.X = offsetX + 200;
            itemTitle.Y = offsetY - itemTitle.Size.Height;

            offsetY += itemTitle.Height;

            _situationBlurb = CreateAndAddTextblock(new SiPoint(offsetX, offsetY), "");
            _situationBlurb.X = offsetX + 300;
            _situationBlurb.Y = offsetY - _situationBlurb.Size.Height;

            //Use reflection to get a list of possible situation types.
            var situationTypes = SiReflection.GetSubClassesOf<SituationBase>().OrderBy(o => o.Name).ToList();

            //Move the debug situation to the top of the list.
            var situations = situationTypes.Where(o => o.Name.Contains("Debug")).FirstOrDefault();
            if (situations != null)
            {
                situationTypes.Remove(situations);
                situationTypes.Insert(0, situations);
            }

            foreach (var situationType in situationTypes)
            {
                var situationInstance = SiReflection.CreateInstanceFromType<SituationBase>(situationType, new object[] { engine, });

                var menuItem = CreateAndAddSelectableItem(new SiPoint(offsetX + 25, offsetY), situationInstance.Name, $"> {situationInstance.Name}");

                menuItem.UserData = situationInstance;

                menuItem.Y -= menuItem.Size.Height / 2;
                offsetY += 50;
            }

            OnSelectionChanged += SituationSelectMenu_OnSelectionChanged;
            OnExecuteSelection += SituationSelectMenu_OnExecuteSelection;
            OnEscape += SpMenuSituationSelect_OnEscape;

            VisibleSelectableItems().First().Selected = true;
        }

        private bool SpMenuSituationSelect_OnEscape()
        {
            _engine.Menus.Show(new MenuStartNewGame(_engine));
            return true;
        }

        private bool SituationSelectMenu_OnExecuteSelection(SpriteMenuItem item)
        {
            if (item.UserData is SituationBase situation)
            {
                _engine.ResetGame();
                _engine.Situations.Select(situation.GetType().Name);
                _engine.Menus.Show(new MenuSelectLoadout(_engine));
            }
            return true;
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
