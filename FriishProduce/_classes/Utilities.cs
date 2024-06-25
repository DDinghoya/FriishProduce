﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FriishProduce
{
    public enum ExportResult
    {
        BIOS_NOT_FOUND,
        FILE_NOT_FOUND,
        FOLDER_NOT_FOUND,
        ROM_SIZE,
        FAILED_INTERNET,
        FAILED_INJECTION,
        FAILED_DOWNLOADED_WAD
    }

    class CustomToolTip : ToolTip
    {
        public CustomToolTip()
        {
            OwnerDraw = true;

            AutoPopDelay = 5000;
            InitialDelay = 500;
            ReshowDelay = 100;
            AutomaticDelay = 400;

            Draw += new DrawToolTipEventHandler(OnDraw);
        }

        private void OnDraw(object sender, DrawToolTipEventArgs e) // use this event to customise the tool tip
        {
            using (SolidBrush b = new SolidBrush(Color.Black))
                e.Graphics.FillRectangle(b, e.Bounds.X, e.Bounds.Y, e.Bounds.Width + 2, e.Bounds.Height + 2);

            using (SolidBrush b = new SolidBrush(SystemColors.Info))
                e.Graphics.FillRectangle(b, e.Bounds.X + 1, e.Bounds.Y + 1, e.Bounds.Width, e.Bounds.Height);

            e.Graphics.DrawString
            (
                e.ToolTipText,
                Program.MainForm.Font,
                Brushes.Black,
                new RectangleF(new PointF(4, 4), new SizeF (e.Bounds.Width - 4, e.Bounds.Height - 4))
            );
        }
    }

    public static class Web
    {
        public static bool InternetTest(string URL = null)
        {
            int timeout = 15;

            try
            {
                var request = (HttpWebRequest)WebRequest.Create
                (
                    !string.IsNullOrWhiteSpace(URL) ? URL :
                    (
                        System.Globalization.CultureInfo.InstalledUICulture.Name.StartsWith("fa") ? "https://www.aparat.com/" :
                        System.Globalization.CultureInfo.InstalledUICulture.Name.Contains("zh-CN") ? "http://www.baidu.com/" :
                        "https://www.google.com/"
                    )
                );

                URL = request.Address.Authority;
                request.Method = "HEAD";
                request.KeepAlive = false;
                request.Timeout = timeout * 500;

                if (CheckDomain(URL, timeout))
                {
                    var response = request.GetResponse();

                    for (int i = 0; i < 2; i++)
                    {
                        char x = response.ResponseUri.ToString()[i];
                    }

                    return true;
                }

                return false;
            }

            catch (Exception ex)
            {
                string message = (ex.Message.Contains(URL) ? ex.Message.Substring(0, ex.Message.IndexOf(':')) : ex.Message) + (ex.Message[ex.Message.Length - 1] != '.' ? "." : string.Empty);
                throw new Exception(string.Format(Program.Lang.Msg(0, true), message));
            }
        }

        private static bool CheckDomain(string URL, int timeout)
        {
            string host;
            try { host = new Uri(URL).Host; } catch { host = URL; }

            using (var tcp = new TcpClient())
            {
                var result = tcp.BeginConnect(host, 80, null, null);
                if (!result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(timeout)))
                {
                    throw new WebException("The domain is not available.", WebExceptionStatus.ConnectFailure);
                }

                tcp.EndConnect(result);
                return true;
            }
        }

        public static byte[] Get(string URL)
        {
            // Actual web connection is done here
            // ****************

            using (MemoryStream ms = new MemoryStream())
            using (WebClient x = new WebClient())
            {
                if (Task.WaitAny
                    (new Task[]{Task.Run(() => { try { x.OpenRead(URL).CopyTo(ms); return ms; } catch { return null; } }
                    )}, 100 * 1000) == -1)
                    throw new TimeoutException();

                if (ms.ToArray().Length > 75) return ms.ToArray();

                throw new WebException();
            }
        }
    }

    public static class Byte
    {
        public static readonly byte[] Dummy = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };

        public static int PatternAt(byte[] source, byte[] pattern)
        {
            for (int i = 0; i < source.Length; i++)
                if (source.Skip(i).Take(pattern.Length).SequenceEqual(pattern))
                    return i;
            return -1;
        }

        public static int PatternAt(byte[] source, string text)
        {
            byte[] pattern = Encoding.ASCII.GetBytes(text);
            for (int i = 0; i < source.Length; i++)
                if (source.Skip(i).Take(pattern.Length).SequenceEqual(pattern))
                    return i;
            return -1;
        }

        public static byte[] FromHex(string hex)
        {
            hex = hex.ToUpper().Replace("-", "").Replace(" ", "");
            return Enumerable.Range(0, hex.Length)
                         .Where(x => x % 2 == 0)
                         .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                         .ToArray();
        }

        public static int IndexOf(byte[] source, byte[] pattern, int start, int end)
        {
            if (start < 0) start = 0;
            if (end > source.Length) end = source.Length - pattern.Length;

            for (int i = start; i < end; i++)
            {
                if (source[i] != pattern[0]) continue;

                for (int x = pattern.Length - 1; x >= 1; x--)
                {
                    try { if (source[i + x] != pattern[x]) break; } catch { }
                    if (x == 1) return i;
                }
            }

            return -1;
        }

        public static int IndexOf(byte[] source, string pattern, int start = 0, int end = -1)
        {
            var pArray = pattern.Split(' ');
            var pBytes = new byte[pArray.Length];
            for (int i = 0; i < pArray.Length; i++)
                pBytes[i] = Convert.ToByte(pArray[i], 16);

            if (end == -1) end = source.Length - pBytes.Length;
            return IndexOf(source, pBytes, start, end);
        }

        public static bool IsSame(byte[] first, byte[] second)
        {
            return first.Length == second.Length && System.Collections.StructuralComparisons.StructuralEqualityComparer.Equals(first, second);
        }
    }

    public static class Utils
    {
        public static void Run(byte[] app, string appName, string arguments, bool showWindow = false)
        {
            string targetPath = Paths.WorkingFolder + Path.GetFileNameWithoutExtension(appName) + ".exe";
            File.WriteAllBytes(targetPath, app);
            Run(targetPath, Paths.WorkingFolder, arguments, showWindow);

            if (File.Exists(targetPath)) File.Delete(targetPath);
        }

        public static void Run(string app, string arguments, bool showWindow = false) => Run(app, Paths.Tools, arguments, showWindow);

        public static void Run(string app, string workingFolder, string arguments, bool showWindow = false)
        {
            var appPath = Path.Combine(Paths.Tools, app.Replace(Paths.Tools, "").Contains('\\') ? app.Replace(Paths.Tools, "") : Path.GetFileName(app));

            if (!appPath.EndsWith(".exe")) appPath += ".exe";

            if (!File.Exists(appPath)) throw new Exception(string.Format(Program.Lang.Msg(5, true), app));

            using (Process p = Process.Start(new ProcessStartInfo
            {
                FileName = appPath,
                WorkingDirectory = workingFolder,
                Arguments = arguments,
                UseShellExecute = false,
                CreateNoWindow = !showWindow
            }))
                p.WaitForExit();
        }

        public static byte[] ExtractContent1(byte[] input)
        {
            if (input.Length < 1048576)
            {
                // Create temporary files at working folder
                // ****************
                File.WriteAllBytes(Paths.WorkingFolder + "content1.app", input);

                // Decompress
                // ****************
                Run
                (
                    Paths.Tools + "wwcxtool.exe",
                    Paths.WorkingFolder,
                    "/u content1.app content1.dec"
                );
                if (!File.Exists(Paths.WorkingFolder + "content1.dec")) throw new Exception(Program.Lang.Msg(2, true));

                return File.ReadAllBytes(Paths.WorkingFolder + "content1.dec");
            }

            else return input;
        }

        public static byte[] PackContent1(byte[] input)
        {
            if (File.Exists(Paths.WorkingFolder + "content1.dec"))
            {
                // Write new to original decompressed file
                // ****************
                File.WriteAllBytes(Paths.WorkingFolder + "content1.dec", input);

                // Pack
                // ****************
                Run
                (
                    Paths.Tools + "wwcxtool.exe",
                    Paths.WorkingFolder,
                    "/cr content1.app content1.dec content1.new"
                );
                if (!File.Exists(Paths.WorkingFolder + "content1.new")) throw new Exception(Program.Lang.Msg(2, true));

                var output = File.ReadAllBytes(Paths.WorkingFolder + "content1.new");

                if (File.Exists(Paths.WorkingFolder + "content1.new")) File.Delete(Paths.WorkingFolder + "content1.new");
                if (File.Exists(Paths.WorkingFolder + "content1.dec")) File.Delete(Paths.WorkingFolder + "content1.dec");
                if (File.Exists(Paths.WorkingFolder + "content1.app")) File.Delete(Paths.WorkingFolder + "content1.app");

                return output;
            }

            else return input;
        }

        public static void ChangeVideoMode(libWiiSharp.WAD wad, int mode = 0, bool force43 = false)
        {
            var content1 = ExtractContent1(wad.Contents[1]);

            if (mode > 0)
            {
                #region List of byte patterns and corresponding video modes
                /// NTSC & PAL60: 60Hz ///
                // NTSC (interlaced)   / 480i       00 02 80 01 E0 01 E0 00 28 00 00 02 80 01 E0
                // NTSC (non interlaced)            01 02 80 01 E0 01 E0 00 28 00 0B 02 80 01 E0
                // NTSC (progressive)  / 480p       02 02 80 01 E0 01 E0 00 28 00 00 02 80 01 E0
                // PAL60 (interlaced)  / 480i       14 02 80 01 E0 01 E0 00 28 00 00 02 80 01 E0
                // PAL60 (non interlaced)           15 02 80 01 E0 01 E0 00 28 00 00 02 80 01 E0
                // PAL60 (progressive) / 480p       16 02 80 01 E0 01 E0 00 28 00 00 02 80 01 E0

                /// MPAL: 50Hz for American region ///
                // MPAL (interlaced)                08 02 80 01 E0 01 E0 00 28 00 00 02 80 01 E0
                // MPAL (non interlaced)            09 02 80 01 E0 01 E0 00 28 00 00 02 80 01 E0
                // MPAL (progressive)               0A 02 80 01 E0 01 E0 00 28 00 00 02 80 01 E0
                /// PAL: replace 01 at end with 02 ///
                // PAL (interlaced)    / 576i       04 02 80 02 10 02 10 00 28 00 17 02 80 02 10
                // PAL (non interlaced)             05 02 80 01 08 01 08 00 28 00 0B 02 80 02 10
                // PAL (progressive)                06 02 80 02 10 02 10 00 28 00 17 02 80 02 10
                /// ONLY PAL50 is different in byte composition !                 ^^ This byte seems to vary ///
                // PAL (progressive/alt)          = 06 02 80 01 08 02 0C 00 28 00 17 02 80 02 0C
                #endregion

                int start = 0x13F000;
                int end = 0x1FFFFF;

                // 0-2: NTSC/PAL
                // 3-5: MPAL/PAL
                // 6: Backup for PAL (progressive)
                Dictionary<int, string> modesList = new Dictionary<int, string>
                {
                    // NTSC
                    { 1, "00 02 80 01 E0 01 E0 00 28 00 xx 02 80 01 E0" },
                    { 2, "01 02 80 01 E0 01 E0 00 28 00 xx 02 80 01 E0" },
                    { 3, "02 02 80 01 E0 01 E0 00 28 00 xx 02 80 01 E0" },
                    // MPAL
                    { 4, "08 02 80 01 E0 01 E0 00 28 00 xx 02 80 01 E0" },
                    { 5, "09 02 80 01 E0 01 E0 00 28 00 xx 02 80 01 E0" },
                    { 6, "0A 02 80 01 E0 01 E0 00 28 00 xx 02 80 01 E0" },
                    // PAL60
                    { 7, "14 02 80 01 E0 01 E0 00 28 00 xx 02 80 01 E0" },
                    { 8, "15 02 80 01 E0 01 E0 00 28 00 xx 02 80 01 E0" },
                    { 9, "16 02 80 01 E0 01 E0 00 28 00 xx 02 80 01 E0" },
                    // PAL50
                    { 10, "04 02 80 02 10 02 10 00 28 00 xx 02 80 02 10" },
                    { 11, "05 02 80 01 08 01 08 00 28 00 xx 02 80 02 10" },
                    { 12, "06 02 80 02 10 02 10 00 28 00 xx 02 80 02 10" },
                };

                // 0 = None
                // 1 = NTSC
                // 2 = MPAL
                // 3 = PAL60
                // 4 = PAL50
                // 5 = NTSC/PAL60
                // 6 = NTSC/MPAL
                // 7 = PAL60/50

                switch (mode)
                {
                    default:
                        int targetMode = mode switch
                        {
                            1 => 1,
                            2 => 4,
                            3 => 7,
                            4 => 10,
                            _ => 1
                        };

                        for (int i = 1; i < modesList.Count; i++)
                        {
                            for (int index; (index = Byte.IndexOf(content1, modesList[i].Substring(0, 23), start, end)) > 0;)
                            {
                                string[] array = i == 1 || i == 4 || i == 7 || i == 10 ? modesList[targetMode + 0].Split(' ')   // Interlaced
                                               : i == 2 || i == 5 || i == 8 || i == 11 ? modesList[targetMode + 1].Split(' ')   // Non-interlaced
                                               : modesList[targetMode + 2].Split(' ');                                          // Progressive

                                for (int x = 0; x < 15; x++)
                                    if (array[x].ToLower() != "xx") content1[index + x] = Convert.ToByte(array[x], 16);
                                break;
                            }
                        }
                        break;
                }
            }

            #region Force 4:3 aspect ratio display on Wii U
            if (force43)
            {
                File.WriteAllBytes(Paths.WorkingFolder + "main.dol", content1);

                Run
                (
                    "szs\\wstrt.exe",
                    Paths.Tools + "szs\\",
                    $"patch \"{Paths.WorkingFolder}main.dol\" --add-section Force43.gct"
                );

                content1 = File.ReadAllBytes(Paths.WorkingFolder + "main.dol");
                if (File.Exists(Paths.WorkingFolder + "main.dol")) File.Delete(Paths.WorkingFolder + "main.dol");
            }
            #endregion

            content1 = PackContent1(content1);

            wad.Unpack(Paths.WAD);
            File.WriteAllBytes(Paths.WAD + "00000001.app", content1);
            wad.CreateNew(Paths.WAD);
            Directory.Delete(Paths.WAD, true);
        }
    }

    public static class RTP
    {
        public static bool IsValid(string path)
        {
            if (!Directory.Exists(path)) return false;
            int isValid = 0;

            string[] folders = new string[]
                {
                    "Battle",
                    "CharSet",
                    "ChipSet",
                    "FaceSet",
                    "GameOver",
                    "Music",
                    "Title",
                    "Sound",
                    "System",
                    "Title"
                };

            try
            {
                foreach (var item in folders)
                    if (Directory.Exists(Path.Combine(path, item)))
                        if (Directory.GetFiles(Path.Combine(path, item)).Length > 0)
                            isValid++;
            }
            catch { }

            return isValid >= 10;
        }
    }
}
