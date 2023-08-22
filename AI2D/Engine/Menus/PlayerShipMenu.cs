using AI2D.Actors;
using AI2D.Types;
using System;
using System.Drawing;
using static AI2D.Engine.Constants;

namespace AI2D.Engine.Menus
{
    public class PlayerShipMenu : BaseMenu
    {
        private readonly ActorMenuItem _shipBlurb;

        public PlayerShipMenu(Core core)
            : base(core)
        {
            var player1 = _core.Actors.Add(new ActorPlayer(_core, PlayerClass.Nimitz) { Tag = "MENU_SHIP_SELECT" });
            var player2 = _core.Actors.Add(new ActorPlayer(_core, PlayerClass.Knox) { Tag = "MENU_SHIP_SELECT" });
            var player3 = _core.Actors.Add(new ActorPlayer(_core, PlayerClass.Luhu) { Tag = "MENU_SHIP_SELECT" });
            var player4 = _core.Actors.Add(new ActorPlayer(_core, PlayerClass.Atlant) { Tag = "MENU_SHIP_SELECT" });
            var player5 = _core.Actors.Add(new ActorPlayer(_core, PlayerClass.Ticonderoga) { Tag = "MENU_SHIP_SELECT" });
            var player6 = _core.Actors.Add(new ActorPlayer(_core, PlayerClass.Kirov) { Tag = "MENU_SHIP_SELECT" });

            double baseX = _core.Display.CurrentScaledScreenBounds.X + 40;
            double baseY = _core.Display.CurrentScaledScreenBounds.Y + 100;

            var itemTitle = NewTitleItem(new Point<double>(baseX, baseY), "Select a Ship Class:", Brushes.OrangeRed);
            itemTitle.X = baseX + 200;
            itemTitle.Y = baseY - itemTitle.Size.Height;

            _shipBlurb = NewTextItem(new Point<double>(baseX, baseY), "", Brushes.LawnGreen, 10);
            _shipBlurb.X = baseX + 200;
            _shipBlurb.Y = baseY - _shipBlurb.Size.Height;

            var player1Select = NewMenuItem(new Point<double>(baseX + 25, baseY), PlayerClass.Nimitz.ToString(), PlayerClass.Nimitz.ToString(), Brushes.OrangeRed);
            player1Select.Y -= player1Select.Size.Height / 2;
            player1Select.Selected = true;
            player1.X = baseX;
            player1.Y = baseY;
            baseY += 50;

            var player2Select = NewMenuItem(new Point<double>(baseX + 25, baseY), PlayerClass.Knox.ToString(), PlayerClass.Knox.ToString(), Brushes.OrangeRed);
            player2Select.Y -= player2Select.Size.Height / 2;
            player2.X = baseX;
            player2.Y = baseY;
            baseY += 50;

            var player3Select = NewMenuItem(new Point<double>(baseX + 25, baseY), PlayerClass.Luhu.ToString(), PlayerClass.Luhu.ToString(), Brushes.OrangeRed);
            player3Select.Y -= player3Select.Size.Height / 2;
            player3.X = baseX;
            player3.Y = baseY;
            baseY += 50;

            var player4Select = NewMenuItem(new Point<double>(baseX + 25, baseY), PlayerClass.Atlant.ToString(), PlayerClass.Atlant.ToString(), Brushes.OrangeRed);
            player4Select.Y -= player4Select.Size.Height / 2;
            player4.X = baseX;
            player4.Y = baseY;
            baseY += 50;

            var player5Select = NewMenuItem(new Point<double>(baseX + 25, baseY), PlayerClass.Ticonderoga.ToString(), PlayerClass.Ticonderoga.ToString(), Brushes.OrangeRed);
            player5Select.Y -= player5Select.Size.Height / 2;
            player5.X = baseX;
            player5.Y = baseY;
            baseY += 50;

            var player6Select = NewMenuItem(new Point<double>(baseX + 25, baseY), PlayerClass.Kirov.ToString(), PlayerClass.Kirov.ToString(), Brushes.OrangeRed);
            player6Select.Y -= player6Select.Size.Height / 2;
            player6.X = baseX;
            player6.Y = baseY;
            baseY += 50;
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
            if (item.Name == PlayerClass.Nimitz.ToString())
            {
                _shipBlurb.Text = GetHelpText(
                    "Nimitz", //Name
                    "Vulcan Cannon", //Primary Weapon
                    "Conventional non-Guided Missiles x2", //Secondary Weapon
                    "1,000", //Sheilds
                    "500", //Hull
                    "3.0", //Speed
                    "1.5", //Warp
                    "\r\n The Nimitz Class fighter is a heavily armored, medium armed \r\n and not-so-nimble fighter."
                  );
            }
            else if (item.Name == PlayerClass.Knox.ToString())
            {
                _shipBlurb.Text = GetHelpText(
                    "Knox", //Name
                    "Vulcan Cannon", //Primary Weapon
                    "Conventional non-Guided Missiles x2", //Secondary Weapon
                    "5,000", //Sheilds
                    "2,500", //Hull
                    "2.2", //Speed
                    "1.0", //Warp
                    "\r\n Heavier than a destroyer, this fighter can take a serious \r\n beating while sacraficing speed."
                  );
            }
            else if (item.Name == PlayerClass.Luhu.ToString())
            {
                _shipBlurb.Text = GetHelpText(
                    "Luhu", //Name
                    "Vulcan Cannon", //Primary Weapon
                    "Conventional Guided Missiles x1", //Secondary Weapon
                    "1,000", //Sheilds
                    "500", //Hull
                    "5.0", //Speed
                    "2.5", //Warp
                    "\r\n This destroyer class ship is a medium armored, medium armament \r\n agile fighter for medium weight enemies."
                  );
            }
            else if (item.Name == PlayerClass.Atlant.ToString())
            {
                _shipBlurb.Text = GetHelpText(
                    "Atlant", //Name
                    "Dual-Vulcan Cannon", //Primary Weapon
                    "Conventional Guided and non-Guided Missiles x4", //Secondary Weapon
                    "1,000", //Sheilds
                    "500", //Hull
                    "4.0", //Speed
                    "2.0", //Warp
                    "\r\n The Atlant is a well rounded heavily armed and armored agile \r\n fighter for heavy enemies."
                  );
            }
            else if (item.Name == PlayerClass.Ticonderoga.ToString())
            {
                _shipBlurb.Text = GetHelpText(
                    "Ticonderoga", //Name
                    "Dual-Vulcan Cannon", //Primary Weapon
                    "Energy non-Guided x2", //Secondary Weapon
                    "1,000", //Sheilds
                    "500", //Hull
                    "6.0", //Speed
                    "3.5", //Warp
                    "\r\n The Ticonderoga is just fast and packs an intense punch with \r\n near-light-speed energy weapons."
                  );
            }
            else if (item.Name == PlayerClass.Kirov.ToString())
            {
                _shipBlurb.Text = GetHelpText(
                    "Kirov", //Name
                    "Vulcan Cannon", //Primary Weapon
                    "Dual-Vulcan Cannon", //Secondary Weapon
                    "15,000", //Sheilds
                    "100", //Hull
                    "5.0", //Speed
                    "3.5", //Warp
                    "\r\n The Kirov employs a fragile light-weight hull but expends tons \r\n of energy on its heavy energy sheilds. A space dog-fighter."
                  );
            }
            else
            {
                _shipBlurb.Text = GetHelpText(
                    item.Name, //Name
                    "∞", //Primary Weapon
                    "∞", //Secondary Weapon
                    "∞", //Sheilds
                    "∞", //Hull
                    "∞", //Speed
                    "∞", //Warp
                    "\r\n What have you done?!."
                  );
            }
        }

        public override void ExecuteSelection(ActorMenuItem item)
        {
            var playerClass = (PlayerClass)Enum.Parse(typeof(PlayerClass), item.Name);

            _core.Actors.Player.Reset(playerClass);

            this.QueueForDelete();
            _core.Actors.DeleteAllActorsByTag("MENU_SHIP_SELECT");

            _core.Actors.NewGame();
        }
    }
}
