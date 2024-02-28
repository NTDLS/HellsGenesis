using SharpDX.DirectInput;
using Si.GameEngine.Core;
using Si.GameEngine.Sprites.MenuItems;
using Si.Library.Types.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Si.Library.SiConstants;

namespace Si.GameEngine.Menus._Superclass
{
    /// <summary>
    /// A menu instance. Allows for setting title text, adding items and managing selections.
    /// </summary>
    public class MenuBase
    {
        protected GameEngineCore _gameEngine;
        private DateTime _lastInputHandled = DateTime.UtcNow;

        public List<SpriteMenuItem> Items { get; private set; } = new();
        public bool QueuedForDeletion { get; private set; }
        public Guid UID { get; private set; } = Guid.NewGuid();

        public T MenuItemByKey<T>(string key) where T : SpriteMenuItem
        {
            return Items.Where(o => o.Key == key).First() as T;
        }

        public List<SpriteMenuItem> VisibleSelectableItems() =>
            Items.Where(o => o.Visable == true
            && (o.ItemType == SiMenuItemType.SelectableItem || o.ItemType == SiMenuItemType.SelectableTextInput)).ToList();

        #region Events.

        public delegate void SelectionChangedEvent(SpriteMenuItem item);
        /// <summary>
        /// The player moved the selection cursor.
        /// </summary>
        /// <param name="item"></param>
        public event SelectionChangedEvent OnSelectionChanged;

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
        public event ExecuteSelectionEvent OnExecuteSelection;


        /// <summary>
        /// The player has hit the escape key.
        /// </summary>
        /// <returns>Return true to close the current menu.</returns>
        public delegate bool EscapeEvent();
        /// <summary>
        /// The player has hit the escape key.
        /// </summary>
        /// <param name="item"></param>
        public event EscapeEvent OnEscape;

        public delegate void CleanupEvent();
        /// <summary>
        /// Called when the menu is being destroyed. This is a good place to cleanup.
        /// </summary>
        /// <param name="item"></param>
        public event CleanupEvent OnCleanup;

        public void InvokeCleanup() => OnCleanup?.Invoke();

        #endregion

        public MenuBase(GameEngineCore gameEngine)
        {
            _gameEngine = gameEngine;
        }

        //public void Show() => Items.ForEach(o => o.Visable = true);
        //public void Hide() => Items.ForEach(o => o.Visable = false);
        public bool HandlesEscape() => (OnEscape != null);
        public void QueueForDelete() => QueuedForDeletion = true;
        public void Close() => QueueForDelete();

        public SpriteMenuItem CreateAndAddTitleItem(SiPoint location, string text)
        {
            var item = new SpriteMenuItem(_gameEngine, this, _gameEngine.Rendering.TextFormats.MenuTitle, _gameEngine.Rendering.Materials.Brushes.OrangeRed, location)
            {
                Text = text,
                ItemType = SiMenuItemType.Title
            };
            AddMenuItem(item);
            return item;
        }

        public SpriteMenuItem CreateAndAddTextblock(SiPoint location, string text)
        {
            var item = new SpriteMenuItem(_gameEngine, this, _gameEngine.Rendering.TextFormats.MenuGeneral, _gameEngine.Rendering.Materials.Brushes.LawnGreen, location)
            {
                Text = text,
                ItemType = SiMenuItemType.Textblock
            };
            AddMenuItem(item);
            return item;
        }

        public SpriteMenuItem CreateAndAddSelectableItem(SiPoint location, string key, string text)
        {
            var item = new SpriteMenuItem(_gameEngine, this, _gameEngine.Rendering.TextFormats.MenuItem, _gameEngine.Rendering.Materials.Brushes.OrangeRed, location)
            {
                Key = key,
                Text = text,
                ItemType = SiMenuItemType.SelectableItem
            };
            AddMenuItem(item);
            return item;
        }

        public SpriteMenuSelectableTextInput CreateAndAddSelectableTextInput(SiPoint location, string key, string text = "")
        {
            var item = new SpriteMenuSelectableTextInput(_gameEngine, this, _gameEngine.Rendering.TextFormats.TextInputItem, _gameEngine.Rendering.Materials.Brushes.Orange, location)
            {
                Key = key,
                Text = text,
                ItemType = SiMenuItemType.SelectableTextInput
            };
            AddMenuItem(item);
            return item;
        }

        public void AddMenuItem(SpriteMenuItem item)
        {
            _gameEngine.Menus.Use(o => // <-- Do we really need this lock?
            {
                Items.Add(item);
            });
        }

        public void HandleInput()
        {
            if (QueuedForDeletion)
            {
                Thread.Sleep(1);
                return;
            }

            var selectedTextInput = Items.OfType<SpriteMenuSelectableTextInput>().Where(o => o.Selected).FirstOrDefault();

            _gameEngine.Input.CollectDetailedKeyInformation(selectedTextInput != null);

            //Text typing is not subject to _lastInputHandled limits because it is based on cycled keys, not depressed keys.
            if (selectedTextInput != null)
            {
                //Since we do allow for backspace repetitions, we will enforce a _lastInputHandled limit.
                if (_gameEngine.Input.DepressedKeys.Contains(Key.Back))
                {
                    if ((DateTime.UtcNow - _lastInputHandled).TotalMilliseconds >= 100)
                    {
                        _lastInputHandled = DateTime.UtcNow;
                        selectedTextInput.Backspace();
                        _gameEngine.Audio.Click.Play();
                    }
                    return;
                }

                if (_gameEngine.Input.TypedString.Length > 0)
                {
                    _gameEngine.Audio.Click.Play();
                    selectedTextInput.Append(_gameEngine.Input.TypedString);
                }
            }

            if ((DateTime.UtcNow - _lastInputHandled).TotalMilliseconds < 200)
            {
                return; //We have to keep the menues from going crazy.
            }

            if (_gameEngine.Input.IsKeyPressed(SiPlayerKey.Enter))
            {
                _gameEngine.Audio.Click.Play();

                _lastInputHandled = DateTime.UtcNow;

                var selectedItem = (from o in Items where o.ItemType == SiMenuItemType.SelectableItem && o.Selected == true select o).FirstOrDefault();
                if (selectedItem != null)
                {
                    //Menu executions may block execution if run in the same thread. For example, the menu executin may be looking to remove all
                    //  items from the screen and wait for them to be removed. Problem is, the same thread that calls the menuexecution is the same
                    //  one that removes items from the screen, therefor the "while(itemsExist)" loop would never finish.
                    //  
                    Task.Run(() => OnExecuteSelection?.Invoke(selectedItem)).ContinueWith(o =>
                    {
                        if (o.Result == true)
                        {
                            Close();
                        }
                        else
                        {
                        }
                    });
                }
            }
            else if (_gameEngine.Input.IsKeyPressed(SiPlayerKey.Escape))
            {
                _gameEngine.Audio.Click.Play();

                _lastInputHandled = DateTime.UtcNow;

                //Menu executions may block execution if run in the same thread. For example, the menu executin may be looking to remove all
                //  items from the screen and wait for them to be removed. Problem is, the same thread that calls the menuexecution is the same
                //  one that removes items from the screen, therefor the "while(itemsExist)" loop would never finish.
                //  
                Task.Run(() => OnEscape?.Invoke()).ContinueWith(o =>
                {
                    if (o.Result == true)
                    {
                        Close();
                    }
                    else
                    {
                    }
                });
            }

            if (_gameEngine.Input.IsKeyPressed(SiPlayerKey.Right)
                || _gameEngine.Input.IsKeyPressed(SiPlayerKey.Down)
                //|| _gameEngine.Input.IsKeyPressed(SiPlayerKey.Reverse)
                //|| _gameEngine.Input.IsKeyPressed(SiPlayerKey.RotateClockwise)
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
                            _gameEngine.Audio.Click.Play();

                            //Menu executions may block execution if run in the same thread. For example, the menu executin may be looking to remove all
                            //  items from the screen and wait for them to be removed. Problem is, the same thread that calls the menuexecution is the same
                            //  one that removes items from the screen, therefor the "while(itemsExist)" loop would never finish.
                            //  
                            Task.Run(() => OnSelectionChanged?.Invoke(selectedItem));
                        }
                    }
                }
            }

            if (_gameEngine.Input.IsKeyPressed(SiPlayerKey.Left)
                || _gameEngine.Input.IsKeyPressed(SiPlayerKey.Up)
                //|| _gameEngine.Input.IsKeyPressed(SiPlayerKey.Forward)
                //|| _gameEngine.Input.IsKeyPressed(SiPlayerKey.RotateCounterClockwise)
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
                            _gameEngine.Audio.Click.Play();

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
                _gameEngine.Rendering.DrawRectangleAt(renderTarget, selectedItem.RawBounds, 0, _gameEngine.Rendering.Materials.Colors.Red, 2, 2);
            }
        }
    }
}
