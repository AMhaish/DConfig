using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using System.Text.RegularExpressions;
using CompetitiveAnalysis.Models;
using DConfigOS_Core.Repositories.Utilities;
using DConfigOS_Core.Repositories;
using DConfigOS_Core.Models;

namespace CompetitiveAnalysis.ProductsManagerServices
{
    public class AdvancedPrivilegesAPI : RepositoryBase<DConfigOS_Core_DBContext>, IAdvancedPrivilegesAPI
    {
        public virtual List<AdvancedPrivilege> GetPrivileges()
        {
            ProductsManager_DBContext context = new ProductsManager_DBContext();
            var prvs = context.AdvancedPrivileges.OrderBy(m => m.Company.Name).ToList();
            return prvs;
        }

        public virtual AdvancedPrivilege GetPrivilege(int Id)
        {
            ProductsManager_DBContext context = new ProductsManager_DBContext();
            var prv = context.AdvancedPrivileges.Where(m => m.Id == Id).FirstOrDefault();
            return prv;
        }

        public virtual List<AdvancedPrivilege> GetCompaniesPrivileges(List<int> companiesIds)
        {
            ProductsManager_DBContext context = new ProductsManager_DBContext();
            if (companiesIds != null && companiesIds.Count > 0)
                return context.AdvancedPrivileges.Where(m => companiesIds.Contains(m.CompanyId)).ToList();
            else
                return null;
        }

        public virtual int CreatePrivilege(AdvancedPrivilege p, List<int> linkedTemplates)
        {
            ProductsManager_DBContext context = new ProductsManager_DBContext();
            var dbPrivilege = context.AdvancedPrivileges.Where(m => m.CompanyId == p.CompanyId).FirstOrDefault();
            List<ProductsTemplate> dbTemplates=null;
            if (linkedTemplates != null && linkedTemplates.Count > 0)
                dbTemplates = context.ProductsTemplates.Where(m => linkedTemplates.Contains(m.Id)).ToList();
            if (dbPrivilege == null)
            {
                context.AdvancedPrivileges.Add(p);
                if (dbTemplates != null && dbTemplates.Count > 0)
                {
                    p.RelatedProdutTemplates = new List<ProductsTemplate>();
                    foreach (ProductsTemplate t in dbTemplates)
                    {
                        p.RelatedProdutTemplates.Add(t);
                    }
                }
                context.SaveChanges();
                return ResultCodes.Succeed;
            }
            else
            {
                return ResultCodes.ObjectAlreadyExists;
            }
        }


        public virtual int UpdatePrivilege(AdvancedPrivilege p, List<int> linkedTemplates)
        {
            ProductsManager_DBContext context = new ProductsManager_DBContext();
            var dbComparison = context.AdvancedPrivileges.Where(m => m.Id == p.Id && m.CompanyId == p.CompanyId).FirstOrDefault();
            List<ProductsTemplate> dbTemplates = null;
            if (linkedTemplates != null && linkedTemplates.Count > 0)
                dbTemplates = context.ProductsTemplates.Where(m => linkedTemplates.Contains(m.Id)).ToList();
            if (dbComparison != null)
            {
                dbComparison.RelatedBrandFactoyTypes = p.RelatedBrandFactoyTypes;
                dbComparison.VisibleSections = p.VisibleSections;
                if (dbComparison.RelatedProdutTemplates == null)
                {
                    dbComparison.RelatedProdutTemplates = new List<ProductsTemplate>();
                }
                else
                {
                    dbComparison.RelatedProdutTemplates.Clear();
                }
                if (dbTemplates != null && dbTemplates.Count > 0)
                {
                    foreach (ProductsTemplate t in dbTemplates)
                    {
                        dbComparison.RelatedProdutTemplates.Add(t);
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

        public virtual int DeletePrivilege(int Id)
        {
            ProductsManager_DBContext context = new ProductsManager_DBContext();
            var dbComparison = context.Comparisons.Where(m => m.Id == Id).FirstOrDefault();
            if (dbComparison != null)
            {
                dbComparison.Filters.Clear();
                context.Comparisons.Remove(dbComparison);
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
