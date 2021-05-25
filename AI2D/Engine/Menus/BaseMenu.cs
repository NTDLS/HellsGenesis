using AI2D.Actors;
using AI2D.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AI2D.Engine.Menus
{
    public class BaseMenu
    {
        public Guid UID { get; private set; } = Guid.NewGuid();
        protected Core _core;

        private List<ActorMenuItem> _menuItems { get; set; } = new List<ActorMenuItem>();

        public void QueueForDelete()
        {
            _readyForDeletion = true;
        }

        private bool _readyForDeletion;
        public bool ReadyForDeletion
        {
            get
            {
                return _readyForDeletion;
            }
        }


        public BaseMenu(Core core)
        {
            _core = core;
        }

        public virtual void ExecuteSelection(ActorMenuItem item)
        {

        }

        public virtual void SelectionChanged(ActorMenuItem item)
        {

        }

        public ActorMenuItem NewTitleItem(Point<double> location, string text, Brush brush, int size = 24)
        {
            var item = new ActorMenuItem(_core, this, "Consolas", brush, size, location)
            {
                Text = text,
                ItemType = ActorMenuItem.MenuItemType.Title
            };
            AddMenuItem(item);
            return item;
        }

        public ActorMenuItem NewTextItem(Point<double> location, string text, Brush brush, int size = 16)
        {
            var item = new ActorMenuItem(_core, this, "Consolas", brush, size, location)
            {
                Text = text,
                ItemType = ActorMenuItem.MenuItemType.Text
            };
            AddMenuItem(item);
            return item;
        }

        public ActorMenuItem NewMenuItem(Point<double> location, string name, string text, Brush brush, int size = 14)
        {
            var item = new ActorMenuItem(_core, this, "Consolas", brush, size, location)
            {
                Name = name,
                Text = text,
                ItemType = ActorMenuItem.MenuItemType.Item
            };
            AddMenuItem(item);
            return item;
        }

        public void AddMenuItem(ActorMenuItem item)
        {
            lock (_core.Actors.Menus)
            {
                _menuItems.Add(item);
            }
        }


        DateTime lastInputHandled = DateTime.UtcNow;

        public void HandleInput()
        {
            if ((DateTime.UtcNow - lastInputHandled).TotalMilliseconds < 250)
            {
                return; //We have to keep the menues from going crazy.
            }

            if (_core.Input.IsKeyPressed(PlayerKey.Enter))
            {
                lastInputHandled = DateTime.UtcNow;

                var selectedItem = (from o in _menuItems where o.ItemType == ActorMenuItem.MenuItemType.Item && o.Selected == true select o).FirstOrDefault();
                if (selectedItem != null)
                {
                    //Menu executions may block execution if run in the same thread. For example, the menu executin may be looking to remove all
                    //  items from the screen and wait for them to be removed. Problem is, the same thread that calls the menuexecution is the same
                    //  one that removes items from the screen, therefor the "while(itemsExist)" loop would never finish.
                    //  
                    Task.Run(() => ExecuteSelection(selectedItem));
                }
            }

            if (_core.Input.IsKeyPressed(PlayerKey.Right) || _core.Input.IsKeyPressed(PlayerKey.Down) || _core.Input.IsKeyPressed(PlayerKey.RotateClockwise))
            {
                lastInputHandled = DateTime.UtcNow;

                int selectIndex = 0;

                var items = (from o in _menuItems where o.ItemType == ActorMenuItem.MenuItemType.Item select o).ToList();
                if (items != null && items.Count > 0)
                {
                    int previouslySelectedIndex = -1;

                    for (int i = 0; i < items.Count; i++)
                    {
                        var item = items[i];
                        if (item.ItemType == ActorMenuItem.MenuItemType.Item)
                        {
                            if (item.Selected)
                            {
                                selectIndex = i + 1;
                                item.Selected = false;
                                previouslySelectedIndex = i;
                            }
                        }
                    }

                    if (selectIndex >= items.Count)
                    {
                        selectIndex = items.Count - 1;
                    }

                    items[selectIndex].Selected = true;

                    if (selectIndex != previouslySelectedIndex)
                    {
                        var selectedItem = (from o in _menuItems where o.ItemType == ActorMenuItem.MenuItemType.Item && o.Selected == true select o).FirstOrDefault();
                        if (selectedItem != null)
                        {
                            //Menu executions may block execution if run in the same thread. For example, the menu executin may be looking to remove all
                            //  items from the screen and wait for them to be removed. Problem is, the same thread that calls the menuexecution is the same
                            //  one that removes items from the screen, therefor the "while(itemsExist)" loop would never finish.
                            //  
                            Task.Run(() => SelectionChanged(selectedItem));
                        }
                    }
                }
            }

            if (_core.Input.IsKeyPressed(PlayerKey.Left) || _core.Input.IsKeyPressed(PlayerKey.Up) || _core.Input.IsKeyPressed(PlayerKey.RotateCounterClockwise))
            {
                lastInputHandled = DateTime.UtcNow;

                int selectIndex = 0;

                var items = (from o in _menuItems where o.ItemType == ActorMenuItem.MenuItemType.Item select o).ToList();
                if (items != null && items.Count > 0)
                {
                    int previouslySelectedIndex = -1;

                    for (int i = 0; i < items.Count; i++)
                    {
                        var item = items[i];
                        if (item.ItemType == ActorMenuItem.MenuItemType.Item)
                        {
                            if (item.Selected)
                            {
                                selectIndex = i - 1;
                                previouslySelectedIndex = i;
                                item.Selected = false;
                            }
                        }
                    }

                    if (selectIndex < 0)
                    {
                        selectIndex = 0;
                    }

                    items[selectIndex].Selected = true;

                    if (selectIndex != previouslySelectedIndex)
                    {
                        var selectedItem = (from o in _menuItems where o.ItemType == ActorMenuItem.MenuItemType.Item && o.Selected == true select o).FirstOrDefault();
                        if (selectedItem != null)
                        {
                            //Menu executions may block execution if run in the same thread. For example, the menu executin may be looking to remove all
                            //  items from the screen and wait for them to be removed. Problem is, the same thread that calls the menuexecution is the same
                            //  one that removes items from the screen, therefor the "while(itemsExist)" loop would never finish.
                            //  
                            Task.Run(() => SelectionChanged(selectedItem));
                        }
                    }
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
