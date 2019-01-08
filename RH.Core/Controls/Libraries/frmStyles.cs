﻿using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RH.Core.Helpers;
using RH.Core.IO;
using RH.Core.Render.Meshes;
using RH.Core.Render.Obj;
using RH.ImageListView;
using OpenTK;

namespace RH.Core.Controls.Libraries
{
    /// <summary> Style library form </summary>
    public partial class frmStyles : FormEx
    {
        /// <summary> Constructor </summary>
        public frmStyles()
        {
            InitializeComponent();
            InitializeListView();       // Initialize style list

            Sizeble = false;
            ProgramCore.MainForm.ctrlRenderControl.pickingController.OnSelectedMeshChanged += pickingController_OnSelectedMeshChanged;
        }

        void pickingController_OnSelectedMeshChanged()
        {
            BeginUpdate();
            try
            {
                if (ProgramCore.MainForm.ctrlRenderControl.pickingController.SelectedMeshes.Count == 0 || ProgramCore.MainForm.ctrlRenderControl.pickingController.SelectedMeshes.All(x => x.meshType != MeshType.Hair))
                {
                    trackBarSize.Value = 1;
                    imageListView.ClearSelection();
                }
                else
                {
                    trackBarSize.Value = (int)(ProgramCore.MainForm.ctrlRenderControl.pickingController.SelectedMeshes[0].MeshSize * 50);

                    tempTransform = ProgramCore.MainForm.ctrlRenderControl.pickingController.SelectedMeshes[0].Transform;
                }
            }
            finally
            {
                EndUpdate();
            }
        }

        #region Supported void's

        private void InitializeListView()
        {
            imageListView.AllowDuplicateFileNames = true;
            imageListView.SetRenderer(new ImageListViewRenderers.DefaultRenderer());

            imageListView.Columns.Add(ColumnType.Name);
            imageListView.Columns.Add(ColumnType.FileSize);
            imageListView.ThumbnailSize = new Size(64, 64);

            imageListView.Items.Clear();
            imageListView.SuspendLayout();
            try
            {
                var directoryPath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "Libraries", "Style");
                var di = new DirectoryInfo(directoryPath);
                if (!di.Exists)
                    return;

                foreach (var p in di.GetFiles("*.jpg"))
                {
                    if (UserConfig.ByName("Options")["Styles", p.FullName, "1"] == "1")
                        imageListView.Items.Add(p.FullName);
                }
            }
            finally
            {
                imageListView.ResumeLayout();
            }
        }

        #endregion

        #region Form's event

        private void frmStyles_Activated(object sender, EventArgs e)
        {
            if (Visible)
                ProgramCore.MainForm.ctrlRenderControl.StagesDeactivate(3);      // disable animations
        }
        private void frmStyles_FormClosing(object sender, FormClosingEventArgs e)
        {
            Hide();
            e.Cancel = true;            // this cancels the close event.
        }

        private void btnAddNewMaterial_Click(object sender, EventArgs e)
        {
            string hairPath;
            string sampleImagePath;
            using (var ofd = new OpenFileDialogEx("Select new style..", "OBJ files|*.obj"))
            {
                ofd.Multiselect = false;
                if (ofd.ShowDialog() != DialogResult.OK)
                    return;
                hairPath = ofd.FileName;
            }
            using (var ofd = new OpenFileDialogEx("Select style image..", "Image files|*.jpg"))
            {
                ofd.Multiselect = false;
                if (ofd.ShowDialog() != DialogResult.OK)
                    return;
                sampleImagePath = ofd.FileName;
            }

            var directoryPath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath),  "Libraries", "Style");
            var oldFileName = Path.GetFileNameWithoutExtension(hairPath);
            var newFileName = oldFileName;
            var filePath = Path.Combine(directoryPath, newFileName + ".obj");

            if (hairPath != filePath)
                File.Copy(hairPath, filePath, true);

            var mtl = oldFileName + ".mtl";
            var newMtlName = newFileName + ".mtl";
            if (mtl != newMtlName)
                ObjLoader.CopyMtl(mtl, newMtlName, Path.GetDirectoryName(hairPath), "", directoryPath, ProgramCore.Project.TextureSize);

            if (mtl != newMtlName)      // situation, when copy attribute and can change mtl filename. so, we need to rename link to this mtl in main obj file
            {
                string lines;
                using (var sd = new StreamReader(filePath, Encoding.Default))
                {
                    lines = sd.ReadToEnd();
                    lines = lines.Replace(mtl, newMtlName);
                }
                using (var sw = new StreamWriter(filePath, false, Encoding.Default))
                    sw.Write(lines);
            }

            var samplePath = Path.Combine(directoryPath, newFileName + ".jpg");
            if (sampleImagePath != samplePath)
                File.Copy(sampleImagePath, samplePath, true);
            UserConfig.ByName("Options")["Styles", samplePath] = "1";

            InitializeListView();
        }
        private void btnDelete_Click(object sender, EventArgs e)
        {
            foreach (var sel in imageListView.SelectedItems)
            {
                var path = Path.Combine(sel.FilePath, sel.FileName);
                UserConfig.ByName("Options")["Styles", path] = "0";     // not delete, just hide it.

            }
            InitializeListView();
        }

        private float meshScale;
        private void trackBarSize_Scroll(object sender, EventArgs e)
        {
            if (ProgramCore.CurrentProgram == ProgramCore.ProgramMode.HeadShop_v11 || ProgramCore.CurrentProgram == ProgramCore.ProgramMode.HeadShop_OneClick_v2 || ProgramCore.CurrentProgram == ProgramCore.ProgramMode.FaceAge2_Partial)
            {
                if (ProgramCore.MainForm.ctrlRenderControl.pickingController.SelectedMeshes.Count != 1 || ProgramCore.MainForm.ctrlRenderControl.pickingController.SelectedMeshes[0].meshType != MeshType.Hair)
                    return;

                var size = trackBarSize.Value / 50f;
                ProgramCore.MainForm.ctrlRenderControl.pickingController.SelectedMeshes[0].MeshSize = size;

                ProgramCore.MainForm.ctrlRenderControl.pickingController.SelectedMeshes[0].Transform = tempTransform;
                ProgramCore.MainForm.ctrlRenderControl.pickingController.SelectedMeshes[0].Transform[3, 0] -= ProgramCore.MainForm.ctrlRenderControl.pickingController.SelectedMeshes[0].Position.X;
                ProgramCore.MainForm.ctrlRenderControl.pickingController.SelectedMeshes[0].Transform[3, 1] -= ProgramCore.MainForm.ctrlRenderControl.pickingController.SelectedMeshes[0].Position.Y;
                ProgramCore.MainForm.ctrlRenderControl.pickingController.SelectedMeshes[0].Transform[3, 2] -= ProgramCore.MainForm.ctrlRenderControl.pickingController.SelectedMeshes[0].Position.Z;
                ProgramCore.MainForm.ctrlRenderControl.pickingController.SelectedMeshes[0].Transform *= Matrix4.CreateScale(size / meshScale);
                ProgramCore.MainForm.ctrlRenderControl.pickingController.SelectedMeshes[0].Transform[3, 0] += ProgramCore.MainForm.ctrlRenderControl.pickingController.SelectedMeshes[0].Position.X;
                ProgramCore.MainForm.ctrlRenderControl.pickingController.SelectedMeshes[0].Transform[3, 1] += ProgramCore.MainForm.ctrlRenderControl.pickingController.SelectedMeshes[0].Position.Y;
                ProgramCore.MainForm.ctrlRenderControl.pickingController.SelectedMeshes[0].Transform[3, 2] += ProgramCore.MainForm.ctrlRenderControl.pickingController.SelectedMeshes[0].Position.Z;

                ProgramCore.MainForm.ctrlRenderControl.pickingController.SelectedMeshes[0].IsChanged = true;
            }
            else
            {
                var k = (trackBarSize.Value - trackBarSize.Minimum) * 1f / (trackBarSize.Maximum - trackBarSize.Minimum);
                foreach (var mesh in ProgramCore.MainForm.ctrlRenderControl.pickingController.HairMeshes)
                    mesh.InterpolateMesh((trackBarSize.Value - trackBarSize.Minimum) * 1f / (trackBarSize.Maximum - trackBarSize.Minimum));
            }
        }

        private void btnClearProperties_Click(object sender, EventArgs e)
        {
            if (ProgramCore.MainForm.ctrlRenderControl.pickingController.HairMeshes.Count == 0)
                return;

            var mesh = ProgramCore.MainForm.ctrlRenderControl.pickingController.HairMeshes[0];
            if (string.IsNullOrEmpty(mesh.Path))
                return;

            UserConfig.ByName("Parts").Remove(mesh.Path);
        }
        private void btnSavePositionAndSize_Click(object sender, EventArgs e)
        {
            if (ProgramCore.MainForm.ctrlRenderControl.pickingController.HairMeshes.Count == 0)
                return;

            var mesh = ProgramCore.MainForm.ctrlRenderControl.pickingController.HairMeshes[0];
            if (string.IsNullOrEmpty(mesh.Path))
                return;

            UserConfig.ByName("Parts")[mesh.Path, "Size"] = ((trackBarSize.Value - trackBarSize.Minimum) * 1f / (trackBarSize.Maximum - trackBarSize.Minimum)).ToString();
            UserConfig.ByName("Parts")[mesh.Path, "Position"] = mesh.Position.X + "/" + mesh.Position.Y + "/" + mesh.Position.Z;
        }
        private void btnExport_Click(object sender, EventArgs e)
        {
            if (!UserConfig.ByName("Parts").HasAny())
                return;

            using (var sfd = new SaveFileDialogEx("Export styles settings", "Text file(*.txt)|*.txt"))
            {
                if (sfd.ShowDialog() != DialogResult.OK)
                    return;
                using (var writer = new StreamWriter(sfd.FileName, false, Encoding.Default))
                {
                    var directoryPath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath),  "Libraries", "Style");
                    var data = UserConfig.ByName("Parts").data;
                    var result = data.Select(x => x.s1).Distinct();
                    foreach (var d in result)
                    {
                        var dir = Path.GetDirectoryName(d);
                        if (dir != directoryPath)
                            continue;
                        writer.WriteLine(d);
                        writer.WriteLine(UserConfig.ByName("Parts")[d, "Size"]);
                        writer.WriteLine(UserConfig.ByName("Parts")[d, "Position"]);
                    }
                }
            }
            MessageBox.Show("Styles settings exported!", "Done");
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialogEx("Import styles settings", "Text file(*.txt)|*.txt"))
            {
                if (ofd.ShowDialog() != DialogResult.OK)
                    return;

                using (var reader = new StreamReader(ofd.FileName, Encoding.Default))
                {
                    while (!reader.EndOfStream)
                    {
                        var path = reader.ReadLine();
                        var size = reader.ReadLine();
                        var position = reader.ReadLine();

                        if (string.IsNullOrEmpty(path) || string.IsNullOrEmpty(size) || string.IsNullOrEmpty(position))
                            continue;

                        UserConfig.ByName("Parts")[path, "Size"] = size;
                        UserConfig.ByName("Parts")[path, "Position"] = position;
                    }
                }
            }
            MessageBox.Show("Styles settings imported!", "Done");
        }

        #endregion

        private Matrix4 tempTransform;          // matrix transformation, require for changing angle and size of selected accessory
        private void InitTempTransform()
        {
            if (ProgramCore.MainForm.ctrlRenderControl.pickingController.SelectedMeshes.Count != 1)
                return;

            tempTransform = ProgramCore.MainForm.ctrlRenderControl.pickingController.SelectedMeshes[0].Transform;           // for accessories always only ONE item can be selected
        }
        private void trackBarSize_MouseDown(object sender, MouseEventArgs e)
        {
            if (ProgramCore.MainForm.ctrlRenderControl.pickingController.SelectedMeshes.Count != 1 || ProgramCore.MainForm.ctrlRenderControl.pickingController.SelectedMeshes[0].meshType != MeshType.Hair)
                return;

            InitTempTransform();
            meshScale = ProgramCore.MainForm.ctrlRenderControl.pickingController.SelectedMeshes[0].MeshSize;
        }
    }
}

