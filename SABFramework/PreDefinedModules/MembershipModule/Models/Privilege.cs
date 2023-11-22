using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using Microsoft.AspNet.Identity.EntityFramework;

namespace SABFramework.PreDefinedModules.MembershipModule.Models
{
    [Serializable]
    public class Privilege
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [Index()]
        [MaxLength(50)]
        public string Controller { get; set; }
        [Index()]
        [MaxLength(50)]
        public string Action { get; set; }
        [MaxLength(50)]
        public string RequestType { get; set; }
        public virtual ICollection<IdentityRole> Roles { get; set; }
        [Required]
        public bool NeedAuthentication { get; set; }
        [Required]
        public bool NeedAuthorization { get; set; }
        [Required]
        public bool RequireHttps { get; set; }
    }
}
