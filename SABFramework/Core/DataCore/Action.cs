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
using Newtonsoft.Json;

namespace SABFramework.Core.DataCore
{
    public class Action
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("requestType")]
        public string RequestType { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public Controller Controller { get; set; }

        [XmlAttribute("handlerTypeName")]
        public string HandlerTypeName { get; set; }


        [XmlAttribute("viewPath")]
        public string ViewPath {get;set;}


        [XmlAttribute("redirectPath")]
        public string RedirectPath { get; set; }

        //[XmlAttribute("needAuthorize")]
        //public bool NeedAuthorize { get; set; }

        [XmlAttribute("cacheMinutes")]
        [DefaultValue(0)]
        public int CacheMinutes { get; set; }

        [XmlIgnore]
        public Module ActionModule
        {
            get
            {
                if (Controller != null)
                {
                    if (Controller.Area != null)
                    {
                        return Controller.Area.AreaModule;
                    }
                    else
                    {
                        return Controller.ControllerModule;
                    }
                }
                else
                {
                    return null;
                }
            }
        }
        //[XmlAttribute("defaultAction")]
        //public bool DefaultAction { get; set; }

        //[XmlArray("actionRoles")]
        //[XmlArrayItem("actionRole", typeof(ActionRole))]
        //public List<ActionRole> ActionRoles { get; set; }
        //[XmlIgnore]
        //public HashSet<string> ActionRolesNames { get; set; }

        [XmlAttribute("description")]
        public String Description { get; set; }
    }
}
