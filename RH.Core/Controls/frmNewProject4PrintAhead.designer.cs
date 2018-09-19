using System.ComponentModel;
using System.Windows.Forms;

namespace RH.Core.Controls
{
    partial class frmNewProject4PrintAhead
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmNewProject4PrintAhead));
            this.label1 = new System.Windows.Forms.Label();
            this.btnApply = new System.Windows.Forms.Button();
            this.textTemplateImage = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.pictureTemplate = new System.Windows.Forms.PictureBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textNewProjectName = new System.Windows.Forms.TextBox();
            this.btnNewProjectFolder = new System.Windows.Forms.Button();
            this.textNewProjectFolder = new System.Windows.Forms.TextBox();
            this.lblNewProject = new System.Windows.Forms.Label();
            this.labelNotes = new System.Windows.Forms.Label();
            this.pictureExample = new System.Windows.Forms.PictureBox();
            this.labelNotes1 = new System.Windows.Forms.Label();
            this.labelPrintAhead11 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label11 = new System.Windows.Forms.Label();
            this.rbImportObj = new System.Windows.Forms.RadioButton();
            this.btnChild = new System.Windows.Forms.PictureBox();
            this.btnFemale = new System.Windows.Forms.PictureBox();
            this.btnMale = new System.Windows.Forms.PictureBox();
            this.btnInfo = new System.Windows.Forms.PictureBox();
            this.btnPlay = new System.Windows.Forms.PictureBox();
            this.btnQuestion = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.RenderTimer = new System.Windows.Forms.Timer(this.components);
            this.labelHelp = new System.Windows.Forms.Label();
            this.rbOpenProject = new System.Windows.Forms.RadioButton();
            this.rbNewProject = new System.Windows.Forms.RadioButton();
            this.groupBoxOpen = new System.Windows.Forms.GroupBox();
            this.btnOpenProject = new System.Windows.Forms.Button();
            this.textOpenProject = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.rbGenesis2 = new System.Windows.Forms.RadioButton();
            this.rbGenesis3 = new System.Windows.Forms.RadioButton();
            this.rbGenesis8 = new System.Windows.Forms.RadioButton();
            ((System.ComponentModel.ISupportInitialize)(this.pictureTemplate)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureExample)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnChild)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnFemale)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnMale)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnInfo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnPlay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnQuestion)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            this.groupBoxOpen.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(6, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(424, 17);
            this.label1.TabIndex = 1;
            this.label1.Text = "Select template jpg image(image you want to use as a template).";
            // 
            // btnApply
            // 
            this.btnApply.Enabled = false;
            this.btnApply.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnApply.Location = new System.Drawing.Point(661, 58);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(104, 35);
            this.btnApply.TabIndex = 8;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // textTemplateImage
            // 
            this.textTemplateImage.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.textTemplateImage.Location = new System.Drawing.Point(109, 45);
            this.textTemplateImage.Name = "textTemplateImage";
            this.textTemplateImage.ReadOnly = true;
            this.textTemplateImage.Size = new System.Drawing.Size(318, 24);
            this.textTemplateImage.TabIndex = 11;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label5.Location = new System.Drawing.Point(9, 49);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(69, 17);
            this.label5.TabIndex = 10;
            this.label5.Text = "Template";
            // 
            // pictureTemplate
            // 
            this.pictureTemplate.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureTemplate.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureTemplate.Location = new System.Drawing.Point(12, 12);
            this.pictureTemplate.Name = "pictureTemplate";
            this.pictureTemplate.Size = new System.Drawing.Size(327, 438);
            this.pictureTemplate.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureTemplate.TabIndex = 0;
            this.pictureTemplate.TabStop = false;
            this.pictureTemplate.Click += new System.EventHandler(this.pictureTemplate_Click);
            this.pictureTemplate.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureTemplate_Paint);
            this.pictureTemplate.DoubleClick += new System.EventHandler(this.pictureTemplate_DoubleClick);
            this.pictureTemplate.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureTemplate_MouseDown);
            this.pictureTemplate.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureTemplate_MouseMove);
            this.pictureTemplate.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureTemplate_MouseUp);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textNewProjectName);
            this.groupBox1.Controls.Add(this.btnNewProjectFolder);
            this.groupBox1.Controls.Add(this.textNewProjectFolder);
            this.groupBox1.Controls.Add(this.lblNewProject);
            this.groupBox1.Controls.Add(this.labelNotes);
            this.groupBox1.Controls.Add(this.pictureExample);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.textTemplateImage);
            this.groupBox1.Controls.Add(this.labelNotes1);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.groupBox1.Location = new System.Drawing.Point(350, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(449, 329);
            this.groupBox1.TabIndex = 13;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Create new project";
            // 
            // textNewProjectName
            // 
            this.textNewProjectName.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.textNewProjectName.Location = new System.Drawing.Point(117, 269);
            this.textNewProjectName.Name = "textNewProjectName";
            this.textNewProjectName.Size = new System.Drawing.Size(286, 24);
            this.textNewProjectName.TabIndex = 18;
            // 
            // btnNewProjectFolder
            // 
            this.btnNewProjectFolder.Location = new System.Drawing.Point(409, 299);
            this.btnNewProjectFolder.Name = "btnNewProjectFolder";
            this.btnNewProjectFolder.Size = new System.Drawing.Size(30, 23);
            this.btnNewProjectFolder.TabIndex = 16;
            this.btnNewProjectFolder.Text = "...";
            this.btnNewProjectFolder.UseVisualStyleBackColor = true;
            this.btnNewProjectFolder.Click += new System.EventHandler(this.btnNewProjectFolder_Click);
            // 
            // textNewProjectFolder
            // 
            this.textNewProjectFolder.Enabled = false;
            this.textNewProjectFolder.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.textNewProjectFolder.Location = new System.Drawing.Point(117, 299);
            this.textNewProjectFolder.Name = "textNewProjectFolder";
            this.textNewProjectFolder.ReadOnly = true;
            this.textNewProjectFolder.Size = new System.Drawing.Size(286, 24);
            this.textNewProjectFolder.TabIndex = 15;
            // 
            // lblNewProject
            // 
            this.lblNewProject.AutoSize = true;
            this.lblNewProject.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblNewProject.Location = new System.Drawing.Point(9, 303);
            this.lblNewProject.Name = "lblNewProject";
            this.lblNewProject.Size = new System.Drawing.Size(102, 17);
            this.lblNewProject.TabIndex = 14;
            this.lblNewProject.Text = "Project folder:";
            // 
            // labelNotes
            // 
            this.labelNotes.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelNotes.Location = new System.Drawing.Point(6, 220);
            this.labelNotes.Name = "labelNotes";
            this.labelNotes.Size = new System.Drawing.Size(433, 63);
            this.labelNotes.TabIndex = 14;
            this.labelNotes.Text = "Use of PrintAhead 2.0 is free. Build a statue using the Front, Style, Color and P" +
    "rint tabs. If you are satisfied with your statue, you may use the print buttons " +
    "to generate a printready file.";
            // 
            // pictureExample
            // 
            this.pictureExample.Image = global::RH.Core.Properties.Resources.lol;
            this.pictureExample.Location = new System.Drawing.Point(9, 75);
            this.pictureExample.Name = "pictureExample";
            this.pictureExample.Size = new System.Drawing.Size(421, 106);
            this.pictureExample.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureExample.TabIndex = 13;
            this.pictureExample.TabStop = false;
            // 
            // labelNotes1
            // 
            this.labelNotes1.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelNotes1.Location = new System.Drawing.Point(6, 283);
            this.labelNotes1.Name = "labelNotes1";
            this.labelNotes1.Size = new System.Drawing.Size(433, 43);
            this.labelNotes1.TabIndex = 15;
            this.labelNotes1.Text = "For B/W files (STL) there will be a $5 charge per save, for full color prints $8." +
    "";
            // 
            // labelPrintAhead11
            // 
            this.labelPrintAhead11.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelPrintAhead11.Location = new System.Drawing.Point(344, 54);
            this.labelPrintAhead11.Name = "labelPrintAhead11";
            this.labelPrintAhead11.Size = new System.Drawing.Size(433, 63);
            this.labelPrintAhead11.TabIndex = 19;
            this.labelPrintAhead11.Text = "Models and Textures will be saved at Users/Public/My DAZ Library/Runtime/Faceshop" +
    "/fs";
            this.labelPrintAhead11.Visible = false;
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::RH.Core.Properties.Resources.bgWizard2;
            this.panel1.Controls.Add(this.rbGenesis8);
            this.panel1.Controls.Add(this.rbGenesis3);
            this.panel1.Controls.Add(this.rbGenesis2);
            this.panel1.Controls.Add(this.label11);
            this.panel1.Controls.Add(this.rbImportObj);
            this.panel1.Controls.Add(this.btnChild);
            this.panel1.Controls.Add(this.btnFemale);
            this.panel1.Controls.Add(this.btnMale);
            this.panel1.Controls.Add(this.btnApply);
            this.panel1.Controls.Add(this.btnInfo);
            this.panel1.Controls.Add(this.btnPlay);
            this.panel1.Controls.Add(this.btnQuestion);
            this.panel1.Controls.Add(this.pictureBox2);
            this.panel1.Controls.Add(this.pictureBox3);
            this.panel1.Controls.Add(this.label8);
            this.panel1.Controls.Add(this.label9);
            this.panel1.Controls.Add(this.label10);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 461);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(811, 100);
            this.panel1.TabIndex = 17;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.BackColor = System.Drawing.Color.Transparent;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label11.Location = new System.Drawing.Point(506, 12);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(95, 18);
            this.label11.TabIndex = 19;
            this.label11.Text = "Custom obj";
            this.label11.Visible = false;
            // 
            // rbImportObj
            // 
            this.rbImportObj.AutoSize = true;
            this.rbImportObj.BackColor = System.Drawing.Color.Transparent;
            this.rbImportObj.Location = new System.Drawing.Point(541, 56);
            this.rbImportObj.Name = "rbImportObj";
            this.rbImportObj.Size = new System.Drawing.Size(14, 13);
            this.rbImportObj.TabIndex = 18;
            this.rbImportObj.UseVisualStyleBackColor = false;
            this.rbImportObj.Visible = false;
            this.rbImportObj.CheckedChanged += new System.EventHandler(this.rbImportObj_CheckedChanged);
            // 
            // btnChild
            // 
            this.btnChild.Image = global::RH.Core.Properties.Resources.btnChildGray;
            this.btnChild.Location = new System.Drawing.Point(417, 29);
            this.btnChild.Name = "btnChild";
            this.btnChild.Size = new System.Drawing.Size(59, 59);
            this.btnChild.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.btnChild.TabIndex = 17;
            this.btnChild.TabStop = false;
            this.btnChild.Tag = "2";
            this.btnChild.Click += new System.EventHandler(this.btnChild_Click);
            // 
            // btnFemale
            // 
            this.btnFemale.Image = global::RH.Core.Properties.Resources.btnFemaleGray;
            this.btnFemale.Location = new System.Drawing.Point(312, 29);
            this.btnFemale.Name = "btnFemale";
            this.btnFemale.Size = new System.Drawing.Size(59, 59);
            this.btnFemale.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.btnFemale.TabIndex = 16;
            this.btnFemale.TabStop = false;
            this.btnFemale.Tag = "2";
            this.btnFemale.Click += new System.EventHandler(this.btnFemale_Click);
            // 
            // btnMale
            // 
            this.btnMale.Image = global::RH.Core.Properties.Resources.btnMaleGray;
            this.btnMale.Location = new System.Drawing.Point(211, 29);
            this.btnMale.Name = "btnMale";
            this.btnMale.Size = new System.Drawing.Size(59, 59);
            this.btnMale.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.btnMale.TabIndex = 15;
            this.btnMale.TabStop = false;
            this.btnMale.Tag = "2";
            this.btnMale.Click += new System.EventHandler(this.btnMale_Click);
            // 
            // btnInfo
            // 
            this.btnInfo.Image = global::RH.Core.Properties.Resources.btnInfoNormal;
            this.btnInfo.Location = new System.Drawing.Point(731, 11);
            this.btnInfo.Name = "btnInfo";
            this.btnInfo.Size = new System.Drawing.Size(34, 34);
            this.btnInfo.TabIndex = 14;
            this.btnInfo.TabStop = false;
            this.btnInfo.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnInfo_MouseDown);
            this.btnInfo.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnInfo_MouseUp);
            // 
            // btnPlay
            // 
            this.btnPlay.Image = global::RH.Core.Properties.Resources.btnPlayNormal;
            this.btnPlay.Location = new System.Drawing.Point(682, 11);
            this.btnPlay.Name = "btnPlay";
            this.btnPlay.Size = new System.Drawing.Size(34, 34);
            this.btnPlay.TabIndex = 13;
            this.btnPlay.TabStop = false;
            this.btnPlay.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnPlay_MouseDown);
            this.btnPlay.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnPlay_MouseUp);
            // 
            // btnQuestion
            // 
            this.btnQuestion.Image = global::RH.Core.Properties.Resources.btnQuestionNormal;
            this.btnQuestion.Location = new System.Drawing.Point(633, 11);
            this.btnQuestion.Name = "btnQuestion";
            this.btnQuestion.Size = new System.Drawing.Size(34, 34);
            this.btnQuestion.TabIndex = 12;
            this.btnQuestion.TabStop = false;
            this.btnQuestion.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnQuestion_MouseDown);
            this.btnQuestion.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnQuestion_MouseUp);
            // 
            // pictureBox2
            // 
            this.pictureBox2.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox2.Image = global::RH.Core.Properties.Resources.splitter;
            this.pictureBox2.Location = new System.Drawing.Point(607, -4);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(10, 97);
            this.pictureBox2.TabIndex = 10;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox3
            // 
            this.pictureBox3.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox3.Image = global::RH.Core.Properties.Resources.splitter;
            this.pictureBox3.Location = new System.Drawing.Point(184, -4);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(10, 97);
            this.pictureBox3.TabIndex = 9;
            this.pictureBox3.TabStop = false;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.BackColor = System.Drawing.Color.Transparent;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label8.Location = new System.Drawing.Point(423, 7);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(46, 18);
            this.label8.TabIndex = 7;
            this.label8.Text = "Child";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.BackColor = System.Drawing.Color.Transparent;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label9.Location = new System.Drawing.Point(308, 7);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(63, 18);
            this.label9.TabIndex = 6;
            this.label9.Text = "Female";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.BackColor = System.Drawing.Color.Transparent;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label10.Location = new System.Drawing.Point(218, 7);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(44, 18);
            this.label10.TabIndex = 5;
            this.label10.Text = "Male";
            // 
            // RenderTimer
            // 
            this.RenderTimer.Interval = 40;
            this.RenderTimer.Tick += new System.EventHandler(this.RenderTimer_Tick);
            // 
            // labelHelp
            // 
            this.labelHelp.Font = new System.Drawing.Font("Times New Roman", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelHelp.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.labelHelp.Location = new System.Drawing.Point(62, 168);
            this.labelHelp.Name = "labelHelp";
            this.labelHelp.Size = new System.Drawing.Size(226, 118);
            this.labelHelp.TabIndex = 18;
            this.labelHelp.Text = "Click Here to Load Frontal photo";
            this.labelHelp.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelHelp.Click += new System.EventHandler(this.pictureTemplate_Click);
            // 
            // rbOpenProject
            // 
            this.rbOpenProject.AutoSize = true;
            this.rbOpenProject.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Bold);
            this.rbOpenProject.Location = new System.Drawing.Point(555, 347);
            this.rbOpenProject.Name = "rbOpenProject";
            this.rbOpenProject.Size = new System.Drawing.Size(112, 21);
            this.rbOpenProject.TabIndex = 20;
            this.rbOpenProject.Text = "Open project";
            this.rbOpenProject.UseVisualStyleBackColor = true;
            this.rbOpenProject.Visible = false;
            this.rbOpenProject.CheckedChanged += new System.EventHandler(this.rbOpenProject_CheckedChanged);
            // 
            // rbNewProject
            // 
            this.rbNewProject.AutoSize = true;
            this.rbNewProject.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Bold);
            this.rbNewProject.Location = new System.Drawing.Point(359, 347);
            this.rbNewProject.Name = "rbNewProject";
            this.rbNewProject.Size = new System.Drawing.Size(106, 21);
            this.rbNewProject.TabIndex = 19;
            this.rbNewProject.Text = "New Project";
            this.rbNewProject.UseVisualStyleBackColor = true;
            this.rbNewProject.Visible = false;
            // 
            // groupBoxOpen
            // 
            this.groupBoxOpen.Controls.Add(this.btnOpenProject);
            this.groupBoxOpen.Controls.Add(this.textOpenProject);
            this.groupBoxOpen.Controls.Add(this.label2);
            this.groupBoxOpen.Enabled = false;
            this.groupBoxOpen.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold);
            this.groupBoxOpen.Location = new System.Drawing.Point(350, 381);
            this.groupBoxOpen.Name = "groupBoxOpen";
            this.groupBoxOpen.Size = new System.Drawing.Size(449, 69);
            this.groupBoxOpen.TabIndex = 21;
            this.groupBoxOpen.TabStop = false;
            this.groupBoxOpen.Text = "Load project";
            this.groupBoxOpen.Visible = false;
            // 
            // btnOpenProject
            // 
            this.btnOpenProject.Location = new System.Drawing.Point(409, 28);
            this.btnOpenProject.Name = "btnOpenProject";
            this.btnOpenProject.Size = new System.Drawing.Size(30, 23);
            this.btnOpenProject.TabIndex = 13;
            this.btnOpenProject.Text = "...";
            this.btnOpenProject.UseVisualStyleBackColor = true;
            this.btnOpenProject.Click += new System.EventHandler(this.btnOpenProject_Click);
            // 
            // textOpenProject
            // 
            this.textOpenProject.Enabled = false;
            this.textOpenProject.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.textOpenProject.Location = new System.Drawing.Point(117, 28);
            this.textOpenProject.Name = "textOpenProject";
            this.textOpenProject.ReadOnly = true;
            this.textOpenProject.Size = new System.Drawing.Size(286, 24);
            this.textOpenProject.TabIndex = 12;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(9, 32);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(84, 17);
            this.label2.TabIndex = 11;
            this.label2.Text = "Project file:";
            // 
            // rbGenesis2
            // 
            this.rbGenesis2.AutoSize = true;
            this.rbGenesis2.BackColor = System.Drawing.Color.Transparent;
            this.rbGenesis2.Checked = true;
            this.rbGenesis2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.rbGenesis2.Location = new System.Drawing.Point(22, 12);
            this.rbGenesis2.Name = "rbGenesis2";
            this.rbGenesis2.Size = new System.Drawing.Size(99, 21);
            this.rbGenesis2.TabIndex = 20;
            this.rbGenesis2.TabStop = true;
            this.rbGenesis2.Text = "Genesis 2";
            this.rbGenesis2.UseVisualStyleBackColor = false;
            this.rbGenesis2.Visible = false;
            this.rbGenesis2.CheckedChanged += new System.EventHandler(this.rbGenesis2_CheckedChanged);
            // 
            // rbGenesis3
            // 
            this.rbGenesis3.AutoSize = true;
            this.rbGenesis3.BackColor = System.Drawing.Color.Transparent;
            this.rbGenesis3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.rbGenesis3.Location = new System.Drawing.Point(22, 39);
            this.rbGenesis3.Name = "rbGenesis3";
            this.rbGenesis3.Size = new System.Drawing.Size(99, 21);
            this.rbGenesis3.TabIndex = 21;
            this.rbGenesis3.Text = "Genesis 3";
            this.rbGenesis3.UseVisualStyleBackColor = false;
            this.rbGenesis3.Visible = false;
            this.rbGenesis3.CheckedChanged += new System.EventHandler(this.rbGenesis3_CheckedChanged);
            // 
            // rbGenesis8
            // 
            this.rbGenesis8.AutoSize = true;
            this.rbGenesis8.BackColor = System.Drawing.Color.Transparent;
            this.rbGenesis8.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.rbGenesis8.Location = new System.Drawing.Point(22, 67);
            this.rbGenesis8.Name = "rbGenesis8";
            this.rbGenesis8.Size = new System.Drawing.Size(99, 21);
            this.rbGenesis8.TabIndex = 22;
            this.rbGenesis8.Text = "Genesis 8";
            this.rbGenesis8.UseVisualStyleBackColor = false;
            this.rbGenesis8.Visible = false;
            this.rbGenesis8.CheckedChanged += new System.EventHandler(this.rbGenesis8_CheckedChanged);
            // 
            // frmNewProject4PrintAhead
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(811, 561);
            this.Controls.Add(this.groupBoxOpen);
            this.Controls.Add(this.rbOpenProject);
            this.Controls.Add(this.rbNewProject);
            this.Controls.Add(this.labelHelp);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.pictureTemplate);
            this.Controls.Add(this.labelPrintAhead11);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(783, 440);
            this.Name = "frmNewProject4PrintAhead";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Tag = "2";
            this.Text = "Create new project";
            this.Resize += new System.EventHandler(this.frmNewProject4PrintAhead_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.pictureTemplate)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureExample)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnChild)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnFemale)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnMale)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnInfo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnPlay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnQuestion)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            this.groupBoxOpen.ResumeLayout(false);
            this.groupBoxOpen.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }        

        #endregion

        private PictureBox pictureTemplate;
        private Label label1;
        private Button btnApply;
        private TextBox textTemplateImage;
        private Label label5;
        private GroupBox groupBox1;
        private PictureBox pictureExample;
        private Panel panel1;
        private PictureBox btnChild;
        private PictureBox btnFemale;
        private PictureBox btnMale;
        private PictureBox btnInfo;
        private PictureBox btnPlay;
        private PictureBox btnQuestion;
        private PictureBox pictureBox2;
        private PictureBox pictureBox3;
        private Label label8;
        private Label label9;
        private Label label10;
        public Timer RenderTimer;
        private RadioButton rbImportObj;
        private Label label11;
        private Label labelHelp;
        private Label labelNotes1;
        private Label labelNotes;
        private RadioButton rbOpenProject;
        private RadioButton rbNewProject;
        private GroupBox groupBoxOpen;
        private Button btnOpenProject;
        private TextBox textOpenProject;
        private Label label2;
        private Button btnNewProjectFolder;
        private TextBox textNewProjectFolder;
        private Label lblNewProject;
        private TextBox textNewProjectName;
        private Label labelPrintAhead11;
        private RadioButton rbGenesis8;
        private RadioButton rbGenesis3;
        private RadioButton rbGenesis2;
    }
}