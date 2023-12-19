namespace StrikeforceInfinity.Game
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
            tabPageDisplay = new System.Windows.Forms.TabPage();
            trackBarResolution = new System.Windows.Forms.TrackBar();
            checkBoxAutoZoomWhenMoving = new System.Windows.Forms.CheckBox();
            labelResolutionLabel = new System.Windows.Forms.Label();
            labelResolution = new System.Windows.Forms.Label();
            tabPageDisplayAdvanced = new System.Windows.Forms.TabPage();
            labelInitialStarCount = new System.Windows.Forms.Label();
            labelFrameTargetStarCount = new System.Windows.Forms.Label();
            textBoxDeltaFrameTargetStarCount = new System.Windows.Forms.TextBox();
            textBoxInitialFrameStarCount = new System.Windows.Forms.TextBox();
            label2 = new System.Windows.Forms.Label();
            labelOverdrawScale = new System.Windows.Forms.Label();
            textBoxOverdrawScale = new System.Windows.Forms.TextBox();
            textBoxFrameLimiter = new System.Windows.Forms.TextBox();
            tabPageDebug = new System.Windows.Forms.TabPage();
            checkBoxEnableSpriteInterrogation = new System.Windows.Forms.CheckBox();
            checkBoxHighlightAllSprites = new System.Windows.Forms.CheckBox();
            checkBoxHighlightNatrualBounds = new System.Windows.Forms.CheckBox();
            buttonCancel = new System.Windows.Forms.Button();
            buttonSave = new System.Windows.Forms.Button();
            tabControl1.SuspendLayout();
            tabPageDisplay.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trackBarResolution).BeginInit();
            tabPageDisplayAdvanced.SuspendLayout();
            tabPageDebug.SuspendLayout();
            SuspendLayout();
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPageDisplay);
            tabControl1.Controls.Add(tabPageDisplayAdvanced);
            tabControl1.Controls.Add(tabPageDebug);
            tabControl1.Location = new System.Drawing.Point(12, 12);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new System.Drawing.Size(550, 332);
            tabControl1.TabIndex = 17;
            // 
            // tabPageDisplay
            // 
            tabPageDisplay.Controls.Add(trackBarResolution);
            tabPageDisplay.Controls.Add(checkBoxAutoZoomWhenMoving);
            tabPageDisplay.Controls.Add(labelResolutionLabel);
            tabPageDisplay.Controls.Add(labelResolution);
            tabPageDisplay.Location = new System.Drawing.Point(4, 24);
            tabPageDisplay.Name = "tabPageDisplay";
            tabPageDisplay.Padding = new System.Windows.Forms.Padding(3);
            tabPageDisplay.Size = new System.Drawing.Size(542, 304);
            tabPageDisplay.TabIndex = 0;
            tabPageDisplay.Text = "Display";
            tabPageDisplay.UseVisualStyleBackColor = true;
            // 
            // trackBarResolution
            // 
            trackBarResolution.LargeChange = 1;
            trackBarResolution.Location = new System.Drawing.Point(16, 31);
            trackBarResolution.Name = "trackBarResolution";
            trackBarResolution.Size = new System.Drawing.Size(223, 45);
            trackBarResolution.TabIndex = 8;
            // 
            // checkBoxAutoZoomWhenMoving
            // 
            checkBoxAutoZoomWhenMoving.AutoSize = true;
            checkBoxAutoZoomWhenMoving.Location = new System.Drawing.Point(16, 82);
            checkBoxAutoZoomWhenMoving.Name = "checkBoxAutoZoomWhenMoving";
            checkBoxAutoZoomWhenMoving.Size = new System.Drawing.Size(168, 19);
            checkBoxAutoZoomWhenMoving.TabIndex = 17;
            checkBoxAutoZoomWhenMoving.Text = "Auto-zoom when moving?";
            checkBoxAutoZoomWhenMoving.UseVisualStyleBackColor = true;
            // 
            // labelResolutionLabel
            // 
            labelResolutionLabel.AutoSize = true;
            labelResolutionLabel.Location = new System.Drawing.Point(16, 13);
            labelResolutionLabel.Name = "labelResolutionLabel";
            labelResolutionLabel.Size = new System.Drawing.Size(69, 15);
            labelResolutionLabel.TabIndex = 9;
            labelResolutionLabel.Text = "Resolution: ";
            // 
            // labelResolution
            // 
            labelResolution.AutoSize = true;
            labelResolution.Location = new System.Drawing.Point(91, 13);
            labelResolution.Name = "labelResolution";
            labelResolution.Size = new System.Drawing.Size(61, 15);
            labelResolution.TabIndex = 10;
            labelResolution.Text = "0000x0000";
            // 
            // tabPageDisplayAdvanced
            // 
            tabPageDisplayAdvanced.Controls.Add(labelInitialStarCount);
            tabPageDisplayAdvanced.Controls.Add(labelFrameTargetStarCount);
            tabPageDisplayAdvanced.Controls.Add(textBoxDeltaFrameTargetStarCount);
            tabPageDisplayAdvanced.Controls.Add(textBoxInitialFrameStarCount);
            tabPageDisplayAdvanced.Controls.Add(label2);
            tabPageDisplayAdvanced.Controls.Add(labelOverdrawScale);
            tabPageDisplayAdvanced.Controls.Add(textBoxOverdrawScale);
            tabPageDisplayAdvanced.Controls.Add(textBoxFrameLimiter);
            tabPageDisplayAdvanced.Location = new System.Drawing.Point(4, 24);
            tabPageDisplayAdvanced.Name = "tabPageDisplayAdvanced";
            tabPageDisplayAdvanced.Padding = new System.Windows.Forms.Padding(3);
            tabPageDisplayAdvanced.Size = new System.Drawing.Size(542, 304);
            tabPageDisplayAdvanced.TabIndex = 1;
            tabPageDisplayAdvanced.Text = "Display (Advanced)";
            tabPageDisplayAdvanced.UseVisualStyleBackColor = true;
            // 
            // labelInitialStarCount
            // 
            labelInitialStarCount.AutoSize = true;
            labelInitialStarCount.Location = new System.Drawing.Point(16, 114);
            labelInitialStarCount.Name = "labelInitialStarCount";
            labelInitialStarCount.Size = new System.Drawing.Size(100, 15);
            labelInitialStarCount.TabIndex = 33;
            labelInitialStarCount.Text = "Initial frame stars:";
            // 
            // labelFrameTargetStarCount
            // 
            labelFrameTargetStarCount.AutoSize = true;
            labelFrameTargetStarCount.Location = new System.Drawing.Point(16, 164);
            labelFrameTargetStarCount.Name = "labelFrameTargetStarCount";
            labelFrameTargetStarCount.Size = new System.Drawing.Size(134, 15);
            labelFrameTargetStarCount.TabIndex = 32;
            labelFrameTargetStarCount.Text = "Delta-frame target stars:";
            // 
            // textBoxDeltaFrameTargetStarCount
            // 
            textBoxDeltaFrameTargetStarCount.Location = new System.Drawing.Point(16, 182);
            textBoxDeltaFrameTargetStarCount.Name = "textBoxDeltaFrameTargetStarCount";
            textBoxDeltaFrameTargetStarCount.Size = new System.Drawing.Size(133, 23);
            textBoxDeltaFrameTargetStarCount.TabIndex = 31;
            // 
            // textBoxInitialFrameStarCount
            // 
            textBoxInitialFrameStarCount.Location = new System.Drawing.Point(16, 132);
            textBoxInitialFrameStarCount.Name = "textBoxInitialFrameStarCount";
            textBoxInitialFrameStarCount.Size = new System.Drawing.Size(133, 23);
            textBoxInitialFrameStarCount.TabIndex = 30;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(16, 13);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(80, 15);
            label2.TabIndex = 27;
            label2.Text = "Frame limiter:";
            // 
            // labelOverdrawScale
            // 
            labelOverdrawScale.AutoSize = true;
            labelOverdrawScale.Location = new System.Drawing.Point(16, 63);
            labelOverdrawScale.Name = "labelOverdrawScale";
            labelOverdrawScale.Size = new System.Drawing.Size(90, 15);
            labelOverdrawScale.TabIndex = 26;
            labelOverdrawScale.Text = "Overdraw scale:";
            // 
            // textBoxOverdrawScale
            // 
            textBoxOverdrawScale.Location = new System.Drawing.Point(16, 81);
            textBoxOverdrawScale.Name = "textBoxOverdrawScale";
            textBoxOverdrawScale.Size = new System.Drawing.Size(133, 23);
            textBoxOverdrawScale.TabIndex = 25;
            // 
            // textBoxFrameLimiter
            // 
            textBoxFrameLimiter.Location = new System.Drawing.Point(16, 31);
            textBoxFrameLimiter.Name = "textBoxFrameLimiter";
            textBoxFrameLimiter.Size = new System.Drawing.Size(133, 23);
            textBoxFrameLimiter.TabIndex = 24;
            // 
            // tabPageDebug
            // 
            tabPageDebug.Controls.Add(checkBoxEnableSpriteInterrogation);
            tabPageDebug.Controls.Add(checkBoxHighlightAllSprites);
            tabPageDebug.Controls.Add(checkBoxHighlightNatrualBounds);
            tabPageDebug.Location = new System.Drawing.Point(4, 24);
            tabPageDebug.Name = "tabPageDebug";
            tabPageDebug.Size = new System.Drawing.Size(542, 304);
            tabPageDebug.TabIndex = 2;
            tabPageDebug.Text = "Debug";
            tabPageDebug.UseVisualStyleBackColor = true;
            // 
            // checkBoxEnableSpriteInterrogation
            // 
            checkBoxEnableSpriteInterrogation.AutoSize = true;
            checkBoxEnableSpriteInterrogation.Location = new System.Drawing.Point(13, 63);
            checkBoxEnableSpriteInterrogation.Name = "checkBoxEnableSpriteInterrogation";
            checkBoxEnableSpriteInterrogation.Size = new System.Drawing.Size(175, 19);
            checkBoxEnableSpriteInterrogation.TabIndex = 37;
            checkBoxEnableSpriteInterrogation.Text = "Enable sprites interrogation?";
            checkBoxEnableSpriteInterrogation.UseVisualStyleBackColor = true;
            // 
            // checkBoxHighlightAllSprites
            // 
            checkBoxHighlightAllSprites.AutoSize = true;
            checkBoxHighlightAllSprites.Location = new System.Drawing.Point(13, 38);
            checkBoxHighlightAllSprites.Name = "checkBoxHighlightAllSprites";
            checkBoxHighlightAllSprites.Size = new System.Drawing.Size(133, 19);
            checkBoxHighlightAllSprites.TabIndex = 36;
            checkBoxHighlightAllSprites.Text = "Highlight all sprites?";
            checkBoxHighlightAllSprites.UseVisualStyleBackColor = true;
            // 
            // checkBoxHighlightNatrualBounds
            // 
            checkBoxHighlightNatrualBounds.AutoSize = true;
            checkBoxHighlightNatrualBounds.Location = new System.Drawing.Point(13, 13);
            checkBoxHighlightNatrualBounds.Name = "checkBoxHighlightNatrualBounds";
            checkBoxHighlightNatrualBounds.Size = new System.Drawing.Size(164, 19);
            checkBoxHighlightNatrualBounds.TabIndex = 35;
            checkBoxHighlightNatrualBounds.Text = "Highlight natrual bounds?";
            checkBoxHighlightNatrualBounds.UseVisualStyleBackColor = true;
            // 
            // buttonCancel
            // 
            buttonCancel.Location = new System.Drawing.Point(402, 350);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new System.Drawing.Size(75, 23);
            buttonCancel.TabIndex = 18;
            buttonCancel.Text = "Cancel";
            buttonCancel.UseVisualStyleBackColor = true;
            buttonCancel.Click += buttonCancel_Click;
            // 
            // buttonSave
            // 
            buttonSave.Location = new System.Drawing.Point(483, 350);
            buttonSave.Name = "buttonSave";
            buttonSave.Size = new System.Drawing.Size(75, 23);
            buttonSave.TabIndex = 19;
            buttonSave.Text = "Save";
            buttonSave.UseVisualStyleBackColor = true;
            buttonSave.Click += buttonSave_Click;
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
            Text = "Strikeforce Infinity : Settings";
            Load += FormSettings_Load;
            tabControl1.ResumeLayout(false);
            tabPageDisplay.ResumeLayout(false);
            tabPageDisplay.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)trackBarResolution).EndInit();
            tabPageDisplayAdvanced.ResumeLayout(false);
            tabPageDisplayAdvanced.PerformLayout();
            tabPageDebug.ResumeLayout(false);
            tabPageDebug.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPageDisplayAdvanced;
        private System.Windows.Forms.TabPage tabPageDisplay;
        private System.Windows.Forms.CheckBox checkBoxAutoZoomWhenMoving;
        private System.Windows.Forms.TrackBar trackBarResolution;
        private System.Windows.Forms.Label labelResolutionLabel;
        private System.Windows.Forms.Label labelResolution;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label labelOverdrawScale;
        private System.Windows.Forms.TextBox textBoxOverdrawScale;
        private System.Windows.Forms.TextBox textBoxFrameLimiter;
        private System.Windows.Forms.Label labelInitialStarCount;
        private System.Windows.Forms.Label labelFrameTargetStarCount;
        private System.Windows.Forms.TextBox textBoxDeltaFrameTargetStarCount;
        private System.Windows.Forms.TextBox textBoxInitialFrameStarCount;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.TabPage tabPageDebug;
        private System.Windows.Forms.CheckBox checkBoxEnableSpriteInterrogation;
        private System.Windows.Forms.CheckBox checkBoxHighlightAllSprites;
        private System.Windows.Forms.CheckBox checkBoxHighlightNatrualBounds;
    }
}