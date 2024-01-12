﻿using Si.GameEngine.Engine;
using Si.GameEngine.Sprites;
using Si.GameEngine.Sprites.Enemies.BasesAndInterfaces;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Si.Game.Forms
{
    public partial class FormDebugSpriteWatch : Form
    {
        private readonly EngineCore _gameCore;
        private readonly SpriteBase _sprite;
        private readonly Timer _timer = new();

        internal FormDebugSpriteWatch(EngineCore gameCore, SpriteBase sprite)
        {
            InitializeComponent();
            _gameCore = gameCore;
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
            UpsertVariable("Ready for Delete?", $"{_sprite.QueuedForDeletion}");
            UpsertVariable("Is Dead?", $"{_sprite.IsDeadOrExploded}");
            UpsertVariable("Location (Real)", $"{_sprite.RealLocation}");
            UpsertVariable("Location (Local)", $"{_sprite.LocalLocation}");
            UpsertVariable("Location (Multiplay)", $"{_sprite.MultiplayLocation}");
            UpsertVariable("Background Offset", $"{_gameCore.Display.BackgroundOffset}");
            UpsertVariable("Location (Virtual)", $"{_sprite.VirtualLocation},");
            UpsertVariable("Angle Degrees", $"{_sprite.Velocity.Angle}");
            UpsertVariable("Angle Radians", $"{_sprite.Velocity.Angle.Radians:n2}");
            UpsertVariable("Angle Radians Unadjusted", $"{_sprite.Velocity.Angle.RadiansUnadjusted:n2}");
            UpsertVariable("Thrust %", $"{(_sprite.Velocity.ThrottlePercentage * 100):n2}");
            UpsertVariable("Boost %", $"{(_sprite.Velocity.BoostPercentage * 100):n2}");
            UpsertVariable("Recoil", $"{(_sprite.Velocity.RecoilPercentage * 100):n2}");
            UpsertVariable("Hull", $"{_sprite.HullHealth:n0}");
            UpsertVariable("Shield", $"{_sprite.ShieldHealth:n0}");
            UpsertVariable("Rotation Mode", $"{_sprite.RotationMode}");
            UpsertVariable("Attachments", $"{(_sprite.Attachments?.Count() ?? 0):n0}");
            UpsertVariable("Highlight", $"{_sprite.Highlight}");
            UpsertVariable("Is Fixed Position", $"{_sprite.IsFixedPosition}");
            UpsertVariable("Is Locked On", $"{_sprite.IsLockedOn}");
            UpsertVariable("Is Locked On (Soft)", $"{_sprite.IsLockedOnSoft:n0}");
            UpsertVariable("In Current Scaled Bounds", $"{_sprite.IsWithinCurrentScaledScreenBounds}");
            UpsertVariable("Quandrant", $"{_sprite.Quadrant}");
            UpsertVariable("Visible Bounds", $"{_sprite.VisibleBounds}");

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
