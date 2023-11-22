using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using SABFramework.PreDefinedModules.MembershipModule.Models;

namespace CompetitiveAnalysis.Models
{
    public class AdvancedPrivilege: SABFramework.PreDefinedModules.MembershipModule.Models.SABCoreEntity
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public virtual Company Company { get; set; }
        public string VisibleSections { get; set; } //Contains the visible sections to company
        public string RelatedBrandFactoyTypes { get; set; } //Contains the factory types that the company has access to it
        [JsonIgnore]
        public virtual ICollection<ProductsTemplate> RelatedProdutTemplates { get; set; }


        //Info related to overrides on Companys data
        public double? PriceOverridePercentage { get; set; }

        public List<int> RelatedProdutTemplatesIds
        {
            get
            {
                if (RelatedProdutTemplates != null)
                    return RelatedProdutTemplates.Select(m => m.Id).ToList();
                else
                    return null;
            }
        }
    }
}
