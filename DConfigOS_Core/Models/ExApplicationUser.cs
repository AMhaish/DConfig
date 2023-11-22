using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SABFramework.PreDefinedModules.MembershipModule.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace DConfigOS_Core.Models
{
    public class ExApplicationUser: SABCoreEntity
    {
        [Key]
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser User{get;set;}

        [MaxLength(500)]
        public string ResourcesRootPath { get; set; }
    }
}
