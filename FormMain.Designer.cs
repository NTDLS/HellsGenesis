namespace AI2D
{
    partial class FormMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.pictureBoxScene = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxScene)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBoxScene
            // 
            this.pictureBoxScene.BackColor = System.Drawing.Color.Black;
            this.pictureBoxScene.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBoxScene.Location = new System.Drawing.Point(0, 0);
            this.pictureBoxScene.Name = "pictureBoxScene";
            this.pictureBoxScene.Size = new System.Drawing.Size(1008, 729);
            this.pictureBoxScene.TabIndex = 0;
            this.pictureBoxScene.TabStop = false;
            this.pictureBoxScene.Paint += new System.Windows.Forms.PaintEventHandler(this.PictureBoxScene_Paint);
            this.pictureBoxScene.MouseEnter += new System.EventHandler(this.PictureBoxScene_MouseEnter);
            this.pictureBoxScene.MouseLeave += new System.EventHandler(this.PictureBoxScene_MouseLeave);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.ClientSize = new System.Drawing.Size(1008, 729);
            this.Controls.Add(this.pictureBoxScene);
            this.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "AI2D";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.Shown += new System.EventHandler(this.FormMain_Shown);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FormMain_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.FormMain_KeyUp);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxScene)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBoxScene;
    }
}

