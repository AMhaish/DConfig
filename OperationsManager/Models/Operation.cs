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
    public class Operation
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        [MaxLength(250)]
        public string Description { get; set; }
        public int? CategoryId { get; set; }
        public virtual OperationsCategory Category { get; set; }
        [MaxLength(1)]
        public string Cycle { get; set; } // Y: Yearly / M: Monthly / D: Daily/ O: One time
        public int RaiseDay { get; set; }
        public int DueOnDay { get; set; }
        //For daily and one time tasks=====================================================
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        [NotMapped]
        //[RegularExpression(@"^(0[1-9]|2[0-3]):[0-5][0-9]$", ErrorMessage = "Invalid Time.")]
        public string StartTimeValue
        {
            get
            {
                return StartTime.HasValue ? StartTime.Value.ToString("hh':'mm") : string.Empty;
            }

            set
            {
                if (!String.IsNullOrEmpty(value))
                    StartTime = TimeSpan.Parse(value);
            }
        }
        [NotMapped]
        //[RegularExpression(@"^(0[1-9]|2[0-3]):[0-5][0-9]$", ErrorMessage = "Invalid Time.")]
        public string EndTimeValue
        {
            get
            {
                return EndTime.HasValue ? EndTime.Value.ToString("hh':'mm") : string.Empty;
            }

            set
            {
                if (!String.IsNullOrEmpty(value))
                    EndTime = TimeSpan.Parse(value);
            }
        }
        //======================================================================
        public DateTime CreatingDate { get; set; }
        public DateTime? StartingDate { get; set; }
        public string CreatingUserId { get; set; }
        public virtual ApplicationUser CreatingUser { get; set; }
        [MaxLength(1)]
        public string Priority { get; set; }//0 Highest,1 High,2 Normal,3 Low,4 Lowest
        public virtual List<OperationCheckListItem> OperationCheckListItems { get; set; }
        public virtual List<CompanyOperation> CompanyOperations { get; set; }
        [JsonIgnore]
        public virtual List<OperationInstance> OperationInstances { get; set; }



        public const string Cycle_Yearly = "Y";
        public const string Cycle_Monthly = "M";
        public const string Cycle_Daily = "D";
        public const string Cycle_OneTime = "O";

        public const string Priority_Highest = "0";
        public const string Priority_High = "1";
        public const string Priority_Normal = "2";
        public const string Priority_Low = "3";
        public const string Priority_Lowest = "4";
    }
}
