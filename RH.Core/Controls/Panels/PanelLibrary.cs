﻿using System;
using System.Drawing;
using System.Windows.Forms;
using RH.Core.Helpers;

namespace RH.Core.Controls.Panels
{
    public partial class PanelLibrary : UserControlEx
    {
        #region Var

        public EventHandler OnOpenLibrary;
        public EventHandler OnDelete;
        public EventHandler OnSave;
        public EventHandler OnExport;

        #endregion

        public PanelLibrary(bool needExport, bool needSaveDelete)
        {
            InitializeComponent();

            switch (ProgramCore.CurrentProgram)
            {
                case ProgramCore.ProgramMode.PrintAhead_PayPal:
                    btnExport.Visible = false;
                    break;
                default:
                    btnExport.Visible = needExport;
                    break;
            }

            btnSave.Visible = btnDelete.Visible = needSaveDelete;
        }

        #region Form's event

        private void btnSave_MouseDown(object sender, MouseEventArgs e)
        {
            btnSave.BackColor = SystemColors.ControlDarkDark;
            btnSave.ForeColor = Color.White;
        }
        private void btnSave_MouseUp(object sender, MouseEventArgs e)
        {
            btnSave.BackColor = SystemColors.Control;
            btnSave.ForeColor = Color.Black;

            OnSave?.Invoke(this, EventArgs.Empty);
        }

        private void btnDelete_MouseDown(object sender, MouseEventArgs e)
        {
            btnDelete.BackColor = SystemColors.ControlDarkDark;
            btnDelete.ForeColor = Color.White;
        }
        private void btnDelete_MouseUp(object sender, MouseEventArgs e)
        {
            btnDelete.BackColor = SystemColors.Control;
            btnDelete.ForeColor = Color.Black;

            OnDelete?.Invoke(this, EventArgs.Empty);
        }

        private void btnExport_MouseDown(object sender, MouseEventArgs e)
        {
            btnExport.BackColor = SystemColors.ControlDarkDark;
            btnExport.ForeColor = Color.White;
        }
        private void btnExport_MouseUp(object sender, MouseEventArgs e)
        {
            btnExport.BackColor = SystemColors.Control;
            btnExport.ForeColor = Color.Black;

            OnExport?.Invoke(this, EventArgs.Empty);
        }

        public void HideControl()
        {
            btnOpen.Tag = "2";
            btnOpen.BackColor = SystemColors.Control;
            btnOpen.ForeColor = Color.Black;
        }
        public void ShowControl()
        {
            btnOpen.Tag = "1";
            btnOpen.BackColor = SystemColors.ControlDarkDark;
            btnOpen.ForeColor = Color.White;
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            if (btnOpen.Tag.ToString() == "2")
            {
                btnOpen.Tag = "1";
                btnOpen.BackColor = SystemColors.ControlDarkDark;
                btnOpen.ForeColor = Color.White;
                OnOpenLibrary?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                btnOpen.Tag = "2";
                btnOpen.BackColor = SystemColors.Control;
                btnOpen.ForeColor = Color.Black;
                OnOpenLibrary?.Invoke(this, EventArgs.Empty);
            }
        }

        #endregion
    }
}
