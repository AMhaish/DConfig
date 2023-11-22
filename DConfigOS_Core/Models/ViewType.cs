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
    public class ViewType : SABFramework.PreDefinedModules.MembershipModule.Models.SABCoreEntity
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        [XmlAttribute("name")]
        public string Name { get; set; }
        public int? PublicViewsPackageId { get; set; }
        //[ForeignKey("PublicViewPackageId")]
        [JsonIgnore]
        public virtual PublicViewsPackage PublicViewsPackage { get; set; }
        [JsonIgnore]
        public virtual ICollection<ViewType> ChildrenTypes { get; set; }
        [JsonIgnore]
        public virtual ICollection<ViewType> ParentTypes { get; set; }
        [JsonIgnore]
        public virtual ICollection<Content> TypeContents { get; set; }
        public virtual ICollection<ViewTemplate> TypeTemplates { get; set; }
        [XmlArray("viewTypeFields")]
        [XmlArrayItem("viewTypeField", typeof(ViewField))]
        public virtual ICollection<ViewField> ViewFields { get; set; }

        public string CreatorId { get; set; }
        [JsonIgnore]
        public virtual SABFramework.PreDefinedModules.MembershipModule.Models.ApplicationUser Creator { get; set; }
    }
}
