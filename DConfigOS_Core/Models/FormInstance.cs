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
    public class FormInstance
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Index()]
        public int? FormId { get; set; }
        [JsonIgnore]
        public virtual Form Form { get; set; }
        public DateTime CreateDate { get; set; }
        public virtual ICollection<FormFieldValue> FieldsValues { get; set; }
        [MaxLength(50)]
        public string UserId { get; set; }
        [JsonIgnore]
        public virtual SABFramework.PreDefinedModules.MembershipModule.Models.ApplicationUser User { get; set; }
        [Index()]
        public int? ParentInstanceId { get; set; }
        [JsonIgnore]
        public virtual FormInstance ParentInstance { get; set; }
        [JsonIgnore]
        public virtual ICollection<FormInstance> ChildrenInstances { get; set; }

    }
}
