using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Calculator
{
    public partial class Form1 : Form
    {
        private double PreviousValue;
        private double Sum;
        private string LastOperation;

        public Form1()
        {
            InitializeComponent();
        }

        private void Numpad_Click(object sender, EventArgs e)
        {
            var Button = sender as Button;

            if (EntryField.Text.Length < 10)
            { 
                EntryField.Text = string.Concat(EntryField.Text, Button.Text);
            }
        }

        private void Numpad_Operation(object sender, EventArgs e)
        {
            var Button = sender as Button;

            try
            {
                switch (LastOperation)
                {
                    case "+": PreviousValue += Double.Parse(EntryField.Text); break;
                    case "-": PreviousValue -= Double.Parse(EntryField.Text); break;
                    case "/": PreviousValue /= Double.Parse(EntryField.Text); break;
                    case "*": PreviousValue *= Double.Parse(EntryField.Text); break;
                    default: PreviousValue = Double.Parse(EntryField.Text); break;
                }

                PreviousEntryField.Text = string.Concat(PreviousEntryField.Text, string.Concat(EntryField.Text, " " + Button.Text + " "));

                LastOperation = Button.Text;

                EntryField.Text = "";
            }
            catch
            {
                PreviousValue = 0;
                EntryField.Text = "Invalid Input";
                LastOperation = null;
            }    
        }

        private void Numpad_Operation_Equals(object sender, EventArgs e)
        {
            switch (LastOperation)
            {
                case "+": Sum = PreviousValue + Double.Parse(EntryField.Text); break;
                case "-": Sum = PreviousValue - Double.Parse(EntryField.Text); break;
                case "/": Sum = PreviousValue / Double.Parse(EntryField.Text); break; 
                case "*": Sum = PreviousValue * Double.Parse(EntryField.Text); break;
            }

            PreviousEntryField.Text = "";

            LastOperation = null;

            EntryField.Text = Sum.ToString();
        }
    }
}
