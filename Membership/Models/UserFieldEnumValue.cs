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
    public class UserFieldEnumValue
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [Index()]
        public int EnumId { get; set; }
        [JsonIgnore]
        public virtual UserFieldEnum Enum { get; set; }
        public string Value { get; set; }
    }
}
