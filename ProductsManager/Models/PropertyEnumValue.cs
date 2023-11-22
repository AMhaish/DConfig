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
    public class PropertyEnumValue
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [Index()]
        public int EnumId { get; set; }
        [JsonIgnore]
        public virtual PropertyEnum Enum { get; set; }
        public string Value { get; set; }
        public int? Weight { get; set; }
        public int? Priority { get; set; }
    }
}
