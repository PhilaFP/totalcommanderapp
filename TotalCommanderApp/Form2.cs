using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TotalCommanderApp
{
    public partial class Form2 : Form
    {

        public Form2()
        {
            InitializeComponent();
        }

        public ProgressBar GetProgressBar()
        {
            return progressBar1;
        }

        public void SetProgressBarValue(int value)
        {
            progressBar1.Value = value;
        }

    }
}
