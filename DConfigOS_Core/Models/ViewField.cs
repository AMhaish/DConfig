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
    public class ViewField
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        [XmlAttribute("name")]
        public string Name { get; set; }
        [Required]
        [MaxLength(50)]
        [XmlAttribute("type")]
        [Ignore]
        public string Type { get; set; }

        [MaxLength(50)]
        [XmlAttribute("columnname")]
        public string ColumnName { get; set; }

        [Ignore]
        public virtual FieldsType TypeObj { get; set; }
        [Required]
        [Index()]
        [Number]
        public int ViewTypeId { get; set; }
        //[ForeignKey("ViewTypeId")]
        [Required]
        [JsonIgnore]
        [Ignore]
        public virtual ViewType ViewType { get; set; }
        [JsonIgnore]
        [Ignore]
        public virtual ICollection<ViewFieldValue> FieldValues { get; set; }
        [Ignore]
        public int? EnumId { get; set; }
        [Ignore]
        public virtual ViewFieldsEnum Enum { get; set; }
        [Required]
        [Number]
        public int Priority { get; set; }
    }
}
