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
using System.IO;

namespace DConfigOS_Core.Models
{
    [Serializable]
    public class ViewFieldsEnumValue
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [Index()]
        public int EnumId { get; set; }
        [JsonIgnore]
        public virtual ViewFieldsEnum Enum { get; set; }
        public string Value { get; set; }

        public int? SubEnumId { get; set; }
        [JsonIgnore]
        public virtual ViewFieldsEnum SubEnum { get; set; }

        // The JSON column
        //[Column(TypeName = "json")]
        public string LangValueJson { get; set; }
        public int? Priority { get; set; }
        /*
        [NotMapped]
        public LangValue[] LangValueDict
        {
            get
            {
                var ser = new JsonSerializer();
                if (LangValueJson != null)
                {
                    var jr = new JsonTextReader(new StringReader(LangValueJson));

                    return ser.Deserialize<LangValue[]>(jr);
                }
                else
                {
                    return null;
                }
            }
            set
            {
                var ser = new JsonSerializer();
                var sw = new StringWriter();
                ser.Serialize(sw, value);
                LangValueJson = sw.ToString();
            }
        }
        */
    }
}
