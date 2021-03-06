// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using O2.Interfaces.Views;
using O2.Kernel.CodeUtils;
using System.Threading;
using O2.UnitTests.Test_O2Kernel;

namespace O2.UnitTests.Test_O2Kernel.Test_O2Messages
{
    [TestFixture]
    public class Test_AscxGui
    {
        [Test]
        public void test_openAscxGui()
        {
            DI.log.info("Opening AscxGui");
            O2Messages.openAscxGui();
            DI.log.info("Opening Ascxs");
            O2Messages.openControlInGUI("ascx_LogViewer O2_External_WinFormsUI",O2DockState.DockTop,"Extra Log Viewer");
            O2Messages.openControlInGUI("ascx_Scripts O2_Views_ASCX", O2DockState.DockRight, "O2 Scripts");
            Thread.Sleep(2000);
            DI.log.info("Waiting for AscxGui close");
            O2Messages.closeAscxGui();    
            //O2Messages.waitForAscxGuiEnd();
            DI.log.info("Closed AscxGui");
        }
    }
}
