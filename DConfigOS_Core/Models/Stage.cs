using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.AspNet.Identity.EntityFramework;

namespace DConfigOS_Core.Models
{
    [Serializable]
    public class Stage : SABFramework.PreDefinedModules.MembershipModule.Models.SABCoreEntity
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        [XmlAttribute("name")]
        public string Name { get; set; }

        [JsonIgnore]
        public virtual ICollection<Stage> NextStages { get; set; }

        [JsonIgnore]
        public virtual ICollection<IdentityRole> Roles { get; set; }

        [JsonIgnore]
        public virtual ICollection<Content> Contents{ get; set; }

        [JsonIgnore]
        public virtual ICollection<ContentInstance> ContentInstances { get; set; }
    }
}
