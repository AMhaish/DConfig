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
    [XmlRoot("sabConfig")]
    public class SABConfig
    {
        [XmlArray("modules")]
        [XmlArrayItem("module", typeof(Module))]
        public List<Module> Modules { get; set; }

        [XmlArray("routes")]
        [XmlArrayItem("route", typeof(Route))]
        public List<Route> Routes { get; set; }

        [XmlArray("settings")]
        [XmlArrayItem("settingsRecord", typeof(SettingsRecord))]
        public List<SettingsRecord> Settings { get; set; }

        public static SABConfig DeSerialize(string XmlString)
        {
            SABConfig sabConfig = new SABConfig();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(XmlString);
            XmlNodeReader reader = new XmlNodeReader(doc.DocumentElement);
            XmlSerializer ser = new XmlSerializer(sabConfig.GetType());
            object obj = ser.Deserialize(reader);
            sabConfig = (SABConfig)obj;
            return sabConfig;
        }

        public string Serialize()
        {
            try
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
            catch (Exception ex)
            {
                SABCoreEngine.Instance.ErrorHandler.ProcessError("Error in parsing SABConfig file, error:", ex);
                return null;
            }
        }
    }
}
