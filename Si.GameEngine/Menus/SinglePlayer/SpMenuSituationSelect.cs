using Si.GameEngine.Engine;
using Si.GameEngine.Menus;
using Si.GameEngine.Menus.BasesAndInterfaces;
using Si.GameEngine.Situations.BasesAndInterfaces;
using Si.GameEngine.Sprites.MenuItems;
using Si.Shared;
using Si.Shared.Types.Geometry;
using System.Linq;

namespace Si.Menus.SinglePlayer
{
    /// <summary>
    /// The menu that is displayed at game start to allow the player to select a situation.
    /// </summary>
    internal class SpMenuSituationSelect : MenuBase
    {
        private readonly SpriteMenuItem _situationBlurb;

        public SpMenuSituationSelect(EngineCore gameCore)
            : base(gameCore)
        {
            var currentScaledScreenBounds = _gameCore.Display.GetCurrentScaledScreenBounds();

            double offsetX = currentScaledScreenBounds.X + 40;
            double offsetY = currentScaledScreenBounds.Y + 100;

            var itemTitle = CreateAndAddTitleItem(new SiPoint(offsetX, offsetY), "Whats the Situation?");
            itemTitle.LocalX = offsetX + 200;
            itemTitle.LocalY = offsetY - itemTitle.Size.Height;

            offsetY += itemTitle.Height;

            _situationBlurb = CreateAndAddTextblock(new SiPoint(offsetX, offsetY), "");
            _situationBlurb.LocalX = offsetX + 300;
            _situationBlurb.LocalY = offsetY - _situationBlurb.Size.Height;

            //Use reflection to get a list of possible player types.
            var situationTypes = SiReflection.GetSubClassesOf<SituationBase>().OrderBy(o => o.Name).ToList();

            //Move the debug player to the top of the list.
            var situations = situationTypes.Where(o => o.Name.Contains("Debug")).FirstOrDefault();
            if (situations != null)
            {
                situationTypes.Remove(situations);
                situationTypes.Insert(0, situations);
            }

            foreach (var situationType in situationTypes)
            {
                var situationInstance = SiReflection.CreateInstanceFromType<SituationBase>(situationType, new object[] { gameCore, });

                var menuItem = CreateAndAddSelectableItem(new SiPoint(offsetX + 25, offsetY), situationInstance.Name, $"> {situationInstance.Name}");

                menuItem.UserData = situationInstance;

                menuItem.LocalY -= menuItem.Size.Height / 2;
                offsetY += 50;
            }

            OnSelectionChanged += SituationSelectMenu_OnSelectionChanged;
            OnExecuteSelection += SituationSelectMenu_OnExecuteSelection;
            OnEscape += SpMenuSituationSelect_OnEscape;

            VisibleSelectableItems().First().Selected = true;
        }

        private bool SpMenuSituationSelect_OnEscape()
        {
            _gameCore.Menus.Add(new MenuStartNewGame(_gameCore));
            return true;
        }

        private bool SituationSelectMenu_OnExecuteSelection(SpriteMenuItem item)
        {
            if (item.UserData is SituationBase situation)
            {
                _gameCore.ResetGame();
                _gameCore.Situations.Select(situation.GetType().Name);
                _gameCore.Menus.Add(new SpMenuSelectLoadout(_gameCore));
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
