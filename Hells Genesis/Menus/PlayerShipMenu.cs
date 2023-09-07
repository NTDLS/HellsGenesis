using HG.Actors.Ordinary;
using HG.Engine;
using HG.Menus.BaseClasses;
using HG.Types;
using HG.Utility;
using System.Drawing;

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

            var itemTitle = NewTitleItem(new HgPoint<double>(offsetX, offsetY), "Select a Ship Class:", Brushes.OrangeRed);
            itemTitle.X = offsetX + 200;
            itemTitle.Y = offsetY - itemTitle.Size.Height;

            _shipBlurb = NewTextItem(new HgPoint<double>(offsetX, offsetY), "", Brushes.LawnGreen, 10);
            _shipBlurb.X = offsetX + 200;
            _shipBlurb.Y = offsetY - _shipBlurb.Size.Height;

            foreach (var loadout in core.PrefabPlayerLoadouts.Collection)
            {
                var menuItem = NewMenuItem(new HgPoint<double>(offsetX + 25, offsetY), loadout.Name, loadout.Name, Brushes.OrangeRed);
                menuItem.Y -= menuItem.Size.Height / 2;
                menuItem.Selected = true;

                var shipIcon = _core.Actors.Insert(new ActorPlayer(_core, loadout) { Name = "MENU_SHIP_SELECT" });
                shipIcon.X = offsetX;
                shipIcon.Y = offsetY;
                offsetY += 50;
            }
        }

        private string GetHelpText(string name, string primaryWeapon, string secondaryWeapons,
            string sheilds, string hullStrength, string maxSpeed, string warpDrive, string blurb)
        {
            string result = $"             Name : {name}\r\n";
            result += $"   Primary weapon : {primaryWeapon}\r\n";
            result += $"Secondary Weapons : {secondaryWeapons}\r\n";
            result += $"          Sheilds : {sheilds}\r\n";
            result += $"    Hull Strength : {hullStrength}\r\n";
            result += $"        Max Speed : {maxSpeed}\r\n";
            result += $"       Warp Drive : {warpDrive}\r\n";
            result += $"{blurb}";
            return result;
        }

        public override void SelectionChanged(ActorMenuItem item)
        {
            var loadout = _core.PrefabPlayerLoadouts.GetByName(item.Key);

            string weaponName = Misc.GetStaticPropertyValue(loadout.PrimaryWeapon.Type, "Name");
            string primaryWeapon = $"{weaponName} x{loadout.PrimaryWeapon.Rounds}";

            string secondaryWeapons = string.Empty;
            foreach (var weapon in loadout.SecondaryWeapons)
            {
                weaponName = Misc.GetStaticPropertyValue(weapon.Type, "Name");
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
                $"\r\n {loadout.Description}"
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
