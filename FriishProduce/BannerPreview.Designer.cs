﻿
namespace FriishProduce
{
    partial class BannerPreview
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.Animation1 = new System.Windows.Forms.Timer(this.components);
            this.BannerPreview_Panel = new System.Windows.Forms.Panel();
            this.BannerPreview_Label = new System.Windows.Forms.Label();
            this.BannerPreview_Line2 = new System.Windows.Forms.PictureBox();
            this.BannerPreview_Line1 = new System.Windows.Forms.PictureBox();
            this.Image = new System.Windows.Forms.PictureBox();
            this.BannerPreview_Players = new System.Windows.Forms.Label();
            this.BannerPreview_Year = new System.Windows.Forms.Label();
            this.BannerPreview_BG = new System.Windows.Forms.PictureBox();
            this.BannerPreview_Buffer = new System.Windows.Forms.PictureBox();
            this.BannerPreview_Panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.BannerPreview_Line2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.BannerPreview_Line1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Image)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.BannerPreview_BG)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.BannerPreview_Buffer)).BeginInit();
            this.SuspendLayout();
            // 
            // Animation1
            // 
            this.Animation1.Interval = 10;
            this.Animation1.Tick += new System.EventHandler(this.Animation1_Tick);
            // 
            // BannerPreview_Panel
            // 
            this.BannerPreview_Panel.BackColor = System.Drawing.SystemColors.ControlLight;
            this.BannerPreview_Panel.Controls.Add(this.BannerPreview_Label);
            this.BannerPreview_Panel.Controls.Add(this.BannerPreview_Line2);
            this.BannerPreview_Panel.Controls.Add(this.BannerPreview_Line1);
            this.BannerPreview_Panel.Controls.Add(this.Image);
            this.BannerPreview_Panel.Controls.Add(this.BannerPreview_Players);
            this.BannerPreview_Panel.Controls.Add(this.BannerPreview_Year);
            this.BannerPreview_Panel.Controls.Add(this.BannerPreview_BG);
            this.BannerPreview_Panel.Controls.Add(this.BannerPreview_Buffer);
            this.BannerPreview_Panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BannerPreview_Panel.Location = new System.Drawing.Point(0, 0);
            this.BannerPreview_Panel.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.BannerPreview_Panel.Name = "BannerPreview_Panel";
            this.BannerPreview_Panel.Size = new System.Drawing.Size(625, 277);
            this.BannerPreview_Panel.TabIndex = 44;
            // 
            // BannerPreview_Label
            // 
            this.BannerPreview_Label.BackColor = System.Drawing.Color.Gainsboro;
            this.BannerPreview_Label.Font = new System.Drawing.Font("Arial", 17F);
            this.BannerPreview_Label.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BannerPreview_Label.Location = new System.Drawing.Point(-45, 196);
            this.BannerPreview_Label.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.BannerPreview_Label.Name = "BannerPreview_Label";
            this.BannerPreview_Label.Size = new System.Drawing.Size(714, 50);
            this.BannerPreview_Label.TabIndex = 43;
            this.BannerPreview_Label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.BannerPreview_Label.UseMnemonic = false;
            // 
            // BannerPreview_Line2
            // 
            this.BannerPreview_Line2.BackColor = System.Drawing.Color.Silver;
            this.BannerPreview_Line2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BannerPreview_Line2.Location = new System.Drawing.Point(-2, 100);
            this.BannerPreview_Line2.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.BannerPreview_Line2.Name = "BannerPreview_Line2";
            this.BannerPreview_Line2.Size = new System.Drawing.Size(117, 3);
            this.BannerPreview_Line2.TabIndex = 45;
            this.BannerPreview_Line2.TabStop = false;
            this.BannerPreview_Line2.Paint += new System.Windows.Forms.PaintEventHandler(this.BannerPreview_Paint);
            // 
            // BannerPreview_Line1
            // 
            this.BannerPreview_Line1.BackColor = System.Drawing.Color.Silver;
            this.BannerPreview_Line1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BannerPreview_Line1.Location = new System.Drawing.Point(-2, 55);
            this.BannerPreview_Line1.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.BannerPreview_Line1.Name = "BannerPreview_Line1";
            this.BannerPreview_Line1.Size = new System.Drawing.Size(117, 3);
            this.BannerPreview_Line1.TabIndex = 44;
            this.BannerPreview_Line1.TabStop = false;
            this.BannerPreview_Line1.Paint += new System.Windows.Forms.PaintEventHandler(this.BannerPreview_Paint);
            // 
            // Image
            // 
            this.Image.BackColor = System.Drawing.SystemColors.ControlLight;
            this.Image.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Image.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.Image.Location = new System.Drawing.Point(232, 54);
            this.Image.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.Image.Name = "Image";
            this.Image.Size = new System.Drawing.Size(160, 133);
            this.Image.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.Image.TabIndex = 32;
            this.Image.TabStop = false;
            // 
            // BannerPreview_Players
            // 
            this.BannerPreview_Players.Font = new System.Drawing.Font("Tahoma", 11F);
            this.BannerPreview_Players.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BannerPreview_Players.Location = new System.Drawing.Point(11, 63);
            this.BannerPreview_Players.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.BannerPreview_Players.Name = "BannerPreview_Players";
            this.BannerPreview_Players.Size = new System.Drawing.Size(153, 35);
            this.BannerPreview_Players.TabIndex = 47;
            this.BannerPreview_Players.Text = "Players:";
            this.BannerPreview_Players.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.BannerPreview_Players.UseMnemonic = false;
            // 
            // BannerPreview_Year
            // 
            this.BannerPreview_Year.Font = new System.Drawing.Font("Tahoma", 11F);
            this.BannerPreview_Year.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BannerPreview_Year.Location = new System.Drawing.Point(11, 17);
            this.BannerPreview_Year.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.BannerPreview_Year.Name = "BannerPreview_Year";
            this.BannerPreview_Year.Size = new System.Drawing.Size(153, 35);
            this.BannerPreview_Year.TabIndex = 46;
            this.BannerPreview_Year.Text = "Released:";
            this.BannerPreview_Year.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.BannerPreview_Year.UseMnemonic = false;
            // 
            // BannerPreview_BG
            // 
            this.BannerPreview_BG.BackColor = System.Drawing.Color.WhiteSmoke;
            this.BannerPreview_BG.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BannerPreview_BG.Location = new System.Drawing.Point(-78, 147);
            this.BannerPreview_BG.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.BannerPreview_BG.Name = "BannerPreview_BG";
            this.BannerPreview_BG.Size = new System.Drawing.Size(781, 50);
            this.BannerPreview_BG.TabIndex = 42;
            this.BannerPreview_BG.TabStop = false;
            this.BannerPreview_BG.Paint += new System.Windows.Forms.PaintEventHandler(this.BannerPreview_Paint);
            // 
            // BannerPreview_Buffer
            // 
            this.BannerPreview_Buffer.BackColor = System.Drawing.Color.Gainsboro;
            this.BannerPreview_Buffer.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BannerPreview_Buffer.Location = new System.Drawing.Point(-45, 194);
            this.BannerPreview_Buffer.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.BannerPreview_Buffer.Name = "BannerPreview_Buffer";
            this.BannerPreview_Buffer.Size = new System.Drawing.Size(714, 221);
            this.BannerPreview_Buffer.TabIndex = 48;
            this.BannerPreview_Buffer.TabStop = false;
            // 
            // BannerPreview
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(4F, 10F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.BannerPreview_Panel);
            this.Font = new System.Drawing.Font("Tahoma", 6F);
            this.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.Name = "BannerPreview";
            this.Size = new System.Drawing.Size(625, 277);
            this.BannerPreview_Panel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.BannerPreview_Line2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.BannerPreview_Line1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Image)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.BannerPreview_BG)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.BannerPreview_Buffer)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer Animation1;
        private System.Windows.Forms.Panel BannerPreview_Panel;
        private System.Windows.Forms.Label BannerPreview_Label;
        private System.Windows.Forms.PictureBox BannerPreview_Line2;
        private System.Windows.Forms.PictureBox BannerPreview_Line1;
        private System.Windows.Forms.PictureBox Image;
        private System.Windows.Forms.Label BannerPreview_Players;
        private System.Windows.Forms.Label BannerPreview_Year;
        private System.Windows.Forms.PictureBox BannerPreview_BG;
        private System.Windows.Forms.PictureBox BannerPreview_Buffer;
    }
}
