// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using O2.Core.CIR.Xsd;
using O2.Interfaces.CIR;

namespace O2.Core.CIR.CirObjects
{
    [Serializable]
    public class SsaVariable : ICirSsaVariable
    {
        public String sBaseName { get; set;}
        public String sName { get; set; }
        public String sPrintableType { get; set; }
        public String sSymbolDef { get; set; }
        public String sSymbolRef { get; set; }

        public SsaVariable(ControlFlowGraphSsaVariable cfgSsaVariable)
        {
            sBaseName = cfgSsaVariable.BaseName;
            sName = cfgSsaVariable.Name;
            sPrintableType = cfgSsaVariable.PrintableType;
            sSymbolDef = cfgSsaVariable.SymbolDef;
            sSymbolRef = cfgSsaVariable.SymbolRef;
        }
    }
}
