using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using O2.Kernel.Interfaces.O2Findings;

namespace O2.DotNetWrappers.O2Findings
{
    public class OzasmtDiff
    {
        public static List<IO2Finding> returnListOfFindingsThatMatch(List<IO2Finding> o2FindindsA, List<IO2Finding> o2FindindsB )
        {
            var results = (from O2Finding o2FindingA in o2FindindsA 
                          from O2Finding o2FindingB in o2FindindsB
                          where (o2FindingA.Source == o2FindingB.Source)
                           select (IO2Finding)o2FindingA).ToList();
            return results;
            //var findingsInA = from O2Finding o2Finding in o2FindindsA where o2FindindsA.Source!="" and o2FindindsA.Sink != ""
            //    select new {o2Finding.Sou} 
            return new List<IO2Finding>();
        }
    }
}