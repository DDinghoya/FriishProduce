﻿using Ionic.Zip;
using libWiiSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FriishProduce
{
    public partial class ProjectForm : Form
    {
        public Console Console;
        protected string TIDCode;
        protected string Untitled;
        protected string oldImgPath = "null";
        protected string newImgPath = "null";
        protected string WADPath = null;

        public bool ReadyToExport
        {
            get
            {
                bool yes = !string.IsNullOrEmpty(Creator?.TitleID) && Creator?.TitleID.Length == 4
                            && !string.IsNullOrWhiteSpace(ChannelTitle.Text)
                            && !string.IsNullOrEmpty(Creator?.BannerTitle)
                            && (Img != null)
                            && ROM?.Path != null;

                return SaveDataTitle.Visible ? yes && !string.IsNullOrEmpty(Creator?.SaveDataTitle[0]) : yes;
            }
        }

        // -----------------------------------
        // Public variables
        // -----------------------------------
        protected ROM ROM { get; set; }
        protected WAD OutWAD { get; set; }
        protected string PatchFile { get; set; }
        protected string Manual { get; set; }
        protected GameDatabase gameData { get; set; }
        protected ChannelDatabase channelData { get; set; }
        protected Preview Preview { get; set; }
        protected ImageHelper Img { get; set; }
        protected Creator Creator { get; set; }

        protected InjectorWiiVC VC { get; set; }

        // -----------------------------------
        // Options
        // -----------------------------------
        protected ContentOptions CO { get; set; }

        // -----------------------------------
        // Connection with parent form
        // -----------------------------------
        public new MainForm Parent { get; set; }
        public event EventHandler ExportCheck;
        private Project ParentProject { get; set; }
        public bool ROMLoaded { get => ROM?.Path != null; }
        private bool FormEnabled { get => groupBox1.Enabled && groupBox2.Enabled; }
        private bool isVCMode { get => InjectorsList.SelectedItem?.ToString().ToLower() == Program.Lang.String("vc").ToLower(); }


        // -----------------------------------

        public void Save()
        {
            var p = new Project()
            {
                Console = Console,
                Creator = Creator,
                ROM = ROM?.Path,
                PatchFile = PatchFile,
                ManualIndex = manual_type_list.SelectedIndex,
                ManualFile = Manual,
                Img = Img?.Source ?? null,
                InjectionMethod = InjectorsList.SelectedIndex,
                ForwarderOptions = (FStorage_USB.Checked, toggleSwitch1.Checked),
                Options = CO?.Options ?? null,
                GameData = gameData,
                WADRegion = TargetRegion.SelectedIndex,
                LinkSaveDataTitle = LinkSaveData.Checked,
                ImageOptions = (imageintpl.SelectedIndex, image_fit.Checked)
            };

            if (!string.IsNullOrWhiteSpace(WADPath)) p.BaseFile = WADPath;
            else p.Base = (Base.SelectedIndex, 0);

            for (int i = 0; i < BaseRegionList.Items.Count; i++)
                if (BaseRegionList.Items[i].GetType() == typeof(ToolStripMenuItem) && (BaseRegionList.Items[i] as ToolStripMenuItem).Checked) p.Base = (Base.SelectedIndex, i);

            using (Stream stream = File.Open(Parent.SaveProject.FileName, FileMode.Create))
            {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                binaryFormatter.Serialize(stream, p);
            }

            Tag = null;
            ExportCheck.Invoke(this, EventArgs.Empty);
        }

        private void ToggleControls(bool value)
        {
            Random.Visible =
            groupBox1.Enabled =
            groupBox2.Enabled =
            groupBox2.Enabled =
            groupBox3.Enabled =
            groupBox4.Enabled =
            groupBox5.Enabled =
            groupBox6.Enabled =
            groupBox7.Enabled =
            groupBox8.Enabled = value;
        }

        public void RefreshForm()
        {
            // ----------------------------
            if (DesignMode) return;
            // ----------------------------
            #region Localization
            Program.Lang.Control(this, "projectform");
            title_id_2.Text = title_id.Text;
            BrowsePatch.Filter = Program.Lang.String("filter.patch");
            // BrowseManualZIP.Filter = Program.Lang.String("filter.zip");

            // Change title text to untitled string
            Untitled = string.Format(Program.Lang.String("untitled_project", "mainform"), Program.Lang.String(Enum.GetName(typeof(Console), Console).ToLower(), "platforms"));
            Text = string.IsNullOrWhiteSpace(ChannelTitle.Text) ? Untitled : ChannelTitle.Text;

            SetROMDataText();

            var baseMax = Math.Max(base_name.Location.X + base_name.Width - 4, title_id.Location.X + title_id.Width - 4);
            baseName.Location = new Point(baseMax, base_name.Location.Y);
            baseID.Location = new Point(baseMax, title_id.Location.Y);

            // Selected index properties
            Program.Lang.Control(imageintpl, Name);
            imageintpl.SelectedIndex = Properties.Settings.Default.image_interpolation;

            // Manual
            Program.Lang.Control(manual_type, Name);
            Program.AutoSizeControl(manual_type_list, manual_type);
            manual_type_list.SelectedIndex = 0;
            Manual = null;

            // Regions lists
            TargetRegion.Items.Clear();
            TargetRegion.Items.Add(Program.Lang.String("keep_original"));
            TargetRegion.Items.Add(Program.Lang.String("region_rf"));
            TargetRegion.SelectedIndex = 0;

            switch (Program.Lang.Current.ToLower())
            {
                default:
                    TargetRegion.Items.Add(Program.Lang.String("region_u"));
                    TargetRegion.Items.Add(Program.Lang.String("region_e"));
                    TargetRegion.Items.Add(Program.Lang.String("region_j"));
                    TargetRegion.Items.Add(Program.Lang.String("region_k"));
                    break;

                case "ja":
                    TargetRegion.Items.Add(Program.Lang.String("region_j"));
                    TargetRegion.Items.Add(Program.Lang.String("region_u"));
                    TargetRegion.Items.Add(Program.Lang.String("region_e"));
                    TargetRegion.Items.Add(Program.Lang.String("region_k"));
                    break;

                case "ko":
                    TargetRegion.Items.Add(Program.Lang.String("region_k"));
                    TargetRegion.Items.Add(Program.Lang.String("region_u"));
                    TargetRegion.Items.Add(Program.Lang.String("region_e"));
                    TargetRegion.Items.Add(Program.Lang.String("region_j"));
                    break;
            }
            #endregion

            baseName.Font = new Font(baseName.Font, FontStyle.Bold);

            if (Base.SelectedIndex >= 0)
                for (int i = 0; i < channelData.Entries[Base.SelectedIndex].Regions.Count; i++)
                {
                    switch (channelData.Entries[Base.SelectedIndex].Regions[i])
                    {
                        default:
                        case 0:
                            BaseRegionList.Items[i].Text = Program.Lang.String("region_j");
                            break;

                        case 1:
                        case 2:
                            BaseRegionList.Items[i].Text = Program.Lang.String("region_u");
                            break;

                        case 3:
                        case 4:
                        case 5:
                            BaseRegionList.Items[i].Text = Program.Lang.String("region_e");
                            break;

                        case 6:
                        case 7:
                            BaseRegionList.Items[i].Text = Program.Lang.String("region_k");
                            break;
                    }
                }


            for (int i = 0; i < Base.Items.Count; i++)
            {
                var title = channelData.Entries[i].Regions.Contains(0) && Program.Lang.Current.StartsWith("ja") ? channelData.Entries[i].Titles[0]
                          : channelData.Entries[i].Regions.Contains(0) && Program.Lang.Current.StartsWith("ko") ? channelData.Entries[i].Titles[channelData.Entries[i].Titles.Count - 1]
                          : channelData.Entries[i].Regions.Contains(0) && channelData.Entries[i].Regions.Count > 1 ? channelData.Entries[i].Titles[1]
                          : channelData.Entries[i].Titles[0];

                Base.Items[i] = title;
            }

            // Injection methods list
            InjectorsList.Items.Clear();

            switch (Console)
            {
                case Console.NES:
                    InjectorsList.Items.Add(Program.Lang.String("vc"));
                    InjectorsList.Items.Add(Forwarder.List[0].Name);
                    InjectorsList.Items.Add(Forwarder.List[1].Name);
                    InjectorsList.Items.Add(Forwarder.List[2].Name);
                    break;

                case Console.SNES:
                    InjectorsList.Items.Add(Program.Lang.String("vc"));
                    InjectorsList.Items.Add(Forwarder.List[3].Name);
                    InjectorsList.Items.Add(Forwarder.List[4].Name);
                    InjectorsList.Items.Add(Forwarder.List[5].Name);
                    break;

                case Console.N64:
                    InjectorsList.Items.Add(Program.Lang.String("vc"));
                    InjectorsList.Items.Add(Forwarder.List[8].Name);
                    InjectorsList.Items.Add(Forwarder.List[9].Name);
                    InjectorsList.Items.Add(Forwarder.List[10].Name);
                    InjectorsList.Items.Add(Forwarder.List[11].Name);
                    break;

                case Console.SMS:
                case Console.SMD:
                    InjectorsList.Items.Add(Program.Lang.String("vc"));
                    InjectorsList.Items.Add(Forwarder.List[7].Name);
                    break;

                case Console.PCE:
                case Console.NEO:
                case Console.MSX:
                case Console.C64:
                    InjectorsList.Items.Add(Program.Lang.String("vc"));
                    break;

                case Console.Flash:
                    InjectorsList.Items.Add(Program.Lang.String("by_default"));
                    break;

                case Console.GBA:
                    InjectorsList.Items.Add(Forwarder.List[6].Name);
                    break;

                case Console.PSX:
                    InjectorsList.Items.Add(Forwarder.List[12].Name);
                    break;

                case Console.RPGM:
                    InjectorsList.Items.Add(Forwarder.List[13].Name);
                    break;

                default:
                    break;
            }

            InjectorsList.SelectedIndex = 0;
            label3.Enabled = InjectorsList.Enabled = InjectorsList.Items.Count > 1;

            if (Properties.Settings.Default.image_fit_aspect_ratio) image_fit.Checked = true; else image_stretch.Checked = true;
        }

        private void LoadChannelDatabase()
        {
            try { channelData = new ChannelDatabase(Console); }
            catch (Exception ex)
            {
                if ((int)Console < 10)
                {
                    System.Windows.Forms.MessageBox.Show($"A fatal error occurred retrieving the {Console} WADs database.\n\nException: {ex.GetType().FullName}\nMessage: {ex.Message}\n\nThe application will now shut down.", "Halt", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    Environment.FailFast("Database initialization failed.");
                }
                else { channelData = new ChannelDatabase(); }
            }
        }

        public ProjectForm(Console c, string ROMpath = null)
        {
            Console = c;
            LoadChannelDatabase();

            InitializeComponent();

            if (ROMpath != null)
            {
                ROM.Path = ROMpath;
                LoadROM(ROM.Path, Properties.Settings.Default.auto_retrieve_game_data);
            }
        }

        public ProjectForm(Project p)
        {
            Console = p.Console;
            LoadChannelDatabase();
            ParentProject = p;

            InitializeComponent();
        }

        private void Form_Shown(object sender, EventArgs e)
        {
            // ----------------------------
            if (DesignMode) return;
            // ----------------------------

            // Declare WAD metadata modifier
            // ********
            Creator = new Creator();
            TIDCode = null;

            using (var icon = new Bitmap(Parent.Icons[Console]))
            {
                icon.MakeTransparent(Color.White);
                Icon = Icon.FromHandle(icon.GetHicon());
            }

            switch (Console)
            {
                case Console.NES:
                    TIDCode = "F";
                    ROM = new ROM_NES();
                    break;

                case Console.SNES:
                    TIDCode = "J";
                    ROM = new ROM_SNES();
                    break;

                case Console.N64:
                    TIDCode = "N";
                    ROM = new ROM_N64();
                    break;

                case Console.SMS:
                    TIDCode = "L";
                    ROM = new ROM_SEGA() { IsSMS = true };
                    break;

                case Console.SMD:
                    TIDCode = "M";
                    ROM = new ROM_SEGA() { IsSMS = false };
                    break;

                case Console.PCE:
                    TIDCode = "P";
                    ROM = new ROM_PCE();
                    break;

                case Console.PCECD:
                    TIDCode = "Q";
                    ROM = new Disc();
                    break;

                case Console.NEO:
                    TIDCode = "E";
                    ROM = new ROM_NEO();
                    break;

                case Console.MSX:
                    TIDCode = "X";
                    ROM = new ROM_MSX();
                    break;

                case Console.Flash:
                    ROM = new SWF();
                    software_name.Enabled = Patch.Enabled = false;
                    break;

                case Console.RPGM:
                    ROM = new RPGM();
                    Patch.Enabled = false;
                    break;

                default:
                    ROM = new Disc();
                    Patch.Enabled = false;
                    break;
            }

            gameData = new GameDatabase();

            // Cosmetic
            // ********
            if (Console == Console.SMS || Console == Console.SMD) SaveIcon_Panel.BackgroundImage = Properties.Resources.SaveIconPlaceholder_SEGA;
            Preview = new Preview();
            IconPreview.Image = Preview.Icon(null, Console, BannerRegion(), true, IconPreview);
            RefreshForm();

            Creator.BannerYear = (int)ReleaseYear.Value;
            Creator.BannerPlayers = (int)Players.Value;
            FStorage_USB.Checked = Options.FORWARDER.Default.root_storage_device.ToLower().Contains("usb");
            toggleSwitch1.Checked = Options.FORWARDER.Default.nand_loader.ToLower().Contains("vwii");
            LinkSaveData.Checked = Properties.Settings.Default.link_save_data;

            manual_type_list.Enabled = false;
            foreach (var customManualConsole in new List<Console>() // Confirmed to have an algorithm exist for NES, SNES, N64, SEGA, PCE, NEO
            {
                Console.NES,
                Console.SNES,
                Console.N64,
                Console.SMS,
                Console.SMD,
                // Console.PCE,
                // Console.NEO
            })
                if (Console == customManualConsole) manual_type_list.Enabled = true;

            AddBases();

            if (ParentProject != null)
            {
                Creator = ParentProject.Creator;
                FStorage_USB.Checked = ParentProject.ForwarderOptions.Item1;
                toggleSwitch1.Checked = ParentProject.ForwarderOptions.Item2;

                if (ParentProject.GameData != null) gameData = ParentProject.GameData;
                if (CO != null) CO.Options = ParentProject.Options;

                foreach (var item in new string[] { ParentProject.ROM, ParentProject.PatchFile, ParentProject.BaseFile })
                    if (!File.Exists(item) && !string.IsNullOrWhiteSpace(item)) MessageBox.Show(string.Format(Program.Lang.Msg(10, true), Path.GetFileName(item)));

                ROM.Path = File.Exists(ParentProject.ROM) ? ParentProject.ROM : null;
                LoadROM(ROM.Path, false);

                Img = new ImageHelper(ParentProject.Console, null);
                Img.LoadToSource(ParentProject.Img);
                LoadImage(ParentProject.Img);

                if (File.Exists(ParentProject.BaseFile))
                {
                    WADPath = ParentProject.BaseFile;
                    ImportWAD.Checked = true;
                    DownloadWAD.Checked = false;
                    LoadWAD(ParentProject.BaseFile);
                }
                else
                {
                    Base.SelectedIndex = ParentProject.Base.Item1;
                    for (int i = 0; i < BaseRegionList.Items.Count; i++)
                        if (BaseRegionList.Items[i].GetType() == typeof(ToolStripMenuItem)) (BaseRegionList.Items[i] as ToolStripMenuItem).Checked = false;
                    UpdateBaseForm(ParentProject.Base.Item2);
                    (BaseRegionList.Items[ParentProject.Base.Item2] as ToolStripMenuItem).Checked = true;
                }

                SetROMDataText();

                ChannelTitle.Text = Creator.ChannelTitles[1];
                BannerTitle.Text = Creator.BannerTitle;
                ReleaseYear.Value = Creator.BannerYear;
                Players.Value = Creator.BannerPlayers;
                SaveDataTitle.Lines = Creator.SaveDataTitle;
                TitleID.Text = Creator.TitleID;
                TargetRegion.SelectedIndex = ParentProject.WADRegion;
                InjectorsList.SelectedIndex = ParentProject.InjectionMethod;
                LinkSaveData.Checked = ParentProject.LinkSaveDataTitle;
                imageintpl.SelectedIndex = ParentProject.ImageOptions.Item1;
                image_fit.Checked = ParentProject.ImageOptions.Item2;
                ToggleControls(!string.IsNullOrEmpty(ROM?.Path));

                PatchFile = File.Exists(ParentProject.PatchFile) ? ParentProject.PatchFile : null;
                Patch.Checked = !string.IsNullOrWhiteSpace(ParentProject.PatchFile);
                LoadManual(ParentProject.ManualIndex, ParentProject.ManualFile);

                ParentProject = null;
            }

            FStorage_SD.Checked = !FStorage_USB.Checked;

            Tag = null;
            ExportCheck.Invoke(this, EventArgs.Empty);
        }

        // -----------------------------------

        protected virtual void CheckExport()
        {
            // ----------------------------
            if (DesignMode || ParentProject != null) return;
            // ----------------------------

            Creator.TitleID = TitleID.Text;
            Creator.BannerTitle = BannerTitle.Text;
            Creator.BannerYear = (int)ReleaseYear.Value;
            Creator.BannerPlayers = (int)Players.Value;
            Creator.SaveDataTitle =
                SaveDataTitle.Lines.Length == 1 ? new string[] { SaveDataTitle.Text } :
                SaveDataTitle.Lines.Length == 0 ? new string[] { "" } :
                SaveDataTitle.Lines;

            SetROMDataText();
            ResetBannerPreview();

            Tag = "dirty";
            ExportCheck.Invoke(this, EventArgs.Empty);
        }

        public bool[] CheckToolStripButtons() => new bool[]
            {
                Console != Console.Flash
                && Console != Console.RPGM
                && Console != Console.PSX
                && (ROM?.Bytes != null || !string.IsNullOrWhiteSpace(ROM?.Path)), // LibRetro / game data

                Console != Console.Flash
                && Console != Console.RPGM
                && Console != Console.PSX
                && isVCMode, // Browse manual
            };

        protected virtual void SetROMDataText()
        {
            filename.Text = string.Format(Program.Lang.String("filename", Name), !string.IsNullOrWhiteSpace(ROM?.Path) ? Path.GetFileName(ROM.Path) : Program.Lang.String("none"));

            if (gameData == null)
                software_name.Text = string.Format(Program.Lang.String("software_name", Name), Program.Lang.String("none"));
            else if (Console == Console.RPGM && (ROM as RPGM)?.GetTitle(ROM.Path) != null)
                software_name.Text = string.Format(Program.Lang.String("software_name", Name), (ROM as RPGM).GetTitle(ROM.Path)?.Replace(Environment.NewLine, " - ") ?? Program.Lang.String("none"));
            else
                software_name.Text = string.Format(Program.Lang.String("software_name", Name), gameData.CleanTitle?.Replace(Environment.NewLine, " - ") ?? Program.Lang.String("none"));

            label11.Text = !string.IsNullOrWhiteSpace(PatchFile) ? Path.GetFileName(PatchFile) : Program.Lang.String("none");
            label11.Enabled = !string.IsNullOrWhiteSpace(PatchFile);
        }

        private void RandomTID() => TitleID.Text = Creator.TitleID = TIDCode != null ? TIDCode + GenerateTitleID().Substring(0, 3) : GenerateTitleID();

        public string GetName()
        {
            string FILENAME = Patch.Checked ? Path.GetFileNameWithoutExtension(PatchFile) : Path.GetFileNameWithoutExtension(ROM?.Path);
            string CHANNELNAME = ChannelTitle.Text;
            string FULLNAME = System.Text.RegularExpressions.Regex.Replace(Creator.BannerTitle.Replace(": ", Environment.NewLine).Replace(" - ", Environment.NewLine), @"\((.*?)\)", "").Replace("\r\n", "\n").Replace("\n", " - ");
            string TITLEID = TitleID.Text.ToUpper();
            string PLATFORM = Console.ToString();

            return Properties.Settings.Default.default_save_as_filename.Replace("FILENAME", FILENAME).Replace("CHANNELNAME", CHANNELNAME).Replace("FULLNAME", FULLNAME).Replace("TITLEID", TITLEID).Replace("PLATFORM", PLATFORM);
        }

        private void isClosing(object sender, FormClosingEventArgs e)
        {
            // ----------------------------
            if (DesignMode) return;
            // ----------------------------

            e.Cancel = !CheckUnsaved();
            if (!e.Cancel) Dispose();
        }

        public bool CheckUnsaved()
        {
            if (Tag?.ToString() == "dirty")
            {
                var result = MessageBox.Show(string.Format(Program.Lang.Msg(1), Text), null, new string[] { Program.Lang.String("b_save"), Program.Lang.String("b_dont_save"), Program.Lang.String("b_cancel") });
                {
                    if (result == MessageBox.Result.Button1)
                    {
                        return Parent.SaveAs_Trigger();
                    }

                    else if (result == MessageBox.Result.Button2)
                    {
                        return true;
                    }

                    else if (result == MessageBox.Result.Cancel || result == MessageBox.Result.Button3)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private void Random_Click(object sender, EventArgs e) => RandomTID();

        private void Value_Changed(object sender, EventArgs e) => CheckExport();

        private void LinkSaveDataTitle()
        {
            if (LinkSaveData.Checked)
            {
                string[] lines = new string[2];
                int limit = SaveDataTitle.Multiline ? SaveDataTitle.MaxLength / 2 : SaveDataTitle.MaxLength;
                if (ChannelTitle.TextLength <= limit) lines[0] = ChannelTitle.Text;
                if (BannerTitle.Lines.Length > 1 && BannerTitle.Lines[1].Length <= limit) lines[1] = BannerTitle.Lines[1];

                SaveDataTitle.Text
                    = string.IsNullOrWhiteSpace(lines[1]) ? lines[0]
                    : string.IsNullOrWhiteSpace(lines[0]) ? lines[1]
                    : string.IsNullOrWhiteSpace(lines[0]) && string.IsNullOrWhiteSpace(lines[1]) ? null
                    : SaveDataTitle.Multiline ? string.Join(Environment.NewLine, lines) : lines[0];

                if (FormEnabled) CheckExport();
            }
        }

        private void LinkSaveData_Changed(object sender, EventArgs e)
        {
            if (sender == LinkSaveData)
            {
                SaveDataTitle.Enabled = !LinkSaveData.Checked;
                if (LinkSaveData.Checked) LinkSaveDataTitle();
            }
        }

        private void TextBox_Changed(object sender, EventArgs e)
        {
            if (sender == ChannelTitle)
            {
                Text = string.IsNullOrWhiteSpace(ChannelTitle.Text) ? Untitled : ChannelTitle.Text;

                if (!ChannelTitle_Locale.Checked)
                {
                    ChannelTitle_Locale.Enabled = !string.IsNullOrWhiteSpace(ChannelTitle.Text);
                    Creator.ChannelTitles = new string[8] { ChannelTitle.Text, ChannelTitle.Text, ChannelTitle.Text, ChannelTitle.Text, ChannelTitle.Text, ChannelTitle.Text, ChannelTitle.Text, ChannelTitle.Text };
                }
            }

            if (sender == BannerTitle || sender == ChannelTitle) LinkSaveDataTitle();

            var currentSender = sender as TextBox;
            if (currentSender.Multiline && currentSender.Lines.Length > 2) currentSender.Lines = new string[] { currentSender.Lines[0], currentSender.Lines[1] };

            CheckExport();
        }

        private void TextBox_Handle(object sender, KeyPressEventArgs e)
        {
            var currentSender = sender as TextBox;
            var currentIndex = currentSender.GetLineFromCharIndex(currentSender.SelectionStart);
            var lineMaxLength = currentSender.Multiline ? Math.Round((double)currentSender.MaxLength / 2) : currentSender.MaxLength;

            if (!string.IsNullOrEmpty(currentSender.Text)
                && currentSender.Lines[currentIndex].Length >= lineMaxLength
                && e.KeyChar != (char)Keys.Delete && e.KeyChar != (char)8 && e.KeyChar != (char)Keys.Enter)
                goto Handled;

            if (currentSender.Multiline && currentSender.Lines.Length == 2 && e.KeyChar == (char)Keys.Enter) goto Handled;

            return;

            Handled:
            System.Media.SystemSounds.Beep.Play();
            e.Handled = true;
        }

        private void OpenWAD_CheckedChanged(object sender, EventArgs e)
        {
            // ----------------------------
            if (DesignMode) return;
            // ----------------------------

            DownloadWAD.Checked = !ImportWAD.Checked;
            Base.Enabled = BaseRegion.Enabled = !ImportWAD.Checked;
            if (Base.Enabled)
            {
                AddBases();
            }
            else
            {
                BaseRegion.Image = null;
            }

            if (ImportWAD.Checked && WADPath == null)
            {
                BrowseWAD.Title = ImportWAD.Text;
                BrowseWAD.Filter = Program.Lang.String("filter.wad");
                var result = BrowseWAD.ShowDialog();

                if (result == DialogResult.OK && !LoadWAD(BrowseWAD.FileName)) ImportWAD.Checked = false;
                else if (result == DialogResult.Cancel) ImportWAD.Checked = false;
            }
            else if (!ImportWAD.Checked)
            {
                WADPath = null;
            }

            if (groupBox2.Enabled) CheckExport();
        }

        private void InterpolationChanged(object sender, EventArgs e)
        {
            // ----------------------------
            if (DesignMode) return;
            // ----------------------------

            if (imageintpl.SelectedIndex != Properties.Settings.Default.image_interpolation) Tag = "dirty";
            LoadImage();
        }

        private void SwitchAspectRatio(object sender, EventArgs e)
        {
            // ----------------------------
            if (DesignMode) return;
            // ----------------------------

            if (sender == image_stretch || sender == image_fit)
            {
                if (sender == image_stretch && image_fit.Checked)
                {
                    Tag = "dirty";
                    image_fit.Checked = !image_stretch.Checked;
                }

                else if (sender == image_fit && image_stretch.Checked)
                {
                    Tag = "dirty";
                    image_stretch.Checked = !image_fit.Checked;
                }

                LoadImage();
            }

            else if (groupBox3.Enabled) CheckExport();
        }

        #region Load Data Functions
        private string GenerateTitleID()
        {
            var r = new Random();
            string allowed = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(allowed, 4).Select(s => s[r.Next(s.Length)]).ToArray());
        }

        public bool LoadWAD(string path)
        {
            WAD Reader = new WAD();

            try
            {
                if (Directory.Exists(Paths.WAD)) Directory.Delete(Paths.WAD, true);
                Reader = WAD.Load(path);
            }
            catch
            {
                goto Failed;
            }

            for (int h = 0; h < channelData.Entries.Count; h++)
                for (int i = 0; i < channelData.Entries[h].Regions.Count; i++)
                    if (channelData.Entries[h].GetUpperID(i) == Reader.UpperTitleID.ToUpper())
                    {
                        WADPath = path;

                        Base.SelectedIndex = h;
                        UpdateBaseForm(i);
                        Reader.Dispose();
                        return true;
                    }

            Failed:
            Reader.Dispose();
            System.Media.SystemSounds.Beep.Play();
            MessageBox.Show(string.Format(Program.Lang.Msg(5), Reader.UpperTitleID));
            return false;
        }

        public void LoadManual(int index, string path = null, bool isFolder = true)
        {
            if (File.Exists(path) && !isFolder)
            {
                index = 2;

                using (ZipFile ZIP = ZipFile.Read(path))
                {
                    int applicable = 0;
                    // bool hasFolder = false;

                    foreach (var item in ZIP.Entries)
                    {
                        // Check if is a valid emanual contents folder
                        // ****************
                        // if ((item.FileName == "emanual" || item.FileName == "html") && item.IsDirectory)
                        //    hasFolder = true;

                        // Check key files
                        // ****************
                        /* else */
                        if ((item.FileName.StartsWith("startup") && Path.GetExtension(item.FileName) == ".html")
              || item.FileName == "standard.css"
              || item.FileName == "contents.css"
              || item.FileName == "vsscript.css") applicable++;
                    }

                    if (applicable >= 2 /* && hasFolder */)
                    {
                        Manual = path;
                        goto End;
                    }

                    goto Failed;
                }
            }

            else if (Directory.Exists(path) && isFolder)
            {
                index = 2;

                // Check if is a valid emanual contents folder
                // ****************
                string folder = path;
                if (Directory.Exists(Path.Combine(path, "emanual")))
                    folder = Path.Combine(path, "emanual");
                else if (Directory.Exists(Path.Combine(path, "html")))
                    folder = Path.Combine(path, "html");

                int validFiles = 0;
                if (folder != null)
                    foreach (var item in Directory.EnumerateFiles(folder))
                    {
                        if ((Path.GetFileNameWithoutExtension(item).StartsWith("startup") && Path.GetExtension(item) == ".html")
                         || Path.GetFileName(item) == "standard.css"
                         || Path.GetFileName(item) == "contents.css"
                         || Path.GetFileName(item) == "vsscript.css") validFiles++;
                    }

                if (validFiles >= 2)
                {
                    Manual = path;
                    goto End;
                }

                goto Failed;
            }

            else
            {
                Manual = null;
                goto End;
            }

            Failed:
            MessageBox.Show(Program.Lang.Msg(7), MessageBox.Buttons.Ok, MessageBox.Icons.Warning);
            Manual = null;
            goto End;

            End:
            if (Manual == null && index >= 2) index = 0;
            manual_type_list.SelectedIndex = index;
        }

        public void LoadImage()
        {
            if (Creator != null && Img != null) LoadImage(Img.Source);
            else CheckExport();
        }

        public void LoadImage(string path)
        {
            if (Img != null) oldImgPath = newImgPath;
            newImgPath = path;

            if (Img == null) Img = new ImageHelper(Console, path);
            else Img.Create(Console, path);

            LoadImage(Img.Source);
        }

        public bool LoadImage(Bitmap src)
        {
            if (src == null) return false;

            try
            {
                Img.Interpolation = (InterpolationMode)imageintpl.SelectedIndex;
                Img.FitAspectRatio = image_fit.Checked;

                Bitmap img = (Bitmap)src.Clone();

                // Additionally edit image before generating files, e.g. with modification of image palette/brightness, used only for images with exact resolution of original screen size
                // ********
                switch (Console)
                {
                    default:
                        break;

                    case Console.NES:
                        if (InjectorsList.SelectedIndex == 0
                            && CO.Options != null
                            && bool.Parse(CO.Options["use_tImg"]))
                        {
                            var CO_NES = CO as Options_VC_NES;
                            var palette = CO_NES.CheckPalette(img);

                            if (palette != -1 && src.Width == 256 && (src.Height == 224 || src.Height == 240))
                                img = CO_NES.SwapColors(img, CO_NES.Palettes[palette], CO_NES.Palettes[int.Parse(CO_NES.Options["palette"])]);
                        }
                        break;

                    case Console.SMS:
                    case Console.SMD:
                        break;
                }

                Img.Generate(img);
                img.Dispose();

                if (Img.Source != null)
                {
                    SaveIcon_Panel.BackgroundImage = Img.SaveIcon();
                    ResetBannerPreview();
                    CheckExport();
                }

                return true;
            }
            catch
            {
                MessageBox.Show(Program.Lang.Msg(1, true));
                return false;
            }
        }

        public void LoadROM(bool LoadGameData = true) => LoadROM(Parent.BrowseROM.FileName, LoadGameData);

        public void LoadROM(string ROMpath, bool LoadGameData = true)
        {
            switch (Console)
            {
                // ROM file formats
                // ****************
                default:
                    if (ROM == null || !ROM.CheckValidity(ROMpath))
                    {
                        MessageBox.Show(Program.Lang.Msg(2), 0, MessageBox.Icons.Warning);
                        return;
                    }
                    break;

                // ZIP format
                // ****************
                case Console.NEO:
                    if (!ROM.CheckZIPValidity(ROMpath, new string[] { "c1", "c2", "m1", "p1", "s1", "v1" }, true, true))
                    {
                        MessageBox.Show(Program.Lang.Msg(2), 0, MessageBox.Icons.Warning);
                        return;
                    }
                    break;

                // Disc format
                // ****************
                case Console.PSX:
                    break;

                // RPG Maker format
                // ****************
                case Console.RPGM:
                    if ((ROM as RPGM).GetTitle(ROMpath) != null)
                    {
                        Creator.BannerTitle = BannerTitle.Text = (ROM as RPGM).GetTitle(ROMpath);
                        if (Creator.BannerTitle.Length <= ChannelTitle.MaxLength) ChannelTitle.Text = Creator.BannerTitle;
                    }
                    break;

                // Other, no verification needed
                // ****************
                case Console.Flash:
                    break;
            }

            if (ROM != null) ROM.Path = ROMpath;

            PatchFile = null;
            Patch.Checked = false;

            ToggleControls(true);
            RandomTID();
            CheckExport();

            Parent.tabControl.Visible = true;

            if (ROM != null && LoadGameData && CheckToolStripButtons()[0]) this.LoadGameData();
        }

        public async void LoadGameData()
        {
            if (ROM == null || ROM.Path == null) return;

            try
            {
                gameData = new GameDatabase { SoftwarePath = ROM.Path };
                var Retrieved = await Task.FromResult(gameData.Get(Console));
                if (Retrieved)
                {
                    // Set banner title
                    BannerTitle.Text = Creator.BannerTitle = gameData.CleanTitle ?? Creator.BannerTitle;

                    // Set channel title text
                    if (gameData.CleanTitle != null)
                    {
                        var text = gameData.CleanTitle.Replace("\r", "").Split('\n');
                        if (text[0].Length <= ChannelTitle.MaxLength) { ChannelTitle_Locale.Checked = false; ChannelTitle.Text = text[0]; }
                    }

                    // Set image
                    if (gameData.ImgURL != null) { LoadImage(gameData.ImgURL); }

                    // Set year and players
                    ReleaseYear.Value = Creator.BannerYear = !string.IsNullOrEmpty(gameData.Year) ? int.Parse(gameData.Year) : Creator.BannerYear;
                    Players.Value = Creator.BannerPlayers = !string.IsNullOrEmpty(gameData.Players) ? int.Parse(gameData.Players) : Creator.BannerPlayers;
                }

                if (Retrieved && LinkSaveData.Checked) LinkSaveDataTitle();
                else if (gameData?.CleanTitle != null && ChannelTitle.TextLength <= SaveDataTitle.MaxLength) SaveDataTitle.Text = ChannelTitle.Text;

                // Show message if partially failed to retrieve data
                if (Retrieved && (gameData.Title == null || gameData.Players == null || gameData.Year == null || gameData.ImgURL == null))
                    MessageBox.Show(Program.Lang.Msg(4));
                else if (!Retrieved) System.Media.SystemSounds.Beep.Play();

            }
            catch (Exception ex)
            {
                MessageBox.Error(ex.Message);
            }
        }

        protected libWiiSharp.Region GetRegionSetting()
        {
            return TargetRegion.SelectedItem?.ToString() == Program.Lang.String("region_j") ? libWiiSharp.Region.Japan
                : TargetRegion.SelectedItem?.ToString() == Program.Lang.String("region_u") ? libWiiSharp.Region.USA
                : TargetRegion.SelectedItem?.ToString() == Program.Lang.String("region_e") ? libWiiSharp.Region.Europe
                : TargetRegion.SelectedItem?.ToString() == Program.Lang.String("region_k") ? libWiiSharp.Region.Korea
                : TargetRegion.SelectedIndex == 0 && OutWAD != null ? OutWAD.Region
                : libWiiSharp.Region.Free;
        }

        public bool SaveToWAD()
        {
            try
            {
                Parent.CleanTemp();

                // Get WAD data
                // *******
                OutWAD = new WAD();
                if (WADPath != null) OutWAD = WAD.Load(WADPath);
                else foreach (var entry in channelData.Entries)
                        for (int i = 0; i < entry.Regions.Count; i++)
                            if (entry.GetUpperID(i) == baseID.Text.ToUpper()) OutWAD = entry.GetWAD(i);
                if (OutWAD == null || OutWAD?.NumOfContents <= 1) throw new Exception(Program.Lang.Msg(8, true));

                Creator.Out = Parent.SaveWAD.FileName;
                if (Patch.Checked) ROM.Patch(PatchFile);

                switch (Console)
                {
                    case Console.NES:
                    case Console.SNES:
                    case Console.N64:
                    case Console.SMS:
                    case Console.SMD:
                    case Console.PCE:
                    case Console.PCECD:
                    case Console.NEO:
                    case Console.MSX:
                        if (isVCMode)
                            WiiVCInject();
                        else
                            ForwarderCreator();
                        break;

                    case Console.GB:
                    case Console.GBC:
                    case Console.GBA:
                    case Console.S32X:
                    case Console.SMCD:
                    case Console.PSX:
                    case Console.RPGM:
                        ForwarderCreator();
                        break;

                    case Console.Flash:
                        FlashInject();
                        break;

                    default:
                        throw new NotImplementedException();
                }

                // Banner
                // *******
                BannerHelper.Modify
                (
                    OutWAD,
                    Console,
                    isVCMode ? OutWAD.Region : (int)BannerRegion() switch { 1 => libWiiSharp.Region.Japan, 2 => libWiiSharp.Region.Korea, 3 => libWiiSharp.Region.Europe, _ => libWiiSharp.Region.USA },
                    Creator.BannerTitle,
                    Creator.BannerYear,
                    Creator.BannerPlayers
                );
                SoundHelper.ReplaceSound(OutWAD, Properties.Resources.Sound_WiiVC);
                if (Img.VCPic != null) Img.ReplaceBanner(OutWAD);

                // Change WAD region
                // *******
                if (TargetRegion.SelectedIndex > 0)
                    OutWAD.Region = GetRegionSetting();

                // Other WAD settings to be changed done by WAD creator helper, which will save to a new file
                // *******
                Creator.MakeWAD(OutWAD);

                // Check new WAD file
                // *******
                if (File.Exists(Creator.Out) && File.ReadAllBytes(Creator.Out).Length > 10)
                {
                    System.Media.SystemSounds.Beep.Play();

                    var Message = MessageBox.Show(Program.Lang.Msg(3), null, MessageBox.Buttons.Custom, MessageBox.Icons.Information);

                    if (Message == MessageBox.Result.Button1)
                    {
                        string args = string.Format("/e, /select, \"{0}\"", Creator.Out);

                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = "explorer",
                            Arguments = args
                        });
                    }

                    return true;
                }
                else throw new Exception(Program.Lang.Msg(6, true));
            }

            catch (Exception ex)
            {
                MessageBox.Error(ex.Message);
                return false;
            }

            finally
            {
                Creator.Out = null;
                OutWAD = null;
                Parent.CleanTemp();
                if (VC != null) VC.Dispose();
            }
        }

        public void ForwarderCreator()
        {
            Forwarder f = new Forwarder()
            {
                ROM = ROM.Path,
                ID = Creator.TitleID,
                Emulator = InjectorsList.SelectedItem.ToString(),
                Storage = FStorage_USB.Checked ? Forwarder.Storages.USB : Forwarder.Storages.SD
            };

            // Get settings from relevant form
            // *******
            if (CO != null) { f.Settings = CO.Options; } else { f.Settings = new Dictionary<string, string> { { "N/A", "N/A" } }; }

            // Actually inject everything
            // *******
            f.CreateZIP(Path.Combine(Path.GetDirectoryName(Creator.Out), Path.GetFileNameWithoutExtension(Creator.Out) + $" ({f.Storage}).zip"));
            OutWAD = f.CreateWAD(OutWAD, toggleSwitch1.Checked);
        }

        public void FlashInject()
        {
            Injectors.Flash.Settings = CO.Options;
            OutWAD = Injectors.Flash.Inject(OutWAD, ROM.Path, Creator.SaveDataTitle, Img);
        }

        public void WiiVCInject()
        {
            // Create Wii VC injector to use
            // *******
            switch (Console)
            {
                default:
                    throw new NotImplementedException();

                // NES
                // *******
                case Console.NES:
                    VC = new Injectors.NES();
                    break;

                // SNES
                // *******
                case Console.SNES:
                    VC = new Injectors.SNES();
                    break;

                // N64
                // *******
                case Console.N64:
                    VC = new Injectors.N64()
                    {
                        Settings = CO.Options,

                        CompressionType = CO.EmuType == 3 ? (CO.Options.ElementAt(3).Value == "0" ? 1 : 2) : 0,
                        Allocate = CO.Options.ElementAt(3).Value == "True" && (CO.EmuType <= 1),
                    };
                    break;

                // SEGA
                // *******
                case Console.SMS:
                case Console.SMD:
                    VC = new Injectors.SEGA()
                    {
                        IsSMS = Console == Console.SMS
                    };
                    break;

                // PCE
                // *******
                case Console.PCE:
                    VC = new Injectors.PCE();
                    break;

                // PCECD
                // *******
                case Console.PCECD:
                    // VC = new Injectors.PCECD();
                    break;

                // NEOGEO
                // *******
                case Console.NEO:
                    VC = new Injectors.NEO();
                    break;

                // MSX
                // *******
                case Console.MSX:
                    VC = new Injectors.MSX();
                    break;
            }

            VC.WAD = OutWAD;

            // Get settings from relevant form
            // *******
            if (CO != null) { VC.Settings = CO.Options; } else { VC.Settings = new Dictionary<string, string> { { "N/A", "N/A" } }; }

            // Set path to manual (if it exists) and load WAD
            // *******
            VC.Manual = Manual != null ? new ZipFile("manual") : null;
            VC.KeepOrigManual = manual_type_list.SelectedIndex == 1;
            if (VC.Manual != null && manual_type_list.SelectedIndex == 2) VC.Manual.AddDirectory(Manual);

            // Actually inject everything
            // *******
            OutWAD = VC.Inject(ROM, Creator.SaveDataTitle, Img);
        }
        #endregion

        #region **Console-Specific Functions**
        // ******************
        // CONSOLE-SPECIFIC
        // ******************
        private void OpenInjectorOptions(object sender, EventArgs e)
        {
            var result = CO.ShowDialog(this) == DialogResult.OK;

            switch (Console)
            {
                default:
                case Console.SNES:
                case Console.MSX:
                    break;

                case Console.NES:
                    if (result) { LoadImage(); }
                    break;

                case Console.N64:
                case Console.SMS:
                case Console.SMD:
                case Console.PCE:
                case Console.PCECD:
                case Console.NEO:
                case Console.Flash:
                    if (result) { CheckExport(); }
                    break;
            }
        }
        #endregion

        #region Base WAD Management/Visual
        private void AddBases()
        {
            Base.Items.Clear();

            foreach (var entry in channelData.Entries)
            {
                var title = entry.Regions.Contains(0) && Program.Lang.Current.StartsWith("ja") ? entry.Titles[0]
                          : entry.Regions.Contains(0) && Program.Lang.Current.StartsWith("ko") ? entry.Titles[entry.Titles.Count - 1]
                          : entry.Regions.Contains(0) && entry.Regions.Count > 1 ? entry.Titles[1]
                          : entry.Titles[0];

                Base.Items.Add(title);
            }

            if (Base.Items.Count > 0) { Base.SelectedIndex = 0; }

            Base.Enabled = Base.Items.Count > 1;
            UpdateBaseForm();
        }


        // -----------------------------------

        private void Base_SelectedIndexChanged(object sender, EventArgs e)
        {
            // ----------------------------
            if (DesignMode || Base.SelectedIndex < 0) return;
            // ----------------------------

            var regions = new List<string>();
            for (int i = 0; i < channelData.Entries[Base.SelectedIndex].Regions.Count; i++)
            {
                switch (channelData.Entries[Base.SelectedIndex].Regions[i])
                {
                    case 0:
                        regions.Add(Program.Lang.String("region_j"));
                        break;

                    case 1:
                    case 2:
                        regions.Add(Program.Lang.String("region_u"));
                        break;

                    case 3:
                    case 4:
                    case 5:
                        regions.Add(Program.Lang.String("region_e"));
                        break;

                    case 6:
                    case 7:
                        regions.Add(Program.Lang.String("region_k"));
                        break;

                    default:
                    case 8:
                        regions.Add(Program.Lang.String("region_rf"));
                        break;
                }
            }

            // Check if language is set to Japanese or Korean
            // If so, make Japan/Korea region item the first in the WAD region context list
            // ********
            var selected = regions.IndexOf(Program.Lang.String("region_u"));

            var altRegions = new Dictionary<string, int>()
            {
                { "ja", 0 },
                { "ko", 1 },
                { "fr", 2 },
                { "de", 2 },
                { "nl", 2 },
                { "it", 2 },
                { "pl", 2 },
                { "ru", 2 },
                { "uk", 2 },
                { "tr", 2 },
                { "hu", 2 },
                { "ca", 2 },
                { "eu", 2 },
                { "gl", 2 },
                { "ast", 2 },
                { "no", 2 },
                { "sv", 2 },
                { "fi", 2 },
                { "-GB", 2 },
                { "-UK", 2 },
                { "-ES", 2 },
                { "-PT", 2 },
                { "-RU", 2 },
                { "-IN", 2 },
                { "-ZA", 2 },
                { "-CA", 3 },
                { "-US", 3 },
                { "-MX", 3 },
                { "-BR", 3 },
            };
            foreach (var item in altRegions) if (Program.Lang.Current.ToLower().StartsWith(item.Key) || (item.Key.Contains("-") && Program.Lang.Current.ToLower().EndsWith(item.Key)))
                    selected = regions.IndexOf(item.Value == 0 ? Program.Lang.String("region_j") : item.Value == 1 ? Program.Lang.String("region_k") : item.Value == 2 ? Program.Lang.String("region_e") : Program.Lang.String("region_u"));

            if (selected == -1) selected = 0;

            // Reset currently-selected base info
            // ********
            BaseRegionList.Items.Clear();

            // Add regions to WAD region context list
            // ********
            for (int i = 0; i < channelData.Entries[Base.SelectedIndex].Regions.Count; i++)
            {
                switch (channelData.Entries[Base.SelectedIndex].Regions[i])
                {
                    case 0:
                        BaseRegionList.Items.Add(Program.Lang.String("region_j"), null, WADRegionList_Click);
                        break;

                    case 1:
                    case 2:
                        BaseRegionList.Items.Add(Program.Lang.String("region_u"), null, WADRegionList_Click);
                        break;

                    case 3:
                    case 4:
                    case 5:
                        BaseRegionList.Items.Add(Program.Lang.String("region_e"), null, WADRegionList_Click);
                        break;

                    case 6:
                    case 7:
                        BaseRegionList.Items.Add(Program.Lang.String("region_k"), null, WADRegionList_Click);
                        break;

                    default:
                    case 8:
                        BaseRegionList.Items.Add(Program.Lang.String("region_rf"), null, WADRegionList_Click);
                        break;
                }
            }

            // Final visual updates
            // ********
            UpdateBaseForm(selected);
            BaseRegion.Cursor = BaseRegionList.Items.Count == 1 ? Cursors.Default : Cursors.Hand;
        }

        private void WADRegion_Click(object sender, EventArgs e)
        {
            if (BaseRegionList.Items.Count > 1)
                BaseRegionList.Show(this, PointToClient(Cursor.Position));
        }

        private void WADRegionList_Click(object sender, EventArgs e)
        {
            string targetRegion = (sender as ToolStripMenuItem).Text;

            for (int i = 0; i < BaseRegionList.Items.Count; i++)
            {
                if ((BaseRegionList.Items[i] as ToolStripMenuItem).Text == targetRegion)
                {
                    UpdateBaseForm(i);
                    CheckExport();
                    return;
                }
            }
        }

        private void UpdateBaseForm(int index = -1)
        {
            if (index == -1)
            {
                for (index = 0; index < channelData.Entries[Base.SelectedIndex].Regions.Count; index++)
                    if (channelData.Entries[Base.SelectedIndex].GetUpperID(index)[3] == baseID.Text[3])
                        goto Set;

                return;
            }

            Set:
            // Native name & Title ID
            // ********
            baseName.Text = channelData.Entries[Base.SelectedIndex].Titles[index];
            baseID.Text = channelData.Entries[Base.SelectedIndex].GetUpperID(index);

            if (BaseRegionList.Items.Count > 0)
            {
                foreach (ToolStripMenuItem item in BaseRegionList.Items.OfType<ToolStripMenuItem>())
                    item.Checked = false;
                (BaseRegionList.Items[index] as ToolStripMenuItem).Checked = true;
            }

            // Flag
            // ********
            switch (channelData.Entries[Base.SelectedIndex].Regions[index])
            {
                case 0:
                    BaseRegion.Image = Properties.Resources.flag_jp;
                    break;

                case 1:
                case 2:
                    BaseRegion.Image = Properties.Resources.flag_us;
                    break;

                case 3:
                    BaseRegion.Image = (int)Console <= 2 ? Properties.Resources.flag_eu50 : Properties.Resources.flag_eu;
                    break;

                case 4:
                case 5:
                    BaseRegion.Image = (int)Console <= 2 ? Properties.Resources.flag_eu60 : Properties.Resources.flag_eu;
                    break;

                case 6:
                case 7:
                    BaseRegion.Image = Properties.Resources.flag_kr;
                    break;

                default:
                    BaseRegion.Image = null;
                    break;
            }

            UpdateBaseGeneral(index);
        }

        private void UpdateBaseGeneral(int index)
        {
            int oldSaveLength = SaveDataTitle.MaxLength;

            // Korean WADs use different encoding format & using two lines or going over max limit cause visual bugs
            // ********
            Creator.OrigRegion = channelData.Entries[Base.SelectedIndex].Regions[index] == 6 || channelData.Entries[Base.SelectedIndex].Regions[index] == 7 ? Creator.RegionType.Korea
                     : channelData.Entries[Base.SelectedIndex].Regions[index] == 0 ? Creator.RegionType.Japan
                     : channelData.Entries[Base.SelectedIndex].Regions[index] >= 3 && channelData.Entries[Base.SelectedIndex].Regions[index] <= 5 ? Creator.RegionType.Europe
                     : Creator.RegionType.Universal;

            // Changing SaveDataTitle max length & clearing text field when needed
            // ----------------------
            if (Console == Console.NES) SaveDataTitle.MaxLength = Creator.OrigRegion == Creator.RegionType.Korea ? 30 : 20;
            else if (Console == Console.SNES) SaveDataTitle.MaxLength = 80;
            else if (Console == Console.N64) SaveDataTitle.MaxLength = 100;
            else if (Console == Console.NEO
                  || Console == Console.MSX) SaveDataTitle.MaxLength = 64;
            else SaveDataTitle.MaxLength = 80;

            // Also, some consoles only support a single line anyway
            // ********
            bool isSingleLine = Creator.OrigRegion == Creator.RegionType.Korea
                             || Console == Console.NES
                             || Console == Console.SMS
                             || Console == Console.SMD
                             || Console == Console.PCE
                             || Console == Console.PCECD;

            // Set textbox to use single line when needed
            // ********
            if (SaveDataTitle.Multiline == isSingleLine)
            {
                SaveDataTitle.Multiline = !isSingleLine;
                SaveDataTitle.Location = isSingleLine ? new Point(SaveDataTitle.Location.X, int.Parse(SaveDataTitle.Tag.ToString()) + 8) : new Point(SaveDataTitle.Location.X, int.Parse(SaveDataTitle.Tag.ToString()));
                SaveDataTitle.Clear();
                goto End;
            }
            if (Creator.OrigRegion == Creator.RegionType.Korea && SaveDataTitle.Multiline) SaveDataTitle.MaxLength /= 2; // Applies to both NES/FC & SNES/SFC

            // Clear text field if at least one line is longer than the maximum limit allowed
            // ********
            double max = SaveDataTitle.Multiline ? Math.Round((double)SaveDataTitle.MaxLength / 2) : SaveDataTitle.MaxLength;
            foreach (var line in SaveDataTitle.Lines)
                if (line.Length > max && SaveDataTitle.MaxLength != oldSaveLength)
                    SaveDataTitle.Clear();

            End:
            ResetBannerPreview();
            LinkSaveDataTitle();
            ResetContentOptions();
        }

        private int emuVer
        {
            get
            {
                if (channelData != null)
                    foreach (var entry in channelData.Entries)
                        for (int i = 0; i < entry.Regions.Count; i++)
                            if (entry.GetUpperID(i) == baseID.Text.ToUpper())
                                return entry.EmuRevs[i];

                return 0;
            }
        }

        /// <summary>
        /// Changes injector settings based on selected base/console
        /// </summary>
        private void ResetContentOptions()
        {
            COPanel_VC.Hide();
            COPanel_Forwarder.Hide();
            bool ShowSaveData = false;

            CO = null;
            if (isVCMode)
            {
                COPanel_VC.Show();
                ShowSaveData = true;

                switch (Console)
                {
                    case Console.NES:
                        CO = new Options_VC_NES() { EmuType = emuVer };
                        break;

                    case Console.SNES:
                        break;

                    case Console.N64:
                        CO = new Options_VC_N64() { EmuType = Creator.OrigRegion == Creator.RegionType.Korea ? 3 : emuVer };
                        break;

                    case Console.SMS:
                    case Console.SMD:
                        CO = new Options_VC_SEGA() { EmuType = emuVer, IsSMS = Console == Console.SMS };
                        break;

                    case Console.PCE:
                    case Console.PCECD:
                        CO = new Options_VC_PCE();
                        break;

                    case Console.NEO:
                        CO = new Options_VC_NEO() { EmuType = emuVer };
                        break;

                    case Console.MSX:
                        break;

                    case Console.C64:
                        break;
                }
            }

            else if (Console == Console.Flash)
            {
                ShowSaveData = true;
                CO = new Options_Flash();
            }

            else
            {
                COPanel_Forwarder.Show();

                switch (Console)
                {
                    case Console.GB:
                    case Console.GBC:
                    case Console.GBA:
                    case Console.S32X:
                    case Console.SMCD:
                    case Console.PSX:
                        CO = new Options_Forwarder(Console);
                        break;
                    case Console.NES:
                        break;
                    case Console.SNES:
                        break;
                    case Console.N64:
                        break;
                    case Console.SMS:
                        break;
                    case Console.SMD:
                        break;
                    case Console.PCE:
                        break;
                    case Console.PCECD:
                        break;
                    case Console.NEO:
                        break;
                    case Console.MSX:
                        break;
                    case Console.C64:
                        break;
                    case Console.Flash:
                        break;
                    case Console.RPGM:
                        break;
                    default:
                        break;
                }
            }

            if (CO != null)
            {
                CO.Font = Font;
                CO.Text = Program.Lang.String("injection_method_options", "projectform");
                CO.Icon = Icon.FromHandle(Properties.Resources.wrench.GetHicon());
            }

            if (!isVCMode && Manual != null)
            {
                Manual = null;
                manual_type_list.SelectedIndex = 0;
            }

            #region Toggle savedata panel
            LinkSaveData.Visible = SaveIcon_Panel.Visible = SaveDataTitle.Visible = ShowSaveData;
            label16.Visible = !ShowSaveData;
            #endregion

            #region Set size of content options panel
            if (groupBox3.MaximumSize.Height == 0) groupBox3.MaximumSize = groupBox3.Size;
            var selected = isVCMode ? COPanel_VC : Console != Console.Flash ? COPanel_Forwarder : null;
            int height = selected == null ? groupBox7.Height : selected.Location.Y + selected.Height + 11;
            groupBox3.Size = new Size(groupBox3.Width, height);
            MethodOptions.Enabled = CO != null;
            #endregion
        }
        #endregion

        private void ChannelTitle_Locale_CheckedChanged(object sender, EventArgs e)
        {
            if (ChannelTitle_Locale.Checked)
            {
                ChannelTitles titles = new ChannelTitles(ChannelTitle.Text) { Font = Font };
                if (titles.ShowDialog() == DialogResult.OK)
                {
                    Creator.ChannelTitles = new string[8]
                    {
                        titles.Japanese.Text,
                        titles.English.Text,
                        titles.German.Text,
                        titles.French.Text,
                        titles.Spanish.Text,
                        titles.Italian.Text,
                        titles.Dutch.Text,
                        titles.Korean.Text,
                    };

                    ChannelTitle.Text = Program.Lang.Current switch
                    {
                        "ja" => titles.Japanese.Text,
                        "ko" => titles.Korean.Text,
                        "nl" => titles.Dutch.Text,
                        "es" => titles.Spanish.Text,
                        "it" => titles.Italian.Text,
                        "fr" => titles.French.Text,
                        "de" => titles.German.Text,
                        _ => titles.English.Text
                    };
                }
            }

            else Creator.ChannelTitles = new string[8] { ChannelTitle.Text, ChannelTitle.Text, ChannelTitle.Text, ChannelTitle.Text, ChannelTitle.Text, ChannelTitle.Text, ChannelTitle.Text, ChannelTitle.Text };

            ChannelTitle.Enabled = !ChannelTitle_Locale.Checked;
        }

        private void CustomManual_CheckedChanged(object sender, EventArgs e)
        {
            if (manual_type_list.Enabled && manual_type_list.SelectedIndex == 2 && Manual == null)
            {
                if (!Properties.Settings.Default.donotshow_000) MessageBox.Show((sender as Control).Text, Program.Lang.Msg(6), 0);

                if (BrowseManual.ShowDialog() == DialogResult.OK) LoadManual(manual_type_list.SelectedIndex, BrowseManual.SelectedPath, true);
                else if (Manual != null) LoadManual(manual_type_list.SelectedIndex, null);
            }

            else if ((manual_type_list.SelectedIndex < 2 && Manual != null) || !manual_type_list.Enabled) LoadManual(0);

            CheckExport();
        }

        private void Patch_CheckedChanged(object sender, EventArgs e)
        {
            if (Patch.Checked && PatchFile == null)
            {
                if (BrowsePatch.ShowDialog() == DialogResult.OK)
                {
                    PatchFile = BrowsePatch.FileName;
                    CheckExport();
                }

                else
                {
                    if (!Patch.Checked && PatchFile != null)
                    {
                        PatchFile = null;
                        CheckExport();
                    }

                    Patch.Checked = false;
                }
            }

            else if (!Patch.Checked && PatchFile != null)
            {
                PatchFile = null;
                CheckExport();
            }
        }

        private void InjectorsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            ResetContentOptions();
            LoadImage();
            if (groupBox3.Enabled) CheckExport();
        }

        private void RegionsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadImage();
            if (groupBox4.Enabled) CheckExport();
        }

        private void ToggleSwitchChanged(object sender, EventArgs e)
        {
            if (sender == toggleSwitch1) toggleSwitchL1.Text = toggleSwitch1.Checked ? "vWii (Wii U)" : "Wii";
        }

        private Preview.Language BannerRegion()
        {
            var lang = isVCMode ? Preview.Language.Auto : GetRegionSetting() switch
            {
                libWiiSharp.Region.Japan => Preview.Language.Japanese,
                libWiiSharp.Region.Korea => Preview.Language.Korean,
                libWiiSharp.Region.Europe => Preview.Language.Europe,
                libWiiSharp.Region.USA => Preview.Language.America,
                _ => Preview.Language.Auto
            };

            if (lang == Preview.Language.Auto) lang = (int)Creator.OrigRegion switch { 1 => Preview.Language.Japanese, 2 => Preview.Language.Korean, 3 => Preview.Language.Europe, _ => Preview.Language.America };
            if (!isVCMode && Program.Lang.Current.StartsWith("ja")) lang = Preview.Language.Japanese;
            if (!isVCMode && Program.Lang.Current.StartsWith("ko")) lang = Preview.Language.Korean;

            if (lang != 0 && lang != Preview.Language.Europe && Console == Console.C64) lang = 0;
            else if (lang != Preview.Language.Japanese && Console == Console.MSX) lang = Preview.Language.Japanese;
            else if (lang == Preview.Language.Korean && Console == Console.SMD) lang = Preview.Language.Europe;
            else if (lang == Preview.Language.Korean && (int)Console >= 3) lang = 0;

            return lang;
        }

        private void ResetBannerPreview()
        {
            IconPreview.Image = Preview.Icon
            (
                Img?.IconVCPic,
                Console,
                BannerRegion(),
                true
            );

            BannerPreview.Image = Preview.Banner
            (
                Img?.VCPic,
                BannerTitle.Text,
                (int)ReleaseYear.Value,
                (int)Players.Value,
                Console,
                BannerRegion()
            );
        }

        private void ShowBannerPreview_Click(object sender, EventArgs e)
        {
            using (Form f = new Form())
            {
                f.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                f.ShowInTaskbar = false;
                f.Text = ShowBannerPreview.Text;
                f.Icon = Icon;

                var p = new PictureBox() { Name = "picture" };
                p.SizeMode = PictureBoxSizeMode.AutoSize;
                p.Location = new Point(0, 0);
                p.Image = Preview.Banner
                (
                    Img?.VCPic,
                    BannerTitle.Text,
                    (int)ReleaseYear.Value,
                    (int)Players.Value,
                    Console,
                    BannerRegion()
                );

                f.ClientSize = p.Image.Size;
                f.StartPosition = FormStartPosition.CenterParent;
                f.Controls.Add(p);
                f.ShowDialog();
            }
        }
    }
}
