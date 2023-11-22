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
    public class Form : SABFramework.PreDefinedModules.MembershipModule.Models.SABCoreEntity
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        [Required]
        public int Priority { get; set; }
        public int? AppId { get; set; }
        [JsonIgnore]
        public virtual App App { get; set; }
        [MaxLength(50)]
        public string Type { get; set; }
        [JsonIgnore]
        public FormsType TypeObj { get; set; }
        [Index()]
        public int? ParentFormId { get; set; }
        [JsonIgnore]
        public virtual Form ParentForm { get; set; }
        [MaxLength(100)]
        public string SubmitRedirectUrl { get; set; }
        public virtual ICollection<Form> ChildrenForms { get; set; }
        [JsonIgnore]
        public virtual ICollection<FormInstance> FormsInstances { get; set; }
        public virtual ICollection<FormsField> FormFields { get; set; }
        public string CustomSubmitPath { get; set; }
        [JsonIgnore]
        public virtual ICollection<FormSubmitEvent> FromSubmitEvents { get; set; }
        [MaxLength(50)]
        public string SubmitButtonText { get; set; }
        [MaxLength(50)]
        public string NextButtonText { get; set; }
        [MaxLength(50)]
        public string BackButtonText { get; set; }
        [MaxLength(50)]
        public string AddItemButtonText { get; set; }
        [MaxLength(50)]
        public string RemoveItemButtonText { get; set; }
        [Index(IsUnique=true)]
        public Guid UrlParam { get; set; }
        public int? PrintTemplateId { get; set; }
        [JsonIgnore]
        public virtual ViewTemplate PrintTemplate { get; set; }

        public bool? ReCapatchaEnabled { get; set; }

        public int? PublicViewPackageId { get; set; }
        [JsonIgnore]
        [ForeignKey("PublicViewPackageId")]
        public virtual PublicViewsPackage PublicViewsPackage { get; set; }

        public string CreatorId { get; set; }
        [JsonIgnore]
        public virtual SABFramework.PreDefinedModules.MembershipModule.Models.ApplicationUser Creator { get; set; }

    }
}
