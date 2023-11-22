using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using DConfigOS_Core.Models;
using OperationsManager.Models;
using SABFramework.PreDefinedModules.MembershipModule.Models;

namespace OperationsManager.Models
{
    public class OperationCheckListItem
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [MaxLength(100)]
        public string Task { get; set; }
        public int OperationId { get; set; }
        [JsonIgnore]
        public virtual Operation Operation { get; set; }
        public int? Priority { get; set; }
    }
}
