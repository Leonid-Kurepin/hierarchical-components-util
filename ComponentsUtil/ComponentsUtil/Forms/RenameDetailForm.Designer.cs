namespace ComponentsUtil.Forms
{
    partial class RenameDetailForm
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
            this.tbNewDetailName = new System.Windows.Forms.TextBox();
            this.labelNewDetailName = new System.Windows.Forms.Label();
            this.labelRenameDetail = new System.Windows.Forms.Label();
            this.labelRenameDetailError = new System.Windows.Forms.Label();
            this.btnRenameDetail = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // tbNewDetailName
            // 
            this.tbNewDetailName.Location = new System.Drawing.Point(56, 79);
            this.tbNewDetailName.Name = "tbNewDetailName";
            this.tbNewDetailName.Size = new System.Drawing.Size(186, 23);
            this.tbNewDetailName.TabIndex = 4;
            this.tbNewDetailName.Enter += new System.EventHandler(this.tbNewDetailName_Enter);
            // 
            // labelNewDetailName
            // 
            this.labelNewDetailName.AutoSize = true;
            this.labelNewDetailName.Location = new System.Drawing.Point(56, 61);
            this.labelNewDetailName.Name = "labelNewDetailName";
            this.labelNewDetailName.Size = new System.Drawing.Size(99, 15);
            this.labelNewDetailName.TabIndex = 5;
            this.labelNewDetailName.Text = "New Detail Name";
            // 
            // labelRenameDetail
            // 
            this.labelRenameDetail.AutoSize = true;
            this.labelRenameDetail.Location = new System.Drawing.Point(119, 24);
            this.labelRenameDetail.Name = "labelRenameDetail";
            this.labelRenameDetail.Size = new System.Drawing.Size(83, 15);
            this.labelRenameDetail.TabIndex = 3;
            this.labelRenameDetail.Text = "Rename Detail";
            // 
            // labelRenameDetailError
            // 
            this.labelRenameDetailError.AutoSize = true;
            this.labelRenameDetailError.ForeColor = System.Drawing.Color.DarkRed;
            this.labelRenameDetailError.Location = new System.Drawing.Point(56, 105);
            this.labelRenameDetailError.Name = "labelRenameDetailError";
            this.labelRenameDetailError.Size = new System.Drawing.Size(130, 15);
            this.labelRenameDetailError.TabIndex = 7;
            this.labelRenameDetailError.Text = "labelRenameDetailError";
            this.labelRenameDetailError.Visible = false;
            // 
            // btnRenameDetail
            // 
            this.btnRenameDetail.Location = new System.Drawing.Point(119, 152);
            this.btnRenameDetail.Name = "btnRenameDetail";
            this.btnRenameDetail.Size = new System.Drawing.Size(75, 23);
            this.btnRenameDetail.TabIndex = 6;
            this.btnRenameDetail.Text = "Rename";
            this.btnRenameDetail.UseVisualStyleBackColor = true;
            this.btnRenameDetail.Click += new System.EventHandler(this.btnRenameDetail_ClickAsync);
            // 
            // RenameDetailForm
            // 
            this.AcceptButton = this.btnRenameDetail;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(310, 187);
            this.Controls.Add(this.btnRenameDetail);
            this.Controls.Add(this.labelRenameDetailError);
            this.Controls.Add(this.labelRenameDetail);
            this.Controls.Add(this.labelNewDetailName);
            this.Controls.Add(this.tbNewDetailName);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "RenameDetailForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "RenameDetailForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbNewDetailName;
        private System.Windows.Forms.Label labelNewDetailName;
        private System.Windows.Forms.Label labelRenameDetail;
        private System.Windows.Forms.Label labelRenameDetailError;
        private System.Windows.Forms.Button btnRenameDetail;
    }
}