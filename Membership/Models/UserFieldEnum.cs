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
    public class UserFieldEnum : SABFramework.PreDefinedModules.MembershipModule.Models.SABCoreEntity
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        public virtual ICollection<UserFieldEnumValue> Values { get; set; }
        [JsonIgnore]
        public virtual ICollection<UserField> UserFields { get; set; }
    }
}
