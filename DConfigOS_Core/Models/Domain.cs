using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Xml.Serialization;
using System.Xml;
using DConfigOS_Core.Providers.HttpContextProviders;

namespace DConfigOS_Core.Models
{
    public class Domain : SABFramework.PreDefinedModules.MembershipModule.Models.SABCoreEntity
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(500)]
        public string DomainAliases { get; set; }
        [JsonIgnore]
        public virtual ICollection<Content> DomainContents { get; set; }

        public string CreatorId { get; set; }
        [JsonIgnore]
        public virtual SABFramework.PreDefinedModules.MembershipModule.Models.ApplicationUser Creator { get; set; }
    }
}
