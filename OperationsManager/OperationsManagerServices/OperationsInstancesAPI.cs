using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OperationsManager.Models;
using DConfigOS_Core.Repositories.Utilities;
using SABFramework.PreDefinedModules.MembershipModule.Models;
using DConfigOS_Core.Models;
using DConfigOS_Core.Repositories;

namespace OperationsManager.OperationsManagerServices
{
    public class OperationsInstancesAPI: RepositoryBase<DConfigOS_Core_DBContext>,IOperationsInstancesAPI
    {
        public virtual List<OperationInstance> GetActiveOperations(int? month, int? year, string userId = null, int? companyId = null, string status = null,int? categoryId=null)
        {
            OperationsManager_DBContext context = new OperationsManager_DBContext();
            var operations = context.OperationInstances.Where(m => (!categoryId.HasValue || m.Operation.CategoryId==categoryId.Value) && (!year.HasValue || m.CreatingDate.Year == year) && (!month.HasValue || m.CreatingDate.Month == month) && ((String.IsNullOrEmpty(status)) || m.Status == status) && (String.IsNullOrEmpty(userId) || m.AssigneesUsers.Any(n => n.Id == userId)) && (!companyId.HasValue || m.ServedCompanyId == companyId.Value)).OrderBy(m => m.Operation.Priority).ToList();
            return operations;
        }

        public virtual void CreateOperationsInstances()
        {
            OperationsManager_DBContext context = new OperationsManager_DBContext();
            var operations = context.Operations.ToList();
            foreach (Operation op in operations)
            {
                var operationInstance = op.OperationInstances.OrderByDescending(m => m.CreatingDate).FirstOrDefault();
                if (op.StartingDate <= DateTime.Now)
                {
                    if (operationInstance == null)
                    {
                        CreateCompaniesOperationsInstances(context, op);
                    }
                    else
                    {
                        switch (op.Cycle)
                        {
                            case Operation.Cycle_Daily:
                                if (operationInstance.CreatingDate.Day != DateTime.Now.Day)
                                {
                                    CreateCompaniesOperationsInstances(context, op);
                                }
                                break;
                            case Operation.Cycle_Monthly:
                            default:
                                if (operationInstance.CreatingDate.Month != DateTime.Now.Month)
                                {
                                    if (DateTime.Now.Day >= op.RaiseDay)
                                    {
                                        CreateCompaniesOperationsInstances(context, op);
                                    }
                                }
                                break;
                            case Operation.Cycle_Yearly:
                                if (operationInstance.CreatingDate.Year != DateTime.Now.Year)
                                {
                                    if (DateTime.Now.DayOfYear >= op.RaiseDay)
                                    {
                                        CreateCompaniesOperationsInstances(context, op);
                                    }
                                }
                                break;
                        }
                    }
                }
            }
            context.SaveChanges();
        }

        public virtual void CreateCompaniesOperationsInstances(OperationsManager_DBContext context, Operation op)
        {
            foreach (CompanyOperation co in op.CompanyOperations)
            {
                OperationInstance opi = new OperationInstance();
                opi.CreatingDate = DateTime.Now;
                opi.AssigneesUsers = new List<ApplicationUser>();
                foreach (ApplicationUser u in co.Assignees)
                {
                    opi.AssigneesUsers.Add(u);
                }
                opi.OperationCheckListItems = new List<OperationCheckListItemInstance>();
                foreach (OperationCheckListItem i in op.OperationCheckListItems)
                {
                    opi.OperationCheckListItems.Add(new OperationCheckListItemInstance() { Checked = false, OperationCheckListItemId = i.Id });
                }
                opi.ServedCompanyId = co.ServedCompanyId;
                opi.Status = OperationInstance.Status_New;
                op.OperationInstances.Add(opi);
            }
        }


        public virtual int SetAnOperationListItemCheckState(int id, bool isChecked)
        {
            OperationsManager_DBContext context = new OperationsManager_DBContext();
            var item = context.OperationCheckListItemInstances.Where(m => m.Id == id).FirstOrDefault();
            if (item != null)
            {
                item.Checked = isChecked;
                context.SaveChanges();
                return ResultCodes.Succeed;
            }
            else
            {
                return ResultCodes.ObjectHasntFound;
            }
        }

        public virtual int ChangeOperationInstanceStatus(int id, string userId, string status, string description)
        {
            OperationsManager_DBContext context = new OperationsManager_DBContext();
            var instance = context.OperationInstances.Where(m => m.Id == id).FirstOrDefault();
            if (instance != null)
            {
                instance.Status = status;
                instance.OperationStatuses.Add(new OperationInstanceStatus() { LoggingDate = DateTime.Now, Status = status, StatusDescription = description, UserId = userId });
                context.SaveChanges();
                return ResultCodes.Succeed;
            }
            else
            {
                return ResultCodes.ObjectHasntFound;
            }
        }

    }
}
