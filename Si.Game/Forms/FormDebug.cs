using Si.GameEngine.Core;
using Si.GameEngine.Core.Debug._Superclass;
using Si.GameEngine.Sprites._Superclass;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Si.Game.Forms
{
    public partial class FormDebug : Form, IDebugForm
    {
        private readonly GameEngineCore _gameEngine;
        private readonly List<string> _commandHistory = new();
        private int _commandHistoryIndex = 0;
        private DateTime _lastTabKeyTimestamp = DateTime.UtcNow;
        private bool _IsInHistoryBrowseMode = false;

        private void FormDebug_Load(object sender, EventArgs e)
        {
            foreach (var command in _gameEngine.Debug.CommandParser.Commands.OrderBy(o => o.Name))
            {
                var item = new ListViewItem(command.Name);

                var toolTipText = new StringBuilder($"{command.Name} (");

                foreach (var param in command.Parameters)
                {
                    if (param.IsRequired)
                    {
                        toolTipText.Append($"{param.CommandParameterType} [{param.Name}]");
                    }
                    else
                    {
                        toolTipText.Append($"{param.CommandParameterType} {param.Name}");
                    }

                    if (string.IsNullOrEmpty(param.DefaultValue) == false)
                    {
                        toolTipText.Append($" = '{param.DefaultValue}'");
                    }

                    toolTipText.Append(", ");
                }

                if (command.Parameters.Count > 0)
                {
                    toolTipText.Length -= 2;
                }

                toolTipText.AppendLine(")");
                toolTipText.Append(command.Description);

                item.ToolTipText = toolTipText.ToString().Trim();
                item.SubItems.Add(command.Description);
                listViewCommands.Items.Add(item);
            }

            var suggestions = _gameEngine.Debug.CommandParser.Commands.Select(o => o.Name).ToArray();

            var allowedTypes = new AutoCompleteStringCollection();
            allowedTypes.AddRange(suggestions);
            textBoxCommand.AutoCompleteCustomSource = allowedTypes;
            textBoxCommand.AutoCompleteMode = AutoCompleteMode.Suggest;
            textBoxCommand.AutoCompleteSource = AutoCompleteSource.CustomSource;
        }

        internal FormDebug(GameEngineCore gameEngine)
        {
            InitializeComponent();

            textBoxCommand.Font = new Font("Courier New", 10, FontStyle.Regular);
            richTextBoxOutput.Font = new Font("Courier New", 10, FontStyle.Regular);

            AcceptButton = buttonExecute;

            _gameEngine = gameEngine;

            Shown += (object sender, EventArgs e) => textBoxCommand.Focus();

            textBoxCommand.KeyUp += TextBoxCommand_KeyUp;
            listViewCommands.MouseDoubleClick += ListViewCommands_MouseDoubleClick;

            FormClosing += (object sender, FormClosingEventArgs e) =>
            {
                gameEngine.Debug.ToggleVisibility();
                e.Cancel = true;
            };

            textBoxCommand.LostFocus += TextBoxCommand_LostFocus;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Tab)
            {
                _lastTabKeyTimestamp = DateTime.UtcNow;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void TextBoxCommand_LostFocus(object sender, EventArgs e)
        {
            if ((DateTime.UtcNow - _lastTabKeyTimestamp).TotalMilliseconds < 100)
            {
                textBoxCommand.Focus();
            }
        }

        private void ListViewCommands_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listViewCommands.SelectedItems.Count > 0)
            {
                var selectedItem = listViewCommands.SelectedItems[0];
                textBoxCommand.Text = selectedItem.Text;
            }
        }

        private void TextBoxCommand_KeyUp(object sender, KeyEventArgs e)
        {
            if (textBoxCommand.Text.Length == 0 && (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down))
            {
                //If there is no text in the box, then hitting UP or DOWN enters history browse mode.
                _IsInHistoryBrowseMode = true;
            }

            if (_IsInHistoryBrowseMode == false)
            {
                return;
            }

            if (e.KeyCode == Keys.Up)
            {
                if (_commandHistoryIndex > 0)
                {
                    _commandHistoryIndex--;
                    textBoxCommand.Text = _commandHistory[_commandHistoryIndex];
                    textBoxCommand.SelectionStart = textBoxCommand.Text.Length;
                    textBoxCommand.SelectionLength = 0;
                }
            }
            else if (e.KeyCode == Keys.Down)
            {
                if (_commandHistoryIndex <= _commandHistory.Count - 1)
                {
                    textBoxCommand.Text = _commandHistory[_commandHistoryIndex];
                    _commandHistoryIndex++;
                    textBoxCommand.SelectionStart = textBoxCommand.Text.Length;
                    textBoxCommand.SelectionLength = 0;
                }
            }
            else
            {
                //Any other key press leaves history browse mode.
                _IsInHistoryBrowseMode = false;
            }
        }

        private void ButtonExecute_Click(object sender, EventArgs e)
        {
            var command = textBoxCommand.Text.Trim();
            textBoxCommand.Text = "";

            if (string.IsNullOrEmpty(command) == false)
            {
                _commandHistory.Add(command);
                _commandHistoryIndex = _commandHistory.Count;

                _gameEngine.Debug.EnqueueCommand(command);
            }
        }

        public void ClearText()
        {
            if (richTextBoxOutput.InvokeRequired)
            {
                richTextBoxOutput.Invoke(new Action(richTextBoxOutput.Clear));
            }
            else
            {
                richTextBoxOutput.Clear();
            }
        }

        public void WriteLine(string text, Color color)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string, Color>(WriteLine), text, color);
                return;
            }

            richTextBoxOutput.SuspendLayout();
            richTextBoxOutput.SelectionStart = richTextBoxOutput.TextLength;
            richTextBoxOutput.SelectionLength = 0;

            richTextBoxOutput.SelectionColor = color;
            richTextBoxOutput.AppendText($"{text}\r\n");
            richTextBoxOutput.SelectionColor = richTextBoxOutput.ForeColor;

            richTextBoxOutput.SelectionStart = richTextBoxOutput.Text.Length;
            richTextBoxOutput.ScrollToCaret();
            richTextBoxOutput.ResumeLayout();
        }

        public void Write(string text, Color color)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string, Color>(Write), text, color);
                return;
            }

            richTextBoxOutput.SuspendLayout();
            richTextBoxOutput.SelectionStart = richTextBoxOutput.TextLength;
            richTextBoxOutput.SelectionLength = 0;

            richTextBoxOutput.SelectionColor = color;
            richTextBoxOutput.AppendText($"{text}");
            richTextBoxOutput.SelectionColor = richTextBoxOutput.ForeColor;

            richTextBoxOutput.SelectionStart = richTextBoxOutput.Text.Length;
            richTextBoxOutput.ScrollToCaret();
            richTextBoxOutput.ResumeLayout();
        }

        public void StartWatch(GameEngineCore gameEngine, SpriteBase sprite)
        {
            new Thread(o =>
            {
                using (var form = new FormDebugSpriteWatch(_gameEngine, sprite))
                {
                    form.ShowDialog();
                }
            }).Start();
        }
    }
}
