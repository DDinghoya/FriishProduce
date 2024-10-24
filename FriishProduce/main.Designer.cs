﻿
namespace FriishProduce
{
    partial class Main
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.a000 = new System.Windows.Forms.Label();
            this.panel = new System.Windows.Forms.Panel();
            this.Settings = new System.Windows.Forms.Button();
            this.Back = new System.Windows.Forms.Button();
            this.Save = new System.Windows.Forms.Button();
            this.Next = new System.Windows.Forms.Button();
            this.Console = new System.Windows.Forms.ComboBox();
            this.page1 = new System.Windows.Forms.Panel();
            this.page2 = new System.Windows.Forms.Panel();
            this.Patch = new System.Windows.Forms.CheckBox();
            this.ROMPath = new System.Windows.Forms.Label();
            this.DeleteBase = new System.Windows.Forms.Button();
            this.AddBase = new System.Windows.Forms.Button();
            this.Bases = new System.Windows.Forms.ComboBox();
            this.OpenROM = new System.Windows.Forms.Button();
            this.a001 = new System.Windows.Forms.Label();
            this.BrowseROM = new System.Windows.Forms.OpenFileDialog();
            this.BrowseWAD = new System.Windows.Forms.OpenFileDialog();
            this.SaveWAD = new System.Windows.Forms.SaveFileDialog();
            this.page3 = new System.Windows.Forms.Panel();
            this.a003 = new System.Windows.Forms.Label();
            this.Banner = new System.Windows.Forms.Panel();
            this.a012 = new System.Windows.Forms.Label();
            this.ImgInterp = new System.Windows.Forms.ComboBox();
            this.a011 = new System.Windows.Forms.Label();
            this.ImgResize = new System.Windows.Forms.ComboBox();
            this.SaveDataTitle = new System.Windows.Forms.TextBox();
            this.a010 = new System.Windows.Forms.Label();
            this.ImportBases = new System.Windows.Forms.ComboBox();
            this.Import = new System.Windows.Forms.CheckBox();
            this.Image = new System.Windows.Forms.PictureBox();
            this.Players = new System.Windows.Forms.NumericUpDown();
            this.a009 = new System.Windows.Forms.Label();
            this.ReleaseYear = new System.Windows.Forms.NumericUpDown();
            this.a008 = new System.Windows.Forms.Label();
            this.BannerTitle = new System.Windows.Forms.TextBox();
            this.a007 = new System.Windows.Forms.Label();
            this.a006 = new System.Windows.Forms.Label();
            this.ChannelTitle = new System.Windows.Forms.TextBox();
            this.Custom = new System.Windows.Forms.CheckBox();
            this.page4 = new System.Windows.Forms.Panel();
            this.RegionFree = new System.Windows.Forms.CheckBox();
            this.TitleID = new System.Windows.Forms.TextBox();
            this.a005 = new System.Windows.Forms.Label();
            this.DisableEmanual = new System.Windows.Forms.CheckBox();
            this.a004 = new System.Windows.Forms.Label();
            this.Options_Flash = new System.Windows.Forms.Panel();
            this.Flash__005 = new System.Windows.Forms.Label();
            this.Flash_StrapReminder = new System.Windows.Forms.ComboBox();
            this.Flash_Controller = new System.Windows.Forms.CheckBox();
            this.Flash_FPS = new System.Windows.Forms.ComboBox();
            this.Flash_CustomFPS = new System.Windows.Forms.CheckBox();
            this.Flash__002 = new System.Windows.Forms.Label();
            this.Flash_TotalSaveDataSize = new System.Windows.Forms.ComboBox();
            this.Flash_UseSaveData = new System.Windows.Forms.CheckBox();
            this.Flash_HBMNoSave = new System.Windows.Forms.CheckBox();
            this.Options_N64 = new System.Windows.Forms.Panel();
            this.N64_Allocation = new System.Windows.Forms.CheckBox();
            this.N64_UseExpansionPak = new System.Windows.Forms.CheckBox();
            this.N64_FixBrightness = new System.Windows.Forms.CheckBox();
            this.Options_NES = new System.Windows.Forms.Panel();
            this.NES_Palette = new System.Windows.Forms.ComboBox();
            this.NES__000 = new System.Windows.Forms.Label();
            this.vWii = new System.Windows.Forms.CheckBox();
            this.ToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.BrowsePatch = new System.Windows.Forms.OpenFileDialog();
            this.BrowseImage = new System.Windows.Forms.OpenFileDialog();
            this.panel.SuspendLayout();
            this.page1.SuspendLayout();
            this.page2.SuspendLayout();
            this.page3.SuspendLayout();
            this.Banner.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Image)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Players)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ReleaseYear)).BeginInit();
            this.page4.SuspendLayout();
            this.Options_Flash.SuspendLayout();
            this.Options_N64.SuspendLayout();
            this.Options_NES.SuspendLayout();
            this.SuspendLayout();
            // 
            // a000
            // 
            resources.ApplyResources(this.a000, "a000");
            this.a000.Name = "a000";
            // 
            // panel
            // 
            this.panel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(55)))), ((int)(((byte)(55)))));
            this.panel.Controls.Add(this.Settings);
            this.panel.Controls.Add(this.Back);
            this.panel.Controls.Add(this.Save);
            this.panel.Controls.Add(this.Next);
            resources.ApplyResources(this.panel, "panel");
            this.panel.Name = "panel";
            this.panel.Tag = "panel";
            // 
            // Settings
            // 
            this.Settings.FlatAppearance.BorderColor = System.Drawing.Color.DimGray;
            this.Settings.FlatAppearance.BorderSize = 0;
            this.Settings.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DimGray;
            this.Settings.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            resources.ApplyResources(this.Settings, "Settings");
            this.Settings.Name = "Settings";
            this.Settings.UseCompatibleTextRendering = true;
            this.Settings.UseVisualStyleBackColor = false;
            this.Settings.Click += new System.EventHandler(this.Settings_Click);
            // 
            // Back
            // 
            resources.ApplyResources(this.Back, "Back");
            this.Back.FlatAppearance.BorderColor = System.Drawing.Color.DimGray;
            this.Back.FlatAppearance.BorderSize = 0;
            this.Back.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DimGray;
            this.Back.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.Back.Name = "Back";
            this.Back.UseVisualStyleBackColor = true;
            this.Back.Click += new System.EventHandler(this.Back_Click);
            // 
            // Save
            // 
            this.Save.FlatAppearance.BorderColor = System.Drawing.Color.DimGray;
            this.Save.FlatAppearance.BorderSize = 0;
            this.Save.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DimGray;
            this.Save.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            resources.ApplyResources(this.Save, "Save");
            this.Save.Name = "Save";
            this.Save.UseVisualStyleBackColor = true;
            this.Save.Click += new System.EventHandler(this.Finish_Click);
            // 
            // Next
            // 
            resources.ApplyResources(this.Next, "Next");
            this.Next.FlatAppearance.BorderColor = System.Drawing.Color.DimGray;
            this.Next.FlatAppearance.BorderSize = 0;
            this.Next.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DimGray;
            this.Next.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.Next.Name = "Next";
            this.Next.UseVisualStyleBackColor = true;
            this.Next.Click += new System.EventHandler(this.Next_Click);
            // 
            // Console
            // 
            this.Console.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.Console, "Console");
            this.Console.FormattingEnabled = true;
            this.Console.Name = "Console";
            this.Console.SelectedIndexChanged += new System.EventHandler(this.Console_Changed);
            // 
            // page1
            // 
            this.page1.Controls.Add(this.Console);
            this.page1.Controls.Add(this.a000);
            resources.ApplyResources(this.page1, "page1");
            this.page1.Name = "page1";
            // 
            // page2
            // 
            this.page2.Controls.Add(this.Patch);
            this.page2.Controls.Add(this.ROMPath);
            this.page2.Controls.Add(this.DeleteBase);
            this.page2.Controls.Add(this.AddBase);
            this.page2.Controls.Add(this.Bases);
            this.page2.Controls.Add(this.OpenROM);
            this.page2.Controls.Add(this.a001);
            resources.ApplyResources(this.page2, "page2");
            this.page2.Name = "page2";
            // 
            // Patch
            // 
            resources.ApplyResources(this.Patch, "Patch");
            this.Patch.Name = "Patch";
            this.Patch.Tag = "a014";
            this.Patch.UseVisualStyleBackColor = true;
            this.Patch.CheckedChanged += new System.EventHandler(this.Patch_CheckedChanged);
            // 
            // ROMPath
            // 
            resources.ApplyResources(this.ROMPath, "ROMPath");
            this.ROMPath.Name = "ROMPath";
            this.ROMPath.Tag = "a002";
            // 
            // DeleteBase
            // 
            resources.ApplyResources(this.DeleteBase, "DeleteBase");
            this.DeleteBase.Name = "DeleteBase";
            this.DeleteBase.UseVisualStyleBackColor = true;
            this.DeleteBase.Click += new System.EventHandler(this.DeleteWAD);
            // 
            // AddBase
            // 
            resources.ApplyResources(this.AddBase, "AddBase");
            this.AddBase.Name = "AddBase";
            this.AddBase.UseVisualStyleBackColor = true;
            this.AddBase.Click += new System.EventHandler(this.AddWAD);
            // 
            // Bases
            // 
            this.Bases.DisplayMember = "Base";
            this.Bases.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.Bases, "Bases");
            this.Bases.FormattingEnabled = true;
            this.Bases.Name = "Bases";
            this.Bases.SelectedIndexChanged += new System.EventHandler(this.BaseList_Changed);
            // 
            // OpenROM
            // 
            resources.ApplyResources(this.OpenROM, "OpenROM");
            this.OpenROM.Name = "OpenROM";
            this.OpenROM.UseVisualStyleBackColor = true;
            this.OpenROM.Click += new System.EventHandler(this.OpenROM_Click);
            // 
            // a001
            // 
            resources.ApplyResources(this.a001, "a001");
            this.a001.Name = "a001";
            // 
            // BrowseWAD
            // 
            resources.ApplyResources(this.BrowseWAD, "BrowseWAD");
            // 
            // SaveWAD
            // 
            this.SaveWAD.DefaultExt = "wad";
            // 
            // page3
            // 
            this.page3.Controls.Add(this.a003);
            this.page3.Controls.Add(this.Banner);
            this.page3.Controls.Add(this.Custom);
            resources.ApplyResources(this.page3, "page3");
            this.page3.Name = "page3";
            // 
            // a003
            // 
            resources.ApplyResources(this.a003, "a003");
            this.a003.Name = "a003";
            // 
            // Banner
            // 
            this.Banner.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Banner.Controls.Add(this.a012);
            this.Banner.Controls.Add(this.ImgInterp);
            this.Banner.Controls.Add(this.a011);
            this.Banner.Controls.Add(this.ImgResize);
            this.Banner.Controls.Add(this.SaveDataTitle);
            this.Banner.Controls.Add(this.a010);
            this.Banner.Controls.Add(this.ImportBases);
            this.Banner.Controls.Add(this.Import);
            this.Banner.Controls.Add(this.Image);
            this.Banner.Controls.Add(this.Players);
            this.Banner.Controls.Add(this.a009);
            this.Banner.Controls.Add(this.ReleaseYear);
            this.Banner.Controls.Add(this.a008);
            this.Banner.Controls.Add(this.BannerTitle);
            this.Banner.Controls.Add(this.a007);
            this.Banner.Controls.Add(this.a006);
            this.Banner.Controls.Add(this.ChannelTitle);
            resources.ApplyResources(this.Banner, "Banner");
            this.Banner.Name = "Banner";
            // 
            // a012
            // 
            resources.ApplyResources(this.a012, "a012");
            this.a012.Name = "a012";
            // 
            // ImgInterp
            // 
            this.ImgInterp.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.ImgInterp, "ImgInterp");
            this.ImgInterp.FormattingEnabled = true;
            this.ImgInterp.Items.AddRange(new object[] {
            resources.GetString("ImgInterp.Items")});
            this.ImgInterp.Name = "ImgInterp";
            this.ImgInterp.SelectedIndexChanged += new System.EventHandler(this.Image_ModeIChanged);
            // 
            // a011
            // 
            resources.ApplyResources(this.a011, "a011");
            this.a011.Name = "a011";
            // 
            // ImgResize
            // 
            this.ImgResize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.ImgResize, "ImgResize");
            this.ImgResize.FormattingEnabled = true;
            this.ImgResize.Items.AddRange(new object[] {
            resources.GetString("ImgResize.Items")});
            this.ImgResize.Name = "ImgResize";
            this.ImgResize.SelectedIndexChanged += new System.EventHandler(this.Image_StretchChanged);
            // 
            // SaveDataTitle
            // 
            resources.ApplyResources(this.SaveDataTitle, "SaveDataTitle");
            this.SaveDataTitle.Name = "SaveDataTitle";
            this.SaveDataTitle.TextChanged += new System.EventHandler(this.BannerText_Changed);
            this.SaveDataTitle.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.BannerText_KeyPress);
            // 
            // a010
            // 
            resources.ApplyResources(this.a010, "a010");
            this.a010.Name = "a010";
            // 
            // ImportBases
            // 
            this.ImportBases.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.ImportBases, "ImportBases");
            this.ImportBases.FormattingEnabled = true;
            this.ImportBases.Name = "ImportBases";
            // 
            // Import
            // 
            resources.ApplyResources(this.Import, "Import");
            this.Import.Name = "Import";
            this.Import.Tag = "a016";
            this.Import.UseVisualStyleBackColor = true;
            this.Import.CheckedChanged += new System.EventHandler(this.CheckedToggles);
            // 
            // Image
            // 
            this.Image.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.Image.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Image.Cursor = System.Windows.Forms.Cursors.Hand;
            resources.ApplyResources(this.Image, "Image");
            this.Image.Name = "Image";
            this.Image.TabStop = false;
            this.Image.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Image_Click);
            // 
            // Players
            // 
            resources.ApplyResources(this.Players, "Players");
            this.Players.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.Players.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.Players.Name = "Players";
            this.Players.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // a009
            // 
            resources.ApplyResources(this.a009, "a009");
            this.a009.Name = "a009";
            // 
            // ReleaseYear
            // 
            resources.ApplyResources(this.ReleaseYear, "ReleaseYear");
            this.ReleaseYear.Maximum = new decimal(new int[] {
            2029,
            0,
            0,
            0});
            this.ReleaseYear.Minimum = new decimal(new int[] {
            1980,
            0,
            0,
            0});
            this.ReleaseYear.Name = "ReleaseYear";
            this.ReleaseYear.Value = new decimal(new int[] {
            1980,
            0,
            0,
            0});
            // 
            // a008
            // 
            resources.ApplyResources(this.a008, "a008");
            this.a008.Name = "a008";
            // 
            // BannerTitle
            // 
            resources.ApplyResources(this.BannerTitle, "BannerTitle");
            this.BannerTitle.Name = "BannerTitle";
            this.BannerTitle.TextChanged += new System.EventHandler(this.BannerText_Changed);
            this.BannerTitle.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.BannerText_KeyPress);
            // 
            // a007
            // 
            resources.ApplyResources(this.a007, "a007");
            this.a007.Name = "a007";
            // 
            // a006
            // 
            resources.ApplyResources(this.a006, "a006");
            this.a006.Name = "a006";
            // 
            // ChannelTitle
            // 
            resources.ApplyResources(this.ChannelTitle, "ChannelTitle");
            this.ChannelTitle.Name = "ChannelTitle";
            this.ChannelTitle.TextChanged += new System.EventHandler(this.BannerText_Changed);
            // 
            // Custom
            // 
            resources.ApplyResources(this.Custom, "Custom");
            this.Custom.Name = "Custom";
            this.Custom.Tag = "a015";
            this.Custom.UseVisualStyleBackColor = true;
            this.Custom.CheckedChanged += new System.EventHandler(this.CheckedToggles);
            // 
            // page4
            // 
            this.page4.Controls.Add(this.DisableEmanual);
            this.page4.Controls.Add(this.RegionFree);
            this.page4.Controls.Add(this.TitleID);
            this.page4.Controls.Add(this.a005);
            this.page4.Controls.Add(this.a004);
            this.page4.Controls.Add(this.Options_Flash);
            this.page4.Controls.Add(this.Options_N64);
            this.page4.Controls.Add(this.Options_NES);
            this.page4.Controls.Add(this.vWii);
            resources.ApplyResources(this.page4, "page4");
            this.page4.Name = "page4";
            // 
            // RegionFree
            // 
            resources.ApplyResources(this.RegionFree, "RegionFree");
            this.RegionFree.Name = "RegionFree";
            this.RegionFree.Tag = "a018";
            this.RegionFree.UseVisualStyleBackColor = true;
            // 
            // TitleID
            // 
            this.TitleID.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            resources.ApplyResources(this.TitleID, "TitleID");
            this.TitleID.Name = "TitleID";
            this.TitleID.TextChanged += new System.EventHandler(this.TitleID_Changed);
            // 
            // a005
            // 
            resources.ApplyResources(this.a005, "a005");
            this.a005.Name = "a005";
            this.a005.Tag = "a005";
            // 
            // DisableEmanual
            // 
            resources.ApplyResources(this.DisableEmanual, "DisableEmanual");
            this.DisableEmanual.Name = "DisableEmanual";
            this.DisableEmanual.Tag = "a017";
            this.DisableEmanual.UseVisualStyleBackColor = true;
            // 
            // a004
            // 
            resources.ApplyResources(this.a004, "a004");
            this.a004.Name = "a004";
            // 
            // Options_Flash
            // 
            this.Options_Flash.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Options_Flash.Controls.Add(this.Flash__005);
            this.Options_Flash.Controls.Add(this.Flash_StrapReminder);
            this.Options_Flash.Controls.Add(this.Flash_Controller);
            this.Options_Flash.Controls.Add(this.Flash_FPS);
            this.Options_Flash.Controls.Add(this.Flash_CustomFPS);
            this.Options_Flash.Controls.Add(this.Flash__002);
            this.Options_Flash.Controls.Add(this.Flash_TotalSaveDataSize);
            this.Options_Flash.Controls.Add(this.Flash_UseSaveData);
            this.Options_Flash.Controls.Add(this.Flash_HBMNoSave);
            resources.ApplyResources(this.Options_Flash, "Options_Flash");
            this.Options_Flash.Name = "Options_Flash";
            // 
            // Flash__005
            // 
            resources.ApplyResources(this.Flash__005, "Flash__005");
            this.Flash__005.Name = "Flash__005";
            // 
            // Flash_StrapReminder
            // 
            this.Flash_StrapReminder.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.Flash_StrapReminder.FormattingEnabled = true;
            this.Flash_StrapReminder.Items.AddRange(new object[] {
            resources.GetString("Flash_StrapReminder.Items")});
            resources.ApplyResources(this.Flash_StrapReminder, "Flash_StrapReminder");
            this.Flash_StrapReminder.Name = "Flash_StrapReminder";
            this.Flash_StrapReminder.Tag = "Flash__006";
            // 
            // Flash_Controller
            // 
            resources.ApplyResources(this.Flash_Controller, "Flash_Controller");
            this.Flash_Controller.Name = "Flash_Controller";
            this.Flash_Controller.Tag = "Flash__004";
            this.Flash_Controller.UseVisualStyleBackColor = true;
            this.Flash_Controller.CheckedChanged += new System.EventHandler(this.Flash_ControllerChanged);
            // 
            // Flash_FPS
            // 
            this.Flash_FPS.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.Flash_FPS, "Flash_FPS");
            this.Flash_FPS.FormattingEnabled = true;
            this.Flash_FPS.Items.AddRange(new object[] {
            resources.GetString("Flash_FPS.Items"),
            resources.GetString("Flash_FPS.Items1"),
            resources.GetString("Flash_FPS.Items2"),
            resources.GetString("Flash_FPS.Items3"),
            resources.GetString("Flash_FPS.Items4"),
            resources.GetString("Flash_FPS.Items5"),
            resources.GetString("Flash_FPS.Items6"),
            resources.GetString("Flash_FPS.Items7")});
            this.Flash_FPS.Name = "Flash_FPS";
            // 
            // Flash_CustomFPS
            // 
            resources.ApplyResources(this.Flash_CustomFPS, "Flash_CustomFPS");
            this.Flash_CustomFPS.Name = "Flash_CustomFPS";
            this.Flash_CustomFPS.Tag = "Flash__003";
            this.Flash_CustomFPS.UseVisualStyleBackColor = true;
            this.Flash_CustomFPS.CheckedChanged += new System.EventHandler(this.CheckedToggles);
            // 
            // Flash__002
            // 
            resources.ApplyResources(this.Flash__002, "Flash__002");
            this.Flash__002.Name = "Flash__002";
            this.Flash__002.Tag = "";
            // 
            // Flash_TotalSaveDataSize
            // 
            this.Flash_TotalSaveDataSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.Flash_TotalSaveDataSize, "Flash_TotalSaveDataSize");
            this.Flash_TotalSaveDataSize.FormattingEnabled = true;
            this.Flash_TotalSaveDataSize.Items.AddRange(new object[] {
            resources.GetString("Flash_TotalSaveDataSize.Items"),
            resources.GetString("Flash_TotalSaveDataSize.Items1"),
            resources.GetString("Flash_TotalSaveDataSize.Items2"),
            resources.GetString("Flash_TotalSaveDataSize.Items3"),
            resources.GetString("Flash_TotalSaveDataSize.Items4")});
            this.Flash_TotalSaveDataSize.Name = "Flash_TotalSaveDataSize";
            // 
            // Flash_UseSaveData
            // 
            resources.ApplyResources(this.Flash_UseSaveData, "Flash_UseSaveData");
            this.Flash_UseSaveData.Name = "Flash_UseSaveData";
            this.Flash_UseSaveData.Tag = "Flash__001";
            this.Flash_UseSaveData.UseVisualStyleBackColor = true;
            this.Flash_UseSaveData.CheckedChanged += new System.EventHandler(this.CheckedToggles);
            // 
            // Flash_HBMNoSave
            // 
            resources.ApplyResources(this.Flash_HBMNoSave, "Flash_HBMNoSave");
            this.Flash_HBMNoSave.Name = "Flash_HBMNoSave";
            this.Flash_HBMNoSave.Tag = "Flash__000";
            this.Flash_HBMNoSave.UseVisualStyleBackColor = true;
            // 
            // Options_N64
            // 
            this.Options_N64.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Options_N64.Controls.Add(this.N64_Allocation);
            this.Options_N64.Controls.Add(this.N64_UseExpansionPak);
            this.Options_N64.Controls.Add(this.N64_FixBrightness);
            resources.ApplyResources(this.Options_N64, "Options_N64");
            this.Options_N64.Name = "Options_N64";
            // 
            // N64_Allocation
            // 
            resources.ApplyResources(this.N64_Allocation, "N64_Allocation");
            this.N64_Allocation.Name = "N64_Allocation";
            this.N64_Allocation.Tag = "N64__003";
            this.N64_Allocation.UseVisualStyleBackColor = true;
            // 
            // N64_UseExpansionPak
            // 
            resources.ApplyResources(this.N64_UseExpansionPak, "N64_UseExpansionPak");
            this.N64_UseExpansionPak.Name = "N64_UseExpansionPak";
            this.N64_UseExpansionPak.Tag = "N64__002";
            this.N64_UseExpansionPak.UseVisualStyleBackColor = true;
            // 
            // N64_FixBrightness
            // 
            resources.ApplyResources(this.N64_FixBrightness, "N64_FixBrightness");
            this.N64_FixBrightness.Name = "N64_FixBrightness";
            this.N64_FixBrightness.Tag = "N64__000";
            this.N64_FixBrightness.UseVisualStyleBackColor = true;
            // 
            // Options_NES
            // 
            this.Options_NES.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Options_NES.Controls.Add(this.NES_Palette);
            this.Options_NES.Controls.Add(this.NES__000);
            resources.ApplyResources(this.Options_NES, "Options_NES");
            this.Options_NES.Name = "Options_NES";
            // 
            // NES_Palette
            // 
            this.NES_Palette.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.NES_Palette.FormattingEnabled = true;
            this.NES_Palette.Items.AddRange(new object[] {
            resources.GetString("NES_Palette.Items")});
            resources.ApplyResources(this.NES_Palette, "NES_Palette");
            this.NES_Palette.Name = "NES_Palette";
            this.NES_Palette.Tag = "NES__001";
            this.NES_Palette.SelectedIndexChanged += new System.EventHandler(this.NES_PaletteChanged);
            // 
            // NES__000
            // 
            resources.ApplyResources(this.NES__000, "NES__000");
            this.NES__000.Name = "NES__000";
            this.NES__000.Tag = "";
            // 
            // vWii
            // 
            resources.ApplyResources(this.vWii, "vWii");
            this.vWii.Name = "vWii";
            this.vWii.Tag = "";
            this.vWii.UseVisualStyleBackColor = true;
            // 
            // ToolTip
            // 
            this.ToolTip.AutoPopDelay = 5000;
            this.ToolTip.BackColor = System.Drawing.Color.LemonChiffon;
            this.ToolTip.ForeColor = System.Drawing.Color.Black;
            this.ToolTip.InitialDelay = 300;
            this.ToolTip.ReshowDelay = 100;
            // 
            // BrowsePatch
            // 
            resources.ApplyResources(this.BrowsePatch, "BrowsePatch");
            // 
            // BrowseImage
            // 
            resources.ApplyResources(this.BrowseImage, "BrowseImage");
            // 
            // Main
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.panel);
            this.Controls.Add(this.page3);
            this.Controls.Add(this.page2);
            this.Controls.Add(this.page1);
            this.Controls.Add(this.page4);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "Main";
            this.panel.ResumeLayout(false);
            this.page1.ResumeLayout(false);
            this.page1.PerformLayout();
            this.page2.ResumeLayout(false);
            this.page2.PerformLayout();
            this.page3.ResumeLayout(false);
            this.page3.PerformLayout();
            this.Banner.ResumeLayout(false);
            this.Banner.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Image)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Players)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ReleaseYear)).EndInit();
            this.page4.ResumeLayout(false);
            this.page4.PerformLayout();
            this.Options_Flash.ResumeLayout(false);
            this.Options_Flash.PerformLayout();
            this.Options_N64.ResumeLayout(false);
            this.Options_N64.PerformLayout();
            this.Options_NES.ResumeLayout(false);
            this.Options_NES.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label a000;
        internal System.Windows.Forms.Panel panel;
        private System.Windows.Forms.ComboBox Console;
        private System.Windows.Forms.Button Next;
        private System.Windows.Forms.Button Settings;
        private System.Windows.Forms.Button Back;
        private System.Windows.Forms.Panel page1;
        private System.Windows.Forms.Panel page2;
        private System.Windows.Forms.Label a001;
        private System.Windows.Forms.Button OpenROM;
        private System.Windows.Forms.OpenFileDialog BrowseROM;
        private System.Windows.Forms.OpenFileDialog BrowseWAD;
        private System.Windows.Forms.SaveFileDialog SaveWAD;
        private System.Windows.Forms.ComboBox Bases;
        private System.Windows.Forms.Button AddBase;
        private System.Windows.Forms.Button Save;
        private System.Windows.Forms.Button DeleteBase;
        private System.Windows.Forms.Label ROMPath;
        private System.Windows.Forms.Panel page3;
        private System.Windows.Forms.Label a003;
        private System.Windows.Forms.Panel page4;
        private System.Windows.Forms.Label a004;
        private System.Windows.Forms.CheckBox DisableEmanual;
        private System.Windows.Forms.CheckBox Custom;
        private System.Windows.Forms.Panel Options_NES;
        private System.Windows.Forms.Label NES__000;
        private System.Windows.Forms.ComboBox NES_Palette;
        private System.Windows.Forms.ToolTip ToolTip;
        private System.Windows.Forms.Panel Banner;
        private System.Windows.Forms.Label a005;
        private System.Windows.Forms.TextBox TitleID;
        private System.Windows.Forms.CheckBox Patch;
        private System.Windows.Forms.OpenFileDialog BrowsePatch;
        private System.Windows.Forms.CheckBox RegionFree;
        private System.Windows.Forms.Label a006;
        private System.Windows.Forms.TextBox ChannelTitle;
        private System.Windows.Forms.TextBox BannerTitle;
        private System.Windows.Forms.Label a007;
        private System.Windows.Forms.Label a008;
        private System.Windows.Forms.NumericUpDown ReleaseYear;
        private System.Windows.Forms.NumericUpDown Players;
        private System.Windows.Forms.Label a009;
        private System.Windows.Forms.PictureBox Image;
        private System.Windows.Forms.CheckBox Import;
        private System.Windows.Forms.ComboBox ImportBases;
        private System.Windows.Forms.Label a010;
        private System.Windows.Forms.TextBox SaveDataTitle;
        private System.Windows.Forms.OpenFileDialog BrowseImage;
        private System.Windows.Forms.ComboBox ImgResize;
        private System.Windows.Forms.Label a011;
        private System.Windows.Forms.Label a012;
        private System.Windows.Forms.ComboBox ImgInterp;
        private System.Windows.Forms.Panel Options_N64;
        private System.Windows.Forms.CheckBox N64_FixBrightness;
        private System.Windows.Forms.CheckBox N64_UseExpansionPak;
        private System.Windows.Forms.CheckBox N64_Allocation;
        private System.Windows.Forms.Panel Options_Flash;
        private System.Windows.Forms.CheckBox Flash_HBMNoSave;
        private System.Windows.Forms.CheckBox Flash_UseSaveData;
        private System.Windows.Forms.ComboBox Flash_TotalSaveDataSize;
        private System.Windows.Forms.Label Flash__002;
        private System.Windows.Forms.CheckBox Flash_CustomFPS;
        private System.Windows.Forms.ComboBox Flash_FPS;
        private System.Windows.Forms.CheckBox Flash_Controller;
        private System.Windows.Forms.ComboBox Flash_StrapReminder;
        private System.Windows.Forms.Label Flash__005;
        private System.Windows.Forms.CheckBox vWii;
    }
}

