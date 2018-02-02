using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace TextEditor
{
    public partial class Form1 : Form
    {
        private class MyRichTextBox : RichTextBox
        {
            public string FilePath { get; set; }
            public string FileName { get; set; }
        }

        public Form1()
        {
            InitializeComponent();
            NewTab();
        }

        private MyRichTextBox CurrentDocument()
        {
            if (TabControl.TabCount == 0)
            {
                NewTab();
            }
                
            return TabControl.SelectedTab.Controls["Body"] as MyRichTextBox;
        }

        private DialogResult UserInput()
        {
            using (var Dialog = new UserDialogWindow())
            {
                Dialog.ShowDialog();

                switch (Dialog.DialogResult)
                {
                    case DialogResult.Yes: return DialogResult.Yes;
                    case DialogResult.No: return DialogResult.No;
                    default: return DialogResult.Cancel;
                }
            }
        }

        private void DocumentModified()
        {
            CurrentDocument().Parent.Text = CurrentDocument().FileName + " *";
        }

        private void UpdateDocumentStatus()
        {
            CurrentDocument().Modified = false;
            CurrentDocument().Parent.Text = CurrentDocument().FileName;
        }

        private int WordCount(string text)
        {
            string pattern = @"\w+";

            return Regex.Matches(text, pattern).Count;
        }

        private void UpdateDocumentStatistics()
        {
            string pattern = @"\S";

            ToolStripStatusLabelWords.Text = "Words: " + WordCount(CurrentDocument().Text);
            ToolStripStatusLabelLines.Text = "Lines: " + CurrentDocument().Lines.Length;
            ToolStripStatusLabelCharacters.Text = "Characters: " + CurrentDocument().Text.Length;
            ToolStripStatusLabelCharactersNoWhiteSpace.Text = "Characters (no ws): " + Regex.Matches(CurrentDocument().Text, pattern).Count;
        }

        private void CloseTab()
        {
            if (CurrentDocument().Modified)
            {
                switch (UserInput())
                {
                    case DialogResult.Yes: SaveFileAs(); TabControl.Controls.Remove(CurrentDocument().Parent); break;
                    case DialogResult.No: TabControl.Controls.Remove(CurrentDocument().Parent); break;
                }
            }
            else
            {
                TabControl.Controls.Remove(CurrentDocument().Parent);
            }
        }

        private void NewTab()
        {
            var Tab = new TabPage
            {
                Text = "New document",
                Parent = TabControl,
            };

            var RichTextBox = new MyRichTextBox
            {
                FileName = "New document",
                Dock = DockStyle.Fill,
                Name = "Body",
                Parent = Tab,
                ContextMenuStrip = contextMenuStrip1,
                AllowDrop = true,
            };

            RichTextBox.KeyDown += new KeyEventHandler(RichTextBox_KeyDown);
            RichTextBox.TextChanged += new EventHandler(RichTextBox_TextChanged);
            RichTextBox.DragDrop += new DragEventHandler(RichTextBox_DragDrop);
            RichTextBox.DragEnter += new DragEventHandler(richTextBox1_DragEnter);

            TabControl.SelectTab(TabControl.TabCount - 1);
        }

        private void OpenFile()
        {
            var Document = CurrentDocument();

            var Dialog = new OpenFileDialog
            {
                Filter = "Text|*.txt"
            };

            if (Dialog.ShowDialog() == DialogResult.OK)
            {
                Document.Text = File.ReadAllText(Dialog.FileName);
                Document.FilePath = Dialog.FileName;
                Document.FileName = Dialog.SafeFileName;
                Document.Parent.Text = Document.FileName;

                UpdateDocumentStatus();
                UpdateDocumentStatistics();
            }
        }

        private void SaveFile()
        {
            if (CurrentDocument().FilePath != null)
            {
                File.WriteAllText(CurrentDocument().FilePath, CurrentDocument().Text);

                UpdateDocumentStatus();
            }
            else
            {
                SaveFileAs();
            }
        }

        private void SaveFileAs()
        {
            var Document = CurrentDocument();

            var Dialog = new SaveFileDialog
            {
                Filter = "Text|*.txt",
                FileName = CurrentDocument().FileName,
            };

            if (Dialog.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(Dialog.FileName, CurrentDocument().Text);

                CurrentDocument().FilePath = Dialog.FileName;
                CurrentDocument().Parent.Text = Path.GetFileName(Dialog.FileName);

                UpdateDocumentStatus();
            }
        }

        #region MenuEvents

        private void MenuItem_NewTab_Click(object sender, EventArgs e)
        {
            NewTab();
        }

        private void MenuItem_Open_Click(object sender, EventArgs e)
        {
            if (CurrentDocument().Modified)
            {
                switch(UserInput())
                {
                    case DialogResult.Yes: SaveFileAs(); OpenFile();  break;
                    case DialogResult.No:  OpenFile();                break;
                }
            }
            else
            {
                OpenFile();
            }
        }

        private void MenuItem_OpenInNew_Click(object sender, EventArgs e)
        {
            NewTab();
            OpenFile();
        }

        private void MenuItem_Save_Click(object sender, EventArgs e)
        {
            SaveFile();
        }

        private void MenuItem_SaveAs_Click(object sender, EventArgs e)
        {
            SaveFileAs();
        }

        private void MenuItem_Exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        #endregion

        #region ContextMenuEvents

        private void ContextMenuItem_Close_Click(object sender, EventArgs e)
        {
            CloseTab();
        }

        private void ContextMenuItem_Copy_Click(object sender, EventArgs e)
        {
            CurrentDocument().Copy();
        }

        private void ContextMenuItem_Undo_Click(object sender, EventArgs e)
        {
            CurrentDocument().Undo();
        }
        
        private void ContextMenuItem_Cut_Click(object sender, EventArgs e)
        {
            CurrentDocument().Cut();
        }

        private void ContextMenuItem_Paste_Click(object sender, EventArgs e)
        {
            CurrentDocument().Paste();
        }

        #endregion

        #region RichTextBoxEvents

        private void RichTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control)
            {
                switch (e.KeyCode)
                {
                    case Keys.T: NewTab(); break;
                    case Keys.S: SaveFile(); break;
                    case Keys.W: CloseTab(); break;
                }
            }
        }

        private void RichTextBox_TextChanged(object sender, EventArgs e)
        {
            DocumentModified();
            UpdateDocumentStatistics();
        }

        private void richTextBox1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Text))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void RichTextBox_DragDrop(object sender, DragEventArgs e)
        {
            int i;
            String s;

            // Get start position to drop the text.  
            i = CurrentDocument().SelectionStart;
            s = CurrentDocument().Text.Substring(i);
            CurrentDocument().Text = CurrentDocument().Text.Substring(0, i);

            // Drop the text on to the RichTextBox.  
            CurrentDocument().Text = CurrentDocument().Text +
            e.Data.GetData(DataFormats.Text).ToString();
            CurrentDocument().Text = CurrentDocument().Text + s;
        }

        #endregion

        #region TabControlEvents

        private void TabControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control)
            {
                if (e.KeyCode == Keys.T)
                {
                    NewTab();
                }
            }
        }

        #endregion

        /*
         
        Funktionalitet för att kunna dra in en fil i fönstret som då öppnas (”drag and drop”). OBS Filen ska droppas in i fönstret, ej på listen. När detta sker och en fil redan finns öppen i fönstret ska:
o – om ”Ctrl” hålls nedtryckt då filen dras in och släpps på textfönstret, lägga till innehållet i den indragna filen sist i aktuell text.
o –Om ”shift” hålls nedtryckt då filen dras in och släpps på textfönstret, lägga till innehållet i den indragna filen vid markörens plats i aktuell text.
o Om ingen tangent hålls nedtryckt: Stänga och eventuellt spara (till befintlig fil om den är ändrad) aktuell text via dialogruta innan den nya filen öppnas. Möjligheten ska här också finnas att avbryta öppnandet av den nya filen.

         */
    }
}
