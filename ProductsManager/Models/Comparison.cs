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
    public class Comparison: SABFramework.PreDefinedModules.MembershipModule.Models.SABCoreEntity
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        [MaxLength(100)]
        public string BrandFactoryTypes { get; set; }
        [MaxLength(200)]
        public string Tags { get; set; }
        public DateTime? CreateDate_From { get; set; }
        public DateTime? CreateDate_To { get; set; }
        public DateTime? UpdateDate_From { get; set; }
        public DateTime? UpdateDate_To { get; set; }
        public virtual ICollection<Product> Products { get; set; }
        public virtual ICollection<ComparisonFilter> Filters { get; set; }
    }
}
