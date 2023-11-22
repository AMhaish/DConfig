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
    public class FieldsType
    {
        [Key]
        [MaxLength(50)]
        [XmlAttribute("name")]
        public string Name { get; set; }
        [MaxLength(50)]
        [XmlAttribute("intentName")]
        public string IntentName { get; set; }
        [JsonIgnore]
        public virtual ICollection<ViewField> ViewFields { get; set; }
        [JsonIgnore]
        public virtual ICollection<FormsField> FormFields { get; set; }

    }
}
