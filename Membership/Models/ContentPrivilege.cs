using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using Microsoft.AspNet.Identity.EntityFramework;
using DConfigOS_Core.Models;

namespace Membership.Models
{
    [Serializable]
    public class ContentPrivilege : SABFramework.PreDefinedModules.MembershipModule.Models.SABCoreEntity
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None)]
        public int ContentId { get; set; }
        [JsonIgnore]
        public virtual Content Content { get; set; }
        public virtual ICollection<IdentityRole> Roles { get; set; }
        [Required]
        public bool NeedAuthentication { get; set; }
        [Required]
        public bool NeedAuthorization { get; set; }
        [Required]
        public bool RequireHttps { get; set; }
    }
}
