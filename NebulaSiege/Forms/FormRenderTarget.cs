using NebulaSiege.Engine;
using NebulaSiege.Engine.Types.Geometry;
using NebulaSiege.Sprites;
using NebulaSiege.Sprites.Enemies.BaseClasses;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NebulaSiege
{
    public partial class FormRenderTarget : Form
    {
        readonly List<SpriteBase> highlightedSprites = new();
        private readonly ToolTip _interrogationTip = new ToolTip();

        private readonly EngineCore _core;
        private readonly bool _fullScreen = false;

        public FormRenderTarget()
        {
            InitializeComponent();

            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);

            var settings = EngineCore.LoadSettings();

            if (settings.AlwaysOnTop)
            {
                TopMost = true;
            }

            if (settings.FullScreen)
            {
                FormBorderStyle = FormBorderStyle.None;
                Width = Screen.PrimaryScreen.Bounds.Width;
                Height = Screen.PrimaryScreen.Bounds.Height;
                ShowInTaskbar = true;
                //TopMost = true; This is a total pain for debugging.
                WindowState = FormWindowState.Maximized;
            }
            else
            {
                ClientSize = settings.Resolution;
                StartPosition = FormStartPosition.CenterScreen;
            }

            var drawingSurface = new Control
            {
                Dock = DockStyle.Fill
            };
            Controls.Add(drawingSurface);

            _core = new EngineCore(drawingSurface);

            _core.OnStopEngine += (EngineCore sender) =>
            {   //If the engine is stopped, close the main form.
                Invoke((MethodInvoker)delegate
                {
                    Close();
                });
            };

            Shown += (object sender, EventArgs e) => _core.StartEngine();
            FormClosing += (sender, e) => _core.StopEngine();

            drawingSurface.MouseEnter += (object sender, EventArgs e) => { if (_fullScreen) { Cursor.Hide(); } };
            drawingSurface.MouseLeave += (object sender, EventArgs e) => { if (_fullScreen) { Cursor.Show(); } };

            drawingSurface.KeyUp += FormRenderTarget_KeyUp;

            if (settings.EnableSpriteInterrogation)
            {
                drawingSurface.MouseDown += FormRenderTarget_MouseDown;
                drawingSurface.MouseMove += FormRenderTarget_MouseMove;
            }
        }

        #region Debug interactions.
        private void FormRenderTarget_MouseMove(object sender, MouseEventArgs e)
        {
            double x = e.X + _core.Display.OverdrawSize.Width / 2;
            double y = e.Y + _core.Display.OverdrawSize.Height / 2;

            //Debug.Print($"x{x:n1}, y{y:n1} => Player x{_core.Player.Sprite.X:n1},x{_core.Player.Sprite.Y:n1}");

            foreach (var sprite in highlightedSprites)
            {
                sprite.Highlight = false;
            }

            highlightedSprites.Clear();

            var sprites = _core.Sprites.Intersections(new NsPoint(x, y), new NsPoint(1, 1));
            if (_core.Player.Sprite.Intersects(new NsPoint(x, y), new NsPoint(1, 1)))
            {
                sprites.Add(_core.Player.Sprite);
            }

            foreach (var sprite in sprites)
            {
                highlightedSprites.Add(sprite);
                sprite.Highlight = true;
            }
        }

        private void FormRenderTarget_MouseDown(object sender, MouseEventArgs e)
        {
            double x = e.X + _core.Display.OverdrawSize.Width / 2;
            double y = e.Y + _core.Display.OverdrawSize.Height / 2;

            var sprites = _core.Sprites.Intersections(new NsPoint(x, y), new NsPoint(1, 1));
            if (_core.Player.Sprite.Intersects(new NsPoint(x, y), new NsPoint(1, 1)))
            {
                sprites.Add(_core.Player.Sprite);
            }

            var sprite = sprites.FirstOrDefault();

            if (sprite != null)
            {
                if (e.Button == MouseButtons.Right)
                {
                    var menu = new ContextMenuStrip();

                    menu.ItemClicked += Menu_ItemClicked;
                    if (sprite is SpriteEnemyBase)
                    {
                        menu.Items.Add("Save Brain").Tag = sprite;
                        menu.Items.Add("View Brain").Tag = sprite;
                    }
                    menu.Items.Add("Delete").Tag = sprite;

                    var location = new Point((int)e.X + 10, (int)e.Y);
                    menu.Show(_core.Display.DrawingSurface, location);
                }
                else if (e.Button == MouseButtons.Left)
                {
                    var text = new StringBuilder();

                    text.AppendLine($"Type: {sprite.GetType().Name}");
                    text.AppendLine($"UID: {sprite.UID}");
                    text.AppendLine($"X,Y: {sprite.X:n2},{sprite.Y:n2}");

                    if (sprite is SpriteEnemyBase)
                    {
                        var enemy = (SpriteEnemyBase)sprite;

                        text.AppendLine($"Hit Points: {enemy.HullHealth:n0}");
                        text.AppendLine($"Is Locked-on: {enemy.IsLockedOn}");
                        text.AppendLine($"Is Locked-on (Soft): {enemy.IsLockedOnSoft:n0}");
                        text.AppendLine($"Shield Points: {enemy.ShieldHealth:n0}");
                        text.AppendLine($"MaxSpeed: {enemy.Velocity.MaxSpeed:n2}");
                        text.AppendLine($"Angle: {enemy.Velocity.Angle.Degrees:n2}° {enemy.Velocity.Angle:n2}");
                        text.AppendLine($"Throttle Percent: {enemy.Velocity.ThrottlePercentage:n2}");

                        if (enemy.CurrentAIController != null)
                        {
                            text.AppendLine($"AI: {enemy.CurrentAIController.GetType().Name}");
                        }
                    }

                    if (text.Length > 0)
                    {
                        var location = new Point((int)e.X, (int)e.Y - sprite.Size.Height);
                        _interrogationTip.Show(text.ToString(), _core.Display.DrawingSurface, location, 5000);
                    }
                }
            }
        }

        private void Menu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (sender == null) return;
            var menu = (ContextMenuStrip)sender;

            menu.Close();

            var sprite = e.ClickedItem?.Tag as SpriteBase;
            if (sprite == null) return;

            if (e.ClickedItem?.Text == "Delete")
            {
                sprite.QueueForDelete();
            }
            else if (e.ClickedItem?.Text == "Save Brain")
            {
                if (sprite is SpriteEnemyBase)
                {
                    var enemy = (SpriteEnemyBase)sprite;

                    bool wasPaused = _core.IsPaused();
                    if (wasPaused == false)
                    {
                        _core.TogglePause();
                    }

                    using (var fbd = new FolderBrowserDialog())
                    {
                        if (fbd.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                        {
                            foreach (var aiController in enemy.AIControllers)
                            {
                                var fullPath = Path.Combine(fbd.SelectedPath, $"{aiController.Key}.txt");
                                aiController.Value.Network.Save(fullPath);
                            }
                        }
                    }

                    if (wasPaused == false)
                    {
                        _core.TogglePause();
                    }
                }
            }
            else if (e.ClickedItem?.Text == "View Brain")
            {
                if (sprite is SpriteEnemyBase)
                {
                    var enemy = (SpriteEnemyBase)sprite;

                    bool wasPaused = _core.IsPaused();
                    if (wasPaused == false)
                    {
                        _core.TogglePause();
                    }

                    var builder = new StringBuilder();

                    foreach (var aiController in enemy.AIControllers)
                    {
                        builder.AppendLine($"<!-- {aiController.Key} -->");
                        builder.AppendLine(aiController.Value.Network.Serialize());
                    }

                    using (var form = new FormViewBrain(builder.ToString()))
                    {
                        form.ShowDialog();
                    }

                    if (wasPaused == false)
                    {
                        _core.TogglePause();
                    }
                }
            }
        }

        #endregion

        private void FormRenderTarget_KeyUp(object sender, KeyEventArgs e)
        {
            _core.Input.HandleSingleKeyPress(e.KeyCode);

            if (e.KeyCode == Keys.Escape)
            {
                //We do not want the escape key to inturrupt menus.
                if (_core.Menus.VisibleMenuHandlesEscape() == false)
                {
                    _core.Pause();

                    if (MessageBox.Show("Are you sure you want to quit?", "Afraid to go on?", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                    {
                        Close();
                    }
                    else
                    {
                        _core.Resume();
                    }
                }
            }
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            // Prevent background painting to avoid flickering
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // Prevent painting to avoid flickering.
        }
    }
}
