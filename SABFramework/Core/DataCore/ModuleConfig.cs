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
    [XmlRoot("moduleConfig")]
    public class ModuleConfig
    {
        [XmlAttribute("initializer")]
        public string Initializer { get; set; }

        [XmlArray("areas")]
        [XmlArrayItem("area", typeof(Area))]
        public List<Area> Areas { get; set; }

        [XmlArray("controllers")]
        [XmlArrayItem("controller", typeof(Controller))]
        public List<Controller> Controllers { get; set; }

        [XmlArray("routes")]
        [XmlArrayItem("route", typeof(Route))]
        public List<Route> Routes { get; set; }

        [XmlArray("settings")]
        [XmlArrayItem("settingsRecord", typeof(SettingsRecord))]
        public List<SettingsRecord> Settings { get; set; }

        public static ModuleConfig DeSerialize(string XmlString)
        {
            try
            {

                ModuleConfig moduleConfig = new ModuleConfig();
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(XmlString);
                XmlNodeReader reader = new XmlNodeReader(doc.DocumentElement);
                XmlSerializer ser = new XmlSerializer(moduleConfig.GetType());
                object obj = ser.Deserialize(reader);
                moduleConfig = (ModuleConfig)obj;
                foreach (Area a in moduleConfig.Areas)
                {
                    a.ControllersDic = a.Controllers.ToDictionary(m => m.Name.ToLower());
                    foreach (Controller c in a.Controllers)
                    {
                        c.Area = a;
                        c.ActionsDic = c.Actions.ToDictionary(m => m.Name.ToLower() + m.RequestType);
                        foreach (Action ac in c.Actions)
                        {
                            ac.Controller = c;
                        }
                    }
                }
                foreach (Controller c in moduleConfig.Controllers)
                {
                    c.ActionsDic = c.Actions.ToDictionary(m => m.Name.ToLower() + m.RequestType);
                    foreach (Action ac in c.Actions)
                    {
                        ac.Controller = c;
                    }
                }
                return moduleConfig;
            }
            catch (Exception ex)
            {
                SABCoreEngine.Instance.ErrorHandler.ProcessError("Error in parsing Module Config file, error:", ex);
                return null;
            }
        }

        public string Serialize()
        {
            StringBuilder XMLRes = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings();
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            settings.OmitXmlDeclaration = true;
            using (XmlWriter writer = XmlWriter.Create(XMLRes, settings))
            {
                XmlSerializer xmlSer = new XmlSerializer(typeof(SABConfig));
                xmlSer.Serialize(writer, this, ns);
            }
            return XMLRes.ToString();
        }
    }
}
