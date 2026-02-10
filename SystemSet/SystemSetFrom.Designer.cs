namespace SystemSet
{
    partial class SystemSetFrom
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
            this.dgv_SystemSet = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btn_SaveSystemSet = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_SystemSet)).BeginInit();
            this.SuspendLayout();
            // 
            // dgv_SystemSet
            // 
            this.dgv_SystemSet.BackgroundColor = System.Drawing.Color.White;
            this.dgv_SystemSet.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_SystemSet.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2,
            this.Column3});
            this.dgv_SystemSet.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgv_SystemSet.Location = new System.Drawing.Point(0, 0);
            this.dgv_SystemSet.Name = "dgv_SystemSet";
            this.dgv_SystemSet.RowTemplate.Height = 23;
            this.dgv_SystemSet.Size = new System.Drawing.Size(1035, 518);
            this.dgv_SystemSet.TabIndex = 1;
            // 
            // Column1
            // 
            this.Column1.DataPropertyName = "PN";
            this.Column1.HeaderText = "名称";
            this.Column1.Name = "Column1";
            this.Column1.Width = 200;
            // 
            // Column2
            // 
            this.Column2.HeaderText = "参数";
            this.Column2.Name = "Column2";
            this.Column2.Width = 300;
            // 
            // Column3
            // 
            this.Column3.HeaderText = "备注";
            this.Column3.Name = "Column3";
            this.Column3.Width = 400;
            // 
            // btn_SaveSystemSet
            // 
            this.btn_SaveSystemSet.Location = new System.Drawing.Point(948, 483);
            this.btn_SaveSystemSet.Name = "btn_SaveSystemSet";
            this.btn_SaveSystemSet.Size = new System.Drawing.Size(75, 23);
            this.btn_SaveSystemSet.TabIndex = 2;
            this.btn_SaveSystemSet.Text = "保存";
            this.btn_SaveSystemSet.UseVisualStyleBackColor = true;
            this.btn_SaveSystemSet.Click += new System.EventHandler(this.btn_SaveSystemSet_Click);
            // 
            // SystemSetFrom
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1035, 518);
            this.Controls.Add(this.btn_SaveSystemSet);
            this.Controls.Add(this.dgv_SystemSet);
            this.Name = "SystemSetFrom";
            this.Text = "SystemSetFrom";
            this.Load += new System.EventHandler(this.SystemSetFrom_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgv_SystemSet)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgv_SystemSet;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.Button btn_SaveSystemSet;
    }
}