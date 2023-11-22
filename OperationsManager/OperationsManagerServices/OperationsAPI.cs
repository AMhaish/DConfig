using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using System.Text.RegularExpressions;
using DConfigOS_Core.Repositories.Utilities;
using OperationsManager.Models;
using SABFramework.PreDefinedModules.MembershipModule.Models;
using DConfigOS_Core.Repositories;
using DConfigOS_Core.Models;

namespace OperationsManager.OperationsManagerServices
{
    public class OperationsAPI : RepositoryBase<DConfigOS_Core_DBContext>, IOperationsAPI
    {
        public virtual List<OperationsCategory> GetOperationsCategories()
        {
            OperationsManager_DBContext context = new OperationsManager_DBContext();
            var operationsCategories = context.OperationsCategories.OrderBy(m => m.Name).ToList();
            return operationsCategories;
        }

        public virtual List<Operation> GetOperations()
        {
            OperationsManager_DBContext context = new OperationsManager_DBContext();
            var operations = context.Operations.OrderBy(m => m.Name).ToList();
            return operations;
        }

        public virtual List<Operation> GetUserOperations(string userId)
        {
            OperationsManager_DBContext context = new OperationsManager_DBContext();
            var operations = context.Operations.Where(m => m.CreatingUserId == userId && (m.Cycle!=Operation.Cycle_OneTime || m.CreatingDate > DateTime.Now.Subtract(new TimeSpan(40,0,0,0)))).OrderBy(m => m.Name).ToList();
            return operations;
        }

        public virtual int CreateOperationsCategory(OperationsCategory t)
        {
            OperationsManager_DBContext context = new OperationsManager_DBContext();
            var dbCategory = context.OperationsCategories.Where(m => m.Name == t.Name).FirstOrDefault();
            if (dbCategory == null)
            {
                context.OperationsCategories.Add(t);
                context.SaveChanges();
                return ResultCodes.Succeed;
            }
            else
            {
                return ResultCodes.ObjectNameAlreadyUsed;
            }
        }

        public virtual int CreateOperation(Operation t, List<OperationCheckListItem> checkListItems)
        {
            OperationsManager_DBContext context = new OperationsManager_DBContext();
            var dbOperation = context.Operations.Where(m => m.Name == t.Name).FirstOrDefault();
            if (dbOperation == null)
            {
                t.CreatingDate = DateTime.Now;
                if (t.Cycle != Operation.Cycle_OneTime && !t.StartingDate.HasValue)
                {
                    t.StartingDate = DateTime.Now;
                }
                t.OperationCheckListItems = new List<OperationCheckListItem>();
                if (checkListItems != null && checkListItems.Count > 0)
                {
                    foreach (var c in checkListItems)
                    {
                        t.OperationCheckListItems.Add(c);
                    }
                }
                context.Operations.Add(t);
                context.SaveChanges();
                return ResultCodes.Succeed;
            }
            else
            {
                return ResultCodes.ObjectNameAlreadyUsed;
            }
        }

        public virtual int UpdateOperationsCategory(OperationsCategory t)
        {
            OperationsManager_DBContext context = new OperationsManager_DBContext();
            var dbOperationCategotry = context.OperationsCategories.Where(m => m.Id == t.Id).FirstOrDefault();
            var sameCategory = context.Operations.Where(m => m.Name == t.Name && m.Id != t.Id).FirstOrDefault();
            if (dbOperationCategotry != null)
            {
                if (sameCategory == null)
                {
                    dbOperationCategotry.Name = t.Name;
                    context.SaveChanges();
                    return ResultCodes.Succeed;
                }
                else
                {
                    return ResultCodes.ObjectNameAlreadyUsed;
                }
            }
            else
            {
                return ResultCodes.ObjectHasntFound;
            }
        }

        public virtual int UpdateOperation(Operation t, List<OperationCheckListItem> checkListItems)
        {
            OperationsManager_DBContext context = new OperationsManager_DBContext();
            var dbOperation = context.Operations.Where(m => m.Id == t.Id).FirstOrDefault();
            var smaeOperation = context.Operations.Where(m => m.Name == t.Name && m.Id != t.Id).FirstOrDefault();
            if (dbOperation != null)
            {
                if (smaeOperation == null)
                {
                    dbOperation.CategoryId = t.CategoryId;
                    dbOperation.CreatingUserId = t.CreatingUserId;
                    dbOperation.Cycle = t.Cycle;
                    dbOperation.Description = t.Description;
                    dbOperation.DueOnDay = t.DueOnDay;
                    dbOperation.Name = t.Name;
                    dbOperation.Priority = t.Priority;
                    dbOperation.RaiseDay = t.RaiseDay;
                    if (dbOperation.OperationCheckListItems == null)
                    {
                        dbOperation.OperationCheckListItems = new List<OperationCheckListItem>();
                    }
                    else
                    {
                        context.OperationCheckListItems.RemoveRange(dbOperation.OperationCheckListItems);
                        context.SaveChanges();
                    }
                    if (checkListItems != null && checkListItems.Count > 0)
                    {
                        foreach (var c in checkListItems)
                        {
                            dbOperation.OperationCheckListItems.Add(c);
                        }
                    }
                    context.SaveChanges();
                    return ResultCodes.Succeed;
                }
                else
                {
                    return ResultCodes.ObjectNameAlreadyUsed;
                }
            }
            else
            {
                return ResultCodes.ObjectHasntFound;
            }
        }

        public virtual int DeleteOperationsCategory(int Id)
        {
            OperationsManager_DBContext context = new OperationsManager_DBContext();
            var dbOperationCategory = context.OperationsCategories.Where(m => m.Id == Id).FirstOrDefault();
            if (dbOperationCategory != null)
            {
                context.OperationsCategories.Remove(dbOperationCategory);
                context.SaveChanges();
                return ResultCodes.Succeed;
            }
            else
            {
                return ResultCodes.ObjectHasntFound;
            }
        }

        public virtual int DeleteOperation(int Id)
        {
            OperationsManager_DBContext context = new OperationsManager_DBContext();
            var dbOperation = context.Operations.Where(m => m.Id == Id).FirstOrDefault();
            if (dbOperation != null)
            {
                context.Operations.Remove(dbOperation);
                context.SaveChanges();
                return ResultCodes.Succeed;
            }
            else
            {
                return ResultCodes.ObjectHasntFound;
            }
        }

        public virtual int LinkOperationToCompany(int operationId, int companyId, out int? newId)
        {
            OperationsManager_DBContext context = new OperationsManager_DBContext();
            var co = context.CompanyOperations.Where(m => m.OperationId == operationId && m.ServedCompanyId == companyId).FirstOrDefault();
            if (co == null)
            {
                var newCo = new CompanyOperation() { ServedCompanyId = companyId, OperationId = operationId };
                context.CompanyOperations.Add(newCo);
                context.SaveChanges();
                newId = newCo.Id;
                return ResultCodes.Succeed;
            }
            else
            {
                newId = null;
                return ResultCodes.ObjectAlreadyExists;
            }
        }

        public virtual int RemoveOperationFromCompany(int operationId, int companyId)
        {
            OperationsManager_DBContext context = new OperationsManager_DBContext();
            var co = context.CompanyOperations.Where(m => m.OperationId == operationId && m.ServedCompanyId == companyId).FirstOrDefault();
            if (co != null)
            {
                context.CompanyOperations.Remove(co);
                context.SaveChanges();
                return ResultCodes.Succeed;
            }
            else
            {
                return ResultCodes.ObjectHasntFound;
            }
        }

        public virtual int AssignUsersToCompanyOperation(int companyOperationId, List<string> userIds)
        {
            OperationsManager_DBContext context = new OperationsManager_DBContext();
            var co = context.CompanyOperations.Where(m => m.Id == companyOperationId).FirstOrDefault();
            OperationInstance currentInstance = context.OperationInstances.Where(m => m.OperationId == co.OperationId && m.ServedCompanyId == co.ServedCompanyId && m.Status != OperationInstance.Status_Finished).FirstOrDefault();

            if (co != null)
            {
                var users = context.Users.Where(m => userIds.Contains(m.Id)).ToList();
                if (co.Assignees == null)
                {
                    co.Assignees = new List<ApplicationUser>();
                }
                if (userIds != null && userIds.Count > 0)
                {
                    //One time operation instance should be created now
                    if (co.Operation.Cycle == Operation.Cycle_OneTime && currentInstance == null)
                    {
                        currentInstance = new OperationInstance();
                        currentInstance.OperationId = co.OperationId;
                        currentInstance.CreatingDate = DateTime.Now;
                        currentInstance.AssigneesUsers = new List<ApplicationUser>();
                        currentInstance.OperationCheckListItems = new List<OperationCheckListItemInstance>();
                        foreach (OperationCheckListItem i in co.Operation.OperationCheckListItems)
                        {
                            currentInstance.OperationCheckListItems.Add(new OperationCheckListItemInstance() { Checked = false, OperationCheckListItemId = i.Id });
                        }
                        currentInstance.ServedCompanyId = co.ServedCompanyId;
                        currentInstance.Status = OperationInstance.Status_New;
                        context.OperationInstances.Add(currentInstance);
                    }
                    foreach (var user in users)
                    {
                        if (!co.Assignees.Contains(user))
                            co.Assignees.Add(user);
                        if (currentInstance != null && !currentInstance.AssigneesUsers.Contains(user))
                            currentInstance.AssigneesUsers.Add(user);
                    }
                }
                context.SaveChanges();
                return ResultCodes.Succeed;
            }
            else
            {
                return ResultCodes.ObjectHasntFound;
            }
        }

        public virtual int UnassignUserFromCompanyOperation(int companyOperationId, string userId)
        {
            OperationsManager_DBContext context = new OperationsManager_DBContext();
            var co = context.CompanyOperations.Where(m => m.Id == companyOperationId).FirstOrDefault();
            var currentInstance = context.OperationInstances.Where(m => m.OperationId == co.OperationId && m.ServedCompanyId == co.ServedCompanyId && m.Status != OperationInstance.Status_Finished).FirstOrDefault();
            if (co != null)
            {
                var user = co.Assignees.Where(m => m.Id == userId).FirstOrDefault();
                if (user != null)
                {
                    co.Assignees.Remove(user);
                    if (currentInstance != null)
                    {
                        var user2 = currentInstance.AssigneesUsers.Where(m => m.Id == userId).FirstOrDefault();
                        if (user2 != null)
                        {
                            currentInstance.AssigneesUsers.Remove(user2);
                        }
                    }
                    context.SaveChanges();
                    return ResultCodes.Succeed;
                }
                else
                {
                    return ResultCodes.ObjectResourceHasntFound;
                }
            }
            else
            {
                return ResultCodes.ObjectHasntFound;
            }
        }
    }
}
