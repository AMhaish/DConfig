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
    public class ViewFieldsEnum : SABFramework.PreDefinedModules.MembershipModule.Models.SABCoreEntity
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        [XmlAttribute("name")]
        public string Name { get; set; }
        public virtual ICollection<ViewFieldsEnumValue> Values { get; set; }
        [JsonIgnore]
        public virtual ICollection<ViewField> ViewFields { get; set; }

        public string CreatorId { get; set; }
        [JsonIgnore]
        public virtual SABFramework.PreDefinedModules.MembershipModule.Models.ApplicationUser Creator { get; set; }
    }
}
