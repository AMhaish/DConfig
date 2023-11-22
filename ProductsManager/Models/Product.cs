using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using SABFramework.PreDefinedModules.MembershipModule.DataProviders;
using SABFramework.PreDefinedModules.MembershipModule.Models;
using DConfigOS_Core.Models;

namespace CompetitiveAnalysis.Models
{
    public class Product: SABFramework.PreDefinedModules.MembershipModule.Models.SABCoreEntity
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        [MaxLength(100)]
        public string Code { get; set; }
        [Required]
        public int TemplateId { get; set; }
        [JsonIgnore]
        public virtual ProductsTemplate Template { get; set; }
        public virtual ICollection<ProductPropertyValue> PropertiesValues { get; set; }
        public virtual ICollection<Price> Prices { get; set; }
        [JsonIgnore]
        public virtual ICollection<ProductTag> Tags { get; set; }
        [JsonIgnore]
        public DateTime? UpdateDate { get; set; }
        public int? CompanyId { get; set; }
        public virtual Company Company { get; set; }
        [MaxLength(512)]
        public string Notes { get; set; }


        public string CreateDate_Short
        {
            get {  if (CreatedDate.HasValue) { return CreatedDate.Value.ToLongDateString(); } else { return null; } }
        }

        public string UpdateDate_Short
        {
            get { if (UpdateDate.HasValue) { return UpdateDate.Value.ToLongDateString(); } else { return null; } }
        }
        public string TagsString
        {
            get
            {
                if(Tags !=null && Tags.Count > 0)
                { 
                StringBuilder result = new StringBuilder();
                foreach (ProductTag pt in Tags)
                {
                    result.Append(pt.Name);
                    result.Append(',');
                }
                return result.ToString().TrimEnd(',');
                }
                else
                {
                    return null;
                }
            }
        }
        public string BrandFactoryType { get; set; }
    }
}
