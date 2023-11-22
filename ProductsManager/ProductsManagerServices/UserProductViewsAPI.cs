using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using System.Text.RegularExpressions;
using CompetitiveAnalysis.Models;
using DConfigOS_Core.Models;
using DConfigOS_Core.Repositories.Utilities;
using SABFramework.ModulesUtilities;
using CompetitiveAnalysis.Helpers;

namespace CompetitiveAnalysis.ProductsManagerServices
{
    public class UserProductViewsAPI : IUserProductViewsAPI
    {
        public virtual List<UserViewTypesStatisticsResult> GetMostViewedProductsTypes(string userId)
        {
            ProductsManager_DBContext context = new ProductsManager_DBContext();
            var views = context.UserProductsViews.Where(m => m.UserId == userId).GroupBy(m => m.ProductTypeId).OrderBy(m => m.Count()).Take(5).ToList();
            List<UserViewTypesStatisticsResult> types = new List<UserViewTypesStatisticsResult>();
            double sum = views.Sum(m => m.Count());
            foreach (var view in views)
            {
                UserViewTypesStatisticsResult uvsr = new UserViewTypesStatisticsResult();
                uvsr.Percentage = view.Count() / sum;
                uvsr.ProductType = context.ProductsTemplates.SingleOrDefault(m => m.Id == view.Key);
                types.Add(uvsr);
            }
            return types;
        }

        public virtual List<UserViewBrandsStatisticsResult> GetMostViewedProductsBrands(string userId)
        {
            ProductsManager_DBContext context = new ProductsManager_DBContext();
            var views = context.UserProductsViews.Where(m => m.UserId == userId).GroupBy(m => m.BrandId).OrderBy(m => m.Count()).Take(5).ToList();
            List<UserViewBrandsStatisticsResult> brands = new List<UserViewBrandsStatisticsResult>();
            double sum = views.Sum(m => m.Count());
            foreach (var view in views)
            {
                UserViewBrandsStatisticsResult uvsr = new UserViewBrandsStatisticsResult();
                uvsr.Percentage = view.Count() / sum;
                uvsr.Company = context.Companies.SingleOrDefault(m => m.Id == view.Key);
                brands.Add(uvsr);
            }
            return brands;
        }

        public virtual int RegisterView(string userId, int productId)
        {
            ProductsManager_DBContext context = new ProductsManager_DBContext();
            var product = context.Products.SingleOrDefault(m => m.Id == productId);
            if (product != null)
            {
                UserProductView pv = new UserProductView();
                pv.CreateDate = DateTime.Now;
                pv.ProductTypeId = product.TemplateId;
                pv.BrandId = product.CompanyId;
                pv.UserId = userId;
                context.UserProductsViews.Add(pv);
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
