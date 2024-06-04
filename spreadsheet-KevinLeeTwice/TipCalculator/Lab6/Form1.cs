using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab6
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (EnterValue.Text.Length != 0 || EnterValue.Text.GetType() == typeof(String))
            {

                double d = 0;
                double tip = 0;
                if (double.TryParse(EnterValue.Text, out d))
                {
                    if (double.TryParse(TipTextBox.Text, out tip))
                    {
                        TotalValue.Text = "" + (d * (tip / 100));
                        TotalAmountTextBox.Text = "" + (d + d * (tip / 100));
                    }
                    else
                    {
                        TotalValue.Text = "" + (d * 0.2);
                    }
                }
            }
        }

        private void EnterValue_TextChanged(object sender, EventArgs e)
        {

        }

        private void TotalValue_TextChanged(object sender, EventArgs e)
        {

        }

        private void EnterTotalBill(object sender, EventArgs e)
        {

        }

        private void CalculateTipButton(object sender, EventArgs e)
        { 
        }

        private bool CheckValidty()
        {
            if (double.TryParse(TotalAmountTextBox.Text, out double unused) || double.TryParse(TipTextBox.Text, out double unused2))
            {
                return ComputeTipButton.Enabled = true;
            }
            else
            {
                return ComputeTipButton.Enabled = false; 
            }
        }

    }
}
