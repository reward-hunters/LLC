﻿using System;
using System.Drawing;
using System.Windows.Forms;
using RH.Core.Helpers;

namespace RH.Core.Controls.Panels
{
    public partial class PanelFeatures : UserControlEx
    {
        #region Var

        public EventHandler OnDelete;
        public EventHandler OnDuplicate;
        public EventHandler OnSave;
        public EventHandler OnUndo;

        #endregion

        public PanelFeatures()
        {
            InitializeComponent();
            switch (ProgramCore.CurrentProgram)
            {
                case ProgramCore.ProgramMode.PrintAhead_PayPal:
                    btnSave.Visible = false;
                    break;
                default:
                    btnSave.Visible = true;
                    break;
            }
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

        private void btnUndo_MouseDown(object sender, MouseEventArgs e)
        {
            btnUndo.BackColor = SystemColors.ControlDarkDark;
            btnUndo.ForeColor = Color.White;
        }
        private void btnUndo_MouseUp(object sender, MouseEventArgs e)
        {
            btnUndo.BackColor = SystemColors.Control;
            btnUndo.ForeColor = Color.Black;

            OnUndo?.Invoke(this, EventArgs.Empty);
        }

        private void trackAge_MouseUp(object sender, MouseEventArgs e)
        {
            var delta = trackAge.Value == trackAge.Minimum ? 0 : trackAge.Value / (trackAge.Maximum * 1f);
            foreach (var m in ProgramCore.MainForm.ctrlRenderControl.OldMorphing)
                m.Value.Delta = delta;

            ProgramCore.Project.AgeCoefficient = delta;
            ProgramCore.MainForm.ctrlRenderControl.DoMorth();
        }
        private void trackWeight_MouseUp(object sender, MouseEventArgs e)
        {
            var delta = trackFat.Value == 0 ? 0 : trackFat.Value / (trackFat.Maximum * 1f);
            foreach (var m in ProgramCore.MainForm.ctrlRenderControl.FatMorphing)
                m.Value.Delta = delta;

            ProgramCore.Project.FatCoefficient = delta;
            ProgramCore.MainForm.ctrlRenderControl.DoMorth();
        }

        private void trackBarSmile_MouseUp(object sender, MouseEventArgs e)
        {
            var delta = trackBarSmile.Value == 0 ? 0 : trackBarSmile.Value / (trackBarSmile.Maximum * 1f);
            delta = ProgramCore.Project.IsOpenSmile ? 1 - delta : delta;
            if (ProgramCore.MainForm.ctrlRenderControl.SmileMorphing == null)
                return;

            foreach (var m in ProgramCore.MainForm.ctrlRenderControl.SmileMorphing)
                m.Value.Delta = delta;

            ProgramCore.Project.SmileCoefficient = delta;
            ProgramCore.MainForm.ctrlRenderControl.DoMorth();
        }

        #endregion

        public void SetAge(float delta)
        {
            trackAge.Value = (int)(trackAge.Maximum * delta);
        }
        public void Setfat(float delta)
        {
            trackFat.Value = (int)(trackFat.Maximum * delta);
        }
        public void SetSmile(bool isSmile)
        {
            trackBarSmile.Value = isSmile ? 100 : 0;
        }

    }
}
