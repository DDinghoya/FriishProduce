﻿using Ionic.Zip;
using libWiiSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Media;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FriishProduce
{
    public partial class ProjectForm : Form
    {
        protected Platform targetPlatform { get; set; }
        private BannerOptions banner_form;

        protected string TIDCode;
        protected string Untitled;
        protected string WADPath = null;

        protected bool isVirtualConsole { get => injection_methods.SelectedItem?.ToString().ToLower() == Program.Lang.String("vc").ToLower(); }
        protected bool showPatch = false;
        protected bool showSaveData
        {
            get => _showSaveData;
            set
            {
                _showSaveData = value;

                fill_save_data.Visible = SaveIcon_Panel.Visible = save_data_title.Visible = value;
                label16.Visible = !value;
            }
        }
        private bool _showSaveData;
        private bool _isShown;
        private bool _isMint;

        #region Public bools (for main form)
        public bool IsModified
        {
            get => _isModified;

            set
            {
                _isModified = value;
                if (value) _isMint = false;
                Program.MainForm.toolbarSave.Enabled = value;
                Program.MainForm.save_project.Enabled = value;
                Program.MainForm.toolbarSaveAs.Enabled = value;
                Program.MainForm.save_project_as.Enabled = value;
                Program.MainForm.toolbarExport.Enabled = IsExportable;
                Program.MainForm.export.Enabled = IsExportable;
            }
        }
        private bool _isModified;

        public bool IsEmpty
        {
            get => _isEmpty;

            set
            {
                _isEmpty = value;

                if (_isShown)
                {
                    title_id_random.Visible =
                    import_image.Enabled =
                    banner.Enabled =
                    groupBox2.Enabled =
                    groupBox3.Enabled =
                    groupBox4.Enabled =
                    groupBox5.Enabled =
                    groupBox6.Enabled =
                    groupBox7.Enabled =
                    groupBox8.Enabled =
                    groupBox9.Enabled =
                    groupBox10.Enabled = !value;

                    import_patch.Enabled = !value && showPatch;
                }
            }
        }
        private bool _isEmpty;

        public bool IsExportable
        {
            get
            {
                bool yes = !string.IsNullOrEmpty(_tID) && _tID.Length == 4
                            && !string.IsNullOrWhiteSpace(channel_title.Text)
                            && !string.IsNullOrEmpty(_bannerTitle)
                            && (img != null)
                            && rom?.FilePath != null
                            && ((use_online_wad.Checked) || (!use_online_wad.Checked && File.Exists(WADPath)));

                if (!File.Exists(WADPath) && !string.IsNullOrWhiteSpace(WADPath))
                {
                    WADPath = null;
                    refreshData();
                }

                return save_data_title.Visible ? yes && !string.IsNullOrEmpty(_saveDataTitle[0]) : yes;
            }
        }
        public bool IsForwarder { get => !isVirtualConsole && targetPlatform != Platform.Flash; }

        public string ProjectPath { get; set; }
        #endregion

        private new enum Region
        {
            America,
            Europe,
            Japan,
            Korea,
            Free,
            Orig
        };

        // -----------------------------------
        // Public variables
        // -----------------------------------
        protected ChannelDatabase channels { get; set; }
        protected (int baseNumber, int region) inWad { get; set; }
        protected string inWadFile { get; set; }
        private Region inWadRegion
        {
            get
            {
                for (int index = 0; index < channels.Entries[Base.SelectedIndex].Regions.Count; index++)
                    if (channels?.Entries[Base.SelectedIndex].GetUpperID(index)[3] == baseID.Text[3])
                        return channels?.Entries[Base.SelectedIndex].Regions[index] == 0 ? Region.Japan
                              : channels?.Entries[Base.SelectedIndex].Regions[index] == 6 || channels.Entries[Base.SelectedIndex].Regions[index] == 7 ? Region.Korea
                              : channels?.Entries[Base.SelectedIndex].Regions[index] >= 3 && channels.Entries[Base.SelectedIndex].Regions[index] <= 5 ? Region.Europe
                              : Region.America;

                throw new InvalidOperationException();
            }
        }

        private Project project;

        private WAD outWad;
        private libWiiSharp.Region outWadRegion
        {
            get => region_list.SelectedItem?.ToString() == Program.Lang.String("region_j") ? libWiiSharp.Region.Japan
                 : region_list.SelectedItem?.ToString() == Program.Lang.String("region_u") ? libWiiSharp.Region.USA
                 : region_list.SelectedItem?.ToString() == Program.Lang.String("region_e") ? libWiiSharp.Region.Europe
                 : region_list.SelectedItem?.ToString() == Program.Lang.String("region_k") ? libWiiSharp.Region.Korea
                 : region_list.SelectedIndex == 0 ? inWadRegion switch { Region.Japan => libWiiSharp.Region.Japan, Region.Korea => libWiiSharp.Region.Korea, Region.Europe => libWiiSharp.Region.Europe, Region.America => libWiiSharp.Region.USA, _ => libWiiSharp.Region.Free }
                 : libWiiSharp.Region.Free;
        }

        protected ROM rom { get; set; }
        protected string patch { get; set; }
        protected string manual { get; set; }
        protected string sound { get; set; }
        protected ImageHelper img { get; set; }

        private Preview preview = new();

        protected ContentOptions contentOptionsForm { get; set; }
        protected IDictionary<string, string> contentOptions { get => contentOptionsForm?.Options; }

        #region Channel/banner parameters
        private string _tID { get => title_id_upper.Text.ToUpper(); }
        private string[] _channelTitles { get => new string[8] { channel_title.Text, channel_title.Text, channel_title.Text, channel_title.Text, channel_title.Text, channel_title.Text, channel_title.Text, channel_title.Text }; } // "無題", "Untitled", "Ohne Titel", "Sans titre", "Sin título", "Senza titolo", "Onbekend", "제목 없음"
        private string _bannerTitle { get => banner_form.title.Text; }
        private int _bannerYear { get => (int)banner_form.released.Value; }
        private int _bannerPlayers { get => (int)banner_form.players.Value; }
        private string[] _saveDataTitle { get => save_data_title.Lines.Length == 1 ? new string[] { save_data_title.Text } : save_data_title.Lines.Length == 0 ? new string[] { "" } : save_data_title.Lines; }
        private int _bannerRegion
        {
            get
            {
                int lang = banner_form.region.SelectedIndex;

                if (lang == 0)
                {
                    lang = channels != null ? inWadRegion switch { Region.Japan => 1, Region.Europe => 3, Region.Korea => 4, _ => 2 } : 2;

                    if (!isVirtualConsole && Program.Lang.Current.StartsWith("ja"))
                        lang = 1;

                    if (!isVirtualConsole && Program.Lang.Current.StartsWith("ko"))
                        lang = 4;
                }

                // Japan/Korea: Use USA banner for C64
                if (lang != 2 && lang != 3 && targetPlatform == Platform.C64)
                    lang = 0;

                // International: Use Japan banner for MSX
                else if (lang != 1 && targetPlatform == Platform.MSX)
                    lang = 1;

                // Korea: Use Europe banner for SMD
                else if (lang == 4 && targetPlatform == Platform.SMD)
                    lang = 3;

                // Korea: Use USA banner for non-available platforms
                else if (lang == 4 && (int)targetPlatform >= 3)
                    lang = 0;

                return lang switch { 4 => 2, 2 => 0, _ => lang }; // Korea and USA need to be swapped for the right value for the banner preview function
            }
        }
        #endregion


        // -----------------------------------

        public void SaveProject(string path)
        {
            ProjectPath = path;

            var p = new Project()
            {
                ProjectPath = path,

                Platform = targetPlatform,

                ROM = rom?.FilePath,
                Patch = patch,
                Manual = (manual_type.SelectedIndex, manual),
                Img = (image_filename.Text == Program.Lang.String("image_supplied", Name) ? "[s]" : img?.FilePath ?? null, img?.Source ?? null),
                ImageOptions = (image_interpolation_mode.SelectedIndex, image_resize.SelectedIndex == 1),
                Sound = sound,

                InjectionMethod = injection_methods.SelectedIndex,
                ForwarderStorageDevice = forwarder_root_device.SelectedIndex,
                ContentOptions = contentOptions ?? null,

                LinkSaveDataTitle = fill_save_data.Checked,
                VideoMode = video_modes.SelectedIndex,

                TitleID = _tID,
                ChannelTitles = _channelTitles,
                SaveDataTitle = _saveDataTitle,
                BannerTitle = _bannerTitle,
                BannerYear = _bannerYear,
                BannerPlayers = _bannerPlayers,

                WADRegion = region_list.SelectedIndex,
            };

            p.BaseFile = WADPath;
            p.BaseOnline = (use_online_wad.Checked, Base.SelectedIndex, 0);

            for (int i = 0; i < baseRegionList.Items.Count; i++)
                if (baseRegionList.Items[i].GetType() == typeof(ToolStripMenuItem) && (baseRegionList.Items[i] as ToolStripMenuItem).Checked) p.BaseOnline = (use_online_wad.Checked, Base.SelectedIndex, i);

            using (Stream stream = File.Open(path, FileMode.Create))
            {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                binaryFormatter.Serialize(stream, p);
            }

            IsModified = false;
            _isMint = true;
        }

        public void RefreshForm()
        {
            // ----------------------------
            if (DesignMode) return;
            // ----------------------------

            bool isMint = _isMint || !Program.MainForm.save_project_as.Enabled;

            tab_main.Checked    = true;
            tab_channel.Checked = false;
            tab1.Visible        = true;
            tab2.Visible        = false;
            tab1.BackColor = tab2.BackColor = tab_main.FlatAppearance.CheckedBackColor;

            #region Localization
            Program.Lang.Control(this, "projectform");
            groupBox4.Text = groupBox4.Text.TrimEnd(':').Trim();
            groupBox7.Text = Program.Lang.String("banner", "banner");
            groupBox9.Text = title_id.Text.TrimEnd(':').Trim();
            import_rom.Text = string.Format(Program.Lang.String(import_rom.Name, Name), targetPlatform switch
            {
                Platform.PSX => Program.Lang.String("disc_image", Name),
                Platform.GCN => Program.Lang.String("disc_image", Name),
                Platform.PCECD => Program.Lang.String("disc_image", Name),
                Platform.SMCD => Program.Lang.String("disc_image", Name),
                Platform.RPGM => Program.Lang.String("game_file", Name),
                Platform.NEO => "ZIP",
                Platform.Flash => "SWF",
                _ => "ROM",
            });
            browsePatch.Filter = Program.Lang.String("filter.patch");
            // BrowseManualZIP.Filter = Program.Lang.String("filter.zip");

            // Banner menu
            banner_customize.Text = Program.Lang.String(banner_customize.Name, "projectform");
            banner_sound.Text = Program.Lang.String(banner_sound.Name, "projectform");
            play_banner_sound.Text = Program.Lang.String(play_banner_sound.Name, "projectform");
            replace_banner_sound.Text = Program.Lang.String(replace_banner_sound.Name, "projectform");
            restore_banner_sound.Text = Program.Lang.String(restore_banner_sound.Name, "projectform");

            // Change title text to untitled string
            Untitled = string.Format(Program.Lang.String("untitled_project", "mainform"), Program.Lang.String(Enum.GetName(typeof(Platform), targetPlatform).ToLower(), "platforms"));
            Text = string.IsNullOrWhiteSpace(channel_title.Text) ? Untitled : channel_title.Text;

            setFilesText();

            var baseMax = Math.Max(base_name.Location.X + base_name.Width - 4, title_id.Location.X + title_id.Width - 4) + 2;
            baseName.Location = new Point(baseMax, base_name.Location.Y);
            baseID.Location = new Point(baseMax, title_id.Location.Y);

            // Selected index properties
            Program.Lang.Control(image_interpolation_mode, Name);
            image_interpolation_mode.SelectedIndex = Properties.Settings.Default.image_interpolation;

            // Manual
            manual_type.SelectedIndex = 0;
            manual = null;

            // Regions lists
            region_list.Items.Clear();
            region_list.Items.Add(Program.Lang.String("original"));
            region_list.Items.Add(Program.Lang.String("region_rf"));
            region_list.SelectedIndex = 0;

            // Video modes
            video_modes.Items[0] = Program.Lang.String("original");
            video_modes.SelectedIndex = 0;

            switch (Program.Lang.Current.ToLower())
            {
                default:
                    region_list.Items.Add(Program.Lang.String("region_u"));
                    region_list.Items.Add(Program.Lang.String("region_e"));
                    region_list.Items.Add(Program.Lang.String("region_j"));
                    region_list.Items.Add(Program.Lang.String("region_k"));
                    break;

                case "ja":
                    region_list.Items.Add(Program.Lang.String("region_j"));
                    region_list.Items.Add(Program.Lang.String("region_u"));
                    region_list.Items.Add(Program.Lang.String("region_e"));
                    region_list.Items.Add(Program.Lang.String("region_k"));
                    break;

                case "ko":
                    region_list.Items.Add(Program.Lang.String("region_k"));
                    region_list.Items.Add(Program.Lang.String("region_u"));
                    region_list.Items.Add(Program.Lang.String("region_e"));
                    region_list.Items.Add(Program.Lang.String("region_j"));
                    break;
            }

            toolTip = new ToolTip();
            toolTip.SetToolTip(banner, Program.Lang.String("banner", Name));
            
            #endregion

            baseName.Font = new Font(baseName.Font, FontStyle.Bold);

            if (Base.SelectedIndex >= 0)
                for (int i = 0; i < channels.Entries[Base.SelectedIndex].Regions.Count; i++)
                {
                    baseRegionList.Items[i].Text = channels.Entries[Base.SelectedIndex].Regions[i] switch
                    {
                        1 or 2      => Program.Lang.String("region_u"),
                        3 or 4 or 5 => Program.Lang.String("region_e"),
                        6 or 7      => Program.Lang.String("region_k"),
                        _           => Program.Lang.String("region_j"),
                    };
                }


            for (int i = 0; i < Base.Items.Count; i++)
            {
                var title = channels.Entries[i].Regions.Contains(0) && Program.Lang.Current.ToLower().StartsWith("ja") ? channels.Entries[i].Titles[0]
                          : channels.Entries[i].Regions.Contains(0) && Program.Lang.Current.ToLower().StartsWith("ko") ? channels.Entries[i].Titles[channels.Entries[i].Titles.Count - 1]
                          : channels.Entries[i].Regions.Contains(0) && channels.Entries[i].Regions.Count > 1 ? channels.Entries[i].Titles[1]
                          : channels.Entries[i].Titles[0];

                Base.Items[i] = title;
            }

            // Injection methods list
            injection_methods.Items.Clear();

            switch (targetPlatform)
            {
                case Platform.NES:
                    injection_methods.Items.Add(Program.Lang.String("vc"));
                    injection_methods.Items.Add(Forwarder.List[0].Name);
                    injection_methods.Items.Add(Forwarder.List[1].Name);
                    injection_methods.Items.Add(Forwarder.List[2].Name);
                    break;

                case Platform.SNES:
                    injection_methods.Items.Add(Program.Lang.String("vc"));
                    injection_methods.Items.Add(Forwarder.List[3].Name);
                    injection_methods.Items.Add(Forwarder.List[4].Name);
                    injection_methods.Items.Add(Forwarder.List[5].Name);
                    break;

                case Platform.N64:
                    injection_methods.Items.Add(Program.Lang.String("vc"));
                    injection_methods.Items.Add(Forwarder.List[8].Name);
                    injection_methods.Items.Add(Forwarder.List[9].Name);
                    injection_methods.Items.Add(Forwarder.List[10].Name);
                    injection_methods.Items.Add(Forwarder.List[11].Name);
                    break;

                case Platform.SMS:
                case Platform.SMD:
                    injection_methods.Items.Add(Program.Lang.String("vc"));
                    injection_methods.Items.Add(Forwarder.List[7].Name);
                    break;

                case Platform.PCE:
                case Platform.NEO:
                case Platform.MSX:
                case Platform.C64:
                    injection_methods.Items.Add(Program.Lang.String("vc"));
                    break;

                case Platform.Flash:
                    injection_methods.Items.Add(Program.Lang.String("by_default"));
                    break;

                case Platform.GBA:
                    injection_methods.Items.Add(Forwarder.List[6].Name);
                    break;

                case Platform.PSX:
                    injection_methods.Items.Add(Forwarder.List[12].Name);
                    break;

                case Platform.RPGM:
                    injection_methods.Items.Add(Forwarder.List[13].Name);
                    break;

                default:
                    break;
            }

            injection_methods.SelectedIndex = targetPlatform switch
            {
                Platform.NES                 => Properties.Settings.Default.default_injection_method_nes,
                Platform.SNES                => Properties.Settings.Default.default_injection_method_snes,
                Platform.N64                 => Properties.Settings.Default.default_injection_method_n64,
                Platform.SMS or Platform.SMD => Properties.Settings.Default.default_injection_method_sega,
                _ => 0
            };
            injection_methods.Enabled = injection_methods.Items.Count > 1;
            groupBox3.Enabled = injection_methods.Enabled && !IsEmpty;
            banner_form.released.Maximum = DateTime.Now.Year;

            image_resize.SelectedIndex = Properties.Settings.Default.image_fit_aspect_ratio ? 1 : 0;
            resetImages();
            if (isMint && IsModified) IsModified = false;
        }

        private void LoadChannelDatabase()
        {
            try { channels = new ChannelDatabase(targetPlatform); }
            catch (Exception ex)
            {
                if ((int)targetPlatform < 10)
                {
                    System.Windows.Forms.MessageBox.Show($"A fatal error occurred retrieving the {targetPlatform} WADs database.\n\nException: {ex.GetType().FullName}\nMessage: {ex.Message}\n\nThe application will now shut down.", "Halt", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    Environment.FailFast("Database initialization failed.");
                }
                else { channels = new ChannelDatabase(); }
            }
        }

        public ProjectForm(Platform platform, string ROMpath = null, Project project = null)
        {
            targetPlatform = platform;
            IsEmpty = true;
            banner_form = new BannerOptions(platform);
            LoadChannelDatabase();

            InitializeComponent();
            AddBases();
            _isShown = true;

            if (project != null && ROMpath == null)
                this.project = project;

            if (ROMpath != null && project == null)
            {
                rom.FilePath = ROMpath;
                LoadROM(rom.FilePath, Properties.Settings.Default.auto_retrieve_game_data);
            }
        }

        private void Form_Shown(object sender, EventArgs e)
        {
            // ----------------------------
            if (DesignMode) return;
            // ----------------------------

            // Declare WAD metadata modifier
            // ********
            TIDCode = null;

            using (var icon = new Bitmap(Program.MainForm.Icons[targetPlatform]))
            {
                icon.MakeTransparent(Color.White);
                Icon = Icon.FromHandle(icon.GetHicon());
            }

            switch (targetPlatform)
            {
                case Platform.NES:
                    TIDCode = "F";
                    rom = new ROM_NES();
                    showPatch = true;
                    break;

                case Platform.SNES:
                    TIDCode = "J";
                    rom = new ROM_SNES();
                    showPatch = true;
                    break;

                case Platform.N64:
                    TIDCode = "N";
                    rom = new ROM_N64();
                    showPatch = true;
                    break;

                case Platform.SMS:
                    TIDCode = "L";
                    rom = new ROM_SEGA() { IsSMS = true };
                    showPatch = true;
                    break;

                case Platform.SMD:
                    TIDCode = "M";
                    rom = new ROM_SEGA() { IsSMS = false };
                    showPatch = true;
                    break;

                case Platform.PCE:
                    TIDCode = "P";
                    rom = new ROM_PCE();
                    showPatch = true;
                    break;

                case Platform.PCECD:
                    TIDCode = "Q";
                    rom = new Disc();
                    break;

                case Platform.NEO:
                    TIDCode = "E";
                    rom = new ROM_NEO();
                    break;

                case Platform.MSX:
                    TIDCode = "X";
                    rom = new ROM_MSX();
                    showPatch = true;
                    break;

                case Platform.Flash:
                    rom = new SWF();
                    break;

                case Platform.RPGM:
                    rom = new RPGM();
                    banner_form.players.Enabled = false;
                    break;

                default:
                    rom = new Disc();
                    break;
            }

            // Cosmetic
            // ********
            RefreshForm();

            manual_type.Enabled = false;
            foreach (var manualConsole in new List<Platform>() // Confirmed to have an algorithm exist for NES, SNES, N64, SEGA, PCE, NEO
            {
                Platform.NES,
                Platform.SNES,
                Platform.N64,
                Platform.SMS,
                Platform.SMD,
                // Platform.PCE,
                // Platform.NEO
            })
                if (targetPlatform == manualConsole) manual_type.Enabled = true;

            // *****************************************************
            // LOADING PROJECT
            // *****************************************************
            if (project != null)
            {
                ProjectPath = project.ProjectPath;

                video_modes.SelectedIndex = project.VideoMode;

                img = new ImageHelper(project.Platform, null);
                if (project.Img.File == "[s]")
                {
                    image_filename.Text = Program.Lang.String("image_supplied", Name);
                    image_filename.Enabled = false;
                }
                else if (File.Exists(project.Img.File))
                {
                    image_filename.Text = project.Img.File;
                    image_filename.Enabled = true;
                }
                img.LoadToSource(project.Img.Bmp);
                LoadROM(project.ROM, false);

                if (!project.BaseOnline.Enabled)
                {
                    use_offline_wad.Checked = true;
                    WADPath = project.BaseFile;
                    if (File.Exists(project.BaseFile)) LoadWAD(project.BaseFile);
                }
                else
                {
                    use_online_wad.Checked = true;
                    Base.SelectedIndex = project.BaseOnline.Index;
                    for (int i = 0; i < baseRegionList.Items.Count; i++)
                        if (baseRegionList.Items[i].GetType() == typeof(ToolStripMenuItem)) (baseRegionList.Items[i] as ToolStripMenuItem).Checked = false;
                    UpdateBaseForm(project.BaseOnline.Region);
                    (baseRegionList.Items[project.BaseOnline.Region] as ToolStripMenuItem).Checked = true;
                }

                patch = File.Exists(project.Patch) ? project.Patch : null;

                channel_title.Text = project.ChannelTitles[1];
                banner_form.title.Text = project.BannerTitle;
                banner_form.released.Value = project.BannerYear;
                banner_form.players.Value = project.BannerPlayers;
                save_data_title.Lines = project.SaveDataTitle;
                title_id_upper.Text = project.TitleID;

                region_list.SelectedIndex = project.WADRegion;
                injection_methods.SelectedIndex = project.InjectionMethod;
                image_interpolation_mode.SelectedIndex = project.ImageOptions.Item1;
                image_resize.SelectedIndex = project.ImageOptions.Item2 ? 1 : 0;

                if (contentOptionsForm != null) contentOptionsForm.Options = project.ContentOptions;
                LoadImage();
                LoadManual(project.Manual.Type, project.Manual.File);
                LoadSound(project.Sound);
                setFilesText();
            }

            fill_save_data.Checked = project == null ? Properties.Settings.Default.auto_fill_save_data : project.LinkSaveDataTitle;

            int forwarderStorageDevice = project == null ? Options.FORWARDER.Default.root_storage_device : project.ForwarderStorageDevice;
            forwarder_root_device.SelectedIndex = forwarderStorageDevice;

            IsEmpty = project == null;
            IsModified = false;
            _isMint = true;

            // Error messages for not found files
            // ********
            if (project != null)
                foreach (var item in new string[] { project.ROM, project.Patch, project.BaseFile, project.Sound })
                    if (!File.Exists(item) && !string.IsNullOrWhiteSpace(item)) MessageBox.Show(string.Format(Program.Lang.Msg(10, true), Path.GetFileName(item)));
            project = null;

            use_online_wad.Enabled = Properties.Settings.Default.use_online_wad_enabled;
            if (use_online_wad.Checked && !use_online_wad.Enabled) use_online_wad.Checked = false;
            if (!use_offline_wad.Checked && !use_online_wad.Checked) use_offline_wad.Checked = true;
        }

        // -----------------------------------

        public void BrowseROMDialog()
        {
            browseROM.Title = import_rom.Text;

            switch (targetPlatform)
            {
                default:
                    browseROM.Filter = Program.Lang.String("filter.disc") + "|" + Program.Lang.String("filter.zip") + Program.Lang.String("filter");
                    break;

                case Platform.NES:
                case Platform.SNES:
                case Platform.N64:
                case Platform.SMS:
                case Platform.SMD:
                case Platform.PCE:
                case Platform.C64:
                case Platform.MSX:
                    browseROM.Filter = Program.Lang.String($"filter.rom_{targetPlatform.ToString().ToLower()}");
                    break;

                case Platform.NEO:
                    browseROM.Filter = Program.Lang.String("filter.zip");
                    break;

                case Platform.Flash:
                    browseROM.Filter = Program.Lang.String("filter.swf");
                    break;

                case Platform.RPGM:
                    browseROM.Filter = Program.Lang.String("filter.rpgm");
                    break;
            }

            if (browseROM.ShowDialog() == DialogResult.OK)
            {
                IsEmpty = false;
                LoadROM(browseROM.FileName, Properties.Settings.Default.auto_retrieve_game_data);
            }
        }

        private void import_rom_Click(object sender, EventArgs e) => BrowseROMDialog();

        public void BrowseImageDialog()
        {
            browseImage.Title = import_image.Text;
            browseImage.Filter = Program.Lang.String("filter.img");

            if (browseImage.ShowDialog() == DialogResult.OK) LoadImage(browseImage.FileName);
        }

        private void import_image_Click(object sender, EventArgs e) => BrowseImageDialog();

        private void refreshData()
        {
            // ----------------------------
            if (DesignMode) return;
            // ----------------------------

            if (!IsEmpty)
            {
                IsModified = true;
            }

            setFilesText();
        }

        public bool[] ToolbarButtons
        {
            get => new bool[]
            {
                targetPlatform != Platform.Flash
                && targetPlatform != Platform.RPGM
                && rom?.FilePath != null, // LibRetro / game data

                targetPlatform != Platform.Flash
                && targetPlatform != Platform.RPGM
                && isVirtualConsole, // Browse manual
            };
        }

        private void setFilesText()
        {
            int maxLength = 75;

            bool hasRom = !string.IsNullOrWhiteSpace(rom?.FilePath);
            bool hasPatch = showPatch && !string.IsNullOrWhiteSpace(patch);
            bool hasImage = img?.Source != null;
            bool hasWad = !string.IsNullOrWhiteSpace(WADPath);

            rom_filename.Text = hasRom ? Path.GetFileName(rom.FilePath) : Program.Lang.String("none");
            patch_filename.Text = !showPatch ? Program.Lang.String("not_available") : hasPatch ? Path.GetFileName(patch) : Program.Lang.String("none");
            if (!hasImage) image_filename.Text = Program.Lang.String("none");
            wad_filename.Text = hasWad ? Path.GetFileName(WADPath) : Program.Lang.String("none");

            if (rom_filename.Text.Length > maxLength) rom_filename.Text = rom_filename.Text.Substring(0, maxLength - 3) + "...";
            if (patch_filename.Text.Length > maxLength) patch_filename.Text = patch_filename.Text.Substring(0, maxLength - 3) + "...";
            if (image_filename.Text.Length > maxLength) image_filename.Text = image_filename.Text.Substring(0, maxLength - 3) + "...";
            if (wad_filename.Text.Length > maxLength) wad_filename.Text = wad_filename.Text.Substring(0, maxLength - 3) + "...";

            rom_filename.Enabled = hasRom;
            patch_filename.Enabled = hasPatch;
            if (!hasImage) image_filename.Enabled = false;
            wad_filename.Enabled = hasWad;

            checkImg1.Image = hasRom ? Properties.Resources.yes : Properties.Resources.no;
            checkImg2.Image = hasImage ? Properties.Resources.yes : Properties.Resources.no;
            checkImg3.Image = hasWad ? Properties.Resources.yes : Properties.Resources.no;
        }

        private void randomTID()
        {
            title_id_upper.Text = TIDCode != null ? TIDCode + GenerateTitleID().Substring(0, 3) : GenerateTitleID();
            refreshData();
        }

        public string GetName(bool full)
        {
            string FILENAME = File.Exists(patch) ? Path.GetFileNameWithoutExtension(patch) : Path.GetFileNameWithoutExtension(rom?.FilePath);
            string CHANNELNAME = channel_title.Text;
            string FULLNAME = System.Text.RegularExpressions.Regex.Replace(_bannerTitle.Replace(": ", Environment.NewLine).Replace(" - ", Environment.NewLine), @"\((.*?)\)", "").Replace("\r\n", "\n").Replace("\n", " - ");
            string TITLEID = title_id_upper.Text.ToUpper();
            string PLATFORM = targetPlatform.ToString();

            string target = full ? Properties.Settings.Default.default_export_filename : Properties.Settings.Default.default_save_as_filename;

            return target.Replace("FILENAME", FILENAME).Replace("CHANNELNAME", CHANNELNAME).Replace("FULLNAME", FULLNAME).Replace("TITLEID", TITLEID).Replace("PLATFORM", PLATFORM);
        }

        private void isClosing(object sender, FormClosingEventArgs e)
        {
            // ----------------------------
            if (DesignMode) return;
            // ----------------------------

            e.Cancel = !CheckUnsaved();

            if (!e.Cancel)
            {
                rom = null;
                channels = null;

                if (img != null) img.Dispose();
                img = null;

                if (contentOptionsForm != null) contentOptionsForm.Dispose();
                contentOptionsForm = null;

                preview.Dispose();
                preview = null;
            }
        }

        public bool CheckUnsaved()
        {
            if (IsModified)
            {
                var result = MessageBox.Show(string.Format(Program.Lang.Msg(1), Text), null, new string[] { Program.Lang.String("b_save"), Program.Lang.String("b_dont_save"), Program.Lang.String("b_cancel") });
                {
                    if (result == MessageBox.Result.Button1)
                    {
                        return Program.MainForm.SaveAs_Trigger();
                    }

                    else if (result == MessageBox.Result.Button2)
                    {
                        IsModified = false;
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

        private void Random_Click(object sender, EventArgs e) => randomTID();

        private void Value_Changed(object sender, EventArgs e) => refreshData();

        private void linkSaveDataTitle()
        {
            if (fill_save_data.Checked && fill_save_data.Enabled && fill_save_data.Visible)
            {
                string[] lines = new string[2];
                int limit = save_data_title.Multiline ? save_data_title.MaxLength / 2 : save_data_title.MaxLength;
                if (channel_title.TextLength <= limit) lines[0] = channel_title.Text;
                if (banner_form.title.Lines.Length > 1 && banner_form.title.Lines[1].Length <= limit) lines[1] = banner_form.title.Lines[1];

                if (string.Join("\n", lines).Length > save_data_title.MaxLength) return;

                save_data_title.Text
                    = string.IsNullOrWhiteSpace(lines[1]) ? lines[0]
                    : string.IsNullOrWhiteSpace(lines[0]) ? lines[1]
                    : string.IsNullOrWhiteSpace(lines[0]) && string.IsNullOrWhiteSpace(lines[1]) ? null
                    : save_data_title.Multiline ? string.Join(Environment.NewLine, lines) : lines[0];
            }

            refreshData();
        }

        private void LinkSaveData_Changed(object sender, EventArgs e)
        {
            if (sender == fill_save_data)
            {
                save_data_title.Enabled = !fill_save_data.Checked;
                linkSaveDataTitle();
            }
        }

        private void TextBox_Changed(object sender, EventArgs e)
        {
            if (sender == channel_title)
            {
                Text = string.IsNullOrWhiteSpace(channel_title.Text) ? Untitled : channel_title.Text;
                linkSaveDataTitle();
            }

            var currentSender = sender as TextBox;
            if (currentSender.Multiline && currentSender.Lines.Length > 2) currentSender.Lines = new string[] { currentSender.Lines[0], currentSender.Lines[1] };

            refreshData();
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
            SystemSounds.Beep.Play();
            e.Handled = true;
        }

        private void OpenWAD_CheckedChanged(object sender, EventArgs e)
        {
            // ----------------------------
            if (DesignMode) return;
            // ----------------------------

            use_offline_wad.Checked = !use_online_wad.Checked;
            base_name.Enabled = title_id.Enabled = baseID.Enabled = baseName.Enabled = Base.Enabled = BaseRegion.Enabled = use_online_wad.Checked;
            checkImg3.Visible = import_wad.Enabled = use_offline_wad.Checked;

            if (Base.Enabled)
            {
                WADPath = null;
                AddBases();
            }
            else
            {
                BaseRegion.Image = null;
                if (Base.Items.Count > 0) Base.SelectedIndex = 0;
            }

            refreshData();
        }

        private void import_wad_Click(object sender, EventArgs e)
        {
            browseInputWad.Title = import_wad.Text;
            browseInputWad.Filter = Program.Lang.String("filter.wad");
            var result = browseInputWad.ShowDialog();

            if (result == DialogResult.OK)
            {
                LoadWAD(browseInputWad.FileName);
                refreshData();
            }
        }

        private void InterpolationChanged(object sender, EventArgs e)
        {
            // ----------------------------
            if (DesignMode) return;
            // ----------------------------

            if (image_interpolation_mode.SelectedIndex != Properties.Settings.Default.image_interpolation) refreshData();
            LoadImage();
        }

        private void SwitchAspectRatio(object sender, EventArgs e)
        {
            // ----------------------------
            if (DesignMode) return;
            // ----------------------------

            if (sender == image_resize)
            {
                LoadImage();
            }

            if (sender == forwarder_root_device)
            {
                refreshData();
            }
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
            WAD Reader = new();

            try
            {
                if (Directory.Exists(Paths.WAD)) Directory.Delete(Paths.WAD, true);
                Reader = WAD.Load(path);
            }
            catch
            {
                goto Failed;
            }

            for (int h = 0; h < channels.Entries.Count; h++)
                for (int i = 0; i < channels.Entries[h].Regions.Count; i++)
                    if (channels.Entries[h].GetUpperID(i) == Reader.UpperTitleID.ToUpper())
                    {
                        WADPath = path;

                        Base.SelectedIndex = h;
                        UpdateBaseForm(i);
                        Reader.Dispose();
                        return true;
                    }

            Failed:
            Reader.Dispose();
            SystemSounds.Beep.Play();
            MessageBox.Show(string.Format(Program.Lang.Msg(5), Reader.UpperTitleID));
            WADPath = null;
            return false;
        }

        protected void LoadManual(int index, string path = null, bool isFolder = true)
        {
            bool failed = false;

            #region Load manual as ZIP file (if exists)
            if (File.Exists(path) && !isFolder)
            {
                int applicable = 0;
                // bool hasFolder = false;

                using ZipFile ZIP = ZipFile.Read(path);
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
                      || item.FileName == "vsscript.css")
                        applicable++;
                }

                if (applicable >= 2 /* && hasFolder */)
                {
                    manual = path;
                    goto End;
                }

                failed = true;
            }
            #endregion

            #region Load manual as folder/directory (if exists)
            else if (Directory.Exists(path) && isFolder)
            {
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
                    manual = path;
                    goto End;
                }

                failed = true;
            }
            #endregion

            else
            {
                manual = null;
                goto End;
            }

            End:
            if (failed)
            {
                MessageBox.Show(Program.Lang.Msg(7), MessageBox.Buttons.Ok, MessageBox.Icons.Warning);
                manual = null;
            }

            manual_type.SelectedIndex = manual == null && index >= 2 ? 0 : manual != null && index < 2 ? 2 : index;
        }

        protected void LoadImage()
        {
            if (img != null) LoadImage(img.Source);
            else refreshData();
        }

        protected void LoadImage(string path)
        {
            image_filename.Text = path;
            image_filename.Enabled = true;
            img = new ImageHelper(targetPlatform, path);
            LoadImage(img.Source);
        }

        #region /////////////////////////////////////////////// Inheritable functions ///////////////////////////////////////////////
        /// <summary>
        /// Additionally edit image before generating files, e.g. with modification of image palette/brightness, used only for images with exact resolution of original screen size
        /// </summary>
        // protected abstract void platformImageFunction(Bitmap src);

        protected void platformImageFunction(Bitmap src)
        {
            Bitmap bmp = null;

            switch (targetPlatform)
            {
                case Platform.NES:
                    bmp = cloneImage(src);
                    if (bmp == null) return;

                    if (contentOptions != null && bool.Parse(contentOptions.ElementAt(1).Value))
                    {
                        var contentOptionsNES = contentOptionsForm as Options_VC_NES;
                        var palette = contentOptionsNES.CheckPalette(src);

                        if (palette != -1 && src.Width == 256 && (src.Height == 224 || src.Height == 240))
                            bmp = contentOptionsNES.SwapColors(bmp, contentOptionsNES.Palettes[palette], contentOptionsNES.Palettes[int.Parse(contentOptions.ElementAt(0).Value)]);
                    }
                    else bmp = src;
                    break;

                case Platform.SMS:
                case Platform.SMD:
                    break;
            }

            img.Generate(bmp ?? src);
        }

        private Bitmap cloneImage(Bitmap src)
        {
            try { return (Bitmap)src.Clone(); } catch { try { return (Bitmap)img?.Source.Clone(); } catch { return null; } }
        }
        #endregion

        private void resetImages(bool bannerOnly = false)
        {
            banner.Image = preview.Banner
                (
                    img?.VCPic,
                    banner_form.title.Text,
                    (int)banner_form.released.Value,
                    (int)banner_form.players.Value,
                    targetPlatform,
                    _bannerRegion
                );

            if (!bannerOnly)
            {
                SaveIcon_Panel.Image = img?.SaveIcon();
            }
        }

        protected bool LoadImage(Bitmap src)
        {
            if (src == null) return false;

            try
            {
                img.InterpMode = (InterpolationMode)image_interpolation_mode.SelectedIndex;
                img.FitAspectRatio = image_resize.SelectedIndex == 1;

                platformImageFunction(src);

                if (img.Source != null)
                {
                    resetImages();
                    refreshData();
                }

                return true;
            }

            catch
            {
                MessageBox.Show(Program.Lang.Msg(1, true));
                return false;
            }
        }

        protected void LoadROM(string ROMpath, bool LoadGameData = true)
        {
            if (ROMpath == null || rom == null || !File.Exists(ROMpath)) return;

            switch (targetPlatform)
            {
                // ROM file formats
                // ****************
                default:
                    if (!rom.CheckValidity(ROMpath))
                    {
                        MessageBox.Show(Program.Lang.Msg(2), 0, MessageBox.Icons.Warning);
                        return;
                    }
                    break;

                // ZIP format
                // ****************
                case Platform.NEO:
                    if (!rom.CheckZIPValidity(new string[] { "c1", "c2", "m1", "p1", "s1", "v1" }, true, true, ROMpath))
                    {
                        MessageBox.Show(Program.Lang.Msg(2), 0, MessageBox.Icons.Warning);
                        return;
                    }
                    break;

                // Disc format
                // ****************
                case Platform.PSX:
                    break;

                // RPG Maker format
                // ****************
                case Platform.RPGM:
                    if ((rom as RPGM).GetTitle(ROMpath) != null)
                    {
                        banner_form.title.Text = (rom as RPGM).GetTitle(ROMpath);
                        if (_bannerTitle.Length <= channel_title.MaxLength) channel_title.Text = banner_form.title.Text;
                        resetImages(true);
                    }
                    break;

                // Other, no verification needed
                // ****************
                case Platform.Flash:
                    break;
            }

            rom.FilePath = ROMpath;

            randomTID();
            patch = null;

            Program.MainForm.toolbarRetrieveGameData.Enabled = Program.MainForm.retrieve_gamedata_online.Enabled = ToolbarButtons[0];
            if (rom != null && LoadGameData && ToolbarButtons[0]) this.LoadGameData();
            setFilesText();
        }

        public async void LoadGameData()
        {
            if (rom == null || rom.FilePath == null) return;

            try
            {
                var gameData = await Task.FromResult(rom.GetData(targetPlatform, rom.FilePath));
                bool retrieved = gameData != (null, null, null, null, null);

                if (retrieved)
                {
                    // Set banner title
                    banner_form.title.Text = rom.CleanTitle ?? banner_form.title.Text;

                    // Set channel title text
                    if (rom.CleanTitle != null)
                    {
                        var text = rom.CleanTitle.Replace("\r", "").Split('\n');
                        if (text[0].Length <= channel_title.MaxLength) { channel_title.Text = text[0]; }
                    }

                    // Set image
                    if (gameData.Image != null)
                    {
                        LoadImage(gameData.Image);
                        image_filename.Text = Program.Lang.String("image_supplied", Name);
                        image_filename.Enabled = false;
                    }

                    // Set year and players
                    banner_form.released.Value = !string.IsNullOrEmpty(gameData.Year) ? int.Parse(gameData.Year) : banner_form.released.Value;
                    banner_form.players.Value = !string.IsNullOrEmpty(gameData.Players) ? int.Parse(gameData.Players) : banner_form.players.Value;
                    
                    resetImages(true);
                }

                if (retrieved && fill_save_data.Checked) linkSaveDataTitle();
                else if (rom.CleanTitle != null && channel_title.TextLength <= save_data_title.MaxLength) save_data_title.Text = channel_title.Text;

                // Show message if partially failed to retrieve data
                if (retrieved && (gameData.Title == null || gameData.Players == null || gameData.Year == null || gameData.Image == null))
                    MessageBox.Show(Program.Lang.Msg(4));
                else if (!retrieved) SystemSounds.Beep.Play();
            }
            catch (Exception ex)
            {
                MessageBox.Error(ex.Message);
            }
        }

        public bool SaveToWAD(string targetFile = null)
        {
            if (targetFile == null) targetFile = Paths.WorkingFolder + "out.wad";

            try
            {
                Program.MainForm.CleanTemp();

                // Get WAD data
                // *******
                outWad = new WAD();
                if (WADPath != null) outWad = WAD.Load(WADPath);
                else foreach (var entry in channels.Entries)
                        for (int i = 0; i < entry.Regions.Count; i++)
                            if (entry.GetUpperID(i) == baseID.Text.ToUpper()) outWad = entry.GetWAD(i);
                if (outWad == null || outWad?.NumOfContents <= 1) throw new Exception(Program.Lang.Msg(8, true));

                if (File.Exists(patch)) rom.Patch(patch);

                switch (targetPlatform)
                {
                    case Platform.NES:
                    case Platform.SNES:
                    case Platform.N64:
                    case Platform.SMS:
                    case Platform.SMD:
                    case Platform.PCE:
                    case Platform.PCECD:
                    case Platform.NEO:
                    case Platform.MSX:
                        if (isVirtualConsole)
                            WiiVCInject();
                        else
                            ForwarderCreator(targetFile);
                        break;

                    case Platform.GB:
                    case Platform.GBC:
                    case Platform.GBA:
                    case Platform.S32X:
                    case Platform.SMCD:
                    case Platform.PSX:
                    case Platform.RPGM:
                        ForwarderCreator(targetFile);
                        break;

                    case Platform.Flash:
                        FlashInject();
                        break;

                    default:
                        throw new NotImplementedException();
                }

                // Banner
                // *******
                BannerHelper.Modify
                (
                    outWad,
                    targetPlatform,
                    isVirtualConsole ? outWad.Region : _bannerRegion switch { 1 => libWiiSharp.Region.Japan, 2 => libWiiSharp.Region.Korea, 3 => libWiiSharp.Region.Europe, _ => libWiiSharp.Region.USA },
                    _bannerTitle,
                    _bannerYear,
                    _bannerPlayers
                );
                if (File.Exists(sound) && sound != null)
                    SoundHelper.ReplaceSound(outWad, sound);
                else
                    SoundHelper.ReplaceSound(outWad, Properties.Resources.Sound_WiiVC);
                if (img.VCPic != null) img.ReplaceBanner(outWad);

                // Change WAD region & internal main.dol things
                // *******
                if (region_list.SelectedIndex > 0)
                    outWad.Region = outWadRegion;
                Utils.ChangeVideoMode(outWad, video_modes.SelectedIndex);

                // Other WAD settings to be changed done by WAD creator helper, which will save to a new file
                // *******
                outWad.ChangeChannelTitles(_channelTitles);
                outWad.ChangeTitleID(LowerTitleID.Channel, _tID);
                outWad.FakeSign = true;

                if (Directory.Exists(Paths.SDUSBRoot))
                {
                    Directory.CreateDirectory(Paths.SDUSBRoot + "wad\\");
                    outWad.Save(Paths.SDUSBRoot + "wad\\" + Path.GetFileNameWithoutExtension(targetFile) + ".wad");

                    // Get ZIP directory path & compress to .ZIP archive
                    // *******
                    if (File.Exists(targetFile)) File.Delete(targetFile);

                    using (ZipFile z = new(targetFile))
                    {
                        z.AddDirectory(Paths.SDUSBRoot, "");
                        z.Save();
                    }

                    // Clean
                    // *******
                    Directory.Delete(Paths.SDUSBRoot, true);
                }
                else outWad.Save(targetFile);

                outWad.Dispose();

                // Check new WAD file
                // *******
                if (File.Exists(targetFile) && File.ReadAllBytes(targetFile).Length > 10)
                {
                    SystemSounds.Beep.Play();

                    switch (MessageBox.Show(Program.Lang.Msg(3), null, MessageBox.Buttons.YesNo, MessageBox.Icons.Information))
                    {
                        case MessageBox.Result.Yes:
                            System.Diagnostics.Process.Start("explorer.exe", $"/select, \"{targetFile}\"");
                            break;
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
                Program.MainForm.CleanTemp();
            }
        }

        public void ForwarderCreator(string path)
        {
            Forwarder f = new()
            {
                ROM = rom.FilePath,
                ID = _tID,
                Emulator = injection_methods.SelectedItem.ToString(),
                Storage = forwarder_root_device.SelectedIndex switch { 1 => Forwarder.Storages.USB, _ => Forwarder.Storages.SD },
                Name = _channelTitles[1]
            };

            // Get settings from relevant form
            // *******
            f.Settings = contentOptions;

            // Actually inject everything
            // *******
            f.CreateZIP(Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path) + $" ({f.Storage}).zip"));
            outWad = f.CreateWAD(outWad);
        }
        #endregion

        #region /////////////////////////////////////////////// To inherit ///////////////////////////////////////////////
        public void FlashInject()
        {
            Injectors.Flash.Settings = contentOptions;
            outWad = Injectors.Flash.Inject(outWad, rom.FilePath, _saveDataTitle, img);
        }

        public void WiiVCInject()
        {
            // Create Wii VC injector to use
            // *******
            InjectorWiiVC VC = null;

            switch (targetPlatform)
            {
                default:
                    throw new NotImplementedException();

                // NES
                // *******
                case Platform.NES:
                    VC = new Injectors.NES();
                    break;

                // SNES
                // *******
                case Platform.SNES:
                    VC = new Injectors.SNES();
                    break;

                // N64
                // *******
                case Platform.N64:
                    VC = new Injectors.N64()
                    {
                        Settings = contentOptions,

                        CompressionType = emuVer == 3 ? (contentOptions["romc"] == "0" ? 1 : 2) : 0,
                        Allocate = contentOptions["rom_autosize"] == "True" && (emuVer <= 1),
                    };
                    break;

                // SEGA
                // *******
                case Platform.SMS:
                case Platform.SMD:
                    VC = new Injectors.SEGA()
                    {
                        IsSMS = targetPlatform == Platform.SMS
                    };
                    break;

                // PCE
                // *******
                case Platform.PCE:
                    VC = new Injectors.PCE();
                    break;

                // PCECD
                // *******
                case Platform.PCECD:
                    // VC = new Injectors.PCECD();
                    break;

                // NEOGEO
                // *******
                case Platform.NEO:
                    VC = new Injectors.NEO();
                    break;

                // MSX
                // *******
                case Platform.MSX:
                    VC = new Injectors.MSX();
                    break;
            }

            // Get settings from relevant form
            // *******
            VC.Settings = contentOptions;

            // Set path to manual (if it exists) and load WAD
            // *******
            VC.RetainOriginalManual = manual_type.SelectedIndex == 1;
            VC.ManualFile = manual != null ? new Ionic.Zip.ZipFile("manual") : null;
            if (VC.ManualFile != null) VC.ManualFile.AddDirectory(manual);

            // Actually inject everything
            // *******
            outWad = VC.Inject(outWad, rom, _saveDataTitle, img);
        }
        #endregion

        #region **Console-Specific Functions**
        // ******************
        // CONSOLE-SPECIFIC
        // ******************
        private void openInjectorOptions(object sender, EventArgs e)
        {
            var result = contentOptionsForm.ShowDialog(this) == DialogResult.OK;

            switch (targetPlatform)
            {
                default:
                    if (result) { refreshData(); }
                    break;

                case Platform.NES:
                    if (result) { LoadImage(); }
                    break;
            }
        }
        #endregion

        #region Base WAD Management/Visual
        private void AddBases()
        {
            Base.Items.Clear();

            foreach (var entry in channels.Entries)
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
            for (int i = 0; i < channels.Entries[Base.SelectedIndex].Regions.Count; i++)
            {
                switch (channels.Entries[Base.SelectedIndex].Regions[i])
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
            {
                selected = regions.IndexOf(item.Value == 0 ? Program.Lang.String("region_j")
                         : item.Value == 1 ? Program.Lang.String("region_k")
                         : item.Value == 2 ? Program.Lang.String("region_e")
                         : Program.Lang.String("region_u"));

                if (selected == -1 && item.Value == 1) selected = regions.IndexOf(Program.Lang.String("region_u"));
                break;
            }

            if (selected == -1) selected = 0;

            // Reset currently-selected base info
            // ********
            baseRegionList.Items.Clear();

            // Add regions to WAD region context list
            // ********
            for (int i = 0; i < channels.Entries[Base.SelectedIndex].Regions.Count; i++)
            {
                switch (channels.Entries[Base.SelectedIndex].Regions[i])
                {
                    case 0:
                        baseRegionList.Items.Add(Program.Lang.String("region_j"), null, WADRegionList_Click);
                        break;

                    case 1:
                    case 2:
                        baseRegionList.Items.Add(Program.Lang.String("region_u"), null, WADRegionList_Click);
                        break;

                    case 3:
                    case 4:
                    case 5:
                        baseRegionList.Items.Add(Program.Lang.String("region_e"), null, WADRegionList_Click);
                        break;

                    case 6:
                    case 7:
                        baseRegionList.Items.Add(Program.Lang.String("region_k"), null, WADRegionList_Click);
                        break;

                    default:
                    case 8:
                        baseRegionList.Items.Add(Program.Lang.String("region_rf"), null, WADRegionList_Click);
                        break;
                }
            }

            // Final visual updates
            // ********
            UpdateBaseForm(selected);
            BaseRegion.Cursor = baseRegionList.Items.Count == 1 ? Cursors.Default : Cursors.Hand;
        }

        private void WADRegion_Click(object sender, EventArgs e)
        {
            if (baseRegionList.Items.Count > 1)
                baseRegionList.Show(this, PointToClient(Cursor.Position));
        }

        private void WADRegionList_Click(object sender, EventArgs e)
        {

        }

        private void UpdateBaseForm(int index = -1)
        {
            if (index == -1)
            {
                for (index = 0; index < channels.Entries[Base.SelectedIndex].Regions.Count; index++)
                    if (channels.Entries[Base.SelectedIndex].GetUpperID(index)[3] == baseID.Text[3])
                        goto Set;

                return;
            }

            Set:
            // Native name & Title ID
            // ********
            baseName.Text = channels.Entries[Base.SelectedIndex].Titles[index];
            baseID.Text = channels.Entries[Base.SelectedIndex].GetUpperID(index);

            if (baseRegionList.Items.Count > 0)
            {
                foreach (ToolStripMenuItem item in baseRegionList.Items.OfType<ToolStripMenuItem>())
                    item.Checked = false;
                (baseRegionList.Items[index] as ToolStripMenuItem).Checked = true;
            }

            // Flag
            // ********
            BaseRegion.Image = channels.Entries[Base.SelectedIndex].Regions[index] switch
            {
                0       => Properties.Resources.flag_jp,
                1 or 2  => Properties.Resources.flag_us,
                3       => (int)targetPlatform <= 2 ? Properties.Resources.flag_eu50 : Properties.Resources.flag_eu,
                4 or 5  => (int)targetPlatform <= 2 ? Properties.Resources.flag_eu60 : Properties.Resources.flag_eu,
                6 or 7  => Properties.Resources.flag_kr,
                _ => null,
            };

            #region Save data text handling
            int oldSaveLength = save_data_title.MaxLength;

            // Changing SaveDataTitle max length & clearing text field when needed
            // ----------------------
            if (targetPlatform == Platform.NES) save_data_title.MaxLength = inWadRegion == Region.Korea ? 30 : 20;
            else if (targetPlatform == Platform.SNES) save_data_title.MaxLength = 80;
            else if (targetPlatform == Platform.N64) save_data_title.MaxLength = 100;
            else if (targetPlatform == Platform.NEO
                  || targetPlatform == Platform.MSX) save_data_title.MaxLength = 64;
            else save_data_title.MaxLength = 80;

            // Also, some consoles only support a single line anyway
            // ********
            bool isSingleLine = inWadRegion == Region.Korea
                             || targetPlatform == Platform.NES
                             || targetPlatform == Platform.SMS
                             || targetPlatform == Platform.SMD
                             || targetPlatform == Platform.PCE
                             || targetPlatform == Platform.PCECD;

            // Set textbox to use single line when needed
            // ********
            if (save_data_title.Multiline == isSingleLine)
            {
                save_data_title.Multiline = !isSingleLine;
                save_data_title.Location = isSingleLine ? new Point(save_data_title.Location.X, int.Parse(save_data_title.Tag.ToString()) + 8) : new Point(save_data_title.Location.X, int.Parse(save_data_title.Tag.ToString()));
                save_data_title.Clear();
                goto End;
            }
            if (inWadRegion == Region.Korea && save_data_title.Multiline) save_data_title.MaxLength /= 2; // Applies to both NES/FC & SNES/SFC

            // Clear text field if at least one line is longer than the maximum limit allowed
            // ********
            double max = save_data_title.Multiline ? Math.Round((double)save_data_title.MaxLength / 2) : save_data_title.MaxLength;
            foreach (var line in save_data_title.Lines)
                if (line.Length > max && save_data_title.MaxLength != oldSaveLength)
                    save_data_title.Clear();
            #endregion

            End:
            resetImages();
            linkSaveDataTitle();
            resetContentOptions();
            refreshData();
        }

        private int emuVer
        {
            get
            {
                if (channels != null)
                    foreach (var entry in channels.Entries)
                        for (int i = 0; i < entry.Regions.Count; i++)
                            if (entry.GetUpperID(i) == baseID.Text.ToUpper())
                                return entry.EmuRevs[i];

                return 0;
            }
        }

        /// <summary>
        /// Changes injector settings based on selected base/console
        /// </summary>
        private void resetContentOptions()
        {
            manual_type.Visible = false;
            forwarder_root_device.Visible = false;
            contentOptionsForm = null;

            if (isVirtualConsole)
            {
                manual_type.Visible = true;

                switch (targetPlatform)
                {
                    case Platform.NES:
                        contentOptionsForm = new Options_VC_NES() { EmuType = emuVer };
                        break;

                    case Platform.SNES:
                        break;

                    case Platform.N64:
                        contentOptionsForm = new Options_VC_N64() { EmuType = inWadRegion == Region.Korea ? 3 : emuVer };
                        break;

                    case Platform.SMS:
                    case Platform.SMD:
                        contentOptionsForm = new Options_VC_SEGA() { EmuType = emuVer, IsSMS = targetPlatform == Platform.SMS };
                        break;

                    case Platform.PCE:
                    case Platform.PCECD:
                        contentOptionsForm = new Options_VC_PCE();
                        break;

                    case Platform.NEO:
                        contentOptionsForm = new Options_VC_NEO() { EmuType = emuVer };
                        break;

                    case Platform.MSX:
                        break;

                    case Platform.C64:
                        break;
                }
            }

            else if (targetPlatform == Platform.Flash)
            {
                contentOptionsForm = new Options_Flash();
            }

            else
            {
                forwarder_root_device.Visible = true;

                switch (targetPlatform)
                {
                    case Platform.GB:
                    case Platform.GBC:
                    case Platform.GBA:
                    case Platform.S32X:
                    case Platform.SMCD:
                    case Platform.PSX:
                        contentOptionsForm = new Options_Forwarder(targetPlatform);
                        break;
                    case Platform.NES:
                        break;
                    case Platform.SNES:
                        break;
                    case Platform.N64:
                        break;
                    case Platform.SMS:
                        break;
                    case Platform.SMD:
                        break;
                    case Platform.PCE:
                        break;
                    case Platform.PCECD:
                        break;
                    case Platform.NEO:
                        break;
                    case Platform.MSX:
                        break;
                    case Platform.C64:
                        break;
                    case Platform.Flash:
                        break;
                    case Platform.RPGM:
                        contentOptionsForm = new Options_RPGM();
                        break;
                    default:
                        break;
                }
            }

            if (contentOptionsForm != null)
            {
                contentOptionsForm.Font = Font;
                contentOptionsForm.Text = Program.Lang.String("injection_method_options", "projectform");
                contentOptionsForm.Icon = Icon.FromHandle(Properties.Resources.wrench.GetHicon());
            }

            if (!isVirtualConsole && manual != null)
            {
                manual = null;
                manual_type.SelectedIndex = 0;
            }

            #region -- Content options panel --
            editContentOptions.Enabled = contentOptionsForm != null;
            #endregion

            showSaveData = isVirtualConsole || targetPlatform == Platform.Flash;

            extra.Text = manual_type.Visible ? Program.Lang.String(manual_type.Name, Name) : Program.Lang.String(forwarder_root_device.Name, Name);
        }
        #endregion

        private void CustomManual_CheckedChanged(object sender, EventArgs e)
        {
            if (manual_type.Enabled && manual_type.SelectedIndex == 2 && manual == null)
            {
                if (!Properties.Settings.Default.donotshow_000) MessageBox.Show((sender as Control).Text, Program.Lang.Msg(6), 0);

                if (browseManual.ShowDialog() == DialogResult.OK) LoadManual(manual_type.SelectedIndex, browseManual.SelectedPath, true);
                else manual_type.SelectedIndex = 0;
            }

            if (manual_type.Enabled && manual_type.SelectedIndex < 2) LoadManual(manual_type.SelectedIndex);

            refreshData();
        }

        private void import_patch_CheckedChanged(object sender, EventArgs e)
        {
            if (browsePatch.ShowDialog() == DialogResult.OK)
            {
                patch = browsePatch.FileName;
                refreshData();
            }

            else if (patch != null)
            {
                patch = null;
                refreshData();
            }
        }

        private void InjectorsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            resetImages();
            resetContentOptions();
            LoadImage();
            refreshData();
        }

        private void RegionsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            resetImages();
            LoadImage();
            refreshData();
        }

        /* private void banner_preview_Click(object sender, EventArgs e)
        {
            using (Form f = new Form())
            {
                f.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                f.ShowInTaskbar = false;
                f.Text = Program.Lang.String("banner_preview", Name);
                f.Icon = Icon;

                var p = new PictureBox() { Name = "picture" };
                p.SizeMode = PictureBoxSizeMode.AutoSize;
                p.Location = new Point(0, 0);

                p.Image = preview.Banner
                (
                    img?.VCPic,
                    banner_form.title.Text,
                    (int)banner_form.released.Value,
                    (int)banner_form.players.Value,
                    targetPlatform,
                    _bannerRegion
                );

                f.ClientSize = p.Image.Size;
                f.StartPosition = FormStartPosition.CenterParent;
                f.Controls.Add(p);
                f.ShowDialog();
            }
        } */

        private void banner_Click(object sender, EventArgs e) => bannerMenu.Show(Cursor.Position);

        private void banner_customize_Click(object sender, EventArgs e)
        {
            banner_form.origTitle = banner_form.title.Text;
            banner_form.origYear = (int)banner_form.released.Value;
            banner_form.origPlayers = (int)banner_form.players.Value;
            banner_form.origRegion = banner_form.region.SelectedIndex;

            if (banner_form.ShowDialog() == DialogResult.OK)
            {
                bool hasBanner = banner_form.origTitle != banner_form.title.Text;
                bool hasYear = banner_form.origYear != (int)banner_form.released.Value;
                bool hasPlayers = banner_form.origPlayers != (int)banner_form.players.Value;
                bool hasRegion = banner_form.origRegion != banner_form.region.SelectedIndex;
                if (hasBanner) linkSaveDataTitle();

                if (hasBanner || hasYear || hasPlayers || hasRegion)
                {
                    resetImages(true);
                    refreshData();
                }
            }
        }

        protected void LoadSound(string file)
        {
            sound = file;
            restore_banner_sound.Enabled = File.Exists(file) && file != null;
            if (!restore_banner_sound.Enabled) sound = null;
            refreshData();
        }

        private void replace_banner_sound_Click(object sender, EventArgs e)
        {
            browseSound.Filter = "WAV (*.wav)|*.wav"; // + Program.Lang.String("filter");
            browseSound.Title = replace_banner_sound.Text;
            if (browseSound.ShowDialog() == DialogResult.OK || File.Exists(browseSound.FileName))
                LoadSound(browseSound.FileName);
        }
        private void restore_banner_sound_Click(object sender, EventArgs e) => LoadSound(null);

        private void play_banner_sound_Click(object sender, EventArgs e)
        {
            SoundPlayer snd = File.Exists(sound) && sound != null ? new(sound) : new(Properties.Resources.Sound_WiiVC);
            snd.PlaySync();
        }

        private void Tab_Switch(object sender, EventArgs e)
        {
            int tab = sender == tab_main ? 0 : sender == tab_channel ? 1 : 0;

            switch (tab)
            {
                case 0:
                default:
                    tab_main.Checked    = true;
                    tab_channel.Checked = false;
                    break;
                case 1:
                    tab_main.Checked    = false;
                    tab_channel.Checked = true;
                    break;
            }

            tab1.Visible = tab_main.Checked;
            tab2.Visible = tab_channel.Checked;

            tab_main.Enabled    = !tab_main.Checked;
            tab_channel.Enabled = !tab_channel.Checked;
        }
    }
}
