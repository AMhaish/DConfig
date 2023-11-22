using SABFramework.PreDefinedModules.MembershipModule.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SABFramework.PreDefinedModules.MembershipModule.DataProviders
{
    public interface ICompaniesAPI
    {
        List<Company> GetCompanies();
        List<Company> GetCompaniesByName(string companyName);
        Company GetCompany(int Id);
        Company GetCompanyByName(string Name);
        List<Company> GetUserCompanies(string userId);
        bool CreateDConfigCompany(Company company, string userId = null);
        bool UpdateDConfigCompany(Company company, string userId = null);
        bool DeleteDConfigCompany(int Id, string userId = null);
        bool AddUsersToCompany(int companyId, List<string> usersIds);
        bool RemoveUsersFromCompany(int companyId, string userId);

    }
}
