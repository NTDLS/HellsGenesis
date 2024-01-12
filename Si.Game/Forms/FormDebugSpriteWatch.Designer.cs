using Si.Game.Controls;

namespace Si.Game.Forms
{
    partial class FormDebugSpriteWatch
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormDebugSpriteWatch));
            splitContainer1 = new System.Windows.Forms.SplitContainer();
            listViewVariables = new BufferedListView();
            columnHeaderName = new System.Windows.Forms.ColumnHeader();
            columnHeaderValue = new System.Windows.Forms.ColumnHeader();
            richTexLog = new System.Windows.Forms.RichTextBox();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            SuspendLayout();
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainer1.Location = new System.Drawing.Point(0, 0);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(listViewVariables);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(richTexLog);
            splitContainer1.Size = new System.Drawing.Size(1039, 563);
            splitContainer1.SplitterDistance = 632;
            splitContainer1.TabIndex = 1;
            // 
            // listViewVariables
            // 
            listViewVariables.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { columnHeaderName, columnHeaderValue });
            listViewVariables.Dock = System.Windows.Forms.DockStyle.Fill;
            listViewVariables.GridLines = true;
            listViewVariables.Location = new System.Drawing.Point(0, 0);
            listViewVariables.Name = "listViewVariables";
            listViewVariables.Size = new System.Drawing.Size(632, 563);
            listViewVariables.TabIndex = 0;
            listViewVariables.UseCompatibleStateImageBehavior = false;
            listViewVariables.View = System.Windows.Forms.View.Details;
            // 
            // columnHeaderName
            // 
            columnHeaderName.Text = "Name";
            columnHeaderName.Width = 200;
            // 
            // columnHeaderValue
            // 
            columnHeaderValue.Text = "Value";
            columnHeaderValue.Width = 400;
            // 
            // richTexLog
            // 
            richTexLog.Dock = System.Windows.Forms.DockStyle.Fill;
            richTexLog.Location = new System.Drawing.Point(0, 0);
            richTexLog.Name = "richTexLog";
            richTexLog.Size = new System.Drawing.Size(403, 563);
            richTexLog.TabIndex = 0;
            richTexLog.Text = "";
            // 
            // FormDebugSpriteWatch
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1039, 563);
            Controls.Add(splitContainer1);
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Name = "FormDebugSpriteWatch";
            Text = "Strikeforce Infinity : Sprite Watch";
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private System.Windows.Forms.RichTextBox richTexLog;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private BufferedListView listViewVariables;
        private System.Windows.Forms.ColumnHeader columnHeaderName;
        private System.Windows.Forms.ColumnHeader columnHeaderValue;
    }
}