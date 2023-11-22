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
    public class ProductsTemplate: SABFramework.PreDefinedModules.MembershipModule.Models.SABCoreEntity
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [MaxLength(100)]
        public string Name { get; set; }

        private List<Property> _Properties;
        [NotMapped]
        public List<Property> Properties {
            get
            {
                if (_Properties == null && PropertiesRelations!=null)
                {
                    return PropertiesRelations.Select(m => m.Property).ToList();
                }
                if (PropertiesRelations == null)
                {
                    return new List<Property>();
                }
                return _Properties;
            }
            set
            {
                _Properties = value;
            }
        }
        public virtual ICollection<ProductTemplatesPropertiesRelation> PropertiesRelations { get; set; }
        [JsonIgnore]
        public virtual ICollection<Product> Products { get; set; }

    }
}
