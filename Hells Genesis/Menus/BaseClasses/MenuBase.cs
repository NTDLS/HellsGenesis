using HG.Actors.Ordinary;
using HG.Engine;
using HG.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HG.Menus.BaseClasses
{
    internal class MenuBase
    {
        public List<ActorMenuItem> Items { get; private set; } = new();
        private DateTime _lastInputHandled = DateTime.UtcNow;
        public Guid UID { get; private set; } = Guid.NewGuid();
        protected Core _core;

        public List<ActorMenuItem> SelectableItems() => Items.Where(o => o.ItemType == ActorMenuItem.MenuItemType.Item).ToList();

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

        public ActorMenuItem CreateAndAddTitleItem(HgPoint<double> location, string text)
        {
            var item = new ActorMenuItem(_core, this, _core.DirectX.TextFormats.MenuTitle, _core.DirectX.Colors.Brushes.OrangeRed, location)
            {
                Text = text,
                ItemType = ActorMenuItem.MenuItemType.Title
            };
            AddMenuItem(item);
            return item;
        }

        public ActorMenuItem CreateAndAddTextItem(HgPoint<double> location, string text)
        {
            var item = new ActorMenuItem(_core, this, _core.DirectX.TextFormats.MenuGeneral, _core.DirectX.Colors.Brushes.LawnGreen, location)
            {
                Text = text,
                ItemType = ActorMenuItem.MenuItemType.Text
            };
            AddMenuItem(item);
            return item;
        }

        public ActorMenuItem CreateAndAddMenuItem(HgPoint<double> location, string key, string text)
        {
            var item = new ActorMenuItem(_core, this, _core.DirectX.TextFormats.MenuItem, _core.DirectX.Colors.Brushes.OrangeRed, location)
            {
                Key = key,
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
                Items.Add(item);
            }
        }

        public void HandleInput()
        {
            if (ReadyForDeletion)
            {
                Thread.Sleep(1);
                return;
            }

            if ((DateTime.UtcNow - _lastInputHandled).TotalMilliseconds < 150)
            {
                return; //We have to keep the menues from going crazy.
            }

            if (_core.Input.IsKeyPressed(HgPlayerKey.Enter))
            {
                _lastInputHandled = DateTime.UtcNow;

                var selectedItem = (from o in Items where o.ItemType == ActorMenuItem.MenuItemType.Item && o.Selected == true select o).FirstOrDefault();
                if (selectedItem != null)
                {
                    QueueForDelete();

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

                var items = (from o in Items where o.ItemType == ActorMenuItem.MenuItemType.Item select o).ToList();
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
                        var selectedItem = (from o in Items where o.ItemType == ActorMenuItem.MenuItemType.Item && o.Selected == true select o).FirstOrDefault();
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

                var items = (from o in Items where o.ItemType == ActorMenuItem.MenuItemType.Item select o).ToList();
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
                        var selectedItem = (from o in Items where o.ItemType == ActorMenuItem.MenuItemType.Item && o.Selected == true select o).FirstOrDefault();
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

        public void Render(SharpDX.Direct2D1.RenderTarget renderTarget)
        {
            foreach (var item in Items)
            {
                item.Render(renderTarget);
            }

            var selectedItem = (from o in Items where o.Selected == true select o).FirstOrDefault();
            if (selectedItem != null)
            {
                _core.DirectX.DrawRectangleAt(renderTarget,
                    new SharpDX.Mathematics.Interop.RawRectangleF(
                        selectedItem.BoundsI.X,
                        selectedItem.BoundsI.Y,
                        selectedItem.BoundsI.X + selectedItem.BoundsI.Width,
                        selectedItem.BoundsI.Y + selectedItem.BoundsI.Height),
                    0,
                    _core.DirectX.Colors.Raw.Red, 2, 2);
            }
        }

        public virtual void Cleanup()
        {
        }
    }
}
