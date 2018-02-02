using System;
using System.Windows.Forms;

namespace TextEditor
{
    public partial class UserDialogWindow : Form
    {
        public UserDialogWindow()
        {
            InitializeComponent();
            CenterToParent();
            DialogResult = DialogResult.Cancel;
            CancelButton = button3;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var ButtonAction = sender as Button;

            switch(ButtonAction.DialogResult)
            {
                case DialogResult.Yes:      DialogResult = DialogResult.Yes;        break;
                case DialogResult.No:       DialogResult = DialogResult.No;         break;
                case DialogResult.Cancel:   DialogResult = DialogResult.Cancel;     break;
            } 
        }
    }
}
