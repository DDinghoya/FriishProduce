﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace FriishProduce
{
    public partial class LanguageXMLEditor : Form
    {
        private XmlDocument orig = new XmlDocument();

        public LanguageXMLEditor()
        {
            InitializeComponent();
            orig.Load(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("FriishProduce.Strings.en.xml"));

            var List = new List<string>();

            comboBox1.Items.Clear();

            foreach (var item in CultureInfo.GetCultures(CultureTypes.NeutralCultures))
                if (item.Name != "en" && item.Name != "")
                    List.Add(item.EnglishName);
            List.Add(new CultureInfo("pt-BR").EnglishName);

            foreach (var item in List.Distinct())
                comboBox1.Items.Add(item);

            comboBox1.SelectedIndex = 0;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            button1.Enabled = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            dataGridView1.Enabled = true;
            dataGridView1.Rows.Clear();

            foreach (XmlNode section in Language.EnglishXML.ChildNodes)
                foreach (XmlNode item in section.ChildNodes)
                    dataGridView1.Rows.Add(item.ParentNode.Attributes[0].InnerText, item.Name, item.InnerText.Replace(@"\n", Environment.NewLine).Replace(@"\\", @"\"));

            try
            {
                string path = Paths.Languages + SelectedCultureInfo().Name + ".xml";

                if (File.Exists(path))
                {
                    XmlDocument doc = new XmlDocument();
                    doc.PreserveWhitespace = false;
                    doc.Load(path);

                    foreach (XmlNode node in doc.ChildNodes)
                        if (node.Name.ToLower() == "language")
                        {
                            foreach (XmlNode section in node.ChildNodes)
                                if (section.ChildNodes.Count > 0)
                                    foreach (XmlNode item in section.ChildNodes)
                                        for (int i = 0; i < dataGridView1.Rows.Count; i++)
                                        {
                                            string sectionName = dataGridView1.Rows[i].Cells[0].Value?.ToString();
                                            string ID = dataGridView1.Rows[i].Cells[1].Value?.ToString();
                                            string originalText = dataGridView1.Rows[i].Cells[2].Value?.ToString();

                                            if (sectionName == section.Attributes[0]?.InnerText && ID == item.Name
                                                && !string.IsNullOrEmpty(item.InnerText))
                                                dataGridView1.Rows[i].Cells[3].Value = item.InnerText.Replace(@"\n", Environment.NewLine).Replace(@"\\", @"\");
                                        }
                        }
                }
            }

            catch (XmlException ex) { MessageBox.Show(ex.GetType().ToString(), ex.Message, MessageBoxButtons.OK, Ookii.Dialogs.WinForms.TaskDialogIcon.Error); }

            button1.Enabled = comboBox1.Enabled = false;
            button5.Enabled = button4.Enabled = button3.Enabled = button2.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SaveAs.FileName = SelectedCultureInfo().Name + ".xml";
            SaveAs.InitialDirectory = Paths.Languages;

            if (SaveAs.ShowDialog() == DialogResult.OK)
                WriteTo(SaveAs.FileName);
        }

        private CultureInfo SelectedCultureInfo()
        {
            foreach (var locale in CultureInfo.GetCultures(CultureTypes.AllCultures))
                if (locale.EnglishName == comboBox1.SelectedItem.ToString())
                    return locale;

            MessageBox.Show($"No culture info equivalent to the currently selected language ({comboBox1.SelectedItem.ToString()}) was found!");
            return new CultureInfo("");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
                dataGridView1.Rows[i].Cells[3].Value = null;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            button1.Enabled = comboBox1.Enabled = true;
            dataGridView1.Enabled = button5.Enabled = button4.Enabled = button3.Enabled = button2.Enabled = false;
            dataGridView1.Rows.Clear();
            textBox1.Text = null;
        }

        private void button5_Click(object sender, EventArgs e) => WriteTo(Paths.Languages + SelectedCultureInfo().Name + ".xml");

        private void WriteTo(string file)
        {
            var x = new XmlDocument();
            x.InnerXml = orig.InnerXml;

            foreach (XmlNode main in x.ChildNodes)
                if (main.Name.ToLower() == "language")
                {
                    foreach (XmlNode section in main.ChildNodes)
                        foreach (XmlNode item in section.ChildNodes)
                        {
                            var oldAttribute = section.Attributes[0];
                            section.RemoveAll();
                            section.Attributes.Append(oldAttribute);
                        }

                    foreach (XmlNode section in main.ChildNodes)
                        for (int i = 0; i < dataGridView1.Rows.Count; i++)
                        {
                            string sectionName = dataGridView1.Rows[i].Cells[0].Value?.ToString();
                            string ID = dataGridView1.Rows[i].Cells[1].Value?.ToString();
                            string originalText = dataGridView1.Rows[i].Cells[2].Value?.ToString();
                            string translatedText = dataGridView1.Rows[i].Cells[3].Value?.ToString().Replace(@"\", @"\\").Replace(Environment.NewLine, @"\n");

                            if (sectionName == section.Attributes[0]?.InnerText && !string.IsNullOrEmpty(translatedText))
                            {
                                XmlNode node = x.CreateNode(XmlNodeType.Element, ID, "");
                                node.InnerText = translatedText;
                                section.AppendChild(node);
                            }
                        }

                    StringBuilder sb = new StringBuilder();

                    XmlWriterSettings opts = new XmlWriterSettings
                    {
                        Indent = true,
                        IndentChars = "\t",
                        Encoding = Encoding.BigEndianUnicode
                    };

                    using (XmlWriter w = XmlWriter.Create(sb, opts))
                    {
                        x.Save(w);

                        var output = sb.ToString().Split(new string[] { "\r\n" }, StringSplitOptions.None);
                        if (output[0].Contains("xml version=")) output[0] = orig.ChildNodes[0].OuterXml;
                        File.WriteAllLines(file, output);
                    }
                }
        }

        private void UpdateList()
        {
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
                dataGridView1.Rows[i].Cells[3].Style.BackColor = dataGridView1.Rows[i].Cells[3].Value == null ? Color.MistyRose : Color.White;
        }

        private void dataGridView1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e) => UpdateList();
        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e) => UpdateList();

        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            try { textBox1.Text = null; textBox1.Text = dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString(); } catch { }
        }

        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                foreach (DataGridViewTextBoxCell x in (sender as DataGridView).SelectedCells)
                {
                    if (x.ColumnIndex == 3) x.Value = null;
                }
            }
        }
    }
}