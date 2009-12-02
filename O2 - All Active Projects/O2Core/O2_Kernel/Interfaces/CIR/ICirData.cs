using System.Collections.Generic;

namespace O2.Kernel.Interfaces.CIR
{
    public interface ICirData
    {        
        Dictionary<string, ICirClass> dClasses_bySignature { get; set; }        
        Dictionary<string, ICirFunction> dFunctions_bySignature { get; set; }


        bool bStoreControlFlowBlockRawDataInsideCirDataFile { get; set; }
        bool bVerbose { get; set; }
        Dictionary<string, ICirFunction> dTemp_Functions_bySymbolDef { get; set; }

        //Dictionary<string, ICirFunction> dFunctions_bySymbolDef { get; set; }       // temp dictionary
        //Dictionary<string, ICirClass> dClasses_bySymbolDef { get; set; }
        Dictionary<string, string> dSymbols { get; set; }
        List<string> lFiles { get; set; }
        string sDbId { get; set; }

        ICirClass getClass(string cirClassToFind);
        ICirClass getClass(string cirClassToFind, bool exactMatch, bool createOnNotMatch);
        ICirClass addClass(string newClassSignature);
        void remapXRefs();
    }
}