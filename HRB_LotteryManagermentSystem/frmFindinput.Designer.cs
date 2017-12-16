namespace HRB_LotteryManagermentSystem
{
    partial class frmFindinput
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
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.stockInDateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.label6 = new System.Windows.Forms.Label();
            this.stockOutDateTimePicker = new System.Windows.Forms.DateTimePicker();
            this.label7 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.textBox8 = new System.Windows.Forms.TextBox();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button1.Location = new System.Drawing.Point(255, 99);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 34);
            this.button1.TabIndex = 0;
            this.button1.Text = "确认";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(356, 99);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 34);
            this.button2.TabIndex = 1;
            this.button2.Text = "取消";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.stockInDateTimePicker1);
            this.groupBox4.Controls.Add(this.label6);
            this.groupBox4.Controls.Add(this.stockOutDateTimePicker);
            this.groupBox4.Controls.Add(this.label7);
            this.groupBox4.Controls.Add(this.label1);
            this.groupBox4.Controls.Add(this.label24);
            this.groupBox4.Controls.Add(this.textBox8);
            this.groupBox4.Location = new System.Drawing.Point(8, 12);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(445, 81);
            this.groupBox4.TabIndex = 96;
            this.groupBox4.TabStop = false;
            // 
            // stockInDateTimePicker1
            // 
            this.stockInDateTimePicker1.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.stockInDateTimePicker1.Location = new System.Drawing.Point(314, 16);
            this.stockInDateTimePicker1.Name = "stockInDateTimePicker1";
            this.stockInDateTimePicker1.Size = new System.Drawing.Size(109, 20);
            this.stockInDateTimePicker1.TabIndex = 110;
            this.stockInDateTimePicker1.Value = new System.DateTime(2017, 3, 31, 0, 0, 0, 0);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(253, 20);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(55, 13);
            this.label6.TabIndex = 112;
            this.label6.Text = "结束时间";
            // 
            // stockOutDateTimePicker
            // 
            this.stockOutDateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.stockOutDateTimePicker.Location = new System.Drawing.Point(81, 16);
            this.stockOutDateTimePicker.Name = "stockOutDateTimePicker";
            this.stockOutDateTimePicker.Size = new System.Drawing.Size(114, 20);
            this.stockOutDateTimePicker.TabIndex = 109;
            this.stockOutDateTimePicker.Value = new System.DateTime(2017, 3, 1, 0, 0, 0, 0);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(13, 20);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(55, 13);
            this.label7.TabIndex = 111;
            this.label7.Text = "开始时间";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(253, 54);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(133, 15);
            this.label1.TabIndex = 108;
            this.label1.Text = "*如果查找所有请填写\"*\"";
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.label24.Location = new System.Drawing.Point(12, 54);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(55, 13);
            this.label24.TabIndex = 107;
            this.label24.Text = "查找期号";
            // 
            // textBox8
            // 
            this.textBox8.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.textBox8.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox8.Location = new System.Drawing.Point(81, 50);
            this.textBox8.Name = "textBox8";
            this.textBox8.Size = new System.Drawing.Size(160, 26);
            this.textBox8.TabIndex = 106;
            // 
            // frmFindinput
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(465, 144);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Name = "frmFindinput";
            this.Text = "查找条件";
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.DateTimePicker stockInDateTimePicker1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.DateTimePicker stockOutDateTimePicker;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.TextBox textBox8;
    }
}