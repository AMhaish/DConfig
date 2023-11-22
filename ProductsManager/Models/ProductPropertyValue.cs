using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CompetitiveAnalysis.Models
{
    public class ProductPropertyValue
    {
        [Key]
        [Column(Order = 2)]
        public int PropertyId { get; set; }
        [JsonIgnore]
        public virtual Property Property { get; set; }
        [Key]
        [Column(Order = 1)]
        public int ProductId { get; set; }
        [JsonIgnore]
        public virtual Product Product { get; set; }
        public string Value { get; set; }
    }
}
