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
    public class OperationInstanceStatus
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int OperationInstanceId { get; set; }
        [JsonIgnore]
        public virtual OperationInstance OperationInstance { get; set; }
        [MaxLength(1)]
        public string Status { get; set; }// N: New / P: Being processed / F: Finished / O: On hold
        [MaxLength(100)]
        public string StatusDescription { get; set; }
        public DateTime LoggingDate { get; set; }
        [NotMapped]
        public string Date
        {
            get
            {
                return LoggingDate.ToShortDateString() + "  " + LoggingDate.ToShortTimeString();
            }
        }
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}
