﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by xsd, Version=2.0.50727.1432.
// 
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace O2.DotNetWrappers.Xsd
{
    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false)]
    public class SourceCodeMappings
    {
        /// <remarks/>
        [XmlElement("Mapping")]
        public SourceCodeMappingsMapping[] Mapping { get; set; }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public class SourceCodeMappingsMapping
    {
        /// <remarks/>
        [XmlAttribute]
        public string replaceThisString { get; set; }

        /// <remarks/>
        [XmlAttribute]
        public string withThisString { get; set; }
    }
}