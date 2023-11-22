using CompetitiveAnalysis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompetitiveAnalysis.ProductsManagerServices
{
    public interface IAdvancedPrivilegesAPI : IDisposable
    {
        List<AdvancedPrivilege> GetPrivileges();
        AdvancedPrivilege GetPrivilege(int Id);
        List<AdvancedPrivilege> GetCompaniesPrivileges(List<int> companiesIds);
        int CreatePrivilege(AdvancedPrivilege p, List<int> linkedTemplates);
        int UpdatePrivilege(AdvancedPrivilege p, List<int> linkedTemplates);
        int DeletePrivilege(int Id);

    }
}
