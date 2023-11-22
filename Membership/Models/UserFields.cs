using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using DConfigOS_Core.Models;

namespace Membership.Models
{
    public class UserField : SABFramework.PreDefinedModules.MembershipModule.Models.SABCoreEntity
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        [Required]
        [MaxLength(50)]
        public string Type { get; set; }
        public virtual FieldsType TypeObj { get; set; }
        [JsonIgnore]
        public virtual ICollection<UserFieldsValue> FieldValues { get; set; }
        [NotMapped]
        public string Value { get; set; }
        public int? EnumId { get; set; }
        public virtual UserFieldEnum Enum { get; set; }
        [Required]
        public int Priority { get; set; }
    }
}
