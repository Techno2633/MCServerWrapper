using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MCServerWrapper.Forms
{
    public partial class IntegerInput : Form
    {
        private int fallbackValue;
        private bool ClosingOk;

        public IntegerInput(int initialValue, int inputMax)
        {
            InitializeComponent();

            fallbackValue = initialValue;
            ClosingOk = false;
            NumUpDown.Maximum = inputMax;
            NumUpDown.Value = initialValue;
            NumUpDown.Select(0, NumUpDown.Value.ToString().Length);
        }

        public int ShowFormDialog()
        {
            this.ShowDialog();
            this.Dispose();
            if (ClosingOk)
            {
                return (int)NumUpDown.Value;
            }
            return fallbackValue;
        }

        private void OK_Click(object sender, EventArgs e)
        {
            ClosingOk = true;
            this.Close();
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            ClosingOk = false;
            this.Close();
        }

        private void NumUpDown_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return || e.KeyCode == Keys.Enter)
                OK_Click(sender, e);
            else if (e.KeyCode == Keys.Escape)
                Cancel_Click(sender, e);
        }
    }
}
