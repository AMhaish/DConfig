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
    public class FormsField
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        [XmlAttribute("name")]
        public string Name { get; set; }
        [MaxLength(500)]
        [XmlAttribute("title")]
        public string Title { get; set; }
        [Required]
        [MaxLength(50)]
        [XmlAttribute("type")]
        public string Type { get; set; }
        public virtual FieldsType TypeObj { get; set; }
        [Required]
        [Index()]
        public int FormId { get; set; }
        //[ForeignKey("ViewTypeId")]
        [Required]
        [JsonIgnore]
        public virtual Form Form { get; set; }
        [JsonIgnore]
        public virtual ICollection<FormFieldValue> FieldsValues { get; set; }
        [Required]
        public bool Required { get; set; }
        public int? EnumId { get; set; }
        [JsonIgnore]
        public virtual FormsFieldsEnum Enum { get; set; }
        [Required]
        public int Priority { get; set; }
        public bool? Invisible { get; set; }
    }
}
