using System;
using System.Linq;
using System.Windows.Forms;

namespace RH.Core.Controls
{
    public partial class frmControlBox : Form
    {
        public frmControlBox()
        {
            InitializeComponent();
        }

        private void frmControlBox_Shown(object sender, EventArgs e)
        {
            var control = Controls.Cast<Control>().FirstOrDefault(c => !(c is Button));
            if (control != null)
                control.Focus();
        }
    }
}