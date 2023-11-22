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
    public class PropertiesGroup: SABFramework.PreDefinedModules.MembershipModule.Models.SABCoreEntity
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
       
        public virtual ICollection<Property> Properties { get; set; }
        [Required]
        public int Priority { get; set; }
        //public string BrandFactoryTypes { get; set; }
        [MaxLength(100)]
        public string DisplayAs { get; set; }
    }
}
