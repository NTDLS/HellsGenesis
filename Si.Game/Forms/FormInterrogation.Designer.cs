namespace Si.Game.Forms
{
    partial class FormInterrogation
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormInterrogation));
            splitContainerBody = new System.Windows.Forms.SplitContainer();
            splitContainer1 = new System.Windows.Forms.SplitContainer();
            richTextBoxOutput = new System.Windows.Forms.RichTextBox();
            listViewCommands = new System.Windows.Forms.ListView();
            columnHeaderName = new System.Windows.Forms.ColumnHeader();
            columnHeaderDescription = new System.Windows.Forms.ColumnHeader();
            textBoxCommand = new System.Windows.Forms.TextBox();
            buttonExecute = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)splitContainerBody).BeginInit();
            splitContainerBody.Panel1.SuspendLayout();
            splitContainerBody.Panel2.SuspendLayout();
            splitContainerBody.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            SuspendLayout();
            // 
            // splitContainerBody
            // 
            splitContainerBody.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainerBody.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            splitContainerBody.IsSplitterFixed = true;
            splitContainerBody.Location = new System.Drawing.Point(0, 0);
            splitContainerBody.Name = "splitContainerBody";
            splitContainerBody.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerBody.Panel1
            // 
            splitContainerBody.Panel1.Controls.Add(splitContainer1);
            // 
            // splitContainerBody.Panel2
            // 
            splitContainerBody.Panel2.Controls.Add(textBoxCommand);
            splitContainerBody.Panel2.Controls.Add(buttonExecute);
            splitContainerBody.Size = new System.Drawing.Size(1039, 563);
            splitContainerBody.SplitterDistance = 532;
            splitContainerBody.TabIndex = 0;
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainer1.Location = new System.Drawing.Point(0, 0);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(richTextBoxOutput);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(listViewCommands);
            splitContainer1.Size = new System.Drawing.Size(1039, 532);
            splitContainer1.SplitterDistance = 683;
            splitContainer1.TabIndex = 1;
            // 
            // richTextBoxOutput
            // 
            richTextBoxOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            richTextBoxOutput.Location = new System.Drawing.Point(0, 0);
            richTextBoxOutput.Name = "richTextBoxOutput";
            richTextBoxOutput.Size = new System.Drawing.Size(683, 532);
            richTextBoxOutput.TabIndex = 0;
            richTextBoxOutput.Text = "";
            // 
            // listViewCommands
            // 
            listViewCommands.Activation = System.Windows.Forms.ItemActivation.OneClick;
            listViewCommands.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { columnHeaderName, columnHeaderDescription });
            listViewCommands.Dock = System.Windows.Forms.DockStyle.Fill;
            listViewCommands.FullRowSelect = true;
            listViewCommands.GridLines = true;
            listViewCommands.HideSelection = true;
            listViewCommands.Location = new System.Drawing.Point(0, 0);
            listViewCommands.Name = "listViewCommands";
            listViewCommands.ShowItemToolTips = true;
            listViewCommands.Size = new System.Drawing.Size(352, 532);
            listViewCommands.TabIndex = 0;
            listViewCommands.UseCompatibleStateImageBehavior = false;
            listViewCommands.View = System.Windows.Forms.View.Details;
            // 
            // columnHeaderName
            // 
            columnHeaderName.Text = "Name";
            columnHeaderName.Width = 225;
            // 
            // columnHeaderDescription
            // 
            columnHeaderDescription.Text = "Description";
            columnHeaderDescription.Width = 500;
            // 
            // textBoxCommand
            // 
            textBoxCommand.AcceptsTab = true;
            textBoxCommand.Dock = System.Windows.Forms.DockStyle.Fill;
            textBoxCommand.Location = new System.Drawing.Point(0, 0);
            textBoxCommand.Name = "textBoxCommand";
            textBoxCommand.Size = new System.Drawing.Size(964, 23);
            textBoxCommand.TabIndex = 2;
            // 
            // buttonExecute
            // 
            buttonExecute.Dock = System.Windows.Forms.DockStyle.Right;
            buttonExecute.Location = new System.Drawing.Point(964, 0);
            buttonExecute.Name = "buttonExecute";
            buttonExecute.Size = new System.Drawing.Size(75, 27);
            buttonExecute.TabIndex = 1;
            buttonExecute.Text = "Execute";
            buttonExecute.UseVisualStyleBackColor = true;
            buttonExecute.Click += ButtonExecute_Click;
            // 
            // FormInterrogation
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1039, 563);
            Controls.Add(splitContainerBody);
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Name = "FormInterrogation";
            Text = "Strikeforce Infinity : Interrogation";
            Load += FormDebug_Load;
            splitContainerBody.Panel1.ResumeLayout(false);
            splitContainerBody.Panel2.ResumeLayout(false);
            splitContainerBody.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainerBody).EndInit();
            splitContainerBody.ResumeLayout(false);
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainerBody;
        private System.Windows.Forms.Button buttonExecute;
        private System.Windows.Forms.RichTextBox richTextBoxOutput;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ListView listViewCommands;
        private System.Windows.Forms.ColumnHeader columnHeaderName;
        private System.Windows.Forms.ColumnHeader columnHeaderDescription;
        private System.Windows.Forms.TextBox textBoxCommand;
    }
}