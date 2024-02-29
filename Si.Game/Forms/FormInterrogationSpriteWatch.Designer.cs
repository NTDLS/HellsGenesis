using Si.Game.Controls;

namespace Si.Game.Forms
{
    partial class FormInterrogationSpriteWatch
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormInterrogationSpriteWatch));
            splitContainerBody = new System.Windows.Forms.SplitContainer();
            listViewVariables = new BufferedListView();
            columnHeaderName = new System.Windows.Forms.ColumnHeader();
            columnHeaderValue = new System.Windows.Forms.ColumnHeader();
            richTexLog = new System.Windows.Forms.RichTextBox();
            ((System.ComponentModel.ISupportInitialize)splitContainerBody).BeginInit();
            splitContainerBody.Panel1.SuspendLayout();
            splitContainerBody.Panel2.SuspendLayout();
            splitContainerBody.SuspendLayout();
            SuspendLayout();
            // 
            // splitContainerBody
            // 
            splitContainerBody.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainerBody.Location = new System.Drawing.Point(0, 0);
            splitContainerBody.Name = "splitContainerBody";
            // 
            // splitContainerBody.Panel1
            // 
            splitContainerBody.Panel1.Controls.Add(listViewVariables);
            // 
            // splitContainerBody.Panel2
            // 
            splitContainerBody.Panel2.Controls.Add(richTexLog);
            splitContainerBody.Size = new System.Drawing.Size(659, 563);
            splitContainerBody.SplitterDistance = 400;
            splitContainerBody.TabIndex = 1;
            // 
            // listViewVariables
            // 
            listViewVariables.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { columnHeaderName, columnHeaderValue });
            listViewVariables.Dock = System.Windows.Forms.DockStyle.Fill;
            listViewVariables.GridLines = true;
            listViewVariables.Location = new System.Drawing.Point(0, 0);
            listViewVariables.Name = "listViewVariables";
            listViewVariables.Size = new System.Drawing.Size(400, 563);
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
            richTexLog.Size = new System.Drawing.Size(255, 563);
            richTexLog.TabIndex = 0;
            richTexLog.Text = "";
            // 
            // FormDebugSpriteWatch
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(659, 563);
            Controls.Add(splitContainerBody);
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Name = "FormDebugSpriteWatch";
            Text = "Strikeforce Infinity : Sprite Watch";
            splitContainerBody.Panel1.ResumeLayout(false);
            splitContainerBody.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerBody).EndInit();
            splitContainerBody.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private System.Windows.Forms.RichTextBox richTexLog;
        private System.Windows.Forms.SplitContainer splitContainerBody;
        private BufferedListView listViewVariables;
        private System.Windows.Forms.ColumnHeader columnHeaderName;
        private System.Windows.Forms.ColumnHeader columnHeaderValue;
    }
}