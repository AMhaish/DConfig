using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;
using System.Xml;
using System.Collections;
using System.IO;
using System.Web.Configuration;
using System.Web.UI.HtmlControls;
using System.Web.Caching;
using System.ComponentModel;

namespace SABFramework.Core.DataCore
{
    public class Module
    {
        [XmlAttribute("namespace")]
        public string Namespace { get; set; }
        [XmlAttribute("path")]
        public string Path { get; set; }
        [XmlAttribute("configPath")]
        public string ConfigPath { get; set; }
        [XmlIgnore]
        public string PhysicalPath { get; set; }

        [XmlAttribute("viewsPath")]
        public string ViewsPath { get; set; }

        [XmlAttribute("builtIn")]
        [DefaultValue(false)]
        public bool BuiltIn { get; set; }

        [XmlAttribute("override")]
        [DefaultValue(false)]
        public bool Override { get; set; }
    }
}
