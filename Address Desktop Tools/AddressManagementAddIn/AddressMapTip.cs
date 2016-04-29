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
    public partial class AddressMapTip : Form
    {
        public AddressMapTip()
        {
            InitializeComponent();
        }

        public void SetLabel(string text)
        {
            this.mapTipLabel.Text = text;
        }
    }
}
