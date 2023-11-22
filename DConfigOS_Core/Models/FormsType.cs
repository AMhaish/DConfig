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
    public class FormsType
    {
        [Key]
        [MaxLength(50)]
        [XmlAttribute("name")]
        public string Name { get; set; }
        [JsonIgnore]
        public virtual ICollection<Form> Forms { get; set; }

    }
}
