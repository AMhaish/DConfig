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
    public class Area
    {
        [XmlAttribute("name")]
        public string Name { get; set; }
        [XmlAttribute("urlRoute")]
        public string UrlRoute { get; set; }

        [XmlArray("controllers")]
        [XmlArrayItem("controller", typeof(Controller))]
        public List<Controller> Controllers { get; set; }
        [XmlIgnore]
        public Dictionary<string, Controller> ControllersDic { get; set; }
        [XmlIgnore]
        public Module AreaModule { get; set; }
    }
}
