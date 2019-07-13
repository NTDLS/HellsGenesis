using AI2D.GraphicObjects;
using AI2D.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI2D.Engine.Menus
{
    public class BaseMenu
    {
        protected Core _core;

        private List<ObjMenuItem> _menuItems { get; set; } = new List<ObjMenuItem>();
        public bool ReadyForDeletion { get; set; }

        public BaseMenu(Core core)
        {
            _core = core;
        }

        public virtual void ExecuteSelection(ObjMenuItem item)
        {

        }

        public ObjMenuItem NewTitleItem(PointD location, string text)
        {
            var item = new ObjMenuItem(_core, "Consolas", Brushes.OrangeRed, 24, location)
            {
                Text = text,
                ItemType = ObjMenuItem.MenuItemType.Title
            };
            AddMenuItem(item);
            return item;
        }

        public ObjMenuItem NewMenuItem(PointD location, string name, string text)
        {
            var item = new ObjMenuItem(_core, "Consolas", Brushes.OrangeRed, 14, location)
            {
                Name = name,
                Text = text,
                ItemType = ObjMenuItem.MenuItemType.Item
            };
            AddMenuItem(item);
            return item;
        }

        public void AddMenuItem(ObjMenuItem item)
        {
            lock (_core.Actors.Menus)
            {
                _menuItems.Add(item);
            }
        }

        public void HandleInput()
        {
            if (_core.Input.IsKeyPressed(PlayerKey.Enter))
            {
                var selectedItem = (from o in _menuItems where o.ItemType == ObjMenuItem.MenuItemType.Item && o.Selected == true select o).FirstOrDefault();
                if (selectedItem != null)
                {
                    this.ExecuteSelection(selectedItem);
                }
            }

            if (_core.Input.IsKeyPressed(PlayerKey.Right))
            {
                int selectIndex = 0;

                var items = (from o in _menuItems where o.ItemType == ObjMenuItem.MenuItemType.Item select o).ToList();
                if (items != null && items.Count > 0)
                {

                    for (int i = 0; i < items.Count; i++)
                    {
                        var item = items[i];
                        if (item.ItemType == ObjMenuItem.MenuItemType.Item)
                        {
                            if (item.Selected)
                            {
                                selectIndex = i + 1;
                                item.Selected = false;
                            }
                        }
                    }

                    if (selectIndex >= items.Count)
                    {
                        selectIndex = items.Count - 1;
                    }

                    items[selectIndex].Selected = true;
                }
            }

            if (_core.Input.IsKeyPressed(PlayerKey.Left))
            {
                int selectIndex = 0;

                var items = (from o in _menuItems where o.ItemType == ObjMenuItem.MenuItemType.Item select o).ToList();
                if (items != null && items.Count > 0)
                {

                    for (int i = 0; i < items.Count; i++)
                    {
                        var item = items[i];
                        if (item.ItemType == ObjMenuItem.MenuItemType.Item)
                        {
                            if (item.Selected)
                            {
                                selectIndex = i - 1;
                                item.Selected = false;
                            }
                        }
                    }

                    if (selectIndex < 0)
                    {
                        selectIndex = 0;
                    }

                    items[selectIndex].Selected = true;
                }
            }
        }
        
        public void Render(Graphics dc)
        {
            foreach (var item in _menuItems)
            {
                item.Render(dc);
            }

            var selectedItem = (from o in _menuItems where o.Selected == true select o).FirstOrDefault();

            if (selectedItem != null)
            {
                Pen blackPen = new Pen(Color.Red, 3);
                //Rectangle rect = new Rectangle(0, 0, 200, 200);
                
                dc.DrawRectangle(blackPen, selectedItem.BoundsI);
            }

        }

        public virtual void Cleanup()
        {

        }
    }
}
