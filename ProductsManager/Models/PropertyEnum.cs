using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace CompetitiveAnalysis.Models
{
    public class PropertyEnum: SABFramework.PreDefinedModules.MembershipModule.Models.SABCoreEntity
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        [JsonIgnore]
        public virtual ICollection<PropertyEnumValue> Values { get; set; }
        [NotMapped]
        public List<PropertyEnumValue> OrderedValues
        {
            get
            {
                if (Values != null)
                    return Values.OrderBy(m => m.Priority).ToList();
                else
                    return null;
            }
        }
        [JsonIgnore]
        public virtual ICollection<Property> Properties { get; set; }
    }
}
