﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using O2.Debugger.Mdbg.Debugging.MdbgEngine;
using O2.Debugger.Mdbg.O2Debugger.Objects;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.Windows;
using O2.Kernel.Interfaces.Messages;

namespace O2.Debugger.Mdbg.O2Debugger.Ascx
{
    public partial class ascx_Variables
    {
        void ascx_CurrentFrameDetails_onMessages(IO2Message o2Message)
        {
            if (o2Message is IM_O2MdbgAction)
            {
                var o2MDbgAction = (IM_O2MdbgAction)o2Message;
                switch (o2MDbgAction.o2MdbgAction)
                {
                    case IM_O2MdbgActions.breakEvent:
                        showFrameVariables();
                        showFrameThreads();
                        showFrameStackTrace();
                        //DI.o2MDbg.executeMDbgCommand(O2MDbgCommands.printLocalVariables());
                        //DI.o2MDbg.executeMDbgCommand("w");
                        break;                    
                }
            }
        }

        private void onVariablesTreeViewBeforeExpand(TreeNode treeNode)
        {
            if (treeNode.Tag != null && treeNode.Tag is O2MDbgVariable)
            {
                O2Thread.mtaThread(() =>
                {
                    var o2MDbgVariable = (O2MDbgVariable)treeNode.Tag;
                    var o2MDbgvariables = DI.o2MDbg.sessionData.getCurrentFrameVariable(o2MDbgVariable);
                    populateTreeNodeCollectionWithVariables(tvVariables, treeNode.Nodes, o2MDbgvariables);
                });
                //  thread.Join();
            }
        }

        public void showFrameVariables()
        {
            var expandDepth = 0;
            var canDoFunceval = true;
            var o2MDbgvariables = DI.o2MDbg.sessionData.getCurrentFrameVariables(expandDepth, canDoFunceval);
            if (o2MDbgvariables.Count > 0)
                populateTreeNodeCollectionWithVariables(tvVariables, tvVariables.Nodes, o2MDbgvariables);
            
        }

        public static void populateTreeNodeCollectionWithVariables(TreeView targetTreeView, TreeNodeCollection nodes, List<O2MDbgVariable> o2MDbgvariables)
        {
            targetTreeView.invokeOnThread(
                () =>
                {
                    nodes.Clear();
                    foreach (var o2MDbgvariable in o2MDbgvariables)
                    {

                        // var nameLvSubItem = new ListViewItem.ListViewSubItem() 
                        var nodeText =
                        string.Format("{0} = {1}  : {2}", o2MDbgvariable.name, o2MDbgvariable.value,
                                      o2MDbgvariable.type);
                        var newTreeNode = O2Forms.newTreeNode(nodes, nodeText, 0, o2MDbgvariable);
                        if (o2MDbgvariable.complexType)
                            newTreeNode.Nodes.Add("DymmyNode");
                    }
                });
        }

        /*lvVariables.invokeOnThread(
                () =>
                    {
        */

        public void showFrameStackTrace()
        {
        }

        public void showFrameThreads()
        {
        }

        private void showVariablesDetails(O2MDbgVariable o2MDbgVariable)
        {
            this.invokeOnThread(
                () =>
                {
                    laVariableType.Text = o2MDbgVariable.type;
                    laVariableFullName.Text = o2MDbgVariable.fullName;
                    tbVariableValue.Text = o2MDbgVariable.value;
                });
        }
        

        private void ascx_Variables_Load(object sender, EventArgs e)
        {
            tvVariables.Sort();
        }
    }
}