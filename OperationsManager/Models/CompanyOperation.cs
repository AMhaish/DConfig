using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SABFramework.PreDefinedModules.MembershipModule.Models;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OperationsManager.Models
{
    public class CompanyOperation
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int? ServedCompanyId { get; set; }
        public virtual Company ServedCompany { get; set; }
        public int OperationId { get; set; }
        [JsonIgnore]
        public virtual Operation Operation { get; set; }
        public virtual List<ApplicationUser> Assignees { get; set; }
    }
}
