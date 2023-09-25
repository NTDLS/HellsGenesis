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
        private readonly List<string> _commandHistory = new();
        private int _commandHistoryIndex = 0;
        private readonly ComboBox _comboBoxAutoComplete = new();

        public FormDebug() => InitializeComponent();

        private void FormDebug_Load(object sender, EventArgs e) { }

        internal FormDebug(EngineCore core)
        {
            InitializeComponent();

            splitContainerBody.Panel2.Controls.Add(_comboBoxAutoComplete);

            textBoxInput.Font = new Font("Courier New", 10, FontStyle.Regular);
            richTextBoxOutput.Font = new Font("Courier New", 10, FontStyle.Regular);

            AcceptButton = buttonExecute;

            _core = core;

            Shown += (object sender, EventArgs e) => textBoxInput.Focus();

            textBoxInput.KeyUp += TextBoxInput_KeyUp;

            FormClosing += (object sender, FormClosingEventArgs e) =>
            {
                core.Debug.ToggleVisibility();
                e.Cancel = true;
            };
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (_comboBoxAutoComplete.Focused)
            {
                if (keyData == Keys.Tab || keyData == Keys.Enter)
                {
                    if (_comboBoxAutoComplete.Items.Count > 0)
                    {
                        if (string.IsNullOrEmpty(_comboBoxAutoComplete.SelectedText) == false)
                        {
                            textBoxInput.Text = _comboBoxAutoComplete.SelectedText;
                        }
                        else
                        {
                            textBoxInput.Text = _comboBoxAutoComplete.Items[0].ToString();
                        }
                        textBoxInput.SelectionStart = textBoxInput.Text.Length;
                        textBoxInput.SelectionLength = 0;
                        _comboBoxAutoComplete.DroppedDown = false;
                        Cursor.Current = Cursors.Default;
                        textBoxInput.Focus();
                    }
                    return true;
                }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void TextBoxInput_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                _comboBoxAutoComplete.DroppedDown = false;
                Cursor.Current = Cursors.Default;
                textBoxInput.Focus();
            }
            else if ((e.KeyCode >= Keys.A && e.KeyCode <= Keys.Z) || e.KeyCode == Keys.Back || e.KeyCode == Keys.OemMinus)
            {
                string input = textBoxInput.Text.ToLower();
                if (input.Length <= 0)
                {
                    return;
                }

                _comboBoxAutoComplete.Items.Clear();
                foreach (var command in _core.Debug.CommandParser.Commands)
                {
                    if (command.Name.ToLower().StartsWith(input))
                    {
                        _comboBoxAutoComplete.Items.Add(command.Name);
                    }
                }

                if (_comboBoxAutoComplete.Items.Count == 1)
                {
                    //No need to suggest a full match.
                    if (_comboBoxAutoComplete.Items[0].ToString().ToLower() == input)
                    {
                        _comboBoxAutoComplete.Items.Clear();
                    }
                }

                _comboBoxAutoComplete.DroppedDown = _comboBoxAutoComplete.Items.Count > 0;
                Cursor.Current = Cursors.Default;
            }
            else if (e.KeyCode == Keys.Down && _comboBoxAutoComplete.DroppedDown == true)
            {
                _comboBoxAutoComplete.Focus();
                SendKeys.Send("{DOWN}");
            }
            else
            {
                if (e.KeyCode == Keys.Up)
                {
                    if (_commandHistoryIndex > 0)
                    {
                        _commandHistoryIndex--;
                        textBoxInput.Text = _commandHistory[_commandHistoryIndex];
                        textBoxInput.SelectionStart = textBoxInput.Text.Length;
                        textBoxInput.SelectionLength = 0;
                    }
                }
                else if (e.KeyCode == Keys.Down)
                {
                    if (_commandHistoryIndex <= _commandHistory.Count - 1)
                    {
                        textBoxInput.Text = _commandHistory[_commandHistoryIndex];
                        _commandHistoryIndex++;
                        textBoxInput.SelectionStart = textBoxInput.Text.Length;
                        textBoxInput.SelectionLength = 0;
                    }
                }
            }
        }

        private void ButtonExecute_Click(object sender, EventArgs e)
        {
            var command = textBoxInput.Text.Trim();
            textBoxInput.Text = "";

            if (string.IsNullOrEmpty(command) == false)
            {
                _commandHistory.Add(command);
                _commandHistoryIndex = _commandHistory.Count;

                _core.Debug.EnqueueCommand(command);
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
    }
}
