using Si.GameEngine.Core;
using Si.GameEngine.Menus._Superclass;
using Si.GameEngine.Situations._Superclass;
using Si.GameEngine.Sprites.MenuItems;
using Si.Library;
using Si.Library.Mathematics.Geometry;
using Si.Menus.SinglePlayer;
using System.Linq;

namespace Si.Menus.MultiPlayer.Host
{
    /// <summary>
    /// The menu that is displayed at game start to allow the player to select a situation.
    /// </summary>
    internal class MpMenuHostSituationSelect : MenuBase
    {
        private readonly SpriteMenuItem _situationBlurb;

        public MpMenuHostSituationSelect(GameEngineCore gameEngine)
            : base(gameEngine)
        {
            var currentScaledScreenBounds = _gameEngine.Display.GetCurrentScaledScreenBounds();

            float offsetX = currentScaledScreenBounds.X + 40;
            float offsetY = currentScaledScreenBounds.Y + 100;

            var itemTitle = CreateAndAddTitleItem(new SiVector(offsetX, offsetY), "Whats the Situation?");
            itemTitle.X = offsetX + 200;
            itemTitle.Y = offsetY - itemTitle.Size.Height;

            offsetY += itemTitle.Height;

            _situationBlurb = CreateAndAddTextblock(new SiVector(offsetX, offsetY), "");
            _situationBlurb.X = offsetX + 300;
            _situationBlurb.Y = offsetY - _situationBlurb.Size.Height;

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
                var situationInstance = SiReflection.CreateInstanceFromType<SituationBase>(situationType, new object[] { gameEngine, });

                var menuItem = CreateAndAddSelectableItem(new SiVector(offsetX + 25, offsetY), situationInstance.Name, $"> {situationInstance.Name}");

                menuItem.UserData = situationInstance;

                menuItem.Y -= menuItem.Size.Height / 2;
                offsetY += 50;
            }

            OnSelectionChanged += SituationSelectMenu_OnSelectionChanged;
            OnExecuteSelection += SituationSelectMenu_OnExecuteSelection;
            OnEscape += MpMenuHostSituationSelect_OnEscape;

            VisibleSelectableItems().First().Selected = true;
        }

        private bool MpMenuHostSituationSelect_OnEscape()
        {
            //Create the game host on the server.
            var lobbyUID = _gameEngine.Multiplay.State.LobbyUID;
            _gameEngine.Multiplay.DeregisterLobbyUID();
            _gameEngine.Multiplay.DeleteLobby(lobbyUID);

            _gameEngine.Menus.Add(new MpMenuHostCreateLobby(_gameEngine));
            return true;
        }

        private bool SituationSelectMenu_OnExecuteSelection(SpriteMenuItem item)
        {
            if (item.UserData is SituationBase situation)
            {
                _gameEngine.ResetGame();
                _gameEngine.Situations.Select(situation.GetType().Name);
                _gameEngine.Menus.Add(new MpMenuHostSelectLoadout(_gameEngine));
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
