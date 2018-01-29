using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Laboration1
{
    public partial class Lotto : Form
    {
        public Random RandomGenerator = new Random();

        public Lotto()
        {
            InitializeComponent();
        }

        private void Button_Start_Click(object sender, EventArgs e)
        {
            ResetGraphics();

            int Five = 0, Six = 0, Seven = 0;

            List<int> LotteryRow = GetLotteryNumbers();

            for(int i = 0; i < Iterations.Value; i++)
            {
                var List = WinningRow();

                var Matches = LotteryRow.Intersect(List).Count();

                switch (Matches)
                {
                    case 5: Five++; break;
                    case 6: Six++; break;
                    case 7: Seven++; break;
                }
            }

            Correct5.Text = Five.ToString();
            Correct6.Text = Six.ToString();
            Correct7.Text = Seven.ToString();
        }

        private List<int> WinningRow()
        {
            var Stack = new List<int>();
            var WinningRow = new List<int>();
            int Random;
            int RandomRoof = 35;

            for (int i = 1; i < 36; i++)
            {
                Stack.Add(i);
            }

            for (int i = 0; i < 7; i++)
            {
                Random = RandomGenerator.Next(1, RandomRoof);

                WinningRow.Add(Stack.ElementAt(Random));

                Stack.RemoveAt(Random);

                RandomRoof--;
            }

            return WinningRow;
        }

        private void ResetGraphics()
        {
            Correct5.Text = "0";
            Correct6.Text = "0";
            Correct7.Text = "0";
        }

        private List<int> GetLotteryNumbers()
        {
            var List = new List<int>();
            
            List.Add((int)RowNumber1.Value);
            List.Add((int)RowNumber2.Value);
            List.Add((int)RowNumber3.Value);
            List.Add((int)RowNumber4.Value);
            List.Add((int)RowNumber5.Value);
            List.Add((int)RowNumber6.Value);
            List.Add((int)RowNumber7.Value);
            
            return List;
        }
    }
}
