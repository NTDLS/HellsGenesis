using NebulaSiege.Engine;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace NebulaSiege.Forms
{
    public partial class FormDebug : Form
    {
        private readonly EngineCore _core;
        private List<string> _commandHistory = new();
        private int _commandHistoryIndex = 0;

        public FormDebug() => InitializeComponent();

        private void FormDebug_Load(object sender, EventArgs e) { }

        ComboBox comboBoxAutoComplete = new();

        internal FormDebug(EngineCore core)
        {
            InitializeComponent();

            splitContainerBody.Panel2.Controls.Add(comboBoxAutoComplete);

            comboBoxInput.Font = new Font("Courier New", 10, FontStyle.Regular);
            textBoxOutput.Font = new Font("Courier New", 10, FontStyle.Regular);

            AcceptButton = buttonExecute;

            _core = core;

            Shown += (object sender, EventArgs e) => comboBoxInput.Focus();

            comboBoxInput.KeyUp += TextBoxInput_KeyUp;

            FormClosing += (object sender, FormClosingEventArgs e) =>
            {
                core.Debug.ToggleVisibility();
                e.Cancel = true;
            };
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Tab)
            {
                if (comboBoxAutoComplete.Items.Count > 0)
                {
                    comboBoxInput.Text = comboBoxAutoComplete.Items[0].ToString();
                    comboBoxInput.SelectionStart = comboBoxInput.Text.Length;
                    comboBoxInput.SelectionLength = 0;
                }
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void TextBoxInput_KeyUp(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode >= Keys.A && e.KeyCode <= Keys.Z) || e.KeyCode == Keys.Back)
            {
                string input = comboBoxInput.Text.ToLower();
                if (input.Length <= 0)
                {
                    return;
                }

                comboBoxAutoComplete.Items.Clear();
                foreach (var command in _core.Debug.CommandParser.Commands)
                {
                    if (command.Name.ToLower().StartsWith(input))
                    {
                        comboBoxAutoComplete.Items.Add(command.Name);
                    }
                }

                if (comboBoxAutoComplete.Items.Count == 1)
                {
                    //No need to suggest a full match.
                    if (comboBoxAutoComplete.Items[0].ToString().ToLower() == input)
                    {
                        comboBoxAutoComplete.Items.Clear();
                    }
                }

                comboBoxAutoComplete.DroppedDown = comboBoxAutoComplete.Items.Count > 0;
            }
            else if (e.KeyCode == Keys.Up)
            {
                if (_commandHistoryIndex > 0)
                {
                    _commandHistoryIndex--;
                    comboBoxInput.Text = _commandHistory[_commandHistoryIndex];
                    comboBoxInput.SelectionStart = comboBoxInput.Text.Length;
                    comboBoxInput.SelectionLength = 0;
                }
            }
            else if (e.KeyCode == Keys.Down)
            {
                if (_commandHistoryIndex <= _commandHistory.Count - 1)
                {
                    comboBoxInput.Text = _commandHistory[_commandHistoryIndex];
                    _commandHistoryIndex++;
                    comboBoxInput.SelectionStart = comboBoxInput.Text.Length;
                    comboBoxInput.SelectionLength = 0;
                }
            }
        }

        private void ButtonExecute_Click(object sender, EventArgs e)
        {
            var command = comboBoxInput.Text.Trim();
            comboBoxInput.Text = "";

            if (string.IsNullOrEmpty(command) == false)
            {
                _commandHistory.Add(command);
                _commandHistoryIndex = _commandHistory.Count;

                _core.Debug.EnqueueCommand(command);
            }
        }

        public void ClearText()
        {
            if (textBoxOutput.InvokeRequired)
            {
                textBoxOutput.Invoke(new Action(() => textBoxOutput.Clear()));
            }
            else
            {
                textBoxOutput.Clear();
            }
        }

        public void Write(string text)
        {
            if (textBoxOutput.InvokeRequired)
            {
                textBoxOutput.Invoke(new Action(() => textBoxOutput.AppendText(text)));
            }
            else
            {
                textBoxOutput.AppendText(text);
            }
        }

        public void WriteLine(string text)
        {
            text += "\r\n";

            if (textBoxOutput.InvokeRequired)
            {
                textBoxOutput.Invoke(new Action(() => textBoxOutput.AppendText(text)));
            }
            else
            {
                textBoxOutput.AppendText(text);
            }
        }
    }
}
