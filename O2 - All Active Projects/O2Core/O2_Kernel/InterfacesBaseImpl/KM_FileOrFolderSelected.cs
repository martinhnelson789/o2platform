using System;
using O2.Kernel.Interfaces.Messages;

namespace O2.Kernel.InterfacesBaseImpl
{
    public class KM_FileOrFolderSelected : KO2Message, IM_FileOrFolderSelected
    {        
        public string pathToFileOrFolder { get; set; }
        public int lineNumber { get; set; }

        public KM_FileOrFolderSelected(string _pathToFileOrFolder)
        {
            messageText = "KM_FileOrFolderSelected";
            pathToFileOrFolder = _pathToFileOrFolder;
        }        

        public KM_FileOrFolderSelected(string _pathToFileOrFolder, int _lineNumber)
        {
            messageText = "KM_OpenControlInGUI";
            pathToFileOrFolder = _pathToFileOrFolder;
            lineNumber = _lineNumber;
        }

    }
}