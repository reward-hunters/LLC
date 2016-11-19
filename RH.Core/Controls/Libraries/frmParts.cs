using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using OpenTK;
using RH.Core.Helpers;
using RH.Core.IO;
using RH.Core.Render.Meshes;

namespace RH.Core.Controls.Libraries
{
    public partial class frmParts : FormEx
    {
        private Dictionary<DynamicRenderMesh, Tuple<Matrix4, TransformSize>> transforms = new Dictionary<DynamicRenderMesh, Tuple<Matrix4, TransformSize>>();   // matrix transformation, and meshSize require for changing angle and size of selected accessory

        public frmParts()
        {
            InitializeComponent();

            Sizeble = false;
            ProgramCore.MainForm.ctrlRenderControl.pickingController.OnSelectedMeshChanged += pickingController_OnSelectedMeshChanged;
        }

        void pickingController_OnSelectedMeshChanged()
        {
            BeginUpdate();
            try
            {
                transforms.Clear();
                if (ProgramCore.MainForm.ctrlRenderControl.pickingController.SelectedMeshes.Count == 0)
                {
                    trackBarSize.Value = 1;
                    trackBarXSize.Value = 1;
                    trackBarYSize.Value = 1;
                    trackBarZSize.Value = 1;
                }
                else
                {
                    var selectedMesh = ProgramCore.MainForm.ctrlRenderControl.pickingController.SelectedMeshes[0];
                    trackBarSize.Value = (int)(selectedMesh.MeshSize * 50);
                         trackBarXSize.Value = (int)(selectedMesh.MeshXSize * 50);
                        trackBarYSize.Value = (int)(selectedMesh.MeshYSize * 50);
                       trackBarZSize.Value = (int)(selectedMesh.MeshZSize * 50);

                    foreach (var mesh in ProgramCore.MainForm.ctrlRenderControl.pickingController.SelectedMeshes)
                    {
                        var sizes = new TransformSize(mesh.MeshSize, mesh.MeshXSize, mesh.MeshYSize, mesh.MeshZSize);
                        transforms.Add(mesh, new Tuple<Matrix4, TransformSize>(mesh.Transform, sizes));
                    }
                }
            }
            finally
            {
                EndUpdate();
            }
        }

        public void UpdateList()
        {
            BeginUpdate();
            try
            {
                tlParts.Nodes.Clear();
                foreach (var element in ProgramCore.MainForm.ctrlRenderControl.PartsLibraryMeshes)
                {
                    var node = new TreeNode(element.Key)
                    {
                        Checked = element.Value.Count == 0 ? false : element.Value[0].IsVisible,
                        Tag = element.Value
                    };
                    tlParts.Nodes.Add(node);
                }
                labelEmpty.Visible = tlParts.Nodes.Count == 0;
            }
            finally
            {
                EndUpdate();
            }
        }

        private void frmParts_Activated(object sender, EventArgs e)
        {
            ProgramCore.MainForm.ctrlRenderControl.StagesDeactivate(0);      // disable animation
        }
        private void frmParts_FormClosing(object sender, FormClosingEventArgs e)
        {
            Hide();
            e.Cancel = true;            // this cancels the close event.
        }

        private void tlParts_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (IsUpdating)
                return;

            var meshes = e.Node.Tag as DynamicRenderMeshes;
            if (meshes != null)
            {
                foreach (var mesh in meshes)
                {
                    mesh.IsVisible = e.Node.Checked;

                    if (!mesh.IsVisible && ProgramCore.MainForm.ctrlRenderControl.pickingController.SelectedMeshes.Contains(mesh))
                        ProgramCore.MainForm.ctrlRenderControl.pickingController.SelectedMeshes.Remove(mesh);
                }
            }
        }
        private void tlParts_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (IsUpdating)
                return;

            var meshes = e.Node.Tag as DynamicRenderMeshes;
            if (meshes != null && meshes.Count > 0 && meshes[0].IsVisible)
            {
                ProgramCore.MainForm.ctrlRenderControl.pickingController.SelectedMeshes.Clear();
                ProgramCore.MainForm.ctrlRenderControl.pickingController.SelectedMeshes.AddRange(meshes);
            }
        }

        private void InitTempTransform()
        {
            if (ProgramCore.MainForm.ctrlRenderControl.pickingController.SelectedMeshes.Count == 0)
                return;

            transforms.Clear();
            foreach (var mesh in ProgramCore.MainForm.ctrlRenderControl.pickingController.SelectedMeshes)
            {
                var sizes = new TransformSize(mesh.MeshSize, mesh.MeshXSize, mesh.MeshYSize, mesh.MeshZSize);
                transforms.Add(mesh, new Tuple<Matrix4, TransformSize>(mesh.Transform, sizes));
            }
        }
        private void trackBarSize_MouseDown(object sender, MouseEventArgs e)
        {
            if (ProgramCore.MainForm.ctrlRenderControl.pickingController.SelectedMeshes == null)
                return;

            InitTempTransform();
        }
        private void trackBarSize_Scroll(object sender, EventArgs e)
        {
            if (ProgramCore.MainForm.ctrlRenderControl.pickingController.SelectedMeshes == null)
                return;

            var size = trackBarSize.Value / 50f;
            foreach (var mesh in ProgramCore.MainForm.ctrlRenderControl.pickingController.SelectedMeshes)
            {
                mesh.MeshSize = size;

                mesh.Transform = transforms[mesh].Item1;
                mesh.Transform[3, 0] -= mesh.Position.X;
                mesh.Transform[3, 1] -= mesh.Position.Y;
                mesh.Transform[3, 2] -= mesh.Position.Z;
                mesh.Transform *= Matrix4.CreateScale(size / transforms[mesh].Item2.TotalSize);
                mesh.Transform[3, 0] += mesh.Position.X;
                mesh.Transform[3, 1] += mesh.Position.Y;
                mesh.Transform[3, 2] += mesh.Position.Z;

                mesh.IsChanged = true;
            }
        }

        private void btnLoadLibrary_Click(object sender, EventArgs e)
        {
            using (var ofd = new FolderDialogEx())
            {
                if (ofd.ShowDialog() != DialogResult.OK)
                    return;

                var dir = new DirectoryInfo(ofd.SelectedFolder[0]);
                foreach (var file in dir.GetFiles("*.obj", SearchOption.AllDirectories))
                {
                    var meshType = MeshType.Hair;
                    using (var sr = new StreamReader(file.FullName, Encoding.Default))
                    {
                        while (!sr.EndOfStream)
                        {
                            var currentLine = sr.ReadLine();
                            if (String.IsNullOrWhiteSpace(currentLine) || currentLine[0] == '#')
                            {
                                if (currentLine == "#Accessories")
                                {
                                    meshType = MeshType.Accessory;
                                    break;
                                }
                            }
                        }
                    }

                    var title = Path.GetFileNameWithoutExtension(file.Name);
                    var meshes = ProgramCore.MainForm.ctrlRenderControl.pickingController.AddMehes(file.FullName, meshType, false, ProgramCore.Project.ManType, false);
                    for (var i = 0; i < meshes.Count; i++)
                    {
                        var mesh = meshes[i];
                        if (mesh.vertexArray.Length == 0)
                            continue;
                        mesh.Title = title + "_" + i;
                        mesh.IsChanged = true;

                        if (!ProgramCore.MainForm.ctrlRenderControl.PartsLibraryMeshes.ContainsKey(title))
                            ProgramCore.MainForm.ctrlRenderControl.PartsLibraryMeshes.Add(title, new DynamicRenderMeshes());

                        ProgramCore.MainForm.ctrlRenderControl.PartsLibraryMeshes[title].Add(mesh);
                    }
                }

                UpdateList();
            }
        }

        private void trackBarXSize_Scroll(object sender, EventArgs e)
        {
            if (ProgramCore.MainForm.ctrlRenderControl.pickingController.SelectedMeshes == null)
                return;

            var size = trackBarXSize.Value / 50f;
            foreach (var mesh in ProgramCore.MainForm.ctrlRenderControl.pickingController.SelectedMeshes)
            {
                mesh.MeshXSize = size;

                mesh.Transform = transforms[mesh].Item1;
                mesh.Transform[3, 0] -= mesh.Position.X;
                mesh.Transform[3, 1] -= mesh.Position.Y;
                mesh.Transform[3, 2] -= mesh.Position.Z;
                mesh.Transform *= Matrix4.CreateScale(size / transforms[mesh].Item2.SizeX, 1, 1);
                mesh.Transform[3, 0] += mesh.Position.X;
                mesh.Transform[3, 1] += mesh.Position.Y;
                mesh.Transform[3, 2] += mesh.Position.Z;

                mesh.IsChanged = true;
            }
        }

        private void trackBarYSize_Scroll(object sender, EventArgs e)
        {
            if (ProgramCore.MainForm.ctrlRenderControl.pickingController.SelectedMeshes == null)
                return;

            var size = trackBarYSize.Value / 50f;
            foreach (var mesh in ProgramCore.MainForm.ctrlRenderControl.pickingController.SelectedMeshes)
            {
                mesh.MeshYSize = size;

                mesh.Transform = transforms[mesh].Item1;
                mesh.Transform[3, 0] -= mesh.Position.X;
                mesh.Transform[3, 1] -= mesh.Position.Y;
                mesh.Transform[3, 2] -= mesh.Position.Z;
                mesh.Transform *= Matrix4.CreateScale(1, size / transforms[mesh].Item2.SizeY, 1);
                mesh.Transform[3, 0] += mesh.Position.X;
                mesh.Transform[3, 1] += mesh.Position.Y;
                mesh.Transform[3, 2] += mesh.Position.Z;

                mesh.IsChanged = true;
            }
        }

        private void trackBarZSize_Scroll(object sender, EventArgs e)
        {
            if (ProgramCore.MainForm.ctrlRenderControl.pickingController.SelectedMeshes == null)
                return;

            var size = trackBarZSize.Value / 50f;
            foreach (var mesh in ProgramCore.MainForm.ctrlRenderControl.pickingController.SelectedMeshes)
            {
                mesh.MeshZSize = size;

                mesh.Transform = transforms[mesh].Item1;
                mesh.Transform[3, 0] -= mesh.Position.X;
                mesh.Transform[3, 1] -= mesh.Position.Y;
                mesh.Transform[3, 2] -= mesh.Position.Z;
                mesh.Transform *= Matrix4.CreateScale(1, 1, size / transforms[mesh].Item2.SizeZ);
                mesh.Transform[3, 0] += mesh.Position.X;
                mesh.Transform[3, 1] += mesh.Position.Y;
                mesh.Transform[3, 2] += mesh.Position.Z;

                mesh.IsChanged = true;
            }
        }

        private struct TransformSize
        {
            public float TotalSize;
            public float SizeX;
            public float SizeY;
            public float SizeZ;

            public TransformSize(float size, float sizex, float sizey, float sizez)
            {
                TotalSize = size;
                SizeX = sizex;
                SizeY = sizey;
                SizeZ = sizez;
            }
        }

    }
}
