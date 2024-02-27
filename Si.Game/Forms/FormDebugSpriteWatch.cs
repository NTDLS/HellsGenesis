using Si.GameEngine.Core;
using Si.GameEngine.Sprites._Superclass;
using Si.GameEngine.Sprites.Enemies._Superclass;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Si.Game.Forms
{
    public partial class FormDebugSpriteWatch : Form
    {
        private readonly GameEngineCore _gameEngine;
        private readonly SpriteBase _sprite;
        private readonly Timer _timer = new();

        internal FormDebugSpriteWatch(GameEngineCore gameEngine, SpriteBase sprite)
        {
            InitializeComponent();
            _gameEngine = gameEngine;
            _sprite = sprite;

            _timer.Interval = 250;
            _timer.Tick += Timer_Tick;
            _timer.Start();

            splitContainerBody.Panel2Collapsed = true;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            UpdateWatch();
        }

        public void UpdateWatch()
        {
            listViewVariables.BeginUpdate();
            listViewVariables.SuspendLayout();

            if (_sprite is SpriteEnemyBase enemy)
            {
                UpsertVariable("AI Controller", $"{enemy.CurrentAIController}");
                UpsertVariable("Is Hostile", $"{enemy.IsHostile}");
            }

            UpsertVariable("UID", $"{_sprite.UID}");
            UpsertVariable("Multiplay UID", $"{_sprite.MultiplayUID}");
            UpsertVariable("Owner UID", $"{_sprite.OwnerUID:n0}");
            UpsertVariable("Name", $"{_sprite.GetType().Name}");
            UpsertVariable("Tag", $"{_sprite.SpriteTag:n0}");
            UpsertVariable("Is Visable?", $"{_sprite.Visable:n0}");
            UpsertVariable("Size", $"{_sprite.Size:n0}");
            UpsertVariable("Bounds", $"{_sprite.Bounds:n0}");
            UpsertVariable("Ready for Delete?", $"{_sprite.IsQueuedForDeletion}");
            UpsertVariable("Is Dead?", $"{_sprite.IsDeadOrExploded}");
            UpsertVariable("Location", $"{_sprite.Location}");
            UpsertVariable("Location (Render)", $"{_sprite.RenderLocation}");
            UpsertVariable("Background Offset", $"{_gameEngine.Display.RenderWindowPosition}");
            UpsertVariable("Angle Degrees 180", $"{_sprite.Velocity.Angle.DegreesSplit:n2}");
            UpsertVariable("Angle Radians", $"{_sprite.Velocity.Angle.Radians:n2}");
            UpsertVariable("Thrust %", $"{(_sprite.Velocity.ThrottlePercentage * 100):n2}");
            UpsertVariable("Boost %", $"{(_sprite.Velocity.BoostPercentage * 100):n2}");
            UpsertVariable("Recoil", $"{(_sprite.Velocity.RecoilPercentage * 100):n2}");
            UpsertVariable("Hull", $"{_sprite.HullHealth:n0}");
            UpsertVariable("Shield", $"{_sprite.ShieldHealth:n0}");
            UpsertVariable("Attachments", $"{_sprite.Attachments?.Count ?? 0:n0}");
            UpsertVariable("Highlight", $"{_sprite.IsHighlighted}");
            UpsertVariable("Is Fixed Position", $"{_sprite.IsFixedPosition}");
            UpsertVariable("Is Locked On", $"{_sprite.IsLockedOnHard}");
            UpsertVariable("Is Locked On (Soft)", $"{_sprite.IsLockedOnSoft:n0}");
            UpsertVariable("In Current Scaled Bounds", $"{_sprite.IsWithinCurrentScaledScreenBounds}");
            UpsertVariable("Visible Bounds", $"{_sprite.Bounds}");

            listViewVariables.ResumeLayout();
            listViewVariables.EndUpdate();
        }

        private void UpsertVariable(string name, string value)
        {
            var existingItem = FindItemByName(name);

            if (existingItem != null)
            {
                existingItem.SubItems[1].Text = value;
            }
            else
            {
                var newItem = new ListViewItem(name);
                newItem.SubItems.Add(value);
                listViewVariables.Items.Add(newItem);
            }
        }

        private ListViewItem FindItemByName(string name)
        {
            foreach (ListViewItem item in listViewVariables.Items)
            {
                if (item.SubItems[0].Text == name)
                {
                    return item;
                }
            }
            return null;
        }

        public void ClearText()
        {
            if (richTexLog.InvokeRequired)
            {
                richTexLog.Invoke(new Action(richTexLog.Clear));
            }
            else
            {
                richTexLog.Clear();
            }
        }

        public void WriteLogLine(string text, Color color)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string, Color>(WriteLogLine), text, color);
                return;
            }

            richTexLog.SuspendLayout();
            richTexLog.SelectionStart = richTexLog.TextLength;
            richTexLog.SelectionLength = 0;

            richTexLog.SelectionColor = color;
            richTexLog.AppendText($"{text}\r\n");
            richTexLog.SelectionColor = richTexLog.ForeColor;

            richTexLog.SelectionStart = richTexLog.Text.Length;
            richTexLog.ScrollToCaret();
            richTexLog.ResumeLayout();
        }

        public void WriteLog(string text, Color color)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string, Color>(WriteLog), text, color);
                return;
            }

            richTexLog.SuspendLayout();
            richTexLog.SelectionStart = richTexLog.TextLength;
            richTexLog.SelectionLength = 0;

            richTexLog.SelectionColor = color;
            richTexLog.AppendText($"{text}");
            richTexLog.SelectionColor = richTexLog.ForeColor;

            richTexLog.SelectionStart = richTexLog.Text.Length;
            richTexLog.ScrollToCaret();
            richTexLog.ResumeLayout();
        }
    }
}
