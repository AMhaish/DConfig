using OperationsManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationsManager.OperationsManagerServices
{
    public interface IOperationsAPI : IDisposable
    {
        List<OperationsCategory> GetOperationsCategories();
        List<Operation> GetOperations();
        List<Operation> GetUserOperations(string userId);
        int CreateOperationsCategory(OperationsCategory t);
        int CreateOperation(Operation t, List<OperationCheckListItem> checkListItems);
        int UpdateOperationsCategory(OperationsCategory t);
        int UpdateOperation(Operation t, List<OperationCheckListItem> checkListItems);
        int DeleteOperationsCategory(int Id);
        int DeleteOperation(int Id);
        int LinkOperationToCompany(int operationId, int companyId, out int? newId);
        int RemoveOperationFromCompany(int operationId, int companyId);
        int AssignUsersToCompanyOperation(int companyOperationId, List<string> userIds);
        int UnassignUserFromCompanyOperation(int companyOperationId, string userId);

    }
}
