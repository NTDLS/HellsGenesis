using System.Drawing;
using System.Windows.Forms;

namespace StrikeforceInfinity.Game
{
    partial class FormViewBrain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormViewBrain));
            textBoxText = new TextBox();
            SuspendLayout();
            // 
            // textBoxText
            // 
            textBoxText.Dock = DockStyle.Fill;
            textBoxText.Location = new Point(0, 0);
            textBoxText.Multiline = true;
            textBoxText.Name = "textBoxText";
            textBoxText.ScrollBars = ScrollBars.Both;
            textBoxText.Size = new Size(395, 553);
            textBoxText.TabIndex = 0;
            // 
            // FormViewBrain
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(395, 553);
            Controls.Add(textBoxText);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "FormViewBrain";
            SizeGripStyle = SizeGripStyle.Show;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Strikeforce Infinity : Brain";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox textBoxText;
    }
}