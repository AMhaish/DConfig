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

namespace SABFramework.Core.DataCore
{
    public class Role
    {
        [XmlAttribute("id")]
        public int id { get; set; }
        [XmlAttribute("name")]
        public string Name { get; set; }
    }
}
