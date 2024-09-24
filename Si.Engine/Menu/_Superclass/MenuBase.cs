using SharpDX.DirectInput;
using Si.Engine.Sprite.MenuItem;
using Si.Library.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using static Si.Library.SiConstants;

namespace Si.Engine.Menu._Superclass
{
    /// <summary>
    /// A menu instance. Allows for setting title text, adding items and managing selections.
    /// </summary>
    public class MenuBase
    {
        protected EngineCore _engine;
        private DateTime _lastInputHandled = DateTime.UtcNow;

        public List<SpriteMenuItem> Items { get; private set; } = new();
        public bool QueuedForDeletion { get; private set; }
        public Guid UID { get; private set; } = Guid.NewGuid();

        public T MenuItemByKey<T>(string key) where T : SpriteMenuItem
        {
            return (T)Items.First(o => o.Key == key);
        }

        public List<SpriteMenuItem> VisibleSelectableItems() =>
            Items.Where(o => o.Visible == true
            && (o.ItemType == SiMenuItemType.SelectableItem || o.ItemType == SiMenuItemType.SelectableTextInput)).ToList();

        #region Events.

        public delegate void SelectionChangedEvent(SpriteMenuItem item);
        /// <summary>
        /// The player moved the selection cursor.
        /// </summary>
        /// <param name="item"></param>
        public event SelectionChangedEvent? OnSelectionChanged;

        public void InvokeSelectionChanged(SpriteMenuItem item) => OnSelectionChanged?.Invoke(item);

        /// <summary>
        /// The player hit enter to select the currently highlighted menu item.
        /// </summary>
        /// <param name="item"></param>
        /// <returns>Return true to close the current menu.</returns>
        public delegate bool ExecuteSelectionEvent(SpriteMenuItem item);
        /// <summary>
        /// The player hit enter to select the currently highlighted menu item.
        /// </summary>
        /// <param name="item"></param>
        public event ExecuteSelectionEvent? OnExecuteSelection;

        /// <summary>
        /// The player has hit the escape key.
        /// </summary>
        /// <returns>Return true to close the current menu.</returns>
        public delegate bool EscapeEvent();
        /// <summary>
        /// The player has hit the escape key.
        /// </summary>
        /// <param name="item"></param>
        public event EscapeEvent? OnEscape;

        public delegate void CleanupEvent();
        /// <summary>
        /// Called when the menu is being destroyed. This is a good place to cleanup.
        /// </summary>
        /// <param name="item"></param>
        public event CleanupEvent? OnCleanup;

        #endregion

        public MenuBase(EngineCore engine)
        {
            _engine = engine;
        }

        public bool HandlesEscape() => OnEscape != null;

        public void Close()
        {
            QueuedForDeletion = true; //Just so we ignore any input until the menu is deleted.
            _engine.Events.Add(() => OnCleanup?.Invoke());
            _engine.Menus.Unload(this);
        }

        public SpriteMenuItem AddTitleItem(SiVector location, string text)
        {
            var item = new SpriteMenuItem(_engine, this, _engine.Rendering.TextFormats.MenuTitle, _engine.Rendering.Materials.Brushes.OrangeRed, location)
            {
                Text = text,
                ItemType = SiMenuItemType.Title
            };
            AddMenuItem(item);
            return item;
        }

        public SpriteMenuItem AddTextBlock(SiVector location, string text)
        {
            var item = new SpriteMenuItem(_engine, this, _engine.Rendering.TextFormats.MenuGeneral, _engine.Rendering.Materials.Brushes.LawnGreen, location)
            {
                Text = text,
                ItemType = SiMenuItemType.TextBlock
            };
            AddMenuItem(item);
            return item;
        }

        public SpriteMenuItem AddSelectableItem(SiVector location, string key, string text)
        {
            var item = new SpriteMenuItem(_engine, this, _engine.Rendering.TextFormats.MenuItem, _engine.Rendering.Materials.Brushes.OrangeRed, location)
            {
                Key = key,
                Text = text,
                ItemType = SiMenuItemType.SelectableItem
            };
            AddMenuItem(item);
            return item;
        }

        public SpriteMenuSelectableTextInput AddSelectableTextInput(SiVector location, string key, string text = "")
        {
            var item = new SpriteMenuSelectableTextInput(_engine, this, _engine.Rendering.TextFormats.TextInputItem, _engine.Rendering.Materials.Brushes.Orange, location)
            {
                Key = key,
                Text = text,
                ItemType = SiMenuItemType.SelectableTextInput
            };
            AddMenuItem(item);
            return item;
        }

        public void AddMenuItem(SpriteMenuItem item) => Items.Add(item);

        public void HandleInput()
        {
            if (QueuedForDeletion)
            {
                Thread.Sleep(1);
                return;
            }

            var selectedTextInput = Items.OfType<SpriteMenuSelectableTextInput>().Where(o => o.Selected).FirstOrDefault();

            _engine.Input.CollectDetailedKeyInformation(selectedTextInput != null);

            //Text typing is not subject to _lastInputHandled limits because it is based on cycled keys, not depressed keys.
            if (selectedTextInput != null)
            {
                //Since we do allow for backspace repetitions, we will enforce a _lastInputHandled limit.
                if (_engine.Input.DepressedKeys.Contains(Key.Back))
                {
                    if ((DateTime.UtcNow - _lastInputHandled).TotalMilliseconds >= 100)
                    {
                        _lastInputHandled = DateTime.UtcNow;
                        selectedTextInput.Backspace();
                        _engine.Audio.Click.Play();
                    }
                    return;
                }

                if (_engine.Input.TypedString?.Length > 0)
                {
                    _engine.Audio.Click.Play();
                    selectedTextInput.Append(_engine.Input.TypedString);
                }
            }

            if ((DateTime.UtcNow - _lastInputHandled).TotalMilliseconds < 200)
            {
                return; //We have to keep the menus from going crazy.
            }

            if (_engine.Input.IsKeyPressed(SiPlayerKey.Enter))
            {
                _engine.Audio.Click.Play();

                _lastInputHandled = DateTime.UtcNow;

                var selectedItem = (from o in Items where o.ItemType == SiMenuItemType.SelectableItem && o.Selected == true select o).FirstOrDefault();
                if (selectedItem != null)
                {
                    //Menu executions may block execution if run in the same thread. For example, the menu execution may be looking to remove all
                    //  items from the screen and wait for them to be removed. Problem is, the same thread that calls the menuExecution is the same
                    //  one that removes items from the screen, therefor the "while(itemsExist)" loop would never finish.

                    _engine.Events.Add(() =>
                    {
                        if (OnExecuteSelection?.Invoke(selectedItem) == true)
                        {
                            Close();
                        }
                    });
                }
            }
            else if (_engine.Input.IsKeyPressed(SiPlayerKey.Escape))
            {
                _engine.Audio.Click.Play();

                _lastInputHandled = DateTime.UtcNow;

                //Menu executions may block execution if run in the same thread. For example, the menu execution may be looking to remove all
                //  items from the screen and wait for them to be removed. Problem is, the same thread that calls the menuExecution is the same
                //  one that removes items from the screen, therefor the "while(itemsExist)" loop would never finish.
                //  
                _engine.Events.Add(() =>
                {
                    if (OnEscape?.Invoke() == true)
                    {
                        Close();
                    }
                });
            }

            if (_engine.Input.IsKeyPressed(SiPlayerKey.Right)
                || _engine.Input.IsKeyPressed(SiPlayerKey.Down)
                //|| _engine.Input.IsKeyPressed(SiPlayerKey.Reverse)
                //|| _engine.Input.IsKeyPressed(SiPlayerKey.RotateClockwise)
                )
            {
                _lastInputHandled = DateTime.UtcNow;

                int selectIndex = 0;

                var items = (from o in Items
                             where o.ItemType == SiMenuItemType.SelectableItem || o.ItemType == SiMenuItemType.SelectableTextInput
                             select o).ToList();
                if (items != null && items.Count > 0)
                {
                    int previouslySelectedIndex = -1;

                    for (int i = 0; i < items.Count; i++)
                    {
                        var item = items[i];
                        if (item.ItemType == SiMenuItemType.SelectableItem || item.ItemType == SiMenuItemType.SelectableTextInput)
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
                        var selectedItem = (from o in Items
                                            where (o.ItemType == SiMenuItemType.SelectableItem || o.ItemType == SiMenuItemType.SelectableTextInput) && o.Selected == true
                                            select o).FirstOrDefault();
                        if (selectedItem != null)
                        {
                            _engine.Audio.Click.Play();

                            //Menu executions may block execution if run in the same thread. For example, the menu execution may be looking to remove all
                            //  items from the screen and wait for them to be removed. Problem is, the same thread that calls the menuExecution is the same
                            //  one that removes items from the screen, therefor the "while(itemsExist)" loop would never finish.
                            //  
                            _engine.Events.Add(() => OnSelectionChanged?.Invoke(selectedItem));
                        }
                    }
                }
            }

            if (_engine.Input.IsKeyPressed(SiPlayerKey.Left)
                || _engine.Input.IsKeyPressed(SiPlayerKey.Up)
                //|| _engine.Input.IsKeyPressed(SiPlayerKey.Forward)
                //|| _engine.Input.IsKeyPressed(SiPlayerKey.RotateCounterClockwise)
                )
            {
                _lastInputHandled = DateTime.UtcNow;

                int selectIndex = 0;

                var items = (from o in Items
                             where o.ItemType == SiMenuItemType.SelectableItem || o.ItemType == SiMenuItemType.SelectableTextInput
                             select o).ToList();
                if (items != null && items.Count > 0)
                {
                    int previouslySelectedIndex = -1;

                    for (int i = 0; i < items.Count; i++)
                    {
                        var item = items[i];
                        if (item.ItemType == SiMenuItemType.SelectableItem || item.ItemType == SiMenuItemType.SelectableTextInput)
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
                        var selectedItem = (from o in Items
                                            where (o.ItemType == SiMenuItemType.SelectableItem || o.ItemType == SiMenuItemType.SelectableTextInput) && o.Selected == true
                                            select o).FirstOrDefault();
                        if (selectedItem != null)
                        {
                            _engine.Audio.Click.Play();

                            //Menu executions may block execution if run in the same thread. For example, the menu execution may be looking to remove all
                            //  items from the screen and wait for them to be removed. Problem is, the same thread that calls the menuExecution is the same
                            //  one that removes items from the screen, therefor the "while(itemsExist)" loop would never finish.
                            //  
                            _engine.Events.Add(() => OnSelectionChanged?.Invoke(selectedItem));
                        }
                    }
                }
            }
        }

        public void Render(SharpDX.Direct2D1.RenderTarget renderTarget)
        {
            foreach (var item in Items.Where(o => o.Visible == true))
            {
                item.Render(renderTarget);
            }

            var selectedItem = (from o in Items where o.Visible == true && o.Selected == true select o).FirstOrDefault();
            if (selectedItem != null)
            {
                _engine.Rendering.DrawRectangle(renderTarget, selectedItem.RawBounds, _engine.Rendering.Materials.Colors.Red, 2, 2, 0);
            }
        }
    }
}
