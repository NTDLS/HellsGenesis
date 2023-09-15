namespace HG
{
    partial class FormStartup
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
            buttonSettings = new System.Windows.Forms.Button();
            buttonStart = new System.Windows.Forms.Button();
            buttonExit = new System.Windows.Forms.Button();
            SuspendLayout();
            // 
            // buttonSettings
            // 
            buttonSettings.Location = new System.Drawing.Point(297, 361);
            buttonSettings.Name = "buttonSettings";
            buttonSettings.Size = new System.Drawing.Size(75, 23);
            buttonSettings.TabIndex = 1;
            buttonSettings.Text = "Settings";
            buttonSettings.UseVisualStyleBackColor = true;
            buttonSettings.Click += buttonSettings_Click;
            // 
            // buttonStart
            // 
            buttonStart.Location = new System.Drawing.Point(420, 361);
            buttonStart.Name = "buttonStart";
            buttonStart.Size = new System.Drawing.Size(75, 23);
            buttonStart.TabIndex = 2;
            buttonStart.Text = "Start";
            buttonStart.UseVisualStyleBackColor = true;
            buttonStart.Click += buttonStart_Click;
            // 
            // buttonExit
            // 
            buttonExit.Location = new System.Drawing.Point(174, 361);
            buttonExit.Name = "buttonExit";
            buttonExit.Size = new System.Drawing.Size(75, 23);
            buttonExit.TabIndex = 0;
            buttonExit.Text = "Exit";
            buttonExit.UseVisualStyleBackColor = true;
            buttonExit.Click += buttonExit_Click;
            // 
            // FormStartup
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackgroundImage = Properties.Resources.HgSpash;
            ClientSize = new System.Drawing.Size(668, 448);
            ControlBox = false;
            Controls.Add(buttonSettings);
            Controls.Add(buttonStart);
            Controls.Add(buttonExit);
            ForeColor = System.Drawing.SystemColors.ControlText;
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            Name = "FormStartup";
            Opacity = 0.9D;
            ShowIcon = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "Hells Genesis";
            Load += FormStartup_Load;
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Button buttonSettings;
        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.Button buttonExit;
    }
}