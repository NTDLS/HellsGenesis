using HG.Actors.Ordinary;
using HG.Engine;
using HG.Menus.BaseClasses;
using HG.Types;
using HG.Utility;
using System.Linq;

namespace HG.Menus
{
    internal class PlayerShipMenu : MenuBase
    {
        private readonly ActorMenuItem _shipBlurb;

        public PlayerShipMenu(Core core)
            : base(core)
        {
            double offsetX = _core.Display.CurrentScaledScreenBounds.X + 40;
            double offsetY = _core.Display.CurrentScaledScreenBounds.Y + 100;

            var itemTitle = CreateAndAddTitleItem(new HgPoint<double>(offsetX, offsetY), "Select a Ship Class:");
            itemTitle.X = offsetX + 200;
            itemTitle.Y = offsetY - itemTitle.Size.Height;

            _shipBlurb = CreateAndAddTextItem(new HgPoint<double>(offsetX, offsetY), "");
            _shipBlurb.X = offsetX + 200;
            _shipBlurb.Y = offsetY - _shipBlurb.Size.Height;

            foreach (var loadout in core.PrefabPlayerLoadouts.Collection)
            {
                var menuItem = CreateAndAddMenuItem(new HgPoint<double>(offsetX + 25, offsetY), loadout.Name, loadout.Name);
                menuItem.Y -= menuItem.Size.Height / 2;

                var shipIcon = _core.Actors.InsertPlayer(new ActorPlayer(_core, loadout) { Name = "MENU_SHIP_SELECT" });

                if (loadout.Name == "Debug")
                {
                    shipIcon.ThrustAnimation.Visable = true;
                }
                else
                {
                    shipIcon.BoostAnimation.Visable = true;
                }
                shipIcon.X = offsetX;
                shipIcon.Y = offsetY;
                offsetY += 50;
            }

            SelectableItems().First().Selected = true;
        }

        private string GetHelpText(string name, string primaryWeapon, string secondaryWeapons,
            string sheilds, string hullStrength, string maxSpeed, string warpDrive, string blurb)
        {
            string result = $"             Name : {name}\n";
            result += $"   Primary weapon : {primaryWeapon}\n";
            result += $"Secondary Weapons : {secondaryWeapons}\n";
            result += $"          Sheilds : {sheilds}\n";
            result += $"    Hull Strength : {hullStrength}\n";
            result += $"        Max Speed : {maxSpeed}\n";
            result += $"       Warp Drive : {warpDrive}\n";
            result += $"\n{blurb}";
            return result;
        }

        public override void SelectionChanged(ActorMenuItem item)
        {
            var loadout = _core.PrefabPlayerLoadouts.GetByName(item.Key);

            string weaponName = HgReflection.GetStaticPropertyValue(loadout.PrimaryWeapon.Type, "Name");
            string primaryWeapon = $"{weaponName} x{loadout.PrimaryWeapon.Rounds}";

            string secondaryWeapons = string.Empty;
            foreach (var weapon in loadout.SecondaryWeapons)
            {
                weaponName = HgReflection.GetStaticPropertyValue(weapon.Type, "Name");
                secondaryWeapons += $"{weaponName} x{weapon.Rounds}\n{new string(' ', 20)}";
            }

            _shipBlurb.Text = GetHelpText(
                loadout.Name,               //Name
                primaryWeapon.Trim(),       //Primary Weapon
                secondaryWeapons.Trim(),    //Secondary Weapon
                $"{loadout.Sheilds:n0}",    //Sheilds
                $"{loadout.Hull:n0}",       //Hull
                $"{loadout.Speed:n1}",      //Speed
                $"{loadout.Boost:n1}",      //Boost
                $"{loadout.Description}"
            );
        }

        public override void ExecuteSelection(ActorMenuItem item)
        {
            var loadout = _core.PrefabPlayerLoadouts.GetByName(item.Key);

            _core.Player.Actor.Reset(loadout);

            QueueForDelete();

            _core.Actors.DeleteAllActorsByAssetTag("MENU_SHIP_SELECT");

            _core.Actors.NewGame();
        }
    }
}
