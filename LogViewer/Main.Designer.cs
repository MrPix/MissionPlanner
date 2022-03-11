
namespace LogViewer
{
    partial class Main
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.mainPlot = new ScottPlot.FormsPlot();
            this.parametersTreeView = new System.Windows.Forms.TreeView();
            this.messagesListView = new System.Windows.Forms.ListView();
            this.textColumn = new System.Windows.Forms.ColumnHeader();
            this.timeColumn = new System.Windows.Forms.ColumnHeader();
            this.SuspendLayout();
            // 
            // mainPlot
            // 
            this.mainPlot.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mainPlot.BackColor = System.Drawing.Color.Transparent;
            this.mainPlot.Location = new System.Drawing.Point(190, 12);
            this.mainPlot.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.mainPlot.Name = "mainPlot";
            this.mainPlot.Size = new System.Drawing.Size(504, 504);
            this.mainPlot.TabIndex = 0;
            // 
            // parametersTreeView
            // 
            this.parametersTreeView.CheckBoxes = true;
            this.parametersTreeView.Location = new System.Drawing.Point(12, 12);
            this.parametersTreeView.Name = "parametersTreeView";
            this.parametersTreeView.Size = new System.Drawing.Size(171, 504);
            this.parametersTreeView.TabIndex = 1;
            this.parametersTreeView.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterCheck);
            // 
            // messagesListView
            // 
            this.messagesListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.textColumn,
            this.timeColumn});
            this.messagesListView.FullRowSelect = true;
            this.messagesListView.GridLines = true;
            this.messagesListView.HideSelection = false;
            this.messagesListView.Location = new System.Drawing.Point(701, 12);
            this.messagesListView.Name = "messagesListView";
            this.messagesListView.Size = new System.Drawing.Size(331, 504);
            this.messagesListView.TabIndex = 2;
            this.messagesListView.UseCompatibleStateImageBehavior = false;
            this.messagesListView.View = System.Windows.Forms.View.Details;
            // 
            // textColumn
            // 
            this.textColumn.Text = "Message";
            this.textColumn.Width = 200;
            // 
            // timeColumn
            // 
            this.timeColumn.Text = "Time";
            this.timeColumn.Width = 120;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1044, 528);
            this.Controls.Add(this.messagesListView);
            this.Controls.Add(this.parametersTreeView);
            this.Controls.Add(this.mainPlot);
            this.Name = "Main";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private ScottPlot.FormsPlot mainPlot;
        private System.Windows.Forms.TreeView parametersTreeView;
        private System.Windows.Forms.ListView messagesListView;
        private System.Windows.Forms.ColumnHeader textColumn;
        private System.Windows.Forms.ColumnHeader timeColumn;
    }
}

