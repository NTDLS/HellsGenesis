using NebulaSiege.Engine;
using NebulaSiege.Engine.Types.Geometry;
using NebulaSiege.Menus.MenuItems;
using SharpDX.DirectInput;
using SharpDX.X3DAudio;
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

        public List<SpriteMenuItem> VisibleSelectableItems() =>
            Items.Where(o => o.Visable == true
            && (o.ItemType == HgMenuItemType.SelectableItem || o.ItemType == HgMenuItemType.SelectableTextInput)).ToList();

        #region Events.

        public delegate void SelectionChangedEvent(SpriteMenuItem item);
        /// <summary>
        /// The player moved the selection cursor.
        /// </summary>
        /// <param name="item"></param>
        public event SelectionChangedEvent OnSelectionChanged;

        public void InvokeSelectionChanged(SpriteMenuItem item) => OnSelectionChanged?.Invoke(item);

        public delegate void ExecuteSelectionEvent(SpriteMenuItem item);
        /// <summary>
        /// The player hit enter to select the currently highlighted menu item.
        /// </summary>
        /// <param name="item"></param>
        public event ExecuteSelectionEvent OnExecuteSelection;


        internal delegate void EscapeEvent();
        /// <summary>
        /// The player has hit the escape key.
        /// </summary>
        /// <param name="item"></param>
        public event EscapeEvent OnEscape;

        internal delegate void CleanupEvent();
        /// <summary>
        /// Called when the menu is being destroyed. This is a good place to cleanup.
        /// </summary>
        /// <param name="item"></param>
        public event CleanupEvent OnCleanup;

        public void InvokeCleanup() => OnCleanup?.Invoke();

        #endregion

        public MenuBase(EngineCore core)
        {
            _core = core;
        }

        public void Show() => Items.ForEach(o => o.Visable = true);
        public void Hide() => Items.ForEach(o => o.Visable = false);
        public bool HandlesEscape() => (OnEscape != null);
        public void QueueForDelete() => ReadyForDeletion = true;

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

        public SpriteMenuItem CreateAndAddTextblock(NsPoint location, string text)
        {
            var item = new SpriteMenuItem(_core, this, _core.Rendering.TextFormats.MenuGeneral, _core.Rendering.Materials.Brushes.LawnGreen, location)
            {
                Text = text,
                ItemType = HgMenuItemType.Textblock
            };
            AddMenuItem(item);
            return item;
        }

        public SpriteMenuItem CreateAndAddSelectableItem(NsPoint location, string key, string text)
        {
            var item = new SpriteMenuItem(_core, this, _core.Rendering.TextFormats.MenuItem, _core.Rendering.Materials.Brushes.OrangeRed, location)
            {
                Key = key,
                Text = text,
                ItemType = HgMenuItemType.SelectableItem
            };
            AddMenuItem(item);
            return item;
        }

        public SpriteMenuItem CreateAndAddSelectableTextInput(NsPoint location, string key, string text = "")
        {
            var item = new SpriteMenuSelectableTextInput(_core, this, _core.Rendering.TextFormats.MenuItem, _core.Rendering.Materials.Brushes.OrangeRed, location)
            {
                Key = key,
                Text = text,
                ItemType = HgMenuItemType.SelectableTextInput
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

            var selectedTextInput = Items.OfType<SpriteMenuSelectableTextInput>().Where(o => o.Selected).FirstOrDefault();

            _core.Input.CollectDetailedKeyInformation(selectedTextInput != null);

            //Text typing is not subject to _lastInputHandled limits because it is based on cycled keys, not depressed keys.
            if (selectedTextInput != null)
            {
                //Since we do allow for backspace repetitions, we will enforce a _lastInputHandled limit.
                if (_core.Input.DepressedKeys.Contains(Key.Back))
                {
                    if ((DateTime.UtcNow - _lastInputHandled).TotalMilliseconds >= 100)
                    {
                        _lastInputHandled = DateTime.UtcNow;
                        selectedTextInput.Backspace();
                        _core.Audio.Click.Play();
                    }
                    return;
                }

                if (_core.Input.TypedString.Length > 0)
                {
                    _core.Audio.Click.Play();
                    selectedTextInput.Append(_core.Input.TypedString);
                }
            }

            if ((DateTime.UtcNow - _lastInputHandled).TotalMilliseconds < 200)
            {
                return; //We have to keep the menues from going crazy.
            }

            if (_core.Input.IsKeyPressed(HgPlayerKey.Enter))
            {
                _core.Audio.Click.Play();

                _lastInputHandled = DateTime.UtcNow;

                var selectedItem = (from o in Items where o.ItemType == HgMenuItemType.SelectableItem && o.Selected == true select o).FirstOrDefault();
                if (selectedItem != null)
                {
                    QueueForDelete();

                    //Menu executions may block execution if run in the same thread. For example, the menu executin may be looking to remove all
                    //  items from the screen and wait for them to be removed. Problem is, the same thread that calls the menuexecution is the same
                    //  one that removes items from the screen, therefor the "while(itemsExist)" loop would never finish.
                    //  
                    Task.Run(() => OnExecuteSelection?.Invoke(selectedItem));
                }
            }
            else if (_core.Input.IsKeyPressed(HgPlayerKey.Escape))
            {
                _core.Audio.Click.Play();

                _lastInputHandled = DateTime.UtcNow;

                //Menu executions may block execution if run in the same thread. For example, the menu executin may be looking to remove all
                //  items from the screen and wait for them to be removed. Problem is, the same thread that calls the menuexecution is the same
                //  one that removes items from the screen, therefor the "while(itemsExist)" loop would never finish.
                //  
                Task.Run(() => OnEscape?.Invoke());
            }

            if (_core.Input.IsKeyPressed(HgPlayerKey.Right) || _core.Input.IsKeyPressed(HgPlayerKey.Down) || _core.Input.IsKeyPressed(HgPlayerKey.Reverse) || _core.Input.IsKeyPressed(HgPlayerKey.RotateClockwise))
            {
                _lastInputHandled = DateTime.UtcNow;

                int selectIndex = 0;

                var items = (from o in Items where o.ItemType == HgMenuItemType.SelectableItem select o).ToList();
                if (items != null && items.Count > 0)
                {
                    int previouslySelectedIndex = -1;

                    for (int i = 0; i < items.Count; i++)
                    {
                        var item = items[i];
                        if (item.ItemType == HgMenuItemType.SelectableItem)
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
                        var selectedItem = (from o in Items where o.ItemType == HgMenuItemType.SelectableItem && o.Selected == true select o).FirstOrDefault();
                        if (selectedItem != null)
                        {
                            _core.Audio.Click.Play();

                            //Menu executions may block execution if run in the same thread. For example, the menu executin may be looking to remove all
                            //  items from the screen and wait for them to be removed. Problem is, the same thread that calls the menuexecution is the same
                            //  one that removes items from the screen, therefor the "while(itemsExist)" loop would never finish.
                            //  
                            Task.Run(() => OnSelectionChanged?.Invoke(selectedItem));
                        }
                    }
                }
            }

            if (_core.Input.IsKeyPressed(HgPlayerKey.Left)
                || _core.Input.IsKeyPressed(HgPlayerKey.Up)
                || _core.Input.IsKeyPressed(HgPlayerKey.Forward)
                || _core.Input.IsKeyPressed(HgPlayerKey.RotateCounterClockwise))
            {
                _lastInputHandled = DateTime.UtcNow;

                int selectIndex = 0;

                var items = (from o in Items where o.ItemType == HgMenuItemType.SelectableItem select o).ToList();
                if (items != null && items.Count > 0)
                {
                    int previouslySelectedIndex = -1;

                    for (int i = 0; i < items.Count; i++)
                    {
                        var item = items[i];
                        if (item.ItemType == HgMenuItemType.SelectableItem)
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
                        var selectedItem = (from o in Items where o.ItemType == HgMenuItemType.SelectableItem && o.Selected == true select o).FirstOrDefault();
                        if (selectedItem != null)
                        {
                            _core.Audio.Click.Play();

                            //Menu executions may block execution if run in the same thread. For example, the menu executin may be looking to remove all
                            //  items from the screen and wait for them to be removed. Problem is, the same thread that calls the menuexecution is the same
                            //  one that removes items from the screen, therefor the "while(itemsExist)" loop would never finish.
                            //  
                            Task.Run(() => OnSelectionChanged?.Invoke(selectedItem));
                        }
                    }
                }
            }
        }

        public void Render(SharpDX.Direct2D1.RenderTarget renderTarget)
        {
            foreach (var item in Items.Where(o => o.Visable == true))
            {
                item.Render(renderTarget);
            }

            var selectedItem = (from o in Items where o.Visable == true && o.Selected == true select o).FirstOrDefault();
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
