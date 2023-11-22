using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace Membership.Models
{
    public class UserFieldsValue
    {
        [Key]
        [Column(Order = 2)]
        public int FieldId { get; set; }
        [JsonIgnore]
        public virtual UserField Field { get; set; }
        [Key]
        [Column(Order = 1)]
        public string UserId { get; set; }
        [Required]
        [JsonIgnore]
        public virtual SABFramework.PreDefinedModules.MembershipModule.Models.ApplicationUser User { get; set; }
        public string Value { get; set; }
    }
}
