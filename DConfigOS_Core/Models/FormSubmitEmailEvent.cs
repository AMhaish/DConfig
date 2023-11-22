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
    [Table("FormSubmitEmailEvent")]
    public class FormSubmitEmailEvent:FormSubmitEvent
    {
        public string From { get; set; }
        public int? FromBindedFieldId { get; set; }
        public string To { get; set; }
        public int? ToBindedFieldId { get; set; }
        public string Bcc { get; set; }
        public int? BccBindedFieldId { get; set; }
        public string Cc { get; set; }
        public int? CcBindedFieldId { get; set; }
        public string Subject { get; set; }
        public int TemplateId { get; set; }
        [JsonIgnore]
        public virtual ViewTemplate Template { get; set; }
    }
}
