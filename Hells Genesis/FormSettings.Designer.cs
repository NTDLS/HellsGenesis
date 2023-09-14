namespace HG
{
    partial class FormSettings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSettings));
            tabControl1 = new System.Windows.Forms.TabControl();
            tabPageDisplayAdvanced = new System.Windows.Forms.TabPage();
            groupBox1 = new System.Windows.Forms.GroupBox();
            labelResolution = new System.Windows.Forms.Label();
            checkBoxFullscreen = new System.Windows.Forms.CheckBox();
            labelResolutionLabel = new System.Windows.Forms.Label();
            trackBarResolution = new System.Windows.Forms.TrackBar();
            checkBox1 = new System.Windows.Forms.CheckBox();
            tabPageDisplay = new System.Windows.Forms.TabPage();
            label2 = new System.Windows.Forms.Label();
            labelOverdrawScale = new System.Windows.Forms.Label();
            textBoxOverdrawScale = new System.Windows.Forms.TextBox();
            textBoxFrameLimiter = new System.Windows.Forms.TextBox();
            checkBox3 = new System.Windows.Forms.CheckBox();
            checkBox2 = new System.Windows.Forms.CheckBox();
            labelInitialStarCount = new System.Windows.Forms.Label();
            labelFrameTargetStarCount = new System.Windows.Forms.Label();
            textBoxFrameTargetStarCount = new System.Windows.Forms.TextBox();
            textBoxInitialStarCount = new System.Windows.Forms.TextBox();
            buttonCancel = new System.Windows.Forms.Button();
            buttonSave = new System.Windows.Forms.Button();
            tabControl1.SuspendLayout();
            tabPageDisplayAdvanced.SuspendLayout();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trackBarResolution).BeginInit();
            tabPageDisplay.SuspendLayout();
            SuspendLayout();
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPageDisplay);
            tabControl1.Controls.Add(tabPageDisplayAdvanced);
            tabControl1.Location = new System.Drawing.Point(12, 12);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new System.Drawing.Size(550, 332);
            tabControl1.TabIndex = 17;
            // 
            // tabPageDisplayAdvanced
            // 
            tabPageDisplayAdvanced.Controls.Add(labelInitialStarCount);
            tabPageDisplayAdvanced.Controls.Add(labelFrameTargetStarCount);
            tabPageDisplayAdvanced.Controls.Add(textBoxFrameTargetStarCount);
            tabPageDisplayAdvanced.Controls.Add(textBoxInitialStarCount);
            tabPageDisplayAdvanced.Controls.Add(checkBox3);
            tabPageDisplayAdvanced.Controls.Add(checkBox2);
            tabPageDisplayAdvanced.Controls.Add(label2);
            tabPageDisplayAdvanced.Controls.Add(labelOverdrawScale);
            tabPageDisplayAdvanced.Controls.Add(textBoxOverdrawScale);
            tabPageDisplayAdvanced.Controls.Add(textBoxFrameLimiter);
            tabPageDisplayAdvanced.Location = new System.Drawing.Point(4, 24);
            tabPageDisplayAdvanced.Name = "tabPageDisplayAdvanced";
            tabPageDisplayAdvanced.Padding = new System.Windows.Forms.Padding(3);
            tabPageDisplayAdvanced.Size = new System.Drawing.Size(542, 269);
            tabPageDisplayAdvanced.TabIndex = 1;
            tabPageDisplayAdvanced.Text = "Display (Advanced)";
            tabPageDisplayAdvanced.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(trackBarResolution);
            groupBox1.Controls.Add(labelResolutionLabel);
            groupBox1.Controls.Add(checkBoxFullscreen);
            groupBox1.Controls.Add(labelResolution);
            groupBox1.Location = new System.Drawing.Point(33, 25);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new System.Drawing.Size(249, 121);
            groupBox1.TabIndex = 16;
            groupBox1.TabStop = false;
            groupBox1.Text = "Rsolution";
            // 
            // labelResolution
            // 
            labelResolution.AutoSize = true;
            labelResolution.Location = new System.Drawing.Point(81, 19);
            labelResolution.Name = "labelResolution";
            labelResolution.Size = new System.Drawing.Size(61, 15);
            labelResolution.TabIndex = 10;
            labelResolution.Text = "0000x0000";
            // 
            // checkBoxFullscreen
            // 
            checkBoxFullscreen.AutoSize = true;
            checkBoxFullscreen.Location = new System.Drawing.Point(6, 88);
            checkBoxFullscreen.Name = "checkBoxFullscreen";
            checkBoxFullscreen.Size = new System.Drawing.Size(84, 19);
            checkBoxFullscreen.TabIndex = 7;
            checkBoxFullscreen.Text = "Fullscreen?";
            checkBoxFullscreen.UseVisualStyleBackColor = true;
            // 
            // labelResolutionLabel
            // 
            labelResolutionLabel.AutoSize = true;
            labelResolutionLabel.Location = new System.Drawing.Point(6, 19);
            labelResolutionLabel.Name = "labelResolutionLabel";
            labelResolutionLabel.Size = new System.Drawing.Size(69, 15);
            labelResolutionLabel.TabIndex = 9;
            labelResolutionLabel.Text = "Resolution: ";
            // 
            // trackBarResolution
            // 
            trackBarResolution.LargeChange = 1;
            trackBarResolution.Location = new System.Drawing.Point(6, 37);
            trackBarResolution.Name = "trackBarResolution";
            trackBarResolution.Size = new System.Drawing.Size(223, 45);
            trackBarResolution.TabIndex = 8;
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.Location = new System.Drawing.Point(33, 169);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new System.Drawing.Size(168, 19);
            checkBox1.TabIndex = 17;
            checkBox1.Text = "Auto-zoom when moving?";
            checkBox1.UseVisualStyleBackColor = true;
            // 
            // tabPageDisplay
            // 
            tabPageDisplay.Controls.Add(checkBox1);
            tabPageDisplay.Controls.Add(groupBox1);
            tabPageDisplay.Location = new System.Drawing.Point(4, 24);
            tabPageDisplay.Name = "tabPageDisplay";
            tabPageDisplay.Padding = new System.Windows.Forms.Padding(3);
            tabPageDisplay.Size = new System.Drawing.Size(542, 304);
            tabPageDisplay.TabIndex = 0;
            tabPageDisplay.Text = "Display";
            tabPageDisplay.UseVisualStyleBackColor = true;
            tabPageDisplay.Click += tabPageDisplay_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(319, 14);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(80, 15);
            label2.TabIndex = 27;
            label2.Text = "Frame limiter:";
            // 
            // labelOverdrawScale
            // 
            labelOverdrawScale.AutoSize = true;
            labelOverdrawScale.Location = new System.Drawing.Point(319, 64);
            labelOverdrawScale.Name = "labelOverdrawScale";
            labelOverdrawScale.Size = new System.Drawing.Size(90, 15);
            labelOverdrawScale.TabIndex = 26;
            labelOverdrawScale.Text = "Overdraw scale:";
            // 
            // textBoxOverdrawScale
            // 
            textBoxOverdrawScale.Location = new System.Drawing.Point(319, 82);
            textBoxOverdrawScale.Name = "textBoxOverdrawScale";
            textBoxOverdrawScale.Size = new System.Drawing.Size(133, 23);
            textBoxOverdrawScale.TabIndex = 25;
            // 
            // textBoxFrameLimiter
            // 
            textBoxFrameLimiter.Location = new System.Drawing.Point(319, 32);
            textBoxFrameLimiter.Name = "textBoxFrameLimiter";
            textBoxFrameLimiter.Size = new System.Drawing.Size(133, 23);
            textBoxFrameLimiter.TabIndex = 24;
            // 
            // checkBox3
            // 
            checkBox3.AutoSize = true;
            checkBox3.Location = new System.Drawing.Point(19, 39);
            checkBox3.Name = "checkBox3";
            checkBox3.Size = new System.Drawing.Size(131, 19);
            checkBox3.TabIndex = 29;
            checkBox3.Text = "Highlight all actors?";
            checkBox3.UseVisualStyleBackColor = true;
            // 
            // checkBox2
            // 
            checkBox2.AutoSize = true;
            checkBox2.Location = new System.Drawing.Point(19, 14);
            checkBox2.Name = "checkBox2";
            checkBox2.Size = new System.Drawing.Size(164, 19);
            checkBox2.TabIndex = 28;
            checkBox2.Text = "Highlight natrual bounds?";
            checkBox2.UseVisualStyleBackColor = true;
            // 
            // labelInitialStarCount
            // 
            labelInitialStarCount.AutoSize = true;
            labelInitialStarCount.Location = new System.Drawing.Point(319, 115);
            labelInitialStarCount.Name = "labelInitialStarCount";
            labelInitialStarCount.Size = new System.Drawing.Size(95, 15);
            labelInitialStarCount.TabIndex = 33;
            labelInitialStarCount.Text = "Initial star count:";
            // 
            // labelFrameTargetStarCount
            // 
            labelFrameTargetStarCount.AutoSize = true;
            labelFrameTargetStarCount.Location = new System.Drawing.Point(319, 165);
            labelFrameTargetStarCount.Name = "labelFrameTargetStarCount";
            labelFrameTargetStarCount.Size = new System.Drawing.Size(133, 15);
            labelFrameTargetStarCount.TabIndex = 32;
            labelFrameTargetStarCount.Text = "Frame target star count:";
            // 
            // textBoxFrameTargetStarCount
            // 
            textBoxFrameTargetStarCount.Location = new System.Drawing.Point(319, 183);
            textBoxFrameTargetStarCount.Name = "textBoxFrameTargetStarCount";
            textBoxFrameTargetStarCount.Size = new System.Drawing.Size(133, 23);
            textBoxFrameTargetStarCount.TabIndex = 31;
            // 
            // textBoxInitialStarCount
            // 
            textBoxInitialStarCount.Location = new System.Drawing.Point(319, 133);
            textBoxInitialStarCount.Name = "textBoxInitialStarCount";
            textBoxInitialStarCount.Size = new System.Drawing.Size(133, 23);
            textBoxInitialStarCount.TabIndex = 30;
            // 
            // buttonCancel
            // 
            buttonCancel.Location = new System.Drawing.Point(402, 350);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new System.Drawing.Size(75, 23);
            buttonCancel.TabIndex = 18;
            buttonCancel.Text = "Cancel";
            buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonSave
            // 
            buttonSave.Location = new System.Drawing.Point(483, 350);
            buttonSave.Name = "buttonSave";
            buttonSave.Size = new System.Drawing.Size(75, 23);
            buttonSave.TabIndex = 19;
            buttonSave.Text = "Save";
            buttonSave.UseVisualStyleBackColor = true;
            // 
            // FormSettings
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(585, 385);
            Controls.Add(buttonSave);
            Controls.Add(buttonCancel);
            Controls.Add(tabControl1);
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FormSettings";
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "FormSettings";
            Load += FormSettings_Load;
            tabControl1.ResumeLayout(false);
            tabPageDisplayAdvanced.ResumeLayout(false);
            tabPageDisplayAdvanced.PerformLayout();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)trackBarResolution).EndInit();
            tabPageDisplay.ResumeLayout(false);
            tabPageDisplay.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPageDisplayAdvanced;
        private System.Windows.Forms.TabPage tabPageDisplay;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TrackBar trackBarResolution;
        private System.Windows.Forms.Label labelResolutionLabel;
        private System.Windows.Forms.CheckBox checkBoxFullscreen;
        private System.Windows.Forms.Label labelResolution;
        private System.Windows.Forms.CheckBox checkBox3;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label labelOverdrawScale;
        private System.Windows.Forms.TextBox textBoxOverdrawScale;
        private System.Windows.Forms.TextBox textBoxFrameLimiter;
        private System.Windows.Forms.Label labelInitialStarCount;
        private System.Windows.Forms.Label labelFrameTargetStarCount;
        private System.Windows.Forms.TextBox textBoxFrameTargetStarCount;
        private System.Windows.Forms.TextBox textBoxInitialStarCount;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonSave;
    }
}