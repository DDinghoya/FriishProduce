﻿
namespace FriishProduce
{
    partial class Options_VCN64
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
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.Cancel = new System.Windows.Forms.Button();
            this.OK = new System.Windows.Forms.Button();
            this.n64000 = new System.Windows.Forms.CheckBox();
            this.gbox008 = new System.Windows.Forms.GroupBox();
            this.n64003 = new System.Windows.Forms.CheckBox();
            this.n64002 = new System.Windows.Forms.CheckBox();
            this.n64001 = new System.Windows.Forms.CheckBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.x009 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.n64004 = new System.Windows.Forms.GroupBox();
            this.ROMCType = new System.Windows.Forms.ComboBox();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.gbox008.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.n64004.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.SystemColors.ControlDark;
            this.panel2.Controls.Add(this.panel1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 233);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(459, 48);
            this.panel2.TabIndex = 12;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ControlLight;
            this.panel1.Controls.Add(this.Cancel);
            this.panel1.Controls.Add(this.OK);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 1);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(459, 47);
            this.panel1.TabIndex = 4;
            // 
            // Cancel
            // 
            this.Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Cancel.Location = new System.Drawing.Point(347, 12);
            this.Cancel.Name = "Cancel";
            this.Cancel.Size = new System.Drawing.Size(100, 23);
            this.Cancel.TabIndex = 4;
            this.Cancel.Tag = "";
            this.Cancel.Text = "Cancel";
            this.Cancel.UseVisualStyleBackColor = true;
            // 
            // OK
            // 
            this.OK.Location = new System.Drawing.Point(237, 12);
            this.OK.Name = "OK";
            this.OK.Size = new System.Drawing.Size(100, 23);
            this.OK.TabIndex = 3;
            this.OK.Tag = "";
            this.OK.Text = "OK";
            this.OK.UseVisualStyleBackColor = true;
            this.OK.Click += new System.EventHandler(this.OK_Click);
            // 
            // n64000
            // 
            this.n64000.AutoSize = true;
            this.n64000.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
            this.n64000.Location = new System.Drawing.Point(14, 23);
            this.n64000.MaximumSize = new System.Drawing.Size(203, 27);
            this.n64000.Name = "n64000";
            this.n64000.Size = new System.Drawing.Size(124, 17);
            this.n64000.TabIndex = 13;
            this.n64000.Text = "Screen brightness fix";
            this.n64000.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.n64000.UseVisualStyleBackColor = true;
            // 
            // gbox008
            // 
            this.gbox008.Controls.Add(this.n64003);
            this.gbox008.Controls.Add(this.n64002);
            this.gbox008.Controls.Add(this.n64001);
            this.gbox008.Controls.Add(this.n64000);
            this.gbox008.Controls.Add(this.panel3);
            this.gbox008.Location = new System.Drawing.Point(14, 14);
            this.gbox008.Name = "gbox008";
            this.gbox008.Size = new System.Drawing.Size(430, 140);
            this.gbox008.TabIndex = 14;
            this.gbox008.TabStop = false;
            this.gbox008.Text = "Patches";
            // 
            // n64003
            // 
            this.n64003.AutoSize = true;
            this.n64003.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
            this.n64003.Location = new System.Drawing.Point(14, 107);
            this.n64003.MaximumSize = new System.Drawing.Size(203, 27);
            this.n64003.Name = "n64003";
            this.n64003.Size = new System.Drawing.Size(128, 17);
            this.n64003.TabIndex = 16;
            this.n64003.Text = "Allocate for ROM size";
            this.n64003.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.n64003.UseVisualStyleBackColor = true;
            // 
            // n64002
            // 
            this.n64002.AutoSize = true;
            this.n64002.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
            this.n64002.Location = new System.Drawing.Point(14, 79);
            this.n64002.MaximumSize = new System.Drawing.Size(203, 27);
            this.n64002.Name = "n64002";
            this.n64002.Size = new System.Drawing.Size(128, 17);
            this.n64002.TabIndex = 15;
            this.n64002.Text = "Extended RAM patch";
            this.n64002.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.n64002.UseVisualStyleBackColor = true;
            // 
            // n64001
            // 
            this.n64001.AutoSize = true;
            this.n64001.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
            this.n64001.Location = new System.Drawing.Point(14, 51);
            this.n64001.MaximumSize = new System.Drawing.Size(203, 27);
            this.n64001.Name = "n64001";
            this.n64001.Size = new System.Drawing.Size(77, 17);
            this.n64001.TabIndex = 14;
            this.n64001.Text = "Crashes fix";
            this.n64001.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.n64001.UseVisualStyleBackColor = true;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(224)))), ((int)(((byte)(234)))));
            this.panel3.Controls.Add(this.x009);
            this.panel3.Controls.Add(this.pictureBox1);
            this.panel3.Location = new System.Drawing.Point(222, 26);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(195, 89);
            this.panel3.TabIndex = 19;
            // 
            // x009
            // 
            this.x009.AutoSize = true;
            this.x009.Font = new System.Drawing.Font("Arial", 8.25F);
            this.x009.Location = new System.Drawing.Point(42, 6);
            this.x009.MaximumSize = new System.Drawing.Size(146, 75);
            this.x009.Name = "x009";
            this.x009.Size = new System.Drawing.Size(144, 42);
            this.x009.TabIndex = 18;
            this.x009.Text = "Please note that these options may not work for all base WADs.";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::FriishProduce.Properties.Resources.info_rhombus_large;
            this.pictureBox1.Location = new System.Drawing.Point(4, 4);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(32, 32);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 17;
            this.pictureBox1.TabStop = false;
            // 
            // n64004
            // 
            this.n64004.Controls.Add(this.ROMCType);
            this.n64004.Location = new System.Drawing.Point(14, 166);
            this.n64004.Name = "n64004";
            this.n64004.Size = new System.Drawing.Size(430, 55);
            this.n64004.TabIndex = 15;
            this.n64004.TabStop = false;
            this.n64004.Text = "ROM compression type";
            // 
            // ROMCType
            // 
            this.ROMCType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ROMCType.FormattingEnabled = true;
            this.ROMCType.Items.AddRange(new object[] {
            "ROMC Type 0",
            "ROMC Type 1"});
            this.ROMCType.Location = new System.Drawing.Point(13, 22);
            this.ROMCType.Name = "ROMCType";
            this.ROMCType.Size = new System.Drawing.Size(404, 21);
            this.ROMCType.TabIndex = 0;
            // 
            // Options_VCN64
            // 
            this.AcceptButton = this.OK;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.CancelButton = this.Cancel;
            this.ClientSize = new System.Drawing.Size(459, 281);
            this.Controls.Add(this.gbox008);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.n64004);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Options_VCN64";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Tag = "x006";
            this.Text = "options";
            this.Load += new System.EventHandler(this.Form_Load);
            this.panel2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.gbox008.ResumeLayout(false);
            this.gbox008.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.n64004.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button Cancel;
        private System.Windows.Forms.Button OK;
        private System.Windows.Forms.CheckBox n64000;
        private System.Windows.Forms.GroupBox gbox008;
        private System.Windows.Forms.CheckBox n64003;
        private System.Windows.Forms.CheckBox n64002;
        private System.Windows.Forms.CheckBox n64001;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label x009;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.GroupBox n64004;
        private System.Windows.Forms.ComboBox ROMCType;
    }
}