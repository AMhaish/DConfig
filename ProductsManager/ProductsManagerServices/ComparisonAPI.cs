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
    public class ComparisonsAPI : RepositoryBase<DConfigOS_Core_DBContext>, IComparisonAPI
    {
        public virtual List<Comparison> GetComparisons()
        {
            ProductsManager_DBContext context = new ProductsManager_DBContext();
            var comparisons = context.Comparisons.OrderBy(m => m.Name).ToList();
            return comparisons;
        }

        public virtual Comparison GetComparison(int Id)
        {
            ProductsManager_DBContext context = new ProductsManager_DBContext();
            var comparison = context.Comparisons.Where(m => m.Id == Id).FirstOrDefault();
            return comparison;
        }

        public virtual int CreateComparison(Comparison t, List<ComparisonFilter> Filters)
        {
            ProductsManager_DBContext context = new ProductsManager_DBContext();
            var dbComparison = context.Comparisons.Where(m => m.Name == t.Name).FirstOrDefault();
            if (dbComparison == null)
            {
                context.Comparisons.Add(t);
                if (Filters != null && Filters.Count > 0)
                {
                    t.Filters = new List<ComparisonFilter>();
                    foreach (ComparisonFilter f in Filters)
                    {
                        t.Filters.Add(f);
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


        public virtual int UpdateComparison(Comparison t, List<ComparisonFilter> Filters)
        {
            ProductsManager_DBContext context = new ProductsManager_DBContext();
            var dbComparison = context.Comparisons.Where(m => m.Id == t.Id).FirstOrDefault();
            var sameComparison = context.Comparisons.Where(m => m.Name == t.Name && m.Id != t.Id).FirstOrDefault();
            if (dbComparison != null)
            {
                if (sameComparison == null)
                {
                    dbComparison.Name = t.Name;
                    dbComparison.BrandFactoryTypes = t.BrandFactoryTypes;
                    dbComparison.Tags = t.Tags;
                    dbComparison.CreateDate_From = t.CreateDate_From;
                    dbComparison.CreateDate_To = t.CreateDate_To;
                    dbComparison.UpdateDate_From = t.UpdateDate_From;
                    dbComparison.UpdateDate_To = t.UpdateDate_To;
                    if(dbComparison.Filters==null)
                    {
                        dbComparison.Filters = new List<ComparisonFilter>();
                    }
                    else
                    {
                        dbComparison.Filters.Clear();
                    }
                    if (Filters != null && Filters.Count > 0)
                    {
                        foreach (ComparisonFilter f in Filters)
                        {
                            dbComparison.Filters.Add(f);
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

        public virtual int DeleteComparison(int Id)
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

        public virtual int UpdateComparisonProducts(int comparisonId, List<int> productsIds)
        {
            ProductsManager_DBContext context = new ProductsManager_DBContext();
            var dbComparison = context.Comparisons.Where(m => m.Id == comparisonId).FirstOrDefault();
            if (dbComparison != null)
            {
                dbComparison.Products.Clear();
                if (productsIds != null && productsIds.Count > 0)
                {
                    var products = context.Products.Where(m => productsIds.Contains(m.Id));
                    if (products != null)
                    {
                        foreach (var p in products)
                        {
                            dbComparison.Products.Add(p);
                        }
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
    }
}
