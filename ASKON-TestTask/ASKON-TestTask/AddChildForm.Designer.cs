namespace ASKON_TestTask
{
    partial class AddChildForm
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
            this.btnAdd = new System.Windows.Forms.Button();
            this.labelAddChildError = new System.Windows.Forms.Label();
            this.labeladdChildTitle = new System.Windows.Forms.Label();
            this.labelDetailName = new System.Windows.Forms.Label();
            this.tbDetailName = new System.Windows.Forms.TextBox();
            this.labelCountToAdd = new System.Windows.Forms.Label();
            this.numericUpDownCountToAdd = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownCountToAdd)).BeginInit();
            this.SuspendLayout();
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(90, 168);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 23);
            this.btnAdd.TabIndex = 6;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // labelAddChildError
            // 
            this.labelAddChildError.AutoSize = true;
            this.labelAddChildError.ForeColor = System.Drawing.Color.DarkRed;
            this.labelAddChildError.Location = new System.Drawing.Point(32, 121);
            this.labelAddChildError.Name = "labelAddChildError";
            this.labelAddChildError.Size = new System.Drawing.Size(107, 15);
            this.labelAddChildError.TabIndex = 7;
            this.labelAddChildError.Text = "labelAddChildError";
            this.labelAddChildError.Visible = false;
            // 
            // labeladdChildTitle
            // 
            this.labeladdChildTitle.AutoSize = true;
            this.labeladdChildTitle.Location = new System.Drawing.Point(99, 9);
            this.labeladdChildTitle.Name = "labeladdChildTitle";
            this.labeladdChildTitle.Size = new System.Drawing.Size(57, 15);
            this.labeladdChildTitle.TabIndex = 3;
            this.labeladdChildTitle.Text = "AddChild";
            // 
            // labelDetailName
            // 
            this.labelDetailName.AutoSize = true;
            this.labelDetailName.Location = new System.Drawing.Point(32, 29);
            this.labelDetailName.Name = "labelDetailName";
            this.labelDetailName.Size = new System.Drawing.Size(72, 15);
            this.labelDetailName.TabIndex = 5;
            this.labelDetailName.Text = "Detail Name";
            // 
            // tbDetailName
            // 
            this.tbDetailName.Location = new System.Drawing.Point(32, 47);
            this.tbDetailName.Name = "tbDetailName";
            this.tbDetailName.Size = new System.Drawing.Size(186, 23);
            this.tbDetailName.TabIndex = 4;
            this.tbDetailName.Enter += new System.EventHandler(this.tbDetailName_Enter);
            // 
            // labelCountToAdd
            // 
            this.labelCountToAdd.AutoSize = true;
            this.labelCountToAdd.Location = new System.Drawing.Point(25, 87);
            this.labelCountToAdd.Name = "labelCountToAdd";
            this.labelCountToAdd.Size = new System.Drawing.Size(82, 15);
            this.labelCountToAdd.TabIndex = 5;
            this.labelCountToAdd.Text = "Count to Add:";
            // 
            // numericUpDownCountToAdd
            // 
            this.numericUpDownCountToAdd.Location = new System.Drawing.Point(110, 85);
            this.numericUpDownCountToAdd.Name = "numericUpDownCountToAdd";
            this.numericUpDownCountToAdd.Size = new System.Drawing.Size(108, 23);
            this.numericUpDownCountToAdd.TabIndex = 2;
            this.numericUpDownCountToAdd.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // AddChildForm
            // 
            this.AcceptButton = this.btnAdd;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(249, 203);
            this.Controls.Add(this.numericUpDownCountToAdd);
            this.Controls.Add(this.labelCountToAdd);
            this.Controls.Add(this.tbDetailName);
            this.Controls.Add(this.labelDetailName);
            this.Controls.Add(this.labeladdChildTitle);
            this.Controls.Add(this.labelAddChildError);
            this.Controls.Add(this.btnAdd);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "AddChildForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "AddChildForm";
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownCountToAdd)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Label labelAddChildError;
        private System.Windows.Forms.Label labeladdChildTitle;
        private System.Windows.Forms.Label labelDetailName;
        private System.Windows.Forms.TextBox tbDetailName;
        private System.Windows.Forms.Label labelCountToAdd;
        private System.Windows.Forms.NumericUpDown numericUpDownCountToAdd;
    }
}