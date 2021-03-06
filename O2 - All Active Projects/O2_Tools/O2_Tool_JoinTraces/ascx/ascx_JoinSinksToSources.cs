// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace O2.Tool.JoinTraces.ascx
{
    public partial class ascx_JoinSinksToSources : UserControl
    {
        public ascx_JoinSinksToSources()
        {
            InitializeComponent();
        }

        private void ascx_JoinSinksToSources_Load(object sender, EventArgs e)
        {
            onLoad();
        }

        private void btCalculateJoinnedTraces_Click(object sender, EventArgs e)
        {
            btCalculateJoinnedTraces.Enabled = false;
            calculateJoinnedTraces();
            btCalculateJoinnedTraces.Enabled = true;
        }        
        
    }
}
    
