﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.3053
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace O2.Cmd.SpringMvc.Scripts {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "2.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class CustomScripts {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal CustomScripts() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("O2.Cmd.SpringMvc.Scripts.CustomScripts", typeof(CustomScripts).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to using System;
        ///using System.Collections.Generic;
        ///using System.IO;
        ///using System.Linq;
        ///using O2.DotNetWrappers.DotNet;
        ///using O2.DotNetWrappers.O2CmdShell;
        ///using O2.DotNetWrappers.O2Findings;
        ///using O2.ImportExport.OunceLabs.Ozasmt_OunceV6;
        ///using O2.Kernel.Interfaces.O2Findings;
        ///
        ///namespace O2.Cmd.SpringMvc.Scripts
        ///{
        ///    public class TraceCreator
        ///    {
        ///        public static bool saveTempFindingsAsSeparateAssessmentFiles = false;
        ///
        ///        public static void test()
        ///        {
        ///            O2Cmd.log.write(&quot;Thi [rest of string was truncated]&quot;;.
        /// </summary>
        public static string TraceCreator {
            get {
                return ResourceManager.GetString("TraceCreator", resourceCulture);
            }
        }
    }
}