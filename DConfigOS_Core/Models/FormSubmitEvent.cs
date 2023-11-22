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
    public abstract class FormSubmitEvent : SABFramework.PreDefinedModules.MembershipModule.Models.SABCoreEntity
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        [Index()]
        [Required]
        public int FormId { get; set; }
        [JsonIgnore]
        public virtual Form Form { get; set; }
        public DateTime? CreateDate { get; set; }
        [Required]
        public string Type { get; set; }
        [JsonIgnore]
        public virtual FormSubmitEventType TypeObj { get; set; }

        public string CreatorId { get; set; }
        [JsonIgnore]
        public virtual SABFramework.PreDefinedModules.MembershipModule.Models.ApplicationUser Creator { get; set; }
    }
}
