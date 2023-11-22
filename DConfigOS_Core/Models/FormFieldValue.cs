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
    public class FormFieldValue
    {
        [Key]
        [Column(Order = 2)]
        public int FieldId { get; set; }
        [JsonIgnore]
        public virtual FormsField Field { get; set; }
        [Key]
        [Column(Order = 1)]
        public int FormInstanceId { get; set; }
        //[ForeignKey("ContentId")]
        [JsonIgnore]
        public virtual FormInstance FormInstance { get; set; }
        [XmlAttribute("value")]
        public string Value { get; set; }
        [NotMapped]
        [JsonIgnore]
        [XmlAttribute("value")]
        public string type { get; set; }
    }
}
