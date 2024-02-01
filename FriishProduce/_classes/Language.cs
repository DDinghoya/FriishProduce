﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static FriishProduce.Properties.Settings;
using System.Threading;

namespace FriishProduce
{
    public class Language
    {
        internal static Dictionary<string, string> English { get; set; }
        internal static Dictionary<string, string> Current { get; set; }

        private static string CurrentCode { get; set; }

        internal static Dictionary<string, string> Read(string jsonFile)
        {
            try
            {
                var jsonReader = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(jsonFile));
                var jsonCode = jsonReader["metadata"]["code"].ToString();
                var jsonName = new CultureInfo(Path.GetFileNameWithoutExtension(jsonFile)).DisplayName;

                var target = new Dictionary<string, string>();

                foreach (JObject category in jsonReader.Children<JToken>().Children<JToken>())
                    foreach (JProperty key in category.Properties())
                        target.Add(key.Name, key.Value.ToString());

                return target;
            }
            catch
            {
                return null;
            }
        }
        public Language()
        {
            CurrentCode = Default.UI_Language;
            string code = Default.UI_Language;
            string json_en = Paths.Languages + "en.json";
            string json = Paths.Languages + $"{code}.json";

            // --------------------------
            // Set up English
            // --------------------------
            if (!Directory.Exists(Paths.Languages))
                Directory.CreateDirectory(Paths.Languages);

            if (!File.Exists(json_en) || File.ReadAllBytes(json_en) != Properties.Resources.English)
                File.WriteAllBytes(json_en, Properties.Resources.English);

            try
            {
                var jsonReader = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(json_en));

                English = new Dictionary<string, string>();
                foreach (JObject category in jsonReader.Children<JToken>().Children<JToken>())
                    foreach (JProperty key in category.Properties())
                        English.Add(key.Name, key.Value.ToString());
            }
            catch
            {
                throw new Exception("Unable to setup language!");
            }

            // --------------------------
            // Localization process
            // --------------------------
            if (string.IsNullOrWhiteSpace(code) || ((Read(json) == null) && code != "sys"))
            {
                code = "sys";
                Default.UI_Language = "sys";
                Default.Save();
            }

            var culture = new CultureInfo(code == "sys" ? "en" : code);
            if (code == "sys")
            {
                foreach (string fn in Directory.GetFiles(Paths.Languages))
                {
                    string fn_code = Path.GetFileNameWithoutExtension(fn);

                    if (fn_code == CultureInfo.InstalledUICulture.TwoLetterISOLanguageName || fn_code == CultureInfo.InstalledUICulture.Name)
                    {
                        CurrentCode = fn_code;
                        json = Paths.Languages + $"{fn_code}.json";
                        culture = new CultureInfo(fn_code);
                        Current = Read(json);
                        goto EndOfFunction;
                    }
                }

                Current = English;
                goto EndOfFunction;
            }
            else
            {
                CurrentCode = culture.Name;
                json = Paths.Languages + $"{culture.Name}.json";
                Current = Read(json);

                if (Current == null)
                {
                    CurrentCode = "en";
                    Current = English;
                }

                goto EndOfFunction;
            }

            EndOfFunction:
                culture.DateTimeFormat = new DateTimeFormatInfo() { DateSeparator = ".", ShortTimePattern = "HH:mm" };
                Thread.CurrentThread.CurrentUICulture = culture;
                Thread.CurrentThread.CurrentCulture = culture;
        }

        public void Localize(Control main)
        {
            Get(main);
            foreach (Control c in main.Controls)
            {
                Get(c);
                foreach (Control d in c.Controls)
                {
                    Get(d);
                    foreach (Control e in d.Controls)
                    {
                        Get(e);
                        foreach (Control f in e.Controls)
                        {
                            Get(f);
                            foreach (Control g in f.Controls)
                                Get(g);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// First check in case-sensitive format, then use ToLower() if no result has been returned.
        /// Returns English by default if no corresponding localized string is found.
        /// </summary>
        public string Get(string id)
        {
            try
            {
                foreach (KeyValuePair<string, string> translation in Current)
                    if (translation.Key == id)
                        return translation.Value.Replace(@"\n", Environment.NewLine).Replace('\"', '"');
                    else if (translation.Key.ToLower() == id.ToLower())
                        return translation.Value.Replace(@"\n", Environment.NewLine).Replace('\"', '"');
            }
            catch (Exception)
            {
                goto End;
            }

            End:
                foreach (KeyValuePair<string, string> default_string in English)
                    if (default_string.Key == id)
                        return default_string.Value.Replace(@"\n", Environment.NewLine).Replace('\"', '"');
                    else if (default_string.Key.ToLower() == id.ToLower())
                        return default_string.Value.Replace(@"\n", Environment.NewLine).Replace('\"', '"');

                return "undefined";
        }

        public void Get(Control c)
        {
            c.Text = Get(c.Name.ToString()) != "undefined" ? Get(c.Name.ToString()) : c.Text;
            if (c.Tag != null) c.Text = Get(c.Tag.ToString()) != "undefined" ? Get(c.Tag.ToString()) : c.Text;

            if (c.GetType() == typeof(ComboBox) && ((ComboBox)c).Items.Contains("null"))
            {
                ((ComboBox)c).Items.Remove("null");
                for (int i = 0; i < 20; i++)
                    if (Get(c.Tag + $"_{i}") != "undefined")
                        try { ((ComboBox)c).Items.Add(Get(c.Tag + $"_{i}")); } catch { }
                    else if (Get(c.Name + $"_{i}") != "undefined")
                        try { ((ComboBox)c).Items.Add(Get(c.Name + $"_{i}")); } catch { }
            }

            if (Current.First().Value.StartsWith("ar") || Current.First().Value.StartsWith("fa") || Current.First().Value.StartsWith("he"))
                c.RightToLeft = RightToLeft.Yes;
        }

        public string[] LangInfo(string code)
        {
            var jsonReader = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(Paths.Languages + $"{code}.json"));
            return new string[] { jsonReader["metadata"]["code"].ToString(), jsonReader["metadata"]["author"].ToString() };
        }

        public string[] LangInfo() => LangInfo(CurrentCode);
    }
}
