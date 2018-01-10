using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Compiler
{ 
    public partial class Form1 : Form
    {
        List<string> keywords;

        string text; // for copy, paste, insert
        bool isCreated;
        string lastPath;

        public Form1()
        {
            InitializeComponent();

            InitialKeyWords();

            text = null;
            isCreated = false;
            lastPath = null;

            richTextBox1.AllowDrop = true;
            richTextBox1.DragEnter += RichTextBox1_DragEnter;
            richTextBox1.DragDrop += RichTextBox1_DragDrop;
        }

        private void RichTextBox1_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            foreach (string file in files)
            {
                if (file.EndsWith(".cs"))
                {
                    if (richTextBox1.Text != string.Empty)
                    {
                        richTextBox1.Text = "";
                        richTextBox1.Text = File.ReadAllText(file);
                        CheckWords();
                    }
                }
            }
        }

        private void RichTextBox1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false) == true)
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void InitialKeyWords()
        {
            keywords = new List<string> { "abstract", "as", "base", "bool", "break", "byte", "case",
"catch", "char", "checked", "class", "const", "continue", "decimal", "default", "delegate", "do", "double",
"else", "enum", "event", "explicit", "extern", "false", "finally", "fixed", "float", "for", "foreach", "goto",
"if", "implicit", "in", "int", "interface", "namespace", "internal", "is", "lock", "long", "namespace", "new",
"null", "object", "operator", "out", "override", "params", "private", "protected", "public", "readonly", "ref",
"return", "sbyte", "sealed", "short", "sizeof", "stackalloc", "static", "string", "struct", "switch", "this",
"throw", "true", "try ", "typeof", "uint", "ulong", "unchecked", "unsafe", "ushort", "using", "static",
"virtual", "void", "volatile", "while", "add", "alias", "ascending", "async", "await", "descending", "dynamic",
"from", "get", "global", "group", "into", "join", "let", "nameof", "orderby", "partial", "remove", "select", "set",
"value", "var", "when", "where", "yield" };
        }

        /// <summary>
        /// About
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Bla-bla-bla v1.0", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Copy
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            text = richTextBox1.SelectedText;
        }

        /// <summary>
        /// CutOut
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            text = richTextBox1.SelectedText;
            richTextBox1.Text = richTextBox1.Text.Remove(richTextBox1.SelectionStart, text.Length);
        }

        /// <summary>
        /// Insert
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void insertToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = richTextBox1.Text.Insert(richTextBox1.SelectionStart, text);
        }

        /// <summary>
        /// Exit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Сохранить изменения в файле?", null, MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (dialogResult == DialogResult.Yes)
            {
                if (lastPath != null)
                {
                    File.WriteAllText(lastPath, string.Empty);
                    using (Stream stream = new FileStream(lastPath, FileMode.Append, FileAccess.Write))
                    {
                        using (StreamWriter writer = new StreamWriter(stream))
                        {
                            writer.Write(richTextBox1.Text);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Прежде чем сохранить файл, его нужно создать!");
                }
            }
            else
            {
                Close();
            }
        }

        /// <summary>
        /// Open file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (richTextBox1.Text != string.Empty)
                {
                    richTextBox1.Text = "";
                }
                if (openFileDialog.FileName.EndsWith(".cs"))
                {
                    richTextBox1.Text = File.ReadAllText(openFileDialog.FileName);
                    lastPath = openFileDialog.FileName;
                    CheckWords();
                }
                else
                {
                    richTextBox1.Text = File.ReadAllText(openFileDialog.FileName);
                    lastPath = openFileDialog.FileName;
                    MessageBox.Show("Чтобы работала подсветка слов, необходимо открыть файл с расширением .cs");
                }
            }
        }

        /// <summary>
        /// Save file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            if (lastPath != null)
            {
                isCreated = true;
                if (isCreated)
                {
                    using (Stream stream = new FileStream(lastPath, FileMode.Create, FileAccess.Write))
                    {
                        using (StreamWriter writer = new StreamWriter(stream))
                        {
                            for (int i = 0; i < richTextBox1.Lines.Length; i++)
                            {
                                writer.WriteLine(richTextBox1.Lines[i]);
                            }
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Нужно создать файл!");
            }
        }

        /// <summary>
        /// Save as
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "All txt files(*.txt)|*.txt|C# files(*.cs)|*.cs";
            saveFileDialog.DefaultExt = ".txt";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (saveFileDialog.CheckPathExists)
                {
                    using (Stream stream = new FileStream(saveFileDialog.FileName, FileMode.Create, FileAccess.Write))
                    {
                        using (StreamWriter writer = new StreamWriter(stream))
                        {
                            for (int i = 0; i < richTextBox1.Lines.Length; i++)
                            {
                                writer.WriteLine(richTextBox1.Lines[i]);
                            }

                            isCreated = true;
                            lastPath = saveFileDialog.FileName;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Check all words on an equality 
        /// </summary>
        private void CheckWords()
        {
            MatchCollection matchCollection = Regex.Matches(richTextBox1.Text, @"\w+");
            foreach (Match match in matchCollection)
            {
                foreach (string word in keywords)
                {
                    if (word == match.Value)
                    {
                        richTextBox1.SelectionStart = match.Index;
                        richTextBox1.SelectionLength = match.Length;
                        richTextBox1.SelectionColor = Color.OrangeRed;
                        richTextBox1.DeselectAll();
                        richTextBox1.Update();
                    }
                }
            }

            MatchCollection matchClasses = Regex.Matches(richTextBox1.Text, @"class (.+[^\W])");
            foreach (Match match in matchClasses)
            {
                richTextBox1.SelectionStart = match.Groups[1].Index;
                richTextBox1.SelectionLength = match.Groups[1].Length;
                richTextBox1.SelectionColor = Color.Aqua;
                richTextBox1.DeselectAll();
            }
        }

    }

}
