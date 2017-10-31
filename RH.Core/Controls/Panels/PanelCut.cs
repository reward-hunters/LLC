﻿using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using RH.Core.Controls.Tutorials.OneClick;
using RH.Core.Helpers;
using RH.Core.IO;
using RH.Core.Properties;
using RH.Core.Render;
using RH.Core.Render.Meshes;

namespace RH.Core.Controls.Panels
{
    public partial class PanelCut : UserControlEx
    {
        #region Var

        public EventHandler OnDelete;
        public EventHandler OnDuplicate;
        public EventHandler OnSave;
        public EventHandler OnUndo;

        #endregion

        public PanelCut()
        {
            InitializeComponent();
        }

        public void ResetModeTools()
        {
            if (btnMirror.Tag.ToString() == "1")
                btnMirror_Click(this, EventArgs.Empty);

            if (btnCut.Tag.ToString() == "1")
                btnCut_Click(this, EventArgs.Empty);

            if (btnLasso.Tag.ToString() == "1")
                btnLasso_Click(this, EventArgs.Empty);
        }

        #region Form's event

        public void btnCut_Click(object sender, EventArgs e)
        {
            if (btnCut.Tag.ToString() == "2")
            {
                if (ProgramCore.MainForm.ctrlRenderControl.pickingController.SelectedMeshes.Count == 0 || ProgramCore.MainForm.ctrlRenderControl.pickingController.SelectedMeshes.All(x => x.meshType != MeshType.Hair))
                {
                    MessageBox.Show("Please select hair mesh!", "Notification", MessageBoxButtons.OK);
                    return;
                }

                ProgramCore.MainForm.ResetModeTools();

                btnCut.Tag = "1";

                btnCut.BackColor = SystemColors.ControlDarkDark;
                btnCut.ForeColor = Color.White;

                ProgramCore.MainForm.ctrlRenderControl.Mode = Mode.HairCut;
            }
            else
            {
                btnCut.Tag = "2";

                btnCut.BackColor = SystemColors.Control;
                btnCut.ForeColor = Color.Black;

                if (ProgramCore.MainForm.ctrlRenderControl.Mode == Mode.HairCut)
                    ProgramCore.MainForm.ctrlRenderControl.EndSlicing();

                ProgramCore.MainForm.ctrlRenderControl.Mode = Mode.None;
            }
        }
        public void btnMirror_Click(object sender, EventArgs e)
        {
            if (btnMirror.Tag.ToString() == "2")
            {
                btnMirror.Tag = "1";

                btnMirror.BackColor = SystemColors.ControlDarkDark;
                btnMirror.ForeColor = Color.White;

                ProgramCore.MainForm.ctrlRenderControl.ToolMirrored = true;
            }
            else
            {
                btnMirror.Tag = "2";

                btnMirror.BackColor = SystemColors.Control;
                btnMirror.ForeColor = Color.Black;

                ProgramCore.MainForm.ctrlRenderControl.ToolMirrored = false;
            }
        }

        private void btnDuplicate_MouseDown(object sender, MouseEventArgs e)
        {
            btnDuplicate.BackColor = SystemColors.ControlDarkDark;
            btnDuplicate.ForeColor = Color.White;
        }
        private void btnDuplicate_MouseUp(object sender, MouseEventArgs e)
        {
            btnDuplicate.BackColor = SystemColors.Control;
            btnDuplicate.ForeColor = Color.Black;

            OnDuplicate?.Invoke(this, EventArgs.Empty);
        }

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

        private void btnLasso_Click(object sender, EventArgs e)
        {
            if (btnLasso.Tag.ToString() == "2")
            {
                btnLasso.Tag = "1";

                btnLasso.BackColor = SystemColors.ControlDarkDark;
                btnLasso.ForeColor = Color.White;

                ProgramCore.MainForm.ctrlRenderControl.pickingController.SelectedMeshes.Clear();
                ProgramCore.MainForm.ctrlRenderControl.Mode = Mode.LassoStart;
            }
            else
            {
                btnLasso.Tag = "2";

                btnLasso.BackColor = SystemColors.Control;
                btnLasso.ForeColor = Color.Black;

                ProgramCore.MainForm.ctrlRenderControl.SelectHairByLasso();
                ProgramCore.MainForm.ctrlRenderControl.Mode = Mode.None;
            }
        }

        public void btnLine_Click(object sender, EventArgs e)
        {
            if (btnLine.Tag.ToString() == "2")
            {
                btnLine.Tag = "1";
                btnPolyLine.Tag = btnArc.Tag = "2";

                btnLine.Image = Resources.btnLinePressed;
                btnPolyLine.Image = Resources.btnPolyLineNormal;
                btnArc.Image = Resources.btnArcNormal;

                ProgramCore.MainForm.ctrlRenderControl.ToolsMode = ToolsMode.HairLine;
            }
            ProgramCore.MainForm.ctrlRenderControl.sliceController.BeginSlice();            // if was selected - reset.
        }

        public readonly frmLineToolTutorial frmTutLineTool = new frmLineToolTutorial();
        public void btnPolyLine_Click(object sender, EventArgs e)
        {
            if (UserConfig.ByName("Options")["Tutorials", "LineTool", "1"] == "1")
                frmTutLineTool.ShowDialog(this);

            if (btnPolyLine.Tag.ToString() == "2")
            {
                btnPolyLine.Tag = "1";
                btnLine.Tag = btnArc.Tag = "2";

                btnPolyLine.Image = Resources.btnPolyLinePressed;
                btnLine.Image = Resources.btnLineNormal;
                btnArc.Image = Resources.btnArcNormal;

                ProgramCore.MainForm.ctrlRenderControl.ToolsMode = ToolsMode.HairPolyLine;
            }
            ProgramCore.MainForm.ctrlRenderControl.sliceController.BeginSlice();            //  if was selected - reset.
        }
        public void btnArc_Click(object sender, EventArgs e)
        {
            if (btnArc.Tag.ToString() == "2")
            {
                btnArc.Tag = "1";
                btnLine.Tag = btnPolyLine.Tag = "2";

                btnArc.Image = Resources.btnArcPressed;
                btnPolyLine.Image = Resources.btnPolyLineNormal;
                btnLine.Image = Resources.btnLineNormal;

                ProgramCore.MainForm.ctrlRenderControl.ToolsMode = ToolsMode.HairArc;
            }
            ProgramCore.MainForm.ctrlRenderControl.sliceController.BeginSlice(true);            //  if was selected - reset.
        }

        #endregion
    }
}
