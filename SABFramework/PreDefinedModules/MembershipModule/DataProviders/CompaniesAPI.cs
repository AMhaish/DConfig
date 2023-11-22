using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using System.Text.RegularExpressions;
using SABFramework.PreDefinedModules.MembershipModule.Models;


namespace SABFramework.PreDefinedModules.MembershipModule.DataProviders
{
    public class CompaniesAPI : ICompaniesAPI
    {
        public virtual List<Company> GetCompanies()
        {
            MembershipDBContext context = new MembershipDBContext();
            var companies = context.Companies.OrderBy(m => m.Name).ToList();
            return companies;
        }

        public virtual List<Company> GetCompaniesByName(string companyName)
        {
            MembershipDBContext context = new MembershipDBContext();
            List<Company> companies;
            if (companyName == "*")
            {
                companies = context.Companies.ToList();
            }
            else {
                companies = context.Companies.Where(m => m.Name.Contains(companyName)).ToList();
            }
            return companies;
        }

        public virtual Company GetCompany(int Id)
        {
            MembershipDBContext context = new MembershipDBContext();
            var company = context.Companies.Where(m => m.Id == Id).FirstOrDefault();
            return company;
        }

        public virtual Company GetCompanyByName(string Name)
        {
            MembershipDBContext context = new MembershipDBContext();
            var company = context.Companies.Where(m => m.Name == Name).FirstOrDefault();
            return company;
        }

        public virtual List<Company> GetUserCompanies(string userId)
        {
            MembershipDBContext context = new MembershipDBContext();
            var companies = context.Companies.Where(m => m.Users.Any(n => n.Id == userId)).ToList();
            return companies;
        }

        public virtual bool CreateDConfigCompany(Company company, string userId = null)
        {
            MembershipDBContext context = new MembershipDBContext();
            var dbCompany = context.Companies.Where(m => m.Name == company.Name).FirstOrDefault();
            if (dbCompany == null)
            {
                company.CreateDate = DateTime.Now;
                if (!String.IsNullOrEmpty(userId))
                {
                    var user = context.Users.Where(m => m.Id == userId).FirstOrDefault();
                    company.Users = new List<ApplicationUser>();
                    if (user != null)
                        company.Users.Add(user);
                }
                context.Companies.Add(company);
                context.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }


        public virtual bool UpdateDConfigCompany(Company company, string userId = null)
        {
            MembershipDBContext context = new MembershipDBContext();
            var dbCompany = context.Companies.Where(m => m.Id == company.Id && (String.IsNullOrEmpty(userId) || m.Users.Any(n => n.Id == userId))).FirstOrDefault();
            var sameCompany = context.Companies.Where(m => m.Name == company.Name && m.Id != company.Id).FirstOrDefault();
            if (dbCompany != null)
            {
                if (sameCompany == null)
                {
                    dbCompany.Name = company.Name;
                    dbCompany.City = company.City;
                    dbCompany.Country = company.Country;
                    dbCompany.Address = company.Address;
                    dbCompany.Website = company.Website;
                    dbCompany.AccountId = company.AccountId;
                    context.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public virtual bool DeleteDConfigCompany(int Id, string userId = null)
        {
            MembershipDBContext context = new MembershipDBContext();
            var dbCompany = context.Companies.Where(m => m.Id == Id && (String.IsNullOrEmpty(userId) || m.Users.Any(n => n.Id == userId))).FirstOrDefault();
            if (dbCompany != null)
            {
                context.Companies.Remove(dbCompany);
                context.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }

        public virtual bool AddUsersToCompany(int companyId, List<string> usersIds)
        {
            MembershipDBContext context = new MembershipDBContext();
            var dbCompany = context.Companies.Where(m => m.Id == companyId).FirstOrDefault();
            var dbUsers = context.Users.Where(m => usersIds.Contains(m.Id)).ToList();
            if (dbCompany != null)
            {
                if (dbUsers != null && dbUsers.Count() > 0)
                {
                    foreach (var u in dbUsers)
                    {
                        if (!dbCompany.Users.Contains(u))
                            dbCompany.Users.Add(u);
                    }
                    context.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public virtual bool RemoveUsersFromCompany(int companyId, string userId)
        {
            MembershipDBContext context = new MembershipDBContext();
            var dbCompany = context.Companies.Where(m => m.Id == companyId).FirstOrDefault();
            if (dbCompany != null)
            {
                var dbUsers = dbCompany.Users.Where(m => m.Id == userId).FirstOrDefault();
                if (dbUsers != null)
                {
                    dbCompany.Users.Remove(dbUsers);
                    context.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
