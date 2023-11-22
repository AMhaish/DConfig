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
    public class Controller
    {
        [XmlAttribute("name")]
        public string Name { get; set; }
        //[XmlAttribute("urlRoute")]
        //public string UrlRoute { get; set; }

        [XmlArray("actions")]
        [XmlArrayItem("action", typeof(Action))]
        //Shouldn't be used as source of actions because of the override operation that missing filling this collection
        public List<Action> Actions { get; set; }
        [XmlIgnore]
        public Dictionary<string, Action> ActionsDic { get; set; }

        [XmlIgnore]
        public Area Area { get; set; }
        [XmlIgnore]
        public Module ControllerModule { get; set; }

        [XmlAttribute("description")]
        public String Description { get; set; }
    }
}
