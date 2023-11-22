using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace CompetitiveAnalysis.Models
{
    public class ComparisonFilter
    {
        [Required]
        [Column(Order=0), Key]
        
        public int ComparisonId { get; set; }
        [JsonIgnore]
        public virtual Comparison Comparison { get; set; }
        [Required]
        [Column(Order = 1), Key]
        public int PropertyId { get; set; }
        public virtual Property Property { get; set; }
        [MaxLength(5)]
        public string ConditionType { get; set; }
        [MaxLength(250)]
        public string Keywords { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public double? FromValue { get; set; }
        public double? ToValue { get; set; }
        public bool? BoolValue { get; set; }
    }
}
