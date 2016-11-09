﻿using System;
using System.Drawing;
using System.Windows.Forms;
using RH.Core.Controls.PopupControl;
using RH.Core.Helpers;
using RH.Core.Properties;

namespace RH.Core.Controls
{
    public partial class ctrlBrushesPopup : UserControlEx
    {
        internal int CurrentBrush = -1;
        private readonly Cursor brushCursor;

        public ctrlBrushesPopup(int currentBrush)
        {
            InitializeComponent();
            MinimumSize = Size;
            MaximumSize = new Size(Size.Width * 2, Size.Height * 2);
            DoubleBuffered = true;
            ResizeRedraw = true;

            CurrentBrush = currentBrush;

            using (var bitmap = new Bitmap(Resources.brush, new Size(24, 24)))
            {
                var ptr = bitmap.GetHicon();
                brushCursor = new Cursor(ptr);
            }

            InitializeControls();
        }

        protected override void WndProc(ref Message m)
        {
            if ((Parent as Popup).ProcessResizing(ref m))
            {
                return;
            }
            base.WndProc(ref m);
        }

        private void pBrush_Click(object sender, EventArgs e)
        {
            var pb = sender as PictureBox;
            var newBrush = int.Parse(pb.Tag.ToString());

            if (newBrush == CurrentBrush) // если туда же щелкнули - отключаем
            {
                CurrentBrush = -1;
                ProgramCore.MainForm.ChangeCursors(DefaultCursor);
            }
            else
            {
                CurrentBrush = newBrush;
                ProgramCore.MainForm.ChangeCursors(brushCursor);
            }

            InitializeControls();

            ProgramCore.MainForm.frmMaterial.CurrentBrusn = CurrentBrush;
            var parent = Parent as Popup;
            parent.Close();
        }
        private void pBrush_MouseHover(object sender, EventArgs e)
        {
            var pb = sender as PictureBox;
            pb.BackColor = SystemColors.ControlLight;
        }
        private void pBrush_MouseLeave(object sender, EventArgs e)
        {
            var pb = sender as PictureBox;
            pb.BackColor = pb.Tag.ToString() == CurrentBrush.ToString() ? SystemColors.ScrollBar : SystemColors.Control;
        }

        private void InitializeControls()
        {
            foreach (var ctrl in Controls)
            {
                if (!(ctrl is PictureBox))
                    continue;
                var pb = ctrl as PictureBox;
                if (pb.Tag == null)
                    continue;
                var pbBrush = int.Parse(pb.Tag.ToString());
                pb.BackColor = pbBrush == CurrentBrush ? SystemColors.ScrollBar : SystemColors.Control;
            }
        }
    }
}
