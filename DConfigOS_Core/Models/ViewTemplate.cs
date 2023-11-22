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
    public class ViewTemplate : SABFramework.PreDefinedModules.MembershipModule.Models.SABCoreEntity
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        [XmlAttribute("name")]
        public string Name { get; set; }
        [MaxLength(500)]
        [XmlAttribute("path")]
        public string Path { get; set; }
        [Index()]
        public int? LayoutTemplateId { get; set; }
        [JsonIgnore]
        public virtual ViewTemplate LayoutTemplate { get; set; }
        public int? PublicViewsPackageId { get; set; }
        //[ForeignKey("PublicViewsPackageId")]
        [JsonIgnore]
        public virtual PublicViewsPackage PublicViewsPackage { get; set; }
        [XmlArray("templates")]
        [XmlArrayItem("template", typeof(ViewTemplate))]
        [JsonIgnore]
        public virtual ICollection<ViewTemplate> ChildrenTemplates { get; set; }
        [JsonIgnore]
        public virtual ICollection<ContentInstance> TemplateContentInstances { get; set; }
        [XmlAttribute("isActive")]
        public bool IsActive { get; set; }
        public int? ViewTypeId { get; set; }
        //[ForeignKey("ViewTypeId")]
        [JsonIgnore]
        public virtual ViewType ViewType { get; set; }
        [XmlAttribute("isContainer")]
        public bool IsContainer { get; set; }

        public string CreatorId { get; set; }
        [JsonIgnore]
        public virtual SABFramework.PreDefinedModules.MembershipModule.Models.ApplicationUser Creator { get; set; }

    }
}
