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
    public class FormSubmitEventType
    {
        [Key]
        [MaxLength(50)]
        [XmlAttribute("name")]
        public string Name { get; set; }
        public int? AppId { get; set; }
        [JsonIgnore]
        public virtual App App { get; set; }
        public string EventPath { get; set; }
        [MaxLength(10)]
        public string IntentName { get; set; }
        [JsonIgnore]
        public virtual ICollection<FormSubmitEvent> FormSubmitEvents { get; set; }

        public static class PredefinedTypes
        {
            public const string EmailType = "Email Event";
        }
    }
}
