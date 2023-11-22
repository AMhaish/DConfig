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
    public class OperationInstance
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int OperationId { get; set; }
        public virtual Operation Operation { get; set; }
        public int? ServedCompanyId { get; set; }
        public virtual Company ServedCompany { get; set; }
        public DateTime CreatingDate { get; set; }
        public virtual List<ApplicationUser> AssigneesUsers { get; set; }
        public virtual List<OperationCheckListItemInstance> OperationCheckListItems { get; set; }
        public virtual List<OperationInstanceStatus> OperationStatuses { get; set; }
        [MaxLength(1)]
        public string Status { get; set; }// N: New / P: Being processed / F: Finished / O: On hold

        [NotMapped]
        public int DeadlineState // 0: Passed the deadline / 1:2 days to deadline / 2: Normal
        {
            get
            {
                switch (Operation.Cycle)
                {
                    case Operation.Cycle_Daily:
                        if((DateTime.Now - CreatingDate).Hours > 24)
                        {
                            return DeadlineState_PassedDeadline;
                        }
                        else
                        {
                            return DeadlineState_2DaysToDeadline;
                        }
                    case Operation.Cycle_Monthly:
                        if (DateTime.Now.Month == CreatingDate.Month)
                        {
                            if(DateTime.Now.Day > Operation.DueOnDay)
                            {
                                return DeadlineState_PassedDeadline;
                            }
                            else if(Operation.DueOnDay - DateTime.Now.Day <= 2)
                            {
                                return DeadlineState_2DaysToDeadline;
                            }
                            else
                            {
                                return DeadlineState_Normal;
                            }
                        }
                        else
                            return DeadlineState_PassedDeadline;
                    case Operation.Cycle_Yearly:
                        if (DateTime.Now.DayOfYear > Operation.DueOnDay)
                        {
                            return DeadlineState_PassedDeadline;
                        }
                        else if (Operation.DueOnDay - DateTime.Now.DayOfYear <= 2)
                        {
                            return DeadlineState_2DaysToDeadline;
                        }
                        else
                        {
                            return DeadlineState_Normal;
                        }
                    default:
                        return DeadlineState_PassedDeadline;
                        break;
                }
            }
        }

        [NotMapped]
        public string DueDate
        {
            get
            {
                switch (Operation.Cycle)
                {
                    case Operation.Cycle_Daily:
                        return CreatingDate.ToShortDateString();
                    case Operation.Cycle_Monthly:
                        var days = Operation.DueOnDay - CreatingDate.Day;
                        return CreatingDate.AddDays(days).ToShortDateString();
                    case Operation.Cycle_Yearly:
                        var yeardays = Operation.DueOnDay - CreatingDate.DayOfYear;
                        return CreatingDate.AddDays(yeardays).ToShortDateString();
                }
                return "";
            }
        }

        public string CreateDate
        {
            get
            {
                return CreatingDate.ToShortDateString();
            }
        }

        public const string Status_New = "N";
        public const string Status_Being_processed = "P";
        public const string Status_Finished = "F";
        public const string Status_On_hold = "O";

        public const int DeadlineState_PassedDeadline = 0;
        public const int DeadlineState_2DaysToDeadline = 1;
        public const int DeadlineState_Normal = 2;
    }
}
