using System;
using System.IO;
using System.Windows.Forms;

namespace TextEditor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void MenuItem_Open_Click(object sender, EventArgs e)
        {
            var Dialog = new OpenFileDialog();
            Dialog.Filter = "Text|*.txt";

            if (Dialog.ShowDialog() == DialogResult.OK)
            {
                using (var Reader = new StreamReader(Dialog.OpenFile()))
                {
                    richTextBox1.Text = Reader.ReadToEnd();
                }
            }   
        }

        public void MenuItem_Save_Click(object sender, EventArgs e)
        {
            var Dialog = new SaveFileDialog();

            if (Dialog.ShowDialog() == DialogResult.OK)
            {
                using (var Writer = new StreamWriter(Dialog.OpenFile()))
                {
                    Writer.Write(richTextBox1.Text);
                }
            }
        }
    }
}
