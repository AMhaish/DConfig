using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using System.Xml.Serialization;
using System.Xml;

namespace DConfigOS_Core.Models
{
    [Serializable]
    [XmlRoot("packageConfig")]
    public class PublicViewsPackage
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        [XmlAttribute("packageName")]
        public string Name { get; set; }
        [MaxLength(10)]
        public string Version { get; set; }
        [Required]
        [MaxLength(500)]
        public string Path { get; set; }
        [NotMapped]
        [JsonIgnore]
        [XmlAttribute("appName")]
        public string appName { get; set; }
        [Index()]
        public int? AppId { get; set; }
        //[ForeignKey("AppId")]
        [JsonIgnore]
        public virtual App App { get; set; }
        [Required]
        public DateTime InstalledDate { get; set; }
        [JsonIgnore]
        [XmlArray("styles")]
        [XmlArrayItem("style", typeof(StyleSheet))]
        public virtual ICollection<StyleSheet> PackageStyleSheets { get; set; }
        [JsonIgnore]
        [XmlArray("scripts")]
        [XmlArrayItem("script", typeof(Script))]
        public virtual ICollection<Script> PackageScripts { get; set; }
        [JsonIgnore]
        [XmlArray("viewTypes")]
        [XmlArrayItem("viewType", typeof(ViewType))]
        public virtual ICollection<ViewType> PackageViewTypes { get; set; }
        [JsonIgnore]
        [XmlArray("templates")]
        [XmlArrayItem("template", typeof(ViewTemplate))]
        public virtual ICollection<ViewTemplate> PackageViewTemplates { get; set; }
        [JsonIgnore]
        [XmlArray("contents")]
        [XmlArrayItem("content", typeof(Content))]
        public virtual ICollection<Content> PackageViewContents { get; set; }
        [NotMapped]
        [JsonIgnore]
        [XmlArray("viewFieldsTypes")]
        [XmlArrayItem("viewFieldType", typeof(FieldsType))]
        public ICollection<FieldsType> ViewFieldTypes { get; set; }

        public static PublicViewsPackage DeSerialize(string XmlString)
        {
            try
            {
                PublicViewsPackage moduleConfig = new PublicViewsPackage();
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(XmlString);
                XmlNodeReader reader = new XmlNodeReader(doc.DocumentElement);
                XmlSerializer ser = new XmlSerializer(moduleConfig.GetType());
                object obj = ser.Deserialize(reader);
                moduleConfig = (PublicViewsPackage)obj;
                //foreach (Area a in moduleConfig.Areas)
                //{
                //    a.ControllersDic = a.Controllers.ToDictionary(m => m.Name.ToLower());
                //    foreach (Controller c in a.Controllers)
                //    {
                //        c.Area = a;
                //        c.ActionsDic = c.Actions.ToDictionary(m => m.Name.ToLower() + m.RequestType);
                //        foreach (Action ac in c.Actions)
                //        {
                //            ac.Controller = c;
                //        }
                //    }
                //}
                //foreach (Controller c in moduleConfig.Controllers)
                //{
                //    c.ActionsDic = c.Actions.ToDictionary(m => m.Name.ToLower() + m.RequestType);
                //    foreach (Action ac in c.Actions)
                //    {
                //        ac.Controller = c;
                //    }
                //}
                return moduleConfig;
            }
            catch (Exception ex)
            {
                SABFramework.Core.SABCoreEngine.Instance.ErrorHandler.ProcessError("Error in parsing Public Package Config file, error:", ex);
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
                XmlSerializer xmlSer = new XmlSerializer(typeof(PublicViewsPackage));
                xmlSer.Serialize(writer, this, ns);
            }
            return XMLRes.ToString();
        }
    }
}
