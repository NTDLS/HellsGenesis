using HG.Actors.Ordinary;
using HG.Engine;
using HG.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace HG.Menus.BaseClasses
{
    internal class MenuBase
    {
        private List<ActorMenuItem> _menuItems { get; set; } = new();
        private DateTime _lastInputHandled = DateTime.UtcNow;
        public Guid UID { get; private set; } = Guid.NewGuid();
        protected Core _core;

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

        public MenuBase(Core core)
        {
            _core = core;
        }

        public virtual void ExecuteSelection(ActorMenuItem item)
        {

        }

        public virtual void SelectionChanged(ActorMenuItem item)
        {

        }

        public ActorMenuItem NewTitleItem(HgPoint<double> location, string text, Brush brush, int size = 24)
        {
            var item = new ActorMenuItem(_core, this, "Consolas", brush, size, location)
            {
                Text = text,
                ItemType = ActorMenuItem.MenuItemType.Title
            };
            AddMenuItem(item);
            return item;
        }

        public ActorMenuItem NewTextItem(HgPoint<double> location, string text, Brush brush, int size = 16)
        {
            var item = new ActorMenuItem(_core, this, "Consolas", brush, size, location)
            {
                Text = text,
                ItemType = ActorMenuItem.MenuItemType.Text
            };
            AddMenuItem(item);
            return item;
        }

        public ActorMenuItem NewMenuItem(HgPoint<double> location, string name, string text, Brush brush, int size = 14)
        {
            var item = new ActorMenuItem(_core, this, "Consolas", brush, size, location)
            {
                Key = name,
                Text = text,
                ItemType = ActorMenuItem.MenuItemType.Item
            };
            AddMenuItem(item);
            return item;
        }

        public void AddMenuItem(ActorMenuItem item)
        {
            lock (_core.Menus._controller)
            {
                _menuItems.Add(item);
            }
        }

        public void HandleInput()
        {
            if ((DateTime.UtcNow - _lastInputHandled).TotalMilliseconds < 250)
            {
                return; //We have to keep the menues from going crazy.
            }

            if (_core.Input.IsKeyPressed(HgPlayerKey.Enter))
            {
                _lastInputHandled = DateTime.UtcNow;

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

            if (_core.Input.IsKeyPressed(HgPlayerKey.Right) || _core.Input.IsKeyPressed(HgPlayerKey.Down) || _core.Input.IsKeyPressed(HgPlayerKey.Reverse) || _core.Input.IsKeyPressed(HgPlayerKey.RotateClockwise))
            {
                _lastInputHandled = DateTime.UtcNow;

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

            if (_core.Input.IsKeyPressed(HgPlayerKey.Left) || _core.Input.IsKeyPressed(HgPlayerKey.Up) || _core.Input.IsKeyPressed(HgPlayerKey.Forward) || _core.Input.IsKeyPressed(HgPlayerKey.RotateCounterClockwise))
            {
                _lastInputHandled = DateTime.UtcNow;

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
                dc.DrawRectangle(new Pen(Color.Red, 1), selectedItem.BoundsI);
            }
        }

        public virtual void Cleanup()
        {
        }
    }
}
