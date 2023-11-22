using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using System.ComponentModel;

namespace SABFramework.PreDefinedModules.MembershipModule.Models
{
    public abstract class SABCoreEntity
    {
        public SABCoreEntity()
        {
            if(MembershipProvider.Instance != null && MembershipProvider.Instance.ContextCompanyId.HasValue)
            {
                ContextCompanyId = MembershipProvider.Instance.ContextCompanyId.Value;
                CreatedDate = DateTime.Now;
            }
        }

        [Index]
        public int? ContextCompanyId { get; set; }
        [JsonIgnore]
        [ForeignKey("ContextCompanyId")]
        public virtual Company ContextOwnerCompany { get; set; }
        [JsonIgnore]
        public DateTime? CreatedDate { get; set; }
    }
}
