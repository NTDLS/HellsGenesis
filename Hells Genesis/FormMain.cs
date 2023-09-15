using HG.Actors.BaseClasses;
using HG.Actors.Enemies.BaseClasses;
using HG.Engine;
using HG.Engine.Types.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HG
{
    public partial class FormMain : Form
    {
        readonly List<ActorBase> highlightedActors = new();
        private readonly ToolTip _interrogationTip = new ToolTip();

        private readonly EngineCore _core;
        private readonly bool _fullScreen = false;

        public FormMain()
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
                TopMost = true;
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

            Shown += (object sender, EventArgs e) => _core.Start();
            FormClosing += (sender, e) => _core.Stop();

            drawingSurface.MouseEnter += (object sender, EventArgs e) => { if (_fullScreen) { Cursor.Hide(); } };
            drawingSurface.MouseLeave += (object sender, EventArgs e) => { if (_fullScreen) { Cursor.Show(); } };
            drawingSurface.KeyDown += FormMain_KeyDown;
            drawingSurface.KeyUp += FormMain_KeyUp;

            if (settings.EnableActorsInterrogation)
            {
                drawingSurface.MouseDown += FormDirect2D_MouseDown;
                drawingSurface.MouseMove += FormDirect2D_MouseMove;
            }
        }

        #region Debug interactions.
        private void FormDirect2D_MouseMove(object sender, MouseEventArgs e)
        {
            double x = e.X + _core.Display.OverdrawSize.Width / 2;
            double y = e.Y + _core.Display.OverdrawSize.Height / 2;

            foreach (var actor in highlightedActors)
            {
                actor.Highlight = false;
            }

            highlightedActors.Clear();

            var actors = _core.Actors.Intersections(new HgPoint(x, y), new HgPoint(1, 1));
            if (_core.Player.Actor.Intersects(new HgPoint(x, y), new HgPoint(1, 1)))
            {
                actors.Add(_core.Player.Actor);
            }

            foreach (var actor in actors)
            {
                highlightedActors.Add(actor);
                actor.Highlight = true;
            }
        }

        private void FormDirect2D_MouseDown(object sender, MouseEventArgs e)
        {
            double x = e.X + _core.Display.OverdrawSize.Width / 2;
            double y = e.Y + _core.Display.OverdrawSize.Height / 2;

            var actors = _core.Actors.Intersections(new HgPoint(x, y), new HgPoint(1, 1));
            if (_core.Player.Actor.Intersects(new HgPoint(x, y), new HgPoint(1, 1)))
            {
                actors.Add(_core.Player.Actor);
            }

            var actor = actors.FirstOrDefault();

            if (actor != null)
            {
                if (e.Button == MouseButtons.Right)
                {
                    var menu = new ContextMenuStrip();

                    menu.ItemClicked += Menu_ItemClicked;
                    if (actor is EnemyBase)
                    {
                        menu.Items.Add("Save Brain").Tag = actor;
                        menu.Items.Add("View Brain").Tag = actor;
                    }
                    menu.Items.Add("Delete").Tag = actor;

                    var location = new Point((int)e.X + 10, (int)e.Y);
                    menu.Show(_core.Display.DrawingSurface, location);
                }
                else if (e.Button == MouseButtons.Left)
                {
                    var text = new StringBuilder();

                    text.AppendLine($"Type: {actor.GetType().Name}");
                    text.AppendLine($"UID: {actor.UID}");
                    text.AppendLine($"X,Y: {actor.X:n2},{actor.Y:n2}");

                    if (actor is EnemyBase)
                    {
                        var enemy = (EnemyBase)actor;

                        text.AppendLine($"Hit Points: {enemy.HullHealth:n0}");
                        text.AppendLine($"Is Locked-on: {enemy.IsLockedOn}");
                        text.AppendLine($"Is Locked-on (Soft): {enemy.IsLockedOnSoft:n0}");
                        text.AppendLine($"Shield Points: {enemy.ShieldHealth:n0}");
                        text.AppendLine($"MaxSpeed: {enemy.Velocity.MaxSpeed:n2}");
                        text.AppendLine($"Angle: {enemy.Velocity.Angle.Degrees:n2}° {enemy.Velocity.Angle:n2}");
                        text.AppendLine($"Throttle Percent: {enemy.Velocity.ThrottlePercentage:n2}");
                        text.AppendLine($"Primary Weapon: {enemy.PrimaryWeapon?.GetType()?.Name}");
                        text.AppendLine($"Selected Weapon: {enemy.SelectedSecondaryWeapon?.GetType()?.Name}");

                        if (enemy.DefaultAIController != null)
                        {
                            text.AppendLine($"AI: {enemy.DefaultAIController.GetType().Name}");
                        }
                    }

                    if (text.Length > 0)
                    {
                        var location = new Point((int)e.X, (int)e.Y - actor.Size.Height);
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

            var actor = e.ClickedItem?.Tag as ActorBase;
            if (actor == null) return;

            if (e.ClickedItem?.Text == "Delete")
            {
                actor.QueueForDelete();
            }
            else if (e.ClickedItem?.Text == "Save Brain")
            {
                if (actor is EnemyBase)
                {
                    var enemy = (EnemyBase)actor;

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
                if (actor is EnemyBase)
                {
                    var enemy = (EnemyBase)actor;

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

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if ((keyData & Keys.KeyCode) == Keys.Left) _core.Input.KeyStateChanged(HgPlayerKey.Left, HgKeyPressState.Down);
            else if ((keyData & Keys.KeyCode) == Keys.Right) _core.Input.KeyStateChanged(HgPlayerKey.Right, HgKeyPressState.Down);
            else if ((keyData & Keys.KeyCode) == Keys.Up) _core.Input.KeyStateChanged(HgPlayerKey.Up, HgKeyPressState.Down);
            else if ((keyData & Keys.KeyCode) == Keys.Down) _core.Input.KeyStateChanged(HgPlayerKey.Down, HgKeyPressState.Down);
            else return base.ProcessCmdKey(ref msg, keyData);

            _core.Input.HandleSingleKeyPress((keyData & Keys.KeyCode));

            return true; // Mark the key as handled
        }

        private void FormMain_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ShiftKey) _core.Input.KeyStateChanged(HgPlayerKey.SpeedBoost, HgKeyPressState.Down);
            if (e.KeyCode == Keys.W) _core.Input.KeyStateChanged(HgPlayerKey.Forward, HgKeyPressState.Down);
            if (e.KeyCode == Keys.A) _core.Input.KeyStateChanged(HgPlayerKey.RotateCounterClockwise, HgKeyPressState.Down);
            if (e.KeyCode == Keys.S) _core.Input.KeyStateChanged(HgPlayerKey.Reverse, HgKeyPressState.Down);
            if (e.KeyCode == Keys.D) _core.Input.KeyStateChanged(HgPlayerKey.RotateClockwise, HgKeyPressState.Down);
            if (e.KeyCode == Keys.Space) _core.Input.KeyStateChanged(HgPlayerKey.PrimaryFire, HgKeyPressState.Down);
            if (e.KeyCode == Keys.ControlKey) _core.Input.KeyStateChanged(HgPlayerKey.SecondaryFire, HgKeyPressState.Down);
            if (e.KeyCode == Keys.Escape) _core.Input.KeyStateChanged(HgPlayerKey.Escape, HgKeyPressState.Down);
            if (e.KeyCode == Keys.Enter) _core.Input.KeyStateChanged(HgPlayerKey.Enter, HgKeyPressState.Down);

            _core.Input.HandleSingleKeyPress(e.KeyCode);
        }

        private void FormMain_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
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

            if (e.KeyCode == Keys.ShiftKey) _core.Input.KeyStateChanged(HgPlayerKey.SpeedBoost, HgKeyPressState.Up);
            if (e.KeyCode == Keys.W) _core.Input.KeyStateChanged(HgPlayerKey.Forward, HgKeyPressState.Up);
            if (e.KeyCode == Keys.A) _core.Input.KeyStateChanged(HgPlayerKey.RotateCounterClockwise, HgKeyPressState.Up);
            if (e.KeyCode == Keys.S) _core.Input.KeyStateChanged(HgPlayerKey.Reverse, HgKeyPressState.Up);
            if (e.KeyCode == Keys.D) _core.Input.KeyStateChanged(HgPlayerKey.RotateClockwise, HgKeyPressState.Up);
            if (e.KeyCode == Keys.Space) _core.Input.KeyStateChanged(HgPlayerKey.PrimaryFire, HgKeyPressState.Up);
            if (e.KeyCode == Keys.ControlKey) _core.Input.KeyStateChanged(HgPlayerKey.SecondaryFire, HgKeyPressState.Up);
            if (e.KeyCode == Keys.Escape) _core.Input.KeyStateChanged(HgPlayerKey.Escape, HgKeyPressState.Up);
            if (e.KeyCode == Keys.Left) _core.Input.KeyStateChanged(HgPlayerKey.Left, HgKeyPressState.Up);
            if (e.KeyCode == Keys.Right) _core.Input.KeyStateChanged(HgPlayerKey.Right, HgKeyPressState.Up);
            if (e.KeyCode == Keys.Up) _core.Input.KeyStateChanged(HgPlayerKey.Up, HgKeyPressState.Up);
            if (e.KeyCode == Keys.Down) _core.Input.KeyStateChanged(HgPlayerKey.Down, HgKeyPressState.Up);
            if (e.KeyCode == Keys.Enter) _core.Input.KeyStateChanged(HgPlayerKey.Enter, HgKeyPressState.Up);
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
