using OperationsManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationsManager.OperationsManagerServices
{
   public interface IOperationsInstancesAPI : IDisposable
    {
        List<OperationInstance> GetActiveOperations(int? month, int? year, string userId = null, int? companyId = null, string status = null, int? categoryId = null);
        void CreateOperationsInstances();
        void CreateCompaniesOperationsInstances(OperationsManager_DBContext context, Operation op);
        int SetAnOperationListItemCheckState(int id, bool isChecked);
        int ChangeOperationInstanceStatus(int id, string userId, string status, string description);

    }
}
