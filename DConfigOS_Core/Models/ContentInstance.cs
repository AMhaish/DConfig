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
using Nest;

namespace DConfigOS_Core.Models
{
    [Serializable]
    public class ContentInstance
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [MaxLength(250)]
        [XmlAttribute("name")]
        [Ignore]
        public string Name { get; set; }
        [MaxLength(2048)]
        [XmlAttribute("metaDescription")]
        [Ignore]
        public string MetaDescription { get; set; }
        [MaxLength(2048)]
        [XmlAttribute("metaKeywords")]
        [Ignore]
        public string MetaKeywords { get; set; }
        [MaxLength(300)]
        [XmlAttribute("title")]
        [Ignore]
        public string Title { get; set; }
        public int? ViewTemplateId { get; set; }
        [JsonIgnore]
        [Ignore]
        public virtual ViewTemplate ViewTemplate { get; set; }
        [Required]
        [XmlAttribute("online")]
        [Ignore]
        public bool Online { get; set; }
        [Required]
        [Index()]
        public int ContentId { get; set; }
        [JsonIgnore]
        [Ignore]
        public virtual Content Content { get; set; }
        [MaxLength(10)]
        [XmlAttribute("language")]
        public string Language { get; set; }
        [XmlAttribute("version")]
        [Ignore]
        public int Version { get; set; }
        [Required]
        [Ignore]
        public DateTime CreateDate { get; set; }
        [XmlArray("fields")]
        [XmlArrayItem("field", typeof(ViewFieldValue))]
        // [JsonIgnore]
        [Nested(IncludeInParent = true, Enabled = true, Name = "values", Store = true)]
        public virtual ICollection<ViewFieldValue> FieldsValues { get; set; }
        //[NotMapped]
        //public ICollection<ViewTemplate> PossibleViewTemplates { get; set; }
        [MaxLength(500)]
        [XmlAttribute("redirectUrl")]
        [Ignore]
        public string RedirectUrl { get; set; }
        [MaxLength(500)]
        [XmlAttribute("downloadPath")]
        [Ignore]
        public string DownloadPath { get; set; }
        [MaxLength(200)]
        [XmlAttribute("downloadName")]
        [Ignore]
        public string DownloadName { get; set; }
        [Ignore]
        public string CreatorId { get; set; }
        [JsonIgnore]
        [Ignore]
        public virtual SABFramework.PreDefinedModules.MembershipModule.Models.ApplicationUser Creator { get; set; }
        [Ignore]
        public int? StageId { get; set; }
        [Ignore]
        public virtual Stage Stage { get; set; }
        [Ignore]
        public string Comments { get; set; }

        [NotMapped]
        public int? ContextCompanyId
        {
            get
            {
                if (Content != null)
                    return Content.ContextCompanyId;
                else
                    return null;
            }
        }
        public string Data { get; set; }
        [NotMapped]
        public Dictionary<string, string> DataDic {
            get { return Data == null ? null : JsonConvert.DeserializeObject<Dictionary<string, string>>(Data); }
            set { Data = JsonConvert.SerializeObject(value); }
        }
    }

}
