﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using O2.DotNetWrappers.DotNet;
using O2.Kernel.CodeUtils;
using O2.Kernel.Interfaces.DotNet;

namespace O2.Core.CIR.Ascx.DotNet
{
    public partial class ascx_GAC_Browser : UserControl
    {
        public ascx_GAC_Browser()
        {
            InitializeComponent();
        }        

        private void llBackUpGAC_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            backupGac(getBackupFilePath(directory_ToBackupGAC.getCurrentDirectory()));
        }

        private void tvListOfGacAssemblies_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (tvListOfGacAssemblies.SelectedNode != null && tvListOfGacAssemblies.SelectedNode.Tag is IGacDll)
                Callbacks.raiseRegistedCallbacks(_onGacDllSelected, new object[]
                                                                        {
                                                                                (IGacDll) tvListOfGacAssemblies.SelectedNode.Tag
                                                                        });
        }
        
    }
}