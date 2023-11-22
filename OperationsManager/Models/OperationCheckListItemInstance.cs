using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using DConfigOS_Core.Models;
using OperationsManager.Models;
using SABFramework.PreDefinedModules.MembershipModule.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace OperationsManager.Models
{
    public class OperationCheckListItemInstance
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int OperationCheckListItemId { get; set; }
        public virtual OperationCheckListItem OperationCheckListItem { get; set; }
        public int OperationInstanceId { get; set; }
        [JsonIgnore]
        public virtual OperationInstance OperationInstance { get; set; }
        public bool Checked { get; set; }
    }
}
