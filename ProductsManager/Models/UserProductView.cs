using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SABFramework.PreDefinedModules.MembershipModule.Models;

namespace CompetitiveAnalysis.Models
{
    public class UserProductView
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Index()]
        public string UserId { get; set; }
        [JsonIgnore]
        public virtual SABFramework.PreDefinedModules.MembershipModule.Models.ApplicationUser User { get; set; }

        public DateTime CreateDate { get; set; }

        public int ProductTypeId { get; set; }
        [JsonIgnore]
        public virtual ProductsTemplate ProductType { get; set; }

        public int? BrandId { get; set; }
        [JsonIgnore]
        public virtual Company Brand { get; set; }

    }
}
