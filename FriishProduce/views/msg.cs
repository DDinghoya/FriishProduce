﻿using System;
using System.Drawing;
using System.Media;
using System.Windows.Forms;

namespace FriishProduce
{
    internal partial class Msg : Form
    {
        public Msg(string msg, string title, MessageBoxButtons buttons = MessageBoxButtons.OK, MessageBoxIcon icon = MessageBoxIcon.Information)
        {
            InitializeComponent();
            Text = title;
            label1.Text = msg;
            label1.AutoSize = true;

            var minWidth = 256;
            var minHeight = 137;
            MinimumSize = new Size(minWidth, minHeight);

            int newWidth = 422;
            int maxWidth = label1.MaximumSize.Width;
            if (label1.Width < maxWidth)
            {
                var oldWidth = Width;
                newWidth = label1.Width + 90;

                if (newWidth > minWidth)
                {
                    OK.Location = new Point(OK.Location.X - (maxWidth - label1.Width), OK.Location.Y);
                    Cancel.Location = new Point(Cancel.Location.X - (maxWidth - label1.Width), Cancel.Location.Y);
                }
                else
                {
                    OK.Location = new Point(55, OK.Location.Y);
                    Cancel.Location = new Point(147, Cancel.Location.Y);
                }
            }

            int newHeight = pictureBox1.Location.Y + label1.Height + pictureBox1.Location.Y + 84;
            Size = new Size(newWidth > minWidth ? newWidth : minWidth, newHeight > minHeight ? newHeight : minHeight);
            border.Location = new Point(border.Location.X, panel.Location.Y - 1);

            #region Themes
            if (Properties.Settings.Default.LightTheme)
            {
                BackColor = Color.FromArgb(248, 248, 248);
                ForeColor = Color.Black;
                panel.BackColor = Color.FromArgb(232, 232, 232);
            }
            else
            {
                BackColor = Color.FromArgb(55, 55, 55);
                ForeColor = Color.White;
                panel.BackColor = Color.FromArgb(75, 75, 75);
            }
            #endregion

            switch (buttons)
            {
                case MessageBoxButtons.OK:
                    OK.Location = Cancel.Location;
                    OK.Text = Program.Language.Get("OK");
                    OK.DialogResult = DialogResult.OK;
                    Cancel.Dispose();
                    break;
                case MessageBoxButtons.OKCancel:
                    OK.Text = Program.Language.Get("OK");
                    OK.DialogResult = DialogResult.OK;
                    OK.Text = Program.Language.Get("Cancel");
                    Cancel.DialogResult = DialogResult.Cancel;
                    break;
                case MessageBoxButtons.YesNo:
                    OK.Text = Program.Language.Get("Yes");
                    OK.DialogResult = DialogResult.Yes;
                    Cancel.Text = Program.Language.Get("No");
                    Cancel.DialogResult = DialogResult.No;
                    break;
            }

            switch (icon)
            {
                case (MessageBoxIcon)16:
                    pictureBox1.Image = SystemIcons.Hand.ToBitmap();
                    SystemSounds.Hand.Play();
                    break;
                case (MessageBoxIcon)32:
                    pictureBox1.Image = SystemIcons.Question.ToBitmap();
                    SystemSounds.Question.Play();
                    break;
                case (MessageBoxIcon)48:
                    pictureBox1.Image = SystemIcons.Exclamation.ToBitmap();
                    SystemSounds.Exclamation.Play();
                    break;
                case (MessageBoxIcon)64:
                    pictureBox1.Image = SystemIcons.Information.ToBitmap();
                    SystemSounds.Asterisk.Play();
                    break;
            }
        }
    }

    public static class MessageBox
    {
        [STAThread]
        public static DialogResult Show(string msg, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            DialogResult r = DialogResult.None;

            using (var msgBox = new Msg(msg, Program.Language.Get("g000"), buttons, icon))
            {
                msgBox.ShowDialog();
                r = msgBox.DialogResult;
                msgBox.Dispose();
            }

            return r;
        }

        [STAThread]
        public static DialogResult Show(string msg, string title, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            DialogResult r = DialogResult.None;

            using (var msgBox = new Msg(msg, title, buttons, icon))
            {
                msgBox.ShowDialog();
                r = msgBox.DialogResult;
                msgBox.Dispose();
            }

            return r;
        }

        [STAThread]
        public static DialogResult Show(string msg)
        {
            DialogResult r = DialogResult.None;

            using (var msgBox = new Msg(msg, Program.Language.Get("g000")))
            {
                msgBox.ShowDialog();
                r = msgBox.DialogResult;
                msgBox.Dispose();
            }

            return r;
        }

        [STAThread]
        public static DialogResult Show(string msg, string title)
        {
            DialogResult r = DialogResult.None;

            using (var msgBox = new Msg(msg, title))
            {
                msgBox.ShowDialog();
                r = msgBox.DialogResult;
                msgBox.Dispose();
            }

            return r;
        }
    }
}
