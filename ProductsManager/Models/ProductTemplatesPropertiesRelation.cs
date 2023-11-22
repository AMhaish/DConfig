using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using SABFramework.ModulesUtilities;

namespace CompetitiveAnalysis.Models
{
    public class ProductTemplatesPropertiesRelation
    {
        [Key, Column(Order = 0)]
        public int TemplateId { get; set; }
        [Key, Column(Order = 1)]
        public int PropertyId { get; set; }
        [JsonIgnore]
        public virtual ProductsTemplate Template { get; set; }
        [JsonIgnore]
        public virtual Property Property { get; set; }
        public bool? IsHighlight { get; set; }
        [NotMapped]
        public int Id { get; set; }
        [MaxLength(25)]
        public string InvisibileToFactoryTypes { get; set; }
    }
}
