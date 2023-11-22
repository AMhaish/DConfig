using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using DConfigOS_Core.Models;

namespace CompetitiveAnalysis.Models
{
    public class Property
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        [Required]
        public int GroupId { get; set; }
        [JsonIgnore]
        public virtual PropertiesGroup Group { get; set; }
        [Required]
        [MaxLength(50)]
        public string Type { get; set; }
        public virtual FieldsType TypeObj { get; set; }
        [JsonIgnore]
        //public virtual ICollection<ProductsTemplate> Templates { get; set; }
        public virtual ICollection<ProductTemplatesPropertiesRelation> Templates { get; set; }
        [JsonIgnore]
        public virtual ICollection<ProductPropertyValue> Values { get; set; }
        public int? EnumId { get; set; }
        public virtual PropertyEnum Enum { get; set; }
        [Required]
        public int Priority { get; set; }
        public bool? LargerIsBetter { get; set; }
        [NotMapped]
        public string Value { get; set; }
        [MaxLength(10)]
        public string Unit { get; set; }
        [MaxLength(100)]
        public string Notes { get; set; }
        [MaxLength(100)]
        public string DisplayAs { get; set; }
        [MaxLength(100)]
        public string ExcelColumnName { get; set; }
    }
}
