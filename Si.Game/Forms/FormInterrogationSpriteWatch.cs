using Si.GameEngine.Sprites._Superclass;
using Si.GameEngine.Sprites.Enemies._Superclass;
using Si.Library.Sprite;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Si.Game.Forms
{
    public partial class FormInterrogationSpriteWatch : Form
    {
        private readonly GameEngine.GameEngineCore _gameEngine;
        private readonly ISprite _sprite;
        private readonly Timer _timer = new();

        internal FormInterrogationSpriteWatch(GameEngine.GameEngineCore gameEngine, ISprite sprite)
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

            var sprite = _sprite as SpriteBase;

            if (sprite is SpriteEnemyBase enemy)
            {
                UpsertVariable("AI Controller", $"{enemy.CurrentAIController}");
                UpsertVariable("Is Hostile", $"{enemy.IsHostile}");
            }

            UpsertVariable("UID", $"{sprite.UID}");
            UpsertVariable("Multiplay UID", $"{sprite.MultiplayUID}");
            UpsertVariable("Owner UID", $"{sprite.OwnerUID:n0}");
            UpsertVariable("Name", $"{sprite.GetType().Name}");
            UpsertVariable("Tag", $"{sprite.SpriteTag:n0}");
            UpsertVariable("Is Visable?", $"{sprite.Visable:n0}");
            UpsertVariable("Size", $"{sprite.Size:n0}");
            UpsertVariable("Bounds", $"{sprite.Bounds:n0}");
            UpsertVariable("Ready for Delete?", $"{sprite.IsQueuedForDeletion}");
            UpsertVariable("Is Dead?", $"{sprite.IsDeadOrExploded}");
            UpsertVariable("Location", $"{sprite.Location}");
            UpsertVariable("Location (Render)", $"{sprite.RenderLocation}");
            UpsertVariable("Background Offset", $"{_gameEngine.Display.RenderWindowPosition}");
            UpsertVariable("Angle", $"{sprite.Velocity.Angle:n2}");
            UpsertVariable("Angle Degrees", $"{sprite.Velocity.Angle.Degrees:n2}");
            UpsertVariable("Angle Degrees (Signed)", $"{sprite.Velocity.Angle.DegreesSigned:n2}");
            UpsertVariable("Angle Radians", $"{sprite.Velocity.Angle.Radians:n2}");
            UpsertVariable("Angle Radians (Signed)", $"{sprite.Velocity.Angle.RadiansSigned:n2}");
            UpsertVariable("Thrust %", $"{(sprite.Velocity.ForwardMomentium * 100):n2}");
            UpsertVariable("Boost %", $"{(sprite.Velocity.ForwardBoostMomentium * 100):n2}");
            UpsertVariable("Hull", $"{sprite.HullHealth:n0}");
            UpsertVariable("Shield", $"{sprite.ShieldHealth:n0}");
            UpsertVariable("Attachments", $"{sprite.Attachments?.Count ?? 0:n0}");
            UpsertVariable("Highlight", $"{sprite.IsHighlighted}");
            UpsertVariable("Is Fixed Position", $"{sprite.IsFixedPosition}");
            UpsertVariable("Is Locked On", $"{sprite.IsLockedOnHard}");
            UpsertVariable("Is Locked On (Soft)", $"{sprite.IsLockedOnSoft:n0}");
            UpsertVariable("In Current Scaled Bounds", $"{sprite.IsWithinCurrentScaledScreenBounds}");
            UpsertVariable("Visible Bounds", $"{sprite.Bounds}");

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
