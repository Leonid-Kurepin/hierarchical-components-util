namespace ASKON_TestTask
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            this.treeView = new System.Windows.Forms.TreeView();
            this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.numericUpDownDetailCount = new System.Windows.Forms.NumericUpDown();
            this.labelAddParentTitle = new System.Windows.Forms.Label();
            this.labelAddChildTitle = new System.Windows.Forms.Label();
            this.tbAddParentName = new System.Windows.Forms.TextBox();
            this.tbAddChildName = new System.Windows.Forms.TextBox();
            this.labelRenameDetailTitle = new System.Windows.Forms.Label();
            this.tbRenameDetail = new System.Windows.Forms.TextBox();
            this.labelDetailNameChild = new System.Windows.Forms.Label();
            this.labelDetailNameRename = new System.Windows.Forms.Label();
            this.labelDetailNameParent = new System.Windows.Forms.Label();
            this.labelChildDetailCount = new System.Windows.Forms.Label();
            this.labelErrorMessage = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDetailCount)).BeginInit();
            this.SuspendLayout();
            // 
            // treeView
            // 
            this.treeView.ContextMenuStrip = this.contextMenu;
            this.treeView.Location = new System.Drawing.Point(12, 31);
            this.treeView.Name = "treeView";
            this.treeView.Size = new System.Drawing.Size(258, 358);
            this.treeView.TabIndex = 1;
            this.treeView.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeView1_BeforeExpand);
            this.treeView.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView_NodeMouseClick);
            // 
            // contextMenu
            // 
            this.contextMenu.Name = "contextMenu";
            this.contextMenu.Size = new System.Drawing.Size(61, 4);
            this.contextMenu.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.contextMenu_ItemClicked);
            // 
            // numericUpDownDetailCount
            // 
            this.numericUpDownDetailCount.Location = new System.Drawing.Point(391, 208);
            this.numericUpDownDetailCount.Name = "numericUpDownDetailCount";
            this.numericUpDownDetailCount.Size = new System.Drawing.Size(108, 23);
            this.numericUpDownDetailCount.TabIndex = 2;
            this.numericUpDownDetailCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // labelAddParentTitle
            // 
            this.labelAddParentTitle.AutoSize = true;
            this.labelAddParentTitle.Location = new System.Drawing.Point(374, 42);
            this.labelAddParentTitle.Name = "labelAddParentTitle";
            this.labelAddParentTitle.Size = new System.Drawing.Size(63, 15);
            this.labelAddParentTitle.TabIndex = 3;
            this.labelAddParentTitle.Text = "AddParent";
            // 
            // labelAddChildTitle
            // 
            this.labelAddChildTitle.AutoSize = true;
            this.labelAddChildTitle.Location = new System.Drawing.Point(380, 132);
            this.labelAddChildTitle.Name = "labelAddChildTitle";
            this.labelAddChildTitle.Size = new System.Drawing.Size(57, 15);
            this.labelAddChildTitle.TabIndex = 3;
            this.labelAddChildTitle.Text = "AddChild";
            // 
            // tbAddParentName
            // 
            this.tbAddParentName.Location = new System.Drawing.Point(313, 80);
            this.tbAddParentName.Name = "tbAddParentName";
            this.tbAddParentName.Size = new System.Drawing.Size(186, 23);
            this.tbAddParentName.TabIndex = 4;
            // 
            // tbAddChildName
            // 
            this.tbAddChildName.Location = new System.Drawing.Point(313, 170);
            this.tbAddChildName.Name = "tbAddChildName";
            this.tbAddChildName.Size = new System.Drawing.Size(186, 23);
            this.tbAddChildName.TabIndex = 4;
            // 
            // labelRenameDetailTitle
            // 
            this.labelRenameDetailTitle.AutoSize = true;
            this.labelRenameDetailTitle.Location = new System.Drawing.Point(374, 263);
            this.labelRenameDetailTitle.Name = "labelRenameDetailTitle";
            this.labelRenameDetailTitle.Size = new System.Drawing.Size(80, 15);
            this.labelRenameDetailTitle.TabIndex = 3;
            this.labelRenameDetailTitle.Text = "RenameDetail";
            // 
            // tbRenameDetail
            // 
            this.tbRenameDetail.Location = new System.Drawing.Point(313, 306);
            this.tbRenameDetail.Name = "tbRenameDetail";
            this.tbRenameDetail.Size = new System.Drawing.Size(186, 23);
            this.tbRenameDetail.TabIndex = 4;
            // 
            // labelDetailNameChild
            // 
            this.labelDetailNameChild.AutoSize = true;
            this.labelDetailNameChild.Location = new System.Drawing.Point(313, 152);
            this.labelDetailNameChild.Name = "labelDetailNameChild";
            this.labelDetailNameChild.Size = new System.Drawing.Size(72, 15);
            this.labelDetailNameChild.TabIndex = 5;
            this.labelDetailNameChild.Text = "Detail Name";
            // 
            // labelDetailNameRename
            // 
            this.labelDetailNameRename.AutoSize = true;
            this.labelDetailNameRename.Location = new System.Drawing.Point(313, 288);
            this.labelDetailNameRename.Name = "labelDetailNameRename";
            this.labelDetailNameRename.Size = new System.Drawing.Size(99, 15);
            this.labelDetailNameRename.TabIndex = 5;
            this.labelDetailNameRename.Text = "New Detail Name";
            // 
            // labelDetailNameParent
            // 
            this.labelDetailNameParent.AutoSize = true;
            this.labelDetailNameParent.Location = new System.Drawing.Point(313, 62);
            this.labelDetailNameParent.Name = "labelDetailNameParent";
            this.labelDetailNameParent.Size = new System.Drawing.Size(72, 15);
            this.labelDetailNameParent.TabIndex = 5;
            this.labelDetailNameParent.Text = "Detail Name";
            // 
            // labelChildDetailCount
            // 
            this.labelChildDetailCount.AutoSize = true;
            this.labelChildDetailCount.Location = new System.Drawing.Point(306, 210);
            this.labelChildDetailCount.Name = "labelChildDetailCount";
            this.labelChildDetailCount.Size = new System.Drawing.Size(79, 15);
            this.labelChildDetailCount.TabIndex = 5;
            this.labelChildDetailCount.Text = "Count to Add";
            // 
            // labelErrorMessage
            // 
            this.labelErrorMessage.AutoSize = true;
            this.labelErrorMessage.ForeColor = System.Drawing.Color.Firebrick;
            this.labelErrorMessage.Location = new System.Drawing.Point(313, 365);
            this.labelErrorMessage.Name = "labelErrorMessage";
            this.labelErrorMessage.Size = new System.Drawing.Size(103, 15);
            this.labelErrorMessage.TabIndex = 6;
            this.labelErrorMessage.Text = "labelErrorMessage";
            this.labelErrorMessage.Visible = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(571, 450);
            this.Controls.Add(this.labelErrorMessage);
            this.Controls.Add(this.labelChildDetailCount);
            this.Controls.Add(this.labelDetailNameParent);
            this.Controls.Add(this.labelDetailNameRename);
            this.Controls.Add(this.labelDetailNameChild);
            this.Controls.Add(this.tbRenameDetail);
            this.Controls.Add(this.labelRenameDetailTitle);
            this.Controls.Add(this.tbAddChildName);
            this.Controls.Add(this.tbAddParentName);
            this.Controls.Add(this.labelAddChildTitle);
            this.Controls.Add(this.labelAddParentTitle);
            this.Controls.Add(this.numericUpDownDetailCount);
            this.Controls.Add(this.treeView);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDetailCount)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TreeView treeView;
        private System.Windows.Forms.ContextMenuStrip contextMenu;
        private System.Windows.Forms.NumericUpDown numericUpDownDetailCount;
        private System.Windows.Forms.Label labelAddParentTitle;
        private System.Windows.Forms.Label labelAddChildTitle;
        private System.Windows.Forms.TextBox tbAddParentName;
        private System.Windows.Forms.TextBox tbAddChildName;
        private System.Windows.Forms.Label labelRenameDetailTitle;
        private System.Windows.Forms.TextBox tbRenameDetail;
        private System.Windows.Forms.Label labelDetailNameChild;
        private System.Windows.Forms.Label labelDetailNameRename;
        private System.Windows.Forms.Label labelDetailNameParent;
        private System.Windows.Forms.Label labelChildDetailCount;
        private System.Windows.Forms.Label labelErrorMessage;
    }
}

