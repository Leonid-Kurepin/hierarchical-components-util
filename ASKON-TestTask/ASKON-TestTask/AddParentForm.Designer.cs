namespace ASKON_TestTask
{
    partial class AddParentForm
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
            this.labelAddParentTitle = new System.Windows.Forms.Label();
            this.labelAddParentName = new System.Windows.Forms.Label();
            this.tbAddParentName = new System.Windows.Forms.TextBox();
            this.btnAddParent = new System.Windows.Forms.Button();
            this.labelAddParentError = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // labelAddParentTitle
            // 
            this.labelAddParentTitle.AutoSize = true;
            this.labelAddParentTitle.Location = new System.Drawing.Point(115, 9);
            this.labelAddParentTitle.Name = "labelAddParentTitle";
            this.labelAddParentTitle.Size = new System.Drawing.Size(63, 15);
            this.labelAddParentTitle.TabIndex = 3;
            this.labelAddParentTitle.Text = "AddParent";
            // 
            // labelAddParentName
            // 
            this.labelAddParentName.AutoSize = true;
            this.labelAddParentName.Location = new System.Drawing.Point(52, 35);
            this.labelAddParentName.Name = "labelAddParentName";
            this.labelAddParentName.Size = new System.Drawing.Size(72, 15);
            this.labelAddParentName.TabIndex = 5;
            this.labelAddParentName.Text = "Detail Name";
            // 
            // tbAddParentName
            // 
            this.tbAddParentName.Location = new System.Drawing.Point(52, 53);
            this.tbAddParentName.Name = "tbAddParentName";
            this.tbAddParentName.Size = new System.Drawing.Size(186, 23);
            this.tbAddParentName.TabIndex = 4;
            this.tbAddParentName.Enter += new System.EventHandler(this.tbAddParentName_Enter);
            // 
            // btnAddParent
            // 
            this.btnAddParent.Location = new System.Drawing.Point(115, 97);
            this.btnAddParent.Name = "btnAddParent";
            this.btnAddParent.Size = new System.Drawing.Size(75, 23);
            this.btnAddParent.TabIndex = 6;
            this.btnAddParent.Text = "Add";
            this.btnAddParent.UseVisualStyleBackColor = true;
            this.btnAddParent.Click += new System.EventHandler(this.btnAddParent_Click);
            // 
            // labelAddParentError
            // 
            this.labelAddParentError.AutoSize = true;
            this.labelAddParentError.ForeColor = System.Drawing.Color.DarkRed;
            this.labelAddParentError.Location = new System.Drawing.Point(52, 79);
            this.labelAddParentError.Name = "labelAddParentError";
            this.labelAddParentError.Size = new System.Drawing.Size(113, 15);
            this.labelAddParentError.TabIndex = 7;
            this.labelAddParentError.Text = "labelAddParentError";
            this.labelAddParentError.Visible = false;
            // 
            // AddParentForm
            // 
            this.AcceptButton = this.btnAddParent;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(290, 132);
            this.Controls.Add(this.labelAddParentError);
            this.Controls.Add(this.btnAddParent);
            this.Controls.Add(this.tbAddParentName);
            this.Controls.Add(this.labelAddParentName);
            this.Controls.Add(this.labelAddParentTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "AddParentForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "AddParentForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelAddParentTitle;
        private System.Windows.Forms.Label labelAddParentName;
        private System.Windows.Forms.TextBox tbAddParentName;
        private System.Windows.Forms.Button btnAddParent;
        private System.Windows.Forms.Label labelAddParentError;
    }
}