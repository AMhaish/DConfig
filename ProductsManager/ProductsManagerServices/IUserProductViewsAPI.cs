using CompetitiveAnalysis.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompetitiveAnalysis.ProductsManagerServices
{
    public interface IUserProductViewsAPI
    {
         List<UserViewTypesStatisticsResult> GetMostViewedProductsTypes(string userId);
        List<UserViewBrandsStatisticsResult> GetMostViewedProductsBrands(string userId);
        int RegisterView(string userId, int productId);
    }
}
