using NebulaSiege.Engine;
using NebulaSiege.Engine.Types.Geometry;
using NebulaSiege.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NebulaSiege.Menus.BaseClasses
{
    /// <summary>
    /// A menu instance. Allows for setting title text, adding items and managing selections.
    /// </summary>
    internal class MenuBase
    {
        protected EngineCore _core;
        private DateTime _lastInputHandled = DateTime.UtcNow;

        public List<SpriteMenuItem> Items { get; private set; } = new();
        public bool ReadyForDeletion { get; private set; }
        public Guid UID { get; private set; } = Guid.NewGuid();

        public List<SpriteMenuItem> SelectableItems() => Items.Where(o => o.ItemType == HgMenuItemType.Item).ToList();

        public void QueueForDelete()
        {
            ReadyForDeletion = true;
        }

        public MenuBase(EngineCore core)
        {
            _core = core;
        }

        public virtual void Cleanup() { }

        public virtual void ExecuteSelection(SpriteMenuItem item) { }

        public virtual void SelectionChanged(SpriteMenuItem item) { }

        public SpriteMenuItem CreateAndAddTitleItem(NsPoint location, string text)
        {
            var item = new SpriteMenuItem(_core, this, _core.Rendering.TextFormats.MenuTitle, _core.Rendering.Materials.Brushes.OrangeRed, location)
            {
                Text = text,
                ItemType = HgMenuItemType.Title
            };
            AddMenuItem(item);
            return item;
        }

        public SpriteMenuItem CreateAndAddTextItem(NsPoint location, string text)
        {
            var item = new SpriteMenuItem(_core, this, _core.Rendering.TextFormats.MenuGeneral, _core.Rendering.Materials.Brushes.LawnGreen, location)
            {
                Text = text,
                ItemType = HgMenuItemType.Text
            };
            AddMenuItem(item);
            return item;
        }

        public SpriteMenuItem CreateAndAddMenuItem(NsPoint location, string key, string text)
        {
            var item = new SpriteMenuItem(_core, this, _core.Rendering.TextFormats.MenuItem, _core.Rendering.Materials.Brushes.OrangeRed, location)
            {
                Key = key,
                Text = text,
                ItemType = HgMenuItemType.Item
            };
            AddMenuItem(item);
            return item;
        }

        public void AddMenuItem(SpriteMenuItem item)
        {
            lock (_core.Menus.Collection)
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
                _core.Audio.Click.Play();

                _lastInputHandled = DateTime.UtcNow;

                var selectedItem = (from o in Items where o.ItemType == HgMenuItemType.Item && o.Selected == true select o).FirstOrDefault();
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

                var items = (from o in Items where o.ItemType == HgMenuItemType.Item select o).ToList();
                if (items != null && items.Count > 0)
                {
                    int previouslySelectedIndex = -1;

                    for (int i = 0; i < items.Count; i++)
                    {
                        var item = items[i];
                        if (item.ItemType == HgMenuItemType.Item)
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
                        var selectedItem = (from o in Items where o.ItemType == HgMenuItemType.Item && o.Selected == true select o).FirstOrDefault();
                        if (selectedItem != null)
                        {
                            _core.Audio.Click.Play();

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

                var items = (from o in Items where o.ItemType == HgMenuItemType.Item select o).ToList();
                if (items != null && items.Count > 0)
                {
                    int previouslySelectedIndex = -1;

                    for (int i = 0; i < items.Count; i++)
                    {
                        var item = items[i];
                        if (item.ItemType == HgMenuItemType.Item)
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
                        var selectedItem = (from o in Items where o.ItemType == HgMenuItemType.Item && o.Selected == true select o).FirstOrDefault();
                        if (selectedItem != null)
                        {
                            _core.Audio.Click.Play();

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
                _core.Rendering.DrawRectangleAt(renderTarget,
                    new SharpDX.Mathematics.Interop.RawRectangleF(
                        selectedItem.BoundsI.X,
                        selectedItem.BoundsI.Y,
                        selectedItem.BoundsI.X + selectedItem.BoundsI.Width,
                        selectedItem.BoundsI.Y + selectedItem.BoundsI.Height),
                    0,
                    _core.Rendering.Materials.Raw.Red, 2, 2);
            }
        }
    }
}
