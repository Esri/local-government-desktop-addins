using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;

using System.Windows.Forms;

namespace A4LGAddressManagement
{
    public partial class AutoIncrementWindow : Form
    {
        public AutoIncrementWindow()
        {
            InitializeComponent();
        }

        public AutoIncrementWindow(int value)
        {
            InitializeComponent();
            this.numericUpDown1.Value = value;
        }

        public decimal GetIncrementValue()
        {
            return this.numericUpDown1.Value;
        }
    }
}
