﻿using libWiiSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using static FriishProduce.FileDatas.Emulators;

namespace FriishProduce
{
    public class Forwarder
    {
        public static readonly (int Index, byte[] File, string Name)[] List = new (int Index, byte[] File, string Name)[]
        {
            (0,     fceugx,             "FCE Ultra GX"),
            (1,     fceurx,             "FCE Ultra RX"),
            (2,     fceuxtx,            "FCEUX TX"),
            (3,     snes9xgx,           "Snes9x GX"),
            (4,     snes9xrx,           "Snes9x RX"),
            (5,     snes9xtx,           "Snes9x TX"),
            (6,     vbagx,              "Visual Boy Advance GX"),
            (7,     genplusgx,          "Genesis Plus GX"),
            (8,     wii64_gln64,        "Wii64 (GLideN64 GFX)"),
            (9,     wii64_rice,         "Wii64 (Rice GFX)"),
            (10,    not64,              "Not64"),
            (11,    mupen64gc_fix94,    "mupen64gc-fix94"),
            (12,    wiistation,         "WiiStation"),
            (13,    easyrpg,            "EasyRPG Player"),
            (14,    mgba,               "mGBA"),
        };

        public string Emulator { get; set; }
        private int EmulatorIndex
        {
            get
            {
                foreach (var item in List)
                {
                    if (item.Name == Emulator) return item.Index;
                }

                return -1;
            }
        }

        public IDictionary<string, string> Settings { get; set; }

        public enum Storages { SD, USB }
        public Storages Storage = Storages.SD;

        public enum WADTypes { Comex, Waninkoko };
        public static WADTypes WADType { get; set; }

        private static string path { get; set; }

        private bool isDisc { get => new Disc().CheckValidity(ROM); }

        public string ROM { get; set; }
        private string romExtension { get => Path.GetExtension(ROM).ToLower(); }

        private string tID;
        public string ID { get => tID; set { tID = value; path = $"%s:/private/VC/{value}/boot.dol"; } }

        public void CreateZIP(string Out)
        {
            // Declare main variables and failsafes
            // *******
            if (ROM == null) throw new FileNotFoundException();
            if (EmulatorIndex == -1) throw new NotSupportedException();

            bool hasBIOS = false;
            string PackageFolder = Paths.SDUSBRoot + path.Substring(4).Replace("/boot.dol", "").Replace('/', '\\') + '\\';
            string ROMFolder = isDisc ? PackageFolder + "title\\" : PackageFolder;
            string ROMName = isDisc ? Path.GetFileName(ROM) : (EmulatorIndex >= 7 ? "title" : "HOME Menu") + romExtension;

            // Create SD folder and copy emulator
            // *******
            Directory.CreateDirectory(ROMFolder);
            File.WriteAllBytes(PackageFolder + "boot.dol", List[EmulatorIndex].File);

            // If RPG Maker game, copy all files within the ROM folder (RTP not included as part of this, yet)
            // *******
            if (EmulatorIndex == 13)
            {
                if (RTP.IsValid(Settings["rtp_folder"]))
                {
                    foreach (var folder in Directory.EnumerateDirectories(Settings["rtp_folder"], "*.*", SearchOption.AllDirectories))
                    {
                        string target = Path.Combine(ROMFolder, folder.Replace(Settings["rtp_folder"], "").TrimStart('\\'));
                        if (!Directory.Exists(target)) Directory.CreateDirectory(target);

                        foreach (var file in Directory.EnumerateFiles(folder, "*.*", SearchOption.AllDirectories))
                        {
                            string rawFile = file.Replace(folder, "");
                            File.Copy(file, target + rawFile, true);
                        }
                    }
                }
                    
                string origPath = Path.GetDirectoryName(ROM);
                foreach (var folder in Directory.EnumerateDirectories(origPath, "*.*", SearchOption.AllDirectories))
                {
                    string target = Path.Combine(ROMFolder, folder.Replace(origPath, "").TrimStart('\\'));
                    if (!Directory.Exists(target)) Directory.CreateDirectory(target);
                }

                foreach (var file in Directory.EnumerateFiles(origPath, "*.*", SearchOption.AllDirectories))
                {
                    string rawFile = file.Replace(origPath, "");
                    if (!Path.GetExtension(file).ToLower().EndsWith("exe")) File.Copy(file, ROMFolder + rawFile.TrimStart('\\'), true);
                }
            }

            else
            {
                // Saves folders
                // *******
                switch (EmulatorIndex)
                {
                    case 0:
                    case 1:
                        Directory.CreateDirectory(Paths.SDUSBRoot + "fceugx\\saves\\");
                        break;
                    case 2:
                        Directory.CreateDirectory(Paths.SDUSBRoot + "fceuxtx\\saves\\");
                        break;
                    case 3:
                    case 4:
                        Directory.CreateDirectory(Paths.SDUSBRoot + "snes9xgx\\saves\\");
                        break;
                    case 5:
                        Directory.CreateDirectory(Paths.SDUSBRoot + "snes9xtx\\saves\\");
                        break;
                    case 6:
                        Directory.CreateDirectory(Paths.SDUSBRoot + "vbagx\\saves\\");
                        break;
                    case 8:
                    case 9:
                    case 11:
                        Directory.CreateDirectory(Paths.SDUSBRoot + "wii64\\roms\\");
                        Directory.CreateDirectory(Paths.SDUSBRoot + "wii64\\saves\\");
                        break;
                    case 10:
                        Directory.CreateDirectory(Paths.SDUSBRoot + "not64\\roms\\");
                        Directory.CreateDirectory(Paths.SDUSBRoot + "not64\\saves\\");
                        break;
                    case 12:
                        Directory.CreateDirectory(Paths.SDUSBRoot + "wiisxrx\\bios\\");
                        Directory.CreateDirectory(Paths.SDUSBRoot + "wiisxrx\\isos\\");
                        Directory.CreateDirectory(Paths.SDUSBRoot + "wiisxrx\\saves\\");
                        Directory.CreateDirectory(Paths.SDUSBRoot + "wiisxrx\\savestates\\");
                        break;
                }

                // Copy game to SD folder
                // *******
                if (!File.Exists(ROMFolder + ROMName))
                {
                    File.Copy(ROM, ROMFolder + ROMName);
                    if (romExtension == ".cue")
                    {
                        foreach (var item in Directory.EnumerateFiles(Path.GetDirectoryName(ROM)))
                        {
                            if ((Path.GetExtension(item).ToLower() == ".bin" || Path.GetExtension(item).ToLower() == ".iso")
                                && (Path.GetFileNameWithoutExtension(item).ToLower() == Path.GetFileNameWithoutExtension(ROM).ToLower()))
                                File.Copy(item, ROMFolder + Path.GetFileName(item));
                        }
                    }
                }

                // Copy BIOS if available
                // *******
                bool validBIOS = false;
                try { validBIOS = File.Exists(Settings["BIOSPath"]); } catch { hasBIOS = false; }

                if (validBIOS)
                {
                    var validList = new List<(Console Platform, int Index, string Directory)>()
                    {
                        (Console.PSX,   12, Paths.SDUSBRoot + "wiisxrx\\bios\\SCPH1001.BIN"),
                    };

                    foreach (var item in validList)
                    {
                        if (BIOS.GetConsole(File.ReadAllBytes(Settings["BIOSPath"])) == item.Platform
                            && EmulatorIndex == item.Index)
                        {
                            File.Copy(Settings["BIOSPath"], item.Directory);
                            hasBIOS = true;
                        }
                    }
                }
            }

            // Prepare for meta.xml creation
            // *******
            ROMFolder = path.Replace("%s:/", Storage == Storages.USB ? "usb:/" : "sd:/").Replace("/boot.dol", "");
            if (isDisc) ROMFolder += "/title";

            // Write main
            // *******
            List<string> meta = new List<string>
            {
                "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>",
                "<app version=\"1\">",
                $"  <name>{ID}</name>",
                "  <coder></coder>",
                "  <version></version>",
                "  <release_date></release_date>",
                "  <short_description>SRL Forwarder</short_description>",
                "  <long_description>This will attempt to load a ROM from the following path:",
                ROMFolder + '/' + ROMName,
                $"using the emulator {List[EmulatorIndex].Name}.</long_description>",
                "  <arguments>",

            };

            // Change or add parameters when needed
            // *******
            switch (EmulatorIndex)
            {
                case 7:
                    meta.Add($"    <arg>{ROMFolder}/</arg>");
                    meta.Add($"    <arg>{ROMName}</arg>");
                    break;

                case 10:
                    meta.Add($"    <arg>rompath=\"{ROMFolder}/{ROMName}\"</arg>");
                    meta.Add("    <arg>SkipMenu=1</arg>");
                    meta.Add("    <arg>ScreenMode=0</arg>");
                    break;

                case 11:
                    meta.Add($"    <arg>{ROMFolder}</arg>");
                    meta.Add($"    <arg>{ROMName}</arg>");
                    meta.Add("    <arg>loader</arg>"); // Mupen64GC-FIX94 needs at least a third argument for be able to autoboot
                    meta.Add("    <arg>ScreenMode = 0</arg>");
                    break;

                case 12:
                    meta.Add($"    <arg>{ROMFolder}</arg>");
                    meta.Add($"    <arg>{ROMName}</arg>");
                    meta.Add($"    <arg>BiosDevice = {(hasBIOS ? "1" : "0")}</arg>");
                    meta.Add($"    <arg>BootThruBios = {(hasBIOS && bool.Parse(Settings["BIOSScreen"]) ? "1" : "0")}</arg>");
                    meta.Add("    <arg>FPS = 0</arg>");
                    meta.Add("    <arg>ScreenMode = 0</arg>");
                    meta.Add("    <arg>VideoMode = 0</arg>");
                    meta.Add("    <arg>Interlaced = 1</arg>");
                    break;

                case 14:
                    meta.Add($"    <arg>{ROMFolder}/{ROMName}</arg>");
                    break;

                default:
                    meta.Add($"    <arg>{ROMFolder}</arg>");
                    meta.Add($"    <arg>{ROMName}</arg>");
                    break;
            }

            // Write footer and create file
            // *******
            meta.Add("  </arguments>");
            meta.Add("  <ahb_access />");
            meta.Add("</app>");

            File.WriteAllLines(PackageFolder + "meta.xml", meta.ToArray());

            // Get ZIP directory path & compress to .ZIP archive
            // *******
            if (File.Exists(Out)) File.Delete(Out);
            ZipFile.CreateFromDirectory(Paths.SDUSBRoot, Out);

            // Clean
            // *******
            Directory.Delete(Paths.SDUSBRoot, true);
        }

        public WAD CreateWAD(WAD WAD, bool vWii = false)
        {
            // > For GenPlus & all emulators based on Wii64 Team's code (e.g. Wii64, WiiSX and forks), use Comex NANDloader
            // > For WiiMednafen, use Waninkoko NANDloader
            // *******
            WADType = (EmulatorIndex >= 7 && EmulatorIndex <= 11) || EmulatorIndex == 13 ? WADTypes.Comex : WADTypes.Waninkoko;

            // Load and unpack WAD
            // *******
            WAD x = WADType == WADTypes.Comex ? WAD.Load(FileDatas.Forwarder.Base_Comex) : WAD.Load(FileDatas.Forwarder.Base_Waninkoko);

            x.Unpack(Paths.WAD);
            WAD.BannerApp.Save(Paths.WAD + "00000000.app");

            // Define forwarder version
            // *******
            bool NeedsOldForwarder = EmulatorIndex == 7 || EmulatorIndex == 13;
            byte[] Forwarder = NeedsOldForwarder ? FileDatas.Forwarder.DOL_V12 : FileDatas.Forwarder.DOL_V14;
            int TargetOffset = NeedsOldForwarder ? 0x77426 : 0x7F979;
            int SecondTargetOffset = NeedsOldForwarder ? 263 : 256;
            string TargetPath = NeedsOldForwarder ? path : path.Substring(4);

            // Create forwarder .app
            // *******
            Encoding.ASCII.GetBytes(TargetPath).CopyTo(Forwarder, TargetOffset);
            File.WriteAllBytes(Paths.WAD + (x.BootIndex == 1 ? "00000002.app" : "00000001.app"), Forwarder);

            // Write NANDloader & save
            // *******
            if (vWii) File.WriteAllBytes(Paths.WAD + $"0000000{x.BootIndex}.app", FileDatas.Forwarder.vWiiNandLoader);

            x.CreateNew(Paths.WAD);
            x.Region = WAD.Region;
            Directory.Delete(Paths.WAD, true);
            return x;
        }
    }
}
