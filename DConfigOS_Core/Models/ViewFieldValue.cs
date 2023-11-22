using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;
using System.Xml;
using Nest;

namespace DConfigOS_Core.Models
{
    [Serializable]
    public class ViewFieldValue
    {
        [Key]
        [Column(Order = 2)]
        [Number]
        public int FieldId { get; set; }
        [JsonIgnore]
        [Ignore]
        //[ForeignKey("FieldId")]
        public virtual ViewField Field { get; set; }
        [Key]
        [Column(Order = 1)]
        [Ignore]
        public int ContentId { get; set; }
        //[ForeignKey("ContentId")]
        [JsonIgnore]
        [Ignore]
        public virtual ContentInstance Content { get; set; }
        [XmlAttribute("value")]
        [Text]
        public string Value { get; set; }
        [NotMapped]
        [JsonIgnore]
        [XmlAttribute("type")]
        [Ignore]
        public string type { get; set; }
    }
}
