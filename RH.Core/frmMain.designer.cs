﻿using System.ComponentModel;
using System.Windows.Forms;
using RH.Core.Render;

namespace RH.Core
{
    partial class frmMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.panelRender = new System.Windows.Forms.Panel();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.panelMenuItems = new System.Windows.Forms.Panel();
            this.panelMenuControl = new System.Windows.Forms.Panel();
            this.panelNavigation = new System.Windows.Forms.Panel();
            this.btnUnscale = new System.Windows.Forms.PictureBox();
            this.checkZoom = new System.Windows.Forms.PictureBox();
            this.checkArrow = new System.Windows.Forms.PictureBox();
            this.checkHand = new System.Windows.Forms.PictureBox();
            this.panelMenu = new System.Windows.Forms.Panel();
            this.panelMenuStage = new System.Windows.Forms.PictureBox();
            this.panelMenuMaterials = new System.Windows.Forms.PictureBox();
            this.panelMenuAccessories = new System.Windows.Forms.PictureBox();
            this.panelMenuShape = new System.Windows.Forms.PictureBox();
            this.panelMenuCut = new System.Windows.Forms.PictureBox();
            this.panelMenuStyle = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panelMenuFeatures = new System.Windows.Forms.PictureBox();
            this.panelMenuFront = new System.Windows.Forms.PictureBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.undoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.partsLibraryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.styleLibraryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.accessoryLibraryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.materialLibraryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stageLibraryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.frontTabToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.autodotsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.saveToolStripMenuItem5 = new System.Windows.Forms.ToolStripMenuItem();
            this.undoToolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripSeparator();
            this.ponitsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.flipToLeftToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.flipToRightToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.profileTabToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem6 = new System.Windows.Forms.ToolStripMenuItem();
            this.undoToolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem8 = new System.Windows.Forms.ToolStripSeparator();
            this.linesToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.handToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.featuresTabToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem8 = new System.Windows.Forms.ToolStripMenuItem();
            this.undoToolStripMenuItem5 = new System.Windows.Forms.ToolStripMenuItem();
            this.styleTabToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem7 = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem5 = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.accessoryTabToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.accessoryLibraryToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.materialtabToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.materialLibraryToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.stageLibraryToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.stageLibraryToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.photoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.navigateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panTopcontinuousPanToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panBottomcontinuousPanToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stepToponeStepToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stepBottomoneStepToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rotateRightcontinuousRotateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rotateLeftcontinuousRotateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.turnRightoneStepToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.turnLeftoneStepToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.zoomInToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.zoomOutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fitWindowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ortoRightToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ortoLeftToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ortoTopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ortoBackToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startHelpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.autodotsHelpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mirrorHelpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.freehandHelpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.profileHelpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.accessoriesHelpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.styleHelpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.colorHelpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.accessoriesHelpToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.childHelpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.materialHelpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.retouchHelpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stageHelpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripSeparator();
            this.videoTutorialPart1CutAndShapeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.videoTutorialPart2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
            this.showManualToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutHeadShopProToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.advancedModellingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fullTutorialToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.frontToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ctrlTemplateImage = new RH.Core.Render.ctrlTemplateImage();
            this.ctrlRenderControl = new RH.Core.Render.ctrlRenderControl();
            this.panelRender.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.panelMenuItems.SuspendLayout();
            this.panelNavigation.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnUnscale)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkZoom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkArrow)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkHand)).BeginInit();
            this.panelMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelMenuStage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelMenuMaterials)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelMenuAccessories)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelMenuShape)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelMenuCut)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelMenuStyle)).BeginInit();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelMenuFeatures)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelMenuFront)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelRender
            // 
            this.panelRender.Controls.Add(this.ctrlRenderControl);
            this.panelRender.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelRender.Location = new System.Drawing.Point(0, 0);
            this.panelRender.Name = "panelRender";
            this.panelRender.Size = new System.Drawing.Size(668, 563);
            this.panelRender.TabIndex = 4;
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.IsSplitterFixed = true;
            this.splitContainer.Location = new System.Drawing.Point(0, 115);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.ctrlTemplateImage);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.panelRender);
            this.splitContainer.Size = new System.Drawing.Size(1337, 563);
            this.splitContainer.SplitterDistance = 668;
            this.splitContainer.SplitterWidth = 1;
            this.splitContainer.TabIndex = 5;
            // 
            // panelMenuItems
            // 
            this.panelMenuItems.BackgroundImage = global::RH.Core.Properties.Resources.menuBackground;
            this.panelMenuItems.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panelMenuItems.Controls.Add(this.panelMenuControl);
            this.panelMenuItems.Controls.Add(this.panelNavigation);
            this.panelMenuItems.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelMenuItems.Location = new System.Drawing.Point(0, 66);
            this.panelMenuItems.Name = "panelMenuItems";
            this.panelMenuItems.Size = new System.Drawing.Size(1337, 49);
            this.panelMenuItems.TabIndex = 2;
            // 
            // panelMenuControl
            // 
            this.panelMenuControl.BackColor = System.Drawing.SystemColors.Control;
            this.panelMenuControl.BackgroundImage = global::RH.Core.Properties.Resources.menuBackground;
            this.panelMenuControl.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panelMenuControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMenuControl.Location = new System.Drawing.Point(0, 0);
            this.panelMenuControl.Name = "panelMenuControl";
            this.panelMenuControl.Size = new System.Drawing.Size(1161, 49);
            this.panelMenuControl.TabIndex = 1;
            // 
            // panelNavigation
            // 
            this.panelNavigation.BackColor = System.Drawing.SystemColors.Control;
            this.panelNavigation.BackgroundImage = global::RH.Core.Properties.Resources.menuBackground;
            this.panelNavigation.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panelNavigation.Controls.Add(this.btnUnscale);
            this.panelNavigation.Controls.Add(this.checkZoom);
            this.panelNavigation.Controls.Add(this.checkArrow);
            this.panelNavigation.Controls.Add(this.checkHand);
            this.panelNavigation.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelNavigation.Location = new System.Drawing.Point(1161, 0);
            this.panelNavigation.Name = "panelNavigation";
            this.panelNavigation.Size = new System.Drawing.Size(176, 49);
            this.panelNavigation.TabIndex = 0;
            // 
            // btnUnscale
            // 
            this.btnUnscale.BackColor = System.Drawing.SystemColors.Control;
            this.btnUnscale.BackgroundImage = global::RH.Core.Properties.Resources.menuBackground;
            this.btnUnscale.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnUnscale.Image = global::RH.Core.Properties.Resources.btnUnscaleNormal;
            this.btnUnscale.Location = new System.Drawing.Point(132, 6);
            this.btnUnscale.Name = "btnUnscale";
            this.btnUnscale.Size = new System.Drawing.Size(36, 36);
            this.btnUnscale.TabIndex = 3;
            this.btnUnscale.TabStop = false;
            this.btnUnscale.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnUnscale_MouseDown);
            this.btnUnscale.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnUnscale_MouseUp);
            // 
            // checkZoom
            // 
            this.checkZoom.BackColor = System.Drawing.SystemColors.Control;
            this.checkZoom.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("checkZoom.BackgroundImage")));
            this.checkZoom.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.checkZoom.Image = global::RH.Core.Properties.Resources.btnZoomNormal;
            this.checkZoom.Location = new System.Drawing.Point(90, 6);
            this.checkZoom.Name = "checkZoom";
            this.checkZoom.Size = new System.Drawing.Size(36, 36);
            this.checkZoom.TabIndex = 2;
            this.checkZoom.TabStop = false;
            this.checkZoom.Tag = "2";
            this.checkZoom.Click += new System.EventHandler(this.checkZoom_Click);
            // 
            // checkArrow
            // 
            this.checkArrow.BackColor = System.Drawing.SystemColors.Control;
            this.checkArrow.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("checkArrow.BackgroundImage")));
            this.checkArrow.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.checkArrow.Image = global::RH.Core.Properties.Resources.btnArrowNormal;
            this.checkArrow.Location = new System.Drawing.Point(48, 6);
            this.checkArrow.Name = "checkArrow";
            this.checkArrow.Size = new System.Drawing.Size(36, 36);
            this.checkArrow.TabIndex = 1;
            this.checkArrow.TabStop = false;
            this.checkArrow.Tag = "2";
            this.checkArrow.Click += new System.EventHandler(this.checkArrow_Click);
            // 
            // checkHand
            // 
            this.checkHand.BackColor = System.Drawing.SystemColors.Control;
            this.checkHand.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("checkHand.BackgroundImage")));
            this.checkHand.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.checkHand.Image = global::RH.Core.Properties.Resources.btnHandNormal;
            this.checkHand.Location = new System.Drawing.Point(6, 6);
            this.checkHand.Name = "checkHand";
            this.checkHand.Size = new System.Drawing.Size(36, 36);
            this.checkHand.TabIndex = 0;
            this.checkHand.TabStop = false;
            this.checkHand.Tag = "2";
            this.checkHand.Click += new System.EventHandler(this.checkHand_Click);
            // 
            // panelMenu
            // 
            this.panelMenu.BackgroundImage = global::RH.Core.Properties.Resources.bgHighMenu;
            this.panelMenu.Controls.Add(this.panelMenuStage);
            this.panelMenu.Controls.Add(this.panelMenuMaterials);
            this.panelMenu.Controls.Add(this.panelMenuAccessories);
            this.panelMenu.Controls.Add(this.panelMenuShape);
            this.panelMenu.Controls.Add(this.panelMenuCut);
            this.panelMenu.Controls.Add(this.panelMenuStyle);
            this.panelMenu.Controls.Add(this.panel1);
            this.panelMenu.Controls.Add(this.panel3);
            this.panelMenu.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelMenu.Location = new System.Drawing.Point(0, 24);
            this.panelMenu.Name = "panelMenu";
            this.panelMenu.Size = new System.Drawing.Size(1337, 42);
            this.panelMenu.TabIndex = 1;
            // 
            // panelMenuStage
            // 
            this.panelMenuStage.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelMenuStage.Image = global::RH.Core.Properties.Resources.btnMenuStageNormal;
            this.panelMenuStage.Location = new System.Drawing.Point(736, 0);
            this.panelMenuStage.Name = "panelMenuStage";
            this.panelMenuStage.Size = new System.Drawing.Size(143, 42);
            this.panelMenuStage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.panelMenuStage.TabIndex = 9;
            this.panelMenuStage.TabStop = false;
            this.panelMenuStage.Tag = "2";
            this.panelMenuStage.Click += new System.EventHandler(this.panelMenuStage_Click);
            // 
            // panelMenuMaterials
            // 
            this.panelMenuMaterials.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelMenuMaterials.Image = global::RH.Core.Properties.Resources.btnMenuColorNormal;
            this.panelMenuMaterials.Location = new System.Drawing.Point(593, 0);
            this.panelMenuMaterials.Name = "panelMenuMaterials";
            this.panelMenuMaterials.Size = new System.Drawing.Size(143, 42);
            this.panelMenuMaterials.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.panelMenuMaterials.TabIndex = 8;
            this.panelMenuMaterials.TabStop = false;
            this.panelMenuMaterials.Tag = "2";
            this.panelMenuMaterials.Click += new System.EventHandler(this.panelMenuMaterials_Click);
            // 
            // panelMenuAccessories
            // 
            this.panelMenuAccessories.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelMenuAccessories.Image = global::RH.Core.Properties.Resources.btnMenuAccessoriesNormal;
            this.panelMenuAccessories.Location = new System.Drawing.Point(450, 0);
            this.panelMenuAccessories.Name = "panelMenuAccessories";
            this.panelMenuAccessories.Size = new System.Drawing.Size(143, 42);
            this.panelMenuAccessories.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.panelMenuAccessories.TabIndex = 7;
            this.panelMenuAccessories.TabStop = false;
            this.panelMenuAccessories.Tag = "2";
            this.panelMenuAccessories.Click += new System.EventHandler(this.panelMenuAccessories_Click);
            // 
            // panelMenuShape
            // 
            this.panelMenuShape.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelMenuShape.Enabled = false;
            this.panelMenuShape.Image = global::RH.Core.Properties.Resources.btnMenuShapeNormal;
            this.panelMenuShape.Location = new System.Drawing.Point(450, 0);
            this.panelMenuShape.Name = "panelMenuShape";
            this.panelMenuShape.Size = new System.Drawing.Size(0, 42);
            this.panelMenuShape.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.panelMenuShape.TabIndex = 6;
            this.panelMenuShape.TabStop = false;
            this.panelMenuShape.Tag = "2";
            this.panelMenuShape.Click += new System.EventHandler(this.panelMenuShape_Click);
            // 
            // panelMenuCut
            // 
            this.panelMenuCut.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelMenuCut.Enabled = false;
            this.panelMenuCut.Image = global::RH.Core.Properties.Resources.btnMenuCutNormal;
            this.panelMenuCut.Location = new System.Drawing.Point(450, 0);
            this.panelMenuCut.Name = "panelMenuCut";
            this.panelMenuCut.Size = new System.Drawing.Size(0, 42);
            this.panelMenuCut.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.panelMenuCut.TabIndex = 5;
            this.panelMenuCut.TabStop = false;
            this.panelMenuCut.Tag = "2";
            this.panelMenuCut.Click += new System.EventHandler(this.panelMenuCut_Click);
            // 
            // panelMenuStyle
            // 
            this.panelMenuStyle.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelMenuStyle.Image = global::RH.Core.Properties.Resources.btnMenuStylePressed;
            this.panelMenuStyle.Location = new System.Drawing.Point(307, 0);
            this.panelMenuStyle.Name = "panelMenuStyle";
            this.panelMenuStyle.Size = new System.Drawing.Size(143, 42);
            this.panelMenuStyle.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.panelMenuStyle.TabIndex = 10;
            this.panelMenuStyle.TabStop = false;
            this.panelMenuStyle.Tag = "2";
            this.panelMenuStyle.Click += new System.EventHandler(this.panelMenuStyle_Click);
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::RH.Core.Properties.Resources.bgHighMenu;
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(288, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(19, 42);
            this.panel1.TabIndex = 4;
            // 
            // panel3
            // 
            this.panel3.BackgroundImage = global::RH.Core.Properties.Resources.bgHighMenuHead;
            this.panel3.Controls.Add(this.panelMenuFeatures);
            this.panel3.Controls.Add(this.panelMenuFront);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(288, 42);
            this.panel3.TabIndex = 14;
            // 
            // panelMenuFeatures
            // 
            this.panelMenuFeatures.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelMenuFeatures.Image = global::RH.Core.Properties.Resources.btnMenuFeaturesNormal;
            this.panelMenuFeatures.Location = new System.Drawing.Point(143, 0);
            this.panelMenuFeatures.Name = "panelMenuFeatures";
            this.panelMenuFeatures.Size = new System.Drawing.Size(143, 42);
            this.panelMenuFeatures.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.panelMenuFeatures.TabIndex = 16;
            this.panelMenuFeatures.TabStop = false;
            this.panelMenuFeatures.Tag = "2";
            this.panelMenuFeatures.Click += new System.EventHandler(this.panelMenuFeatures_Click);
            // 
            // panelMenuFront
            // 
            this.panelMenuFront.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelMenuFront.Image = global::RH.Core.Properties.Resources.btnMenuFrontNormal;
            this.panelMenuFront.Location = new System.Drawing.Point(0, 0);
            this.panelMenuFront.Name = "panelMenuFront";
            this.panelMenuFront.Size = new System.Drawing.Size(143, 42);
            this.panelMenuFront.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.panelMenuFront.TabIndex = 14;
            this.panelMenuFront.TabStop = false;
            this.panelMenuFront.Tag = "2";
            this.panelMenuFront.Click += new System.EventHandler(this.panelMenuFront_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.SystemColors.Control;
            this.menuStrip1.BackgroundImage = global::RH.Core.Properties.Resources.BgGradient;
            this.menuStrip1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.frontTabToolStripMenuItem,
            this.profileTabToolStripMenuItem,
            this.featuresTabToolStripMenuItem,
            this.styleTabToolStripMenuItem,
            this.accessoryTabToolStripMenuItem,
            this.materialtabToolStripMenuItem,
            this.stageLibraryToolStripMenuItem1,
            this.navigateToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1337, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.toolStripMenuItem1,
            this.exportToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.newToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.newToolStripMenuItem.Text = "New";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.saveAsToolStripMenuItem.Text = "Save as...";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(143, 6);
            // 
            // exportToolStripMenuItem
            // 
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            this.exportToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.exportToolStripMenuItem.Text = "Export";
            this.exportToolStripMenuItem.Click += new System.EventHandler(this.exportToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.undoToolStripMenuItem,
            this.toolStripMenuItem2,
            this.partsLibraryToolStripMenuItem,
            this.styleLibraryToolStripMenuItem,
            this.accessoryLibraryToolStripMenuItem,
            this.materialLibraryToolStripMenuItem,
            this.stageLibraryToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "Edit";
            // 
            // undoToolStripMenuItem
            // 
            this.undoToolStripMenuItem.Name = "undoToolStripMenuItem";
            this.undoToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
            this.undoToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.undoToolStripMenuItem.Text = "Undo";
            this.undoToolStripMenuItem.Click += new System.EventHandler(this.undoToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(198, 6);
            // 
            // partsLibraryToolStripMenuItem
            // 
            this.partsLibraryToolStripMenuItem.Name = "partsLibraryToolStripMenuItem";
            this.partsLibraryToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.P)));
            this.partsLibraryToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.partsLibraryToolStripMenuItem.Text = "PartsLibrary";
            this.partsLibraryToolStripMenuItem.Click += new System.EventHandler(this.partsLibraryOnOpen_Click);
            // 
            // styleLibraryToolStripMenuItem
            // 
            this.styleLibraryToolStripMenuItem.Name = "styleLibraryToolStripMenuItem";
            this.styleLibraryToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.T)));
            this.styleLibraryToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.styleLibraryToolStripMenuItem.Text = "StyleLibrary";
            this.styleLibraryToolStripMenuItem.Click += new System.EventHandler(this.styleLibraryToolStripMenuItem_Click);
            // 
            // accessoryLibraryToolStripMenuItem
            // 
            this.accessoryLibraryToolStripMenuItem.Name = "accessoryLibraryToolStripMenuItem";
            this.accessoryLibraryToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.A)));
            this.accessoryLibraryToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.accessoryLibraryToolStripMenuItem.Text = "AccessoryLibrary";
            this.accessoryLibraryToolStripMenuItem.Click += new System.EventHandler(this.accessoryLibraryOnOpen_Click);
            // 
            // materialLibraryToolStripMenuItem
            // 
            this.materialLibraryToolStripMenuItem.Name = "materialLibraryToolStripMenuItem";
            this.materialLibraryToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.M)));
            this.materialLibraryToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.materialLibraryToolStripMenuItem.Text = "MaterialLibrary";
            this.materialLibraryToolStripMenuItem.Click += new System.EventHandler(this.materialLibraryOnOpen_Click);
            // 
            // stageLibraryToolStripMenuItem
            // 
            this.stageLibraryToolStripMenuItem.Name = "stageLibraryToolStripMenuItem";
            this.stageLibraryToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.S)));
            this.stageLibraryToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.stageLibraryToolStripMenuItem.Text = "StageLibrary";
            this.stageLibraryToolStripMenuItem.Click += new System.EventHandler(this.stagesLibraryOnOpen_Click);
            // 
            // frontTabToolStripMenuItem
            // 
            this.frontTabToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.autodotsToolStripMenuItem,
            this.toolStripMenuItem3,
            this.saveToolStripMenuItem5,
            this.undoToolStripMenuItem3,
            this.toolStripMenuItem6,
            this.ponitsToolStripMenuItem,
            this.flipToLeftToolStripMenuItem,
            this.flipToRightToolStripMenuItem});
            this.frontTabToolStripMenuItem.Name = "frontTabToolStripMenuItem";
            this.frontTabToolStripMenuItem.Size = new System.Drawing.Size(71, 20);
            this.frontTabToolStripMenuItem.Text = "Front_Tab";
            // 
            // autodotsToolStripMenuItem
            // 
            this.autodotsToolStripMenuItem.Name = "autodotsToolStripMenuItem";
            this.autodotsToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.autodotsToolStripMenuItem.Text = "Autodots";
            this.autodotsToolStripMenuItem.Click += new System.EventHandler(this.autodotsToolStripMenuItem_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(146, 6);
            // 
            // saveToolStripMenuItem5
            // 
            this.saveToolStripMenuItem5.Name = "saveToolStripMenuItem5";
            this.saveToolStripMenuItem5.Size = new System.Drawing.Size(149, 22);
            this.saveToolStripMenuItem5.Text = "Save";
            this.saveToolStripMenuItem5.Click += new System.EventHandler(this.saveToolStripMenuItem5_Click);
            // 
            // undoToolStripMenuItem3
            // 
            this.undoToolStripMenuItem3.Name = "undoToolStripMenuItem3";
            this.undoToolStripMenuItem3.Size = new System.Drawing.Size(149, 22);
            this.undoToolStripMenuItem3.Text = "Undo";
            this.undoToolStripMenuItem3.Click += new System.EventHandler(this.OnUndo_Click);
            // 
            // toolStripMenuItem6
            // 
            this.toolStripMenuItem6.Name = "toolStripMenuItem6";
            this.toolStripMenuItem6.Size = new System.Drawing.Size(146, 6);
            // 
            // ponitsToolStripMenuItem
            // 
            this.ponitsToolStripMenuItem.Name = "ponitsToolStripMenuItem";
            this.ponitsToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.ponitsToolStripMenuItem.Text = "Lasso";
            this.ponitsToolStripMenuItem.Click += new System.EventHandler(this.ponitsToolStripMenuItem_Click);
            // 
            // flipToLeftToolStripMenuItem
            // 
            this.flipToLeftToolStripMenuItem.Name = "flipToLeftToolStripMenuItem";
            this.flipToLeftToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.flipToLeftToolStripMenuItem.Text = "Mirror to left";
            this.flipToLeftToolStripMenuItem.Click += new System.EventHandler(this.flipToLeftToolStripMenuItem_Click);
            // 
            // flipToRightToolStripMenuItem
            // 
            this.flipToRightToolStripMenuItem.Name = "flipToRightToolStripMenuItem";
            this.flipToRightToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.flipToRightToolStripMenuItem.Text = "Mirror to right";
            this.flipToRightToolStripMenuItem.Click += new System.EventHandler(this.flipToRightToolStripMenuItem_Click);
            // 
            // profileTabToolStripMenuItem
            // 
            this.profileTabToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveToolStripMenuItem6,
            this.undoToolStripMenuItem4,
            this.toolStripMenuItem8,
            this.linesToolStripMenuItem1,
            this.handToolStripMenuItem1});
            this.profileTabToolStripMenuItem.Name = "profileTabToolStripMenuItem";
            this.profileTabToolStripMenuItem.Size = new System.Drawing.Size(77, 20);
            this.profileTabToolStripMenuItem.Text = "Profile_Tab";
            // 
            // saveToolStripMenuItem6
            // 
            this.saveToolStripMenuItem6.Name = "saveToolStripMenuItem6";
            this.saveToolStripMenuItem6.Size = new System.Drawing.Size(123, 22);
            this.saveToolStripMenuItem6.Text = "Save";
            this.saveToolStripMenuItem6.Click += new System.EventHandler(this.saveToolStripMenuItem6_Click);
            // 
            // undoToolStripMenuItem4
            // 
            this.undoToolStripMenuItem4.Name = "undoToolStripMenuItem4";
            this.undoToolStripMenuItem4.Size = new System.Drawing.Size(123, 22);
            this.undoToolStripMenuItem4.Text = "Undo";
            this.undoToolStripMenuItem4.Click += new System.EventHandler(this.OnUndo_Click);
            // 
            // toolStripMenuItem8
            // 
            this.toolStripMenuItem8.Name = "toolStripMenuItem8";
            this.toolStripMenuItem8.Size = new System.Drawing.Size(120, 6);
            // 
            // linesToolStripMenuItem1
            // 
            this.linesToolStripMenuItem1.Name = "linesToolStripMenuItem1";
            this.linesToolStripMenuItem1.Size = new System.Drawing.Size(123, 22);
            this.linesToolStripMenuItem1.Text = "Line";
            this.linesToolStripMenuItem1.Click += new System.EventHandler(this.linesToolStripMenuItem1_Click);
            // 
            // handToolStripMenuItem1
            // 
            this.handToolStripMenuItem1.Name = "handToolStripMenuItem1";
            this.handToolStripMenuItem1.Size = new System.Drawing.Size(123, 22);
            this.handToolStripMenuItem1.Text = "Freehand";
            this.handToolStripMenuItem1.Click += new System.EventHandler(this.handToolStripMenuItem1_Click);
            // 
            // featuresTabToolStripMenuItem
            // 
            this.featuresTabToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveToolStripMenuItem8,
            this.undoToolStripMenuItem5});
            this.featuresTabToolStripMenuItem.Name = "featuresTabToolStripMenuItem";
            this.featuresTabToolStripMenuItem.Size = new System.Drawing.Size(87, 20);
            this.featuresTabToolStripMenuItem.Text = "Features_Tab";
            // 
            // saveToolStripMenuItem8
            // 
            this.saveToolStripMenuItem8.Name = "saveToolStripMenuItem8";
            this.saveToolStripMenuItem8.Size = new System.Drawing.Size(103, 22);
            this.saveToolStripMenuItem8.Text = "Save";
            this.saveToolStripMenuItem8.Click += new System.EventHandler(this.saveToolStripMenuItem8_Click);
            // 
            // undoToolStripMenuItem5
            // 
            this.undoToolStripMenuItem5.Name = "undoToolStripMenuItem5";
            this.undoToolStripMenuItem5.Size = new System.Drawing.Size(103, 22);
            this.undoToolStripMenuItem5.Text = "Undo";
            this.undoToolStripMenuItem5.Click += new System.EventHandler(this.undoToolStripMenuItem5_Click);
            // 
            // styleTabToolStripMenuItem
            // 
            this.styleTabToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem1,
            this.saveToolStripMenuItem7,
            this.deleteToolStripMenuItem5,
            this.exportToolStripMenuItem3});
            this.styleTabToolStripMenuItem.Name = "styleTabToolStripMenuItem";
            this.styleTabToolStripMenuItem.Size = new System.Drawing.Size(68, 20);
            this.styleTabToolStripMenuItem.Text = "Style_Tab";
            // 
            // openToolStripMenuItem1
            // 
            this.openToolStripMenuItem1.Name = "openToolStripMenuItem1";
            this.openToolStripMenuItem1.Size = new System.Drawing.Size(107, 22);
            this.openToolStripMenuItem1.Text = "Open";
            this.openToolStripMenuItem1.Click += new System.EventHandler(this.openToolStripMenuItem1_Click);
            // 
            // saveToolStripMenuItem7
            // 
            this.saveToolStripMenuItem7.Name = "saveToolStripMenuItem7";
            this.saveToolStripMenuItem7.Size = new System.Drawing.Size(107, 22);
            this.saveToolStripMenuItem7.Text = "Save";
            this.saveToolStripMenuItem7.Click += new System.EventHandler(this.saveToolStripMenuItem7_Click);
            // 
            // deleteToolStripMenuItem5
            // 
            this.deleteToolStripMenuItem5.Name = "deleteToolStripMenuItem5";
            this.deleteToolStripMenuItem5.Size = new System.Drawing.Size(107, 22);
            this.deleteToolStripMenuItem5.Text = "Delete";
            this.deleteToolStripMenuItem5.Click += new System.EventHandler(this.deleteToolStripMenuItem5_Click);
            // 
            // exportToolStripMenuItem3
            // 
            this.exportToolStripMenuItem3.Name = "exportToolStripMenuItem3";
            this.exportToolStripMenuItem3.Size = new System.Drawing.Size(107, 22);
            this.exportToolStripMenuItem3.Text = "Export";
            this.exportToolStripMenuItem3.Click += new System.EventHandler(this.exportToolStripMenuItem3_Click);
            // 
            // accessoryTabToolStripMenuItem
            // 
            this.accessoryTabToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.accessoryLibraryToolStripMenuItem1,
            this.saveToolStripMenuItem3,
            this.deleteToolStripMenuItem1,
            this.exportToolStripMenuItem2});
            this.accessoryTabToolStripMenuItem.Name = "accessoryTabToolStripMenuItem";
            this.accessoryTabToolStripMenuItem.Size = new System.Drawing.Size(96, 20);
            this.accessoryTabToolStripMenuItem.Text = "Accessory_Tab";
            // 
            // accessoryLibraryToolStripMenuItem1
            // 
            this.accessoryLibraryToolStripMenuItem1.Name = "accessoryLibraryToolStripMenuItem1";
            this.accessoryLibraryToolStripMenuItem1.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
            this.accessoryLibraryToolStripMenuItem1.Size = new System.Drawing.Size(205, 22);
            this.accessoryLibraryToolStripMenuItem1.Text = "AccessoryLibrary";
            this.accessoryLibraryToolStripMenuItem1.Click += new System.EventHandler(this.accessoryLibraryOnOpen_Click);
            // 
            // saveToolStripMenuItem3
            // 
            this.saveToolStripMenuItem3.Name = "saveToolStripMenuItem3";
            this.saveToolStripMenuItem3.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveToolStripMenuItem3.Size = new System.Drawing.Size(205, 22);
            this.saveToolStripMenuItem3.Text = "Save";
            this.saveToolStripMenuItem3.Click += new System.EventHandler(this.OnSavePart_Click);
            // 
            // deleteToolStripMenuItem1
            // 
            this.deleteToolStripMenuItem1.Name = "deleteToolStripMenuItem1";
            this.deleteToolStripMenuItem1.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.deleteToolStripMenuItem1.Size = new System.Drawing.Size(205, 22);
            this.deleteToolStripMenuItem1.Text = "Delete";
            this.deleteToolStripMenuItem1.Click += new System.EventHandler(this.accessoryLibraryOnDelete_Click);
            // 
            // exportToolStripMenuItem2
            // 
            this.exportToolStripMenuItem2.Name = "exportToolStripMenuItem2";
            this.exportToolStripMenuItem2.Size = new System.Drawing.Size(205, 22);
            this.exportToolStripMenuItem2.Text = "Export";
            this.exportToolStripMenuItem2.Click += new System.EventHandler(this.exportToolStripMenuItem2_Click);
            // 
            // materialtabToolStripMenuItem
            // 
            this.materialtabToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.materialLibraryToolStripMenuItem1,
            this.saveToolStripMenuItem4,
            this.deleteToolStripMenuItem2,
            this.exportToolStripMenuItem1});
            this.materialtabToolStripMenuItem.Name = "materialtabToolStripMenuItem";
            this.materialtabToolStripMenuItem.Size = new System.Drawing.Size(91, 20);
            this.materialtabToolStripMenuItem.Text = "Materials_Tab";
            // 
            // materialLibraryToolStripMenuItem1
            // 
            this.materialLibraryToolStripMenuItem1.Name = "materialLibraryToolStripMenuItem1";
            this.materialLibraryToolStripMenuItem1.Size = new System.Drawing.Size(153, 22);
            this.materialLibraryToolStripMenuItem1.Text = "MaterialLibrary";
            this.materialLibraryToolStripMenuItem1.Click += new System.EventHandler(this.materialLibraryOnOpen_Click);
            // 
            // saveToolStripMenuItem4
            // 
            this.saveToolStripMenuItem4.Name = "saveToolStripMenuItem4";
            this.saveToolStripMenuItem4.Size = new System.Drawing.Size(153, 22);
            this.saveToolStripMenuItem4.Text = "Save";
            this.saveToolStripMenuItem4.Click += new System.EventHandler(this.OnSavePart_Click);
            // 
            // deleteToolStripMenuItem2
            // 
            this.deleteToolStripMenuItem2.Name = "deleteToolStripMenuItem2";
            this.deleteToolStripMenuItem2.Size = new System.Drawing.Size(153, 22);
            this.deleteToolStripMenuItem2.Text = "Delete";
            this.deleteToolStripMenuItem2.Click += new System.EventHandler(this.materialLibraryOnDelete_Click);
            // 
            // exportToolStripMenuItem1
            // 
            this.exportToolStripMenuItem1.Name = "exportToolStripMenuItem1";
            this.exportToolStripMenuItem1.Size = new System.Drawing.Size(153, 22);
            this.exportToolStripMenuItem1.Text = "Export";
            this.exportToolStripMenuItem1.Click += new System.EventHandler(this.exportToolStripMenuItem1_Click);
            // 
            // stageLibraryToolStripMenuItem1
            // 
            this.stageLibraryToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.stageLibraryToolStripMenuItem2,
            this.photoToolStripMenuItem});
            this.stageLibraryToolStripMenuItem1.Name = "stageLibraryToolStripMenuItem1";
            this.stageLibraryToolStripMenuItem1.Size = new System.Drawing.Size(72, 20);
            this.stageLibraryToolStripMenuItem1.Text = "Stage_Tab";
            // 
            // stageLibraryToolStripMenuItem2
            // 
            this.stageLibraryToolStripMenuItem2.Name = "stageLibraryToolStripMenuItem2";
            this.stageLibraryToolStripMenuItem2.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.S)));
            this.stageLibraryToolStripMenuItem2.Size = new System.Drawing.Size(175, 22);
            this.stageLibraryToolStripMenuItem2.Text = "StageLibrary";
            this.stageLibraryToolStripMenuItem2.Click += new System.EventHandler(this.stagesLibraryOnOpen_Click);
            // 
            // photoToolStripMenuItem
            // 
            this.photoToolStripMenuItem.Name = "photoToolStripMenuItem";
            this.photoToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.photoToolStripMenuItem.Text = "Photo";
            this.photoToolStripMenuItem.Click += new System.EventHandler(this.photoToolStripMenuItem_Click);
            // 
            // navigateToolStripMenuItem
            // 
            this.navigateToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.panTopcontinuousPanToolStripMenuItem,
            this.panBottomcontinuousPanToolStripMenuItem,
            this.stepToponeStepToolStripMenuItem,
            this.stepBottomoneStepToolStripMenuItem,
            this.rotateRightcontinuousRotateToolStripMenuItem,
            this.rotateLeftcontinuousRotateToolStripMenuItem,
            this.turnRightoneStepToolStripMenuItem,
            this.turnLeftoneStepToolStripMenuItem,
            this.zoomInToolStripMenuItem,
            this.zoomOutToolStripMenuItem,
            this.stopToolStripMenuItem,
            this.fitWindowToolStripMenuItem,
            this.ortoRightToolStripMenuItem,
            this.ortoLeftToolStripMenuItem,
            this.ortoTopToolStripMenuItem,
            this.ortoBackToolStripMenuItem});
            this.navigateToolStripMenuItem.Name = "navigateToolStripMenuItem";
            this.navigateToolStripMenuItem.Size = new System.Drawing.Size(66, 20);
            this.navigateToolStripMenuItem.Text = "Navigate";
            // 
            // panTopcontinuousPanToolStripMenuItem
            // 
            this.panTopcontinuousPanToolStripMenuItem.Name = "panTopcontinuousPanToolStripMenuItem";
            this.panTopcontinuousPanToolStripMenuItem.Size = new System.Drawing.Size(238, 22);
            this.panTopcontinuousPanToolStripMenuItem.Text = "Pan top(continuous pan)";
            this.panTopcontinuousPanToolStripMenuItem.Click += new System.EventHandler(this.panTopcontinuousPanToolStripMenuItem_Click);
            // 
            // panBottomcontinuousPanToolStripMenuItem
            // 
            this.panBottomcontinuousPanToolStripMenuItem.Name = "panBottomcontinuousPanToolStripMenuItem";
            this.panBottomcontinuousPanToolStripMenuItem.Size = new System.Drawing.Size(238, 22);
            this.panBottomcontinuousPanToolStripMenuItem.Text = "Pan bottom(continuous pan)";
            this.panBottomcontinuousPanToolStripMenuItem.Click += new System.EventHandler(this.panBottomcontinuousPanToolStripMenuItem_Click);
            // 
            // stepToponeStepToolStripMenuItem
            // 
            this.stepToponeStepToolStripMenuItem.Name = "stepToponeStepToolStripMenuItem";
            this.stepToponeStepToolStripMenuItem.Size = new System.Drawing.Size(238, 22);
            this.stepToponeStepToolStripMenuItem.Text = "Step top(one step)";
            this.stepToponeStepToolStripMenuItem.Click += new System.EventHandler(this.stepToponeStepToolStripMenuItem_Click);
            // 
            // stepBottomoneStepToolStripMenuItem
            // 
            this.stepBottomoneStepToolStripMenuItem.Name = "stepBottomoneStepToolStripMenuItem";
            this.stepBottomoneStepToolStripMenuItem.Size = new System.Drawing.Size(238, 22);
            this.stepBottomoneStepToolStripMenuItem.Text = "Step bottom(one step)";
            this.stepBottomoneStepToolStripMenuItem.Click += new System.EventHandler(this.stepBottomoneStepToolStripMenuItem_Click);
            // 
            // rotateRightcontinuousRotateToolStripMenuItem
            // 
            this.rotateRightcontinuousRotateToolStripMenuItem.Name = "rotateRightcontinuousRotateToolStripMenuItem";
            this.rotateRightcontinuousRotateToolStripMenuItem.Size = new System.Drawing.Size(238, 22);
            this.rotateRightcontinuousRotateToolStripMenuItem.Text = "Rotate right(continuous rotate)";
            this.rotateRightcontinuousRotateToolStripMenuItem.Click += new System.EventHandler(this.rotateRightcontinuousRotateToolStripMenuItem_Click);
            // 
            // rotateLeftcontinuousRotateToolStripMenuItem
            // 
            this.rotateLeftcontinuousRotateToolStripMenuItem.Name = "rotateLeftcontinuousRotateToolStripMenuItem";
            this.rotateLeftcontinuousRotateToolStripMenuItem.Size = new System.Drawing.Size(238, 22);
            this.rotateLeftcontinuousRotateToolStripMenuItem.Text = "Rotate left(continuous rotate)";
            this.rotateLeftcontinuousRotateToolStripMenuItem.Click += new System.EventHandler(this.rotateLeftcontinuousRotateToolStripMenuItem_Click);
            // 
            // turnRightoneStepToolStripMenuItem
            // 
            this.turnRightoneStepToolStripMenuItem.Name = "turnRightoneStepToolStripMenuItem";
            this.turnRightoneStepToolStripMenuItem.Size = new System.Drawing.Size(238, 22);
            this.turnRightoneStepToolStripMenuItem.Text = "Turn right(one step)";
            this.turnRightoneStepToolStripMenuItem.Click += new System.EventHandler(this.turnRightoneStepToolStripMenuItem_Click);
            // 
            // turnLeftoneStepToolStripMenuItem
            // 
            this.turnLeftoneStepToolStripMenuItem.Name = "turnLeftoneStepToolStripMenuItem";
            this.turnLeftoneStepToolStripMenuItem.Size = new System.Drawing.Size(238, 22);
            this.turnLeftoneStepToolStripMenuItem.Text = "Turn left(one step)";
            this.turnLeftoneStepToolStripMenuItem.Click += new System.EventHandler(this.turnLeftoneStepToolStripMenuItem_Click);
            // 
            // zoomInToolStripMenuItem
            // 
            this.zoomInToolStripMenuItem.Name = "zoomInToolStripMenuItem";
            this.zoomInToolStripMenuItem.Size = new System.Drawing.Size(238, 22);
            this.zoomInToolStripMenuItem.Text = "Zoom in";
            this.zoomInToolStripMenuItem.Click += new System.EventHandler(this.zoomInToolStripMenuItem_Click);
            // 
            // zoomOutToolStripMenuItem
            // 
            this.zoomOutToolStripMenuItem.Name = "zoomOutToolStripMenuItem";
            this.zoomOutToolStripMenuItem.Size = new System.Drawing.Size(238, 22);
            this.zoomOutToolStripMenuItem.Text = "Zoom out";
            this.zoomOutToolStripMenuItem.Click += new System.EventHandler(this.zoomOutToolStripMenuItem_Click);
            // 
            // stopToolStripMenuItem
            // 
            this.stopToolStripMenuItem.Name = "stopToolStripMenuItem";
            this.stopToolStripMenuItem.Size = new System.Drawing.Size(238, 22);
            this.stopToolStripMenuItem.Text = "Stop";
            this.stopToolStripMenuItem.Click += new System.EventHandler(this.stopToolStripMenuItem_Click);
            // 
            // fitWindowToolStripMenuItem
            // 
            this.fitWindowToolStripMenuItem.Name = "fitWindowToolStripMenuItem";
            this.fitWindowToolStripMenuItem.Size = new System.Drawing.Size(238, 22);
            this.fitWindowToolStripMenuItem.Text = "Fit window";
            this.fitWindowToolStripMenuItem.Click += new System.EventHandler(this.fitWindowToolStripMenuItem_Click);
            // 
            // ortoRightToolStripMenuItem
            // 
            this.ortoRightToolStripMenuItem.Name = "ortoRightToolStripMenuItem";
            this.ortoRightToolStripMenuItem.Size = new System.Drawing.Size(238, 22);
            this.ortoRightToolStripMenuItem.Text = "Orto_Right";
            this.ortoRightToolStripMenuItem.Click += new System.EventHandler(this.ortoRightToolStripMenuItem_Click);
            // 
            // ortoLeftToolStripMenuItem
            // 
            this.ortoLeftToolStripMenuItem.Name = "ortoLeftToolStripMenuItem";
            this.ortoLeftToolStripMenuItem.Size = new System.Drawing.Size(238, 22);
            this.ortoLeftToolStripMenuItem.Text = "Orto_Left";
            this.ortoLeftToolStripMenuItem.Click += new System.EventHandler(this.ortoLeftToolStripMenuItem_Click);
            // 
            // ortoTopToolStripMenuItem
            // 
            this.ortoTopToolStripMenuItem.Name = "ortoTopToolStripMenuItem";
            this.ortoTopToolStripMenuItem.Size = new System.Drawing.Size(238, 22);
            this.ortoTopToolStripMenuItem.Text = "Orto_Top";
            this.ortoTopToolStripMenuItem.Click += new System.EventHandler(this.ortoTopToolStripMenuItem_Click);
            // 
            // ortoBackToolStripMenuItem
            // 
            this.ortoBackToolStripMenuItem.Name = "ortoBackToolStripMenuItem";
            this.ortoBackToolStripMenuItem.Size = new System.Drawing.Size(238, 22);
            this.ortoBackToolStripMenuItem.Text = "Orto_Back";
            this.ortoBackToolStripMenuItem.Click += new System.EventHandler(this.ortoBackToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fullTutorialToolStripMenuItem,
            this.toolStripMenuItem5,
            this.frontToolStripMenuItem,
            this.videoTutorialPart1CutAndShapeToolStripMenuItem,
            this.autodotsHelpToolStripMenuItem,
            this.accessoriesHelpToolStripMenuItem,
            this.styleHelpToolStripMenuItem,
            this.accessoriesHelpToolStripMenuItem1,
            this.colorHelpToolStripMenuItem,
            this.advancedModellingToolStripMenuItem,
            this.startHelpToolStripMenuItem,
            this.mirrorHelpToolStripMenuItem,
            this.freehandHelpToolStripMenuItem,
            this.profileHelpToolStripMenuItem,
            this.childHelpToolStripMenuItem,
            this.materialHelpToolStripMenuItem,
            this.retouchHelpToolStripMenuItem,
            this.stageHelpToolStripMenuItem,
            this.videoTutorialPart2ToolStripMenuItem,
            this.toolStripMenuItem4,
            this.showManualToolStripMenuItem,
            this.aboutHeadShopProToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // startHelpToolStripMenuItem
            // 
            this.startHelpToolStripMenuItem.Name = "startHelpToolStripMenuItem";
            this.startHelpToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
            this.startHelpToolStripMenuItem.Text = "Setup help";
            this.startHelpToolStripMenuItem.Click += new System.EventHandler(this.startHelpToolStripMenuItem_Click);
            // 
            // autodotsHelpToolStripMenuItem
            // 
            this.autodotsHelpToolStripMenuItem.Name = "autodotsHelpToolStripMenuItem";
            this.autodotsHelpToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
            this.autodotsHelpToolStripMenuItem.Text = "Autodots";
            this.autodotsHelpToolStripMenuItem.Click += new System.EventHandler(this.autodotsHelpToolStripMenuItem_Click);
            // 
            // mirrorHelpToolStripMenuItem
            // 
            this.mirrorHelpToolStripMenuItem.Name = "mirrorHelpToolStripMenuItem";
            this.mirrorHelpToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
            this.mirrorHelpToolStripMenuItem.Text = "LineTool Help";
            this.mirrorHelpToolStripMenuItem.Click += new System.EventHandler(this.mirrorHelpToolStripMenuItem_Click);
            // 
            // freehandHelpToolStripMenuItem
            // 
            this.freehandHelpToolStripMenuItem.Name = "freehandHelpToolStripMenuItem";
            this.freehandHelpToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
            this.freehandHelpToolStripMenuItem.Text = "Freehand Help";
            this.freehandHelpToolStripMenuItem.Click += new System.EventHandler(this.freehandHelpToolStripMenuItem_Click);
            // 
            // profileHelpToolStripMenuItem
            // 
            this.profileHelpToolStripMenuItem.Name = "profileHelpToolStripMenuItem";
            this.profileHelpToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
            this.profileHelpToolStripMenuItem.Text = "Profile Help";
            this.profileHelpToolStripMenuItem.Click += new System.EventHandler(this.profileHelpToolStripMenuItem_Click);
            // 
            // accessoriesHelpToolStripMenuItem
            // 
            this.accessoriesHelpToolStripMenuItem.Name = "accessoriesHelpToolStripMenuItem";
            this.accessoriesHelpToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
            this.accessoriesHelpToolStripMenuItem.Text = "Features";
            this.accessoriesHelpToolStripMenuItem.Click += new System.EventHandler(this.accessoriesHelpToolStripMenuItem_Click);
            // 
            // styleHelpToolStripMenuItem
            // 
            this.styleHelpToolStripMenuItem.Name = "styleHelpToolStripMenuItem";
            this.styleHelpToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
            this.styleHelpToolStripMenuItem.Text = "Style";
            this.styleHelpToolStripMenuItem.Click += new System.EventHandler(this.styleHelpToolStripMenuItem_Click);
            // 
            // colorHelpToolStripMenuItem
            // 
            this.colorHelpToolStripMenuItem.Name = "colorHelpToolStripMenuItem";
            this.colorHelpToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
            this.colorHelpToolStripMenuItem.Text = "Color";
            this.colorHelpToolStripMenuItem.Click += new System.EventHandler(this.colorHelpToolStripMenuItem_Click);
            // 
            // accessoriesHelpToolStripMenuItem1
            // 
            this.accessoriesHelpToolStripMenuItem1.Name = "accessoriesHelpToolStripMenuItem1";
            this.accessoriesHelpToolStripMenuItem1.Size = new System.Drawing.Size(219, 22);
            this.accessoriesHelpToolStripMenuItem1.Text = "Accessory";
            this.accessoriesHelpToolStripMenuItem1.Click += new System.EventHandler(this.accessoriesHelpToolStripMenuItem1_Click);
            // 
            // childHelpToolStripMenuItem
            // 
            this.childHelpToolStripMenuItem.Name = "childHelpToolStripMenuItem";
            this.childHelpToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
            this.childHelpToolStripMenuItem.Text = "Child Help";
            this.childHelpToolStripMenuItem.Click += new System.EventHandler(this.childHelpToolStripMenuItem_Click);
            // 
            // materialHelpToolStripMenuItem
            // 
            this.materialHelpToolStripMenuItem.Name = "materialHelpToolStripMenuItem";
            this.materialHelpToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
            this.materialHelpToolStripMenuItem.Text = "Export Help";
            this.materialHelpToolStripMenuItem.Click += new System.EventHandler(this.materialHelpToolStripMenuItem_Click);
            // 
            // retouchHelpToolStripMenuItem
            // 
            this.retouchHelpToolStripMenuItem.Name = "retouchHelpToolStripMenuItem";
            this.retouchHelpToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
            this.retouchHelpToolStripMenuItem.Text = "Retouch Help";
            this.retouchHelpToolStripMenuItem.Click += new System.EventHandler(this.retouchHelpToolStripMenuItem_Click);
            // 
            // stageHelpToolStripMenuItem
            // 
            this.stageHelpToolStripMenuItem.Name = "stageHelpToolStripMenuItem";
            this.stageHelpToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
            this.stageHelpToolStripMenuItem.Text = "Print Help";
            this.stageHelpToolStripMenuItem.Click += new System.EventHandler(this.stageHelpToolStripMenuItem_Click);
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(216, 6);
            // 
            // videoTutorialPart1CutAndShapeToolStripMenuItem
            // 
            this.videoTutorialPart1CutAndShapeToolStripMenuItem.Name = "videoTutorialPart1CutAndShapeToolStripMenuItem";
            this.videoTutorialPart1CutAndShapeToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
            this.videoTutorialPart1CutAndShapeToolStripMenuItem.Text = "QuickStart";
            this.videoTutorialPart1CutAndShapeToolStripMenuItem.Click += new System.EventHandler(this.videoTutorialPart1CutAndShapeToolStripMenuItem_Click);
            // 
            // videoTutorialPart2ToolStripMenuItem
            // 
            this.videoTutorialPart2ToolStripMenuItem.Name = "videoTutorialPart2ToolStripMenuItem";
            this.videoTutorialPart2ToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
            this.videoTutorialPart2ToolStripMenuItem.Text = "Video Tutorial (Full tutorial)";
            this.videoTutorialPart2ToolStripMenuItem.Click += new System.EventHandler(this.videoTutorialPart2ToolStripMenuItem_Click);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(216, 6);
            // 
            // showManualToolStripMenuItem
            // 
            this.showManualToolStripMenuItem.Name = "showManualToolStripMenuItem";
            this.showManualToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
            this.showManualToolStripMenuItem.Text = "Show Manual";
            this.showManualToolStripMenuItem.Click += new System.EventHandler(this.showManualToolStripMenuItem_Click);
            // 
            // aboutHeadShopProToolStripMenuItem
            // 
            this.aboutHeadShopProToolStripMenuItem.Name = "aboutHeadShopProToolStripMenuItem";
            this.aboutHeadShopProToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
            this.aboutHeadShopProToolStripMenuItem.Text = "About PrintAhead";
            this.aboutHeadShopProToolStripMenuItem.Click += new System.EventHandler(this.aboutHeadShopProToolStripMenuItem_Click);
            // 
            // advancedModellingToolStripMenuItem
            // 
            this.advancedModellingToolStripMenuItem.Name = "advancedModellingToolStripMenuItem";
            this.advancedModellingToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
            this.advancedModellingToolStripMenuItem.Text = "Advanced modelling";
            this.advancedModellingToolStripMenuItem.Click += new System.EventHandler(this.advancedModellingToolStripMenuItem_Click);
            // 
            // fullTutorialToolStripMenuItem
            // 
            this.fullTutorialToolStripMenuItem.Name = "fullTutorialToolStripMenuItem";
            this.fullTutorialToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
            this.fullTutorialToolStripMenuItem.Text = "Full tutorial";
            this.fullTutorialToolStripMenuItem.Click += new System.EventHandler(this.fullTutorialToolStripMenuItem_Click);
            // 
            // frontToolStripMenuItem
            // 
            this.frontToolStripMenuItem.Name = "frontToolStripMenuItem";
            this.frontToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
            this.frontToolStripMenuItem.Text = "Front";
            this.frontToolStripMenuItem.Click += new System.EventHandler(this.frontToolStripMenuItem_Click);
            // 
            // ctrlTemplateImage
            // 
            this.ctrlTemplateImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ctrlTemplateImage.Location = new System.Drawing.Point(0, 0);
            this.ctrlTemplateImage.Name = "ctrlTemplateImage";
            this.ctrlTemplateImage.Size = new System.Drawing.Size(668, 563);
            this.ctrlTemplateImage.TabIndex = 0;
            // 
            // ctrlRenderControl
            // 
            this.ctrlRenderControl.BackgroundTexture = null;
            this.ctrlRenderControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ctrlRenderControl.InStageMode = false;
            this.ctrlRenderControl.IsFullPointsInfo = true;
            this.ctrlRenderControl.Location = new System.Drawing.Point(0, 0);
            this.ctrlRenderControl.Mode = RH.Core.Render.Mode.None;
            this.ctrlRenderControl.Name = "ctrlRenderControl";
            this.ctrlRenderControl.PlayAnimation = true;
            this.ctrlRenderControl.Size = new System.Drawing.Size(668, 563);
            this.ctrlRenderControl.TabIndex = 0;
            this.ctrlRenderControl.ToolMirrored = false;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1337, 678);
            this.Controls.Add(this.splitContainer);
            this.Controls.Add(this.panelMenuItems);
            this.Controls.Add(this.panelMenu);
            this.Controls.Add(this.menuStrip1);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "PrintAhead";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmMain_KeyDown);
            this.panelRender.ResumeLayout(false);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.panelMenuItems.ResumeLayout(false);
            this.panelNavigation.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.btnUnscale)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkZoom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkArrow)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkHand)).EndInit();
            this.panelMenu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.panelMenuStage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelMenuMaterials)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelMenuAccessories)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelMenuShape)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelMenuCut)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelMenuStyle)).EndInit();
            this.panel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.panelMenuFeatures)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelMenuFront)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem newToolStripMenuItem;
        private ToolStripMenuItem openToolStripMenuItem;
        private ToolStripMenuItem saveToolStripMenuItem;
        private ToolStripMenuItem saveAsToolStripMenuItem;
        private ToolStripMenuItem editToolStripMenuItem;
        private ToolStripMenuItem undoToolStripMenuItem;
        private ToolStripSeparator toolStripMenuItem2;
        private ToolStripMenuItem partsLibraryToolStripMenuItem;
        private ToolStripMenuItem accessoryLibraryToolStripMenuItem;
        private ToolStripMenuItem materialLibraryToolStripMenuItem;
        private ToolStripMenuItem stageLibraryToolStripMenuItem;
        private ToolStripMenuItem accessoryTabToolStripMenuItem;
        private ToolStripMenuItem accessoryLibraryToolStripMenuItem1;
        private ToolStripMenuItem saveToolStripMenuItem3;
        private ToolStripMenuItem deleteToolStripMenuItem1;
        private ToolStripMenuItem materialtabToolStripMenuItem;
        private ToolStripMenuItem materialLibraryToolStripMenuItem1;
        private ToolStripMenuItem saveToolStripMenuItem4;
        private ToolStripMenuItem deleteToolStripMenuItem2;
        private ToolStripMenuItem stageLibraryToolStripMenuItem1;
        private ToolStripMenuItem stageLibraryToolStripMenuItem2;
        private ToolStripMenuItem photoToolStripMenuItem;
        private ToolStripMenuItem navigateToolStripMenuItem;
        private ToolStripMenuItem panTopcontinuousPanToolStripMenuItem;
        private ToolStripMenuItem panBottomcontinuousPanToolStripMenuItem;
        private ToolStripMenuItem stepToponeStepToolStripMenuItem;
        private ToolStripMenuItem stepBottomoneStepToolStripMenuItem;
        private ToolStripMenuItem rotateLeftcontinuousRotateToolStripMenuItem;
        private ToolStripMenuItem rotateRightcontinuousRotateToolStripMenuItem;
        private ToolStripMenuItem turnRightoneStepToolStripMenuItem;
        private ToolStripMenuItem turnLeftoneStepToolStripMenuItem;
        private ToolStripMenuItem zoomInToolStripMenuItem;
        private ToolStripMenuItem zoomOutToolStripMenuItem;
        private ToolStripMenuItem stopToolStripMenuItem;
        private ToolStripMenuItem fitWindowToolStripMenuItem;
        private ToolStripMenuItem ortoRightToolStripMenuItem;
        private ToolStripMenuItem ortoLeftToolStripMenuItem;
        private ToolStripMenuItem ortoTopToolStripMenuItem;
        private ToolStripMenuItem ortoBackToolStripMenuItem;
        private ToolStripMenuItem helpToolStripMenuItem;
        private ToolStripMenuItem showManualToolStripMenuItem;
        private ToolStripMenuItem aboutHeadShopProToolStripMenuItem;
        private Panel panelMenu;
        private Panel panelMenuItems;
        public Panel panelMenuControl;
        private Panel panelNavigation;
        private PictureBox btnUnscale;
        private Panel panelRender;
        private PictureBox panelMenuStage;
        private PictureBox panelMenuMaterials;
        private PictureBox panelMenuAccessories;
        private PictureBox panelMenuShape;
        private PictureBox panelMenuCut;
        public ctrlRenderControl ctrlRenderControl;
        internal PictureBox checkHand;
        internal PictureBox checkZoom;
        internal PictureBox checkArrow;
        private ToolStripSeparator toolStripMenuItem1;
        private ToolStripMenuItem videoTutorialPart1CutAndShapeToolStripMenuItem;
        private ToolStripMenuItem videoTutorialPart2ToolStripMenuItem;
        private ToolStripSeparator toolStripMenuItem4;
        private ToolStripMenuItem startHelpToolStripMenuItem;
        private ToolStripMenuItem accessoriesHelpToolStripMenuItem;
        private ToolStripMenuItem materialHelpToolStripMenuItem;
        private ToolStripMenuItem stageHelpToolStripMenuItem;
        private ToolStripSeparator toolStripMenuItem5;
        private PictureBox panelMenuStyle;
        private ToolStripMenuItem styleLibraryToolStripMenuItem;
        private ToolStripMenuItem styleHelpToolStripMenuItem;
        private Panel panel3;
        private PictureBox panelMenuFeatures;
        private PictureBox panelMenuFront;
        private Panel panel1;
        private ToolStripMenuItem frontTabToolStripMenuItem;
        private ToolStripMenuItem profileTabToolStripMenuItem;
        private ToolStripMenuItem featuresTabToolStripMenuItem;
        private ToolStripMenuItem autodotsToolStripMenuItem;
        private ToolStripSeparator toolStripMenuItem3;
        private ToolStripMenuItem saveToolStripMenuItem5;
        private ToolStripMenuItem undoToolStripMenuItem3;
        private ToolStripSeparator toolStripMenuItem6;
        private ToolStripMenuItem ponitsToolStripMenuItem;
        private ToolStripMenuItem flipToLeftToolStripMenuItem;
        private ToolStripMenuItem flipToRightToolStripMenuItem;
        private ToolStripMenuItem saveToolStripMenuItem6;
        private ToolStripMenuItem undoToolStripMenuItem4;
        private ToolStripSeparator toolStripMenuItem8;
        private ToolStripMenuItem linesToolStripMenuItem1;
        private ToolStripMenuItem handToolStripMenuItem1;
        private SplitContainer splitContainer;
        public ctrlTemplateImage ctrlTemplateImage;
        private ToolStripMenuItem exportToolStripMenuItem;
        private ToolStripMenuItem autodotsHelpToolStripMenuItem;
        private ToolStripMenuItem mirrorHelpToolStripMenuItem;
        private ToolStripMenuItem freehandHelpToolStripMenuItem;
        private ToolStripMenuItem profileHelpToolStripMenuItem;
        private ToolStripMenuItem exportToolStripMenuItem2;
        private ToolStripMenuItem exportToolStripMenuItem1;
        private ToolStripMenuItem styleTabToolStripMenuItem;
        private ToolStripMenuItem openToolStripMenuItem1;
        private ToolStripMenuItem saveToolStripMenuItem7;
        private ToolStripMenuItem deleteToolStripMenuItem5;
        private ToolStripMenuItem exportToolStripMenuItem3;
        private ToolStripMenuItem saveToolStripMenuItem8;
        private ToolStripMenuItem undoToolStripMenuItem5;
        private ToolStripMenuItem accessoriesHelpToolStripMenuItem1;
        private ToolStripMenuItem colorHelpToolStripMenuItem;
        private ToolStripMenuItem childHelpToolStripMenuItem;
        private ToolStripMenuItem retouchHelpToolStripMenuItem;
        private ToolStripMenuItem exitToolStripMenuItem;
        private ToolStripMenuItem fullTutorialToolStripMenuItem;
        private ToolStripMenuItem frontToolStripMenuItem;
        private ToolStripMenuItem advancedModellingToolStripMenuItem;
    }
}

