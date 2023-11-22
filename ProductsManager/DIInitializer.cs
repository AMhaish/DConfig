using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SABFramework.Core;
using CompetitiveAnalysis.ProductsManagerServices;

namespace CompetitiveAnalysis
{
    public class DIInitializer : NinjectModule
    {
        public override void Load()
        {
            Bind<IAdvancedPrivilegesAPI>().To<AdvancedPrivilegesAPI>();
            Bind<IComparisonAPI>().To<ComparisonsAPI>();
            Bind<IProductsAPI>().To<ProductsAPI>();
            Bind<IPropertiesAPI>().To<PropertiesAPI>();
            Bind<IPropertiesEnumsAPI>().To<PropertiesEnumsAPI>();
            Bind<IUserProductViewsAPI>().To<UserProductViewsAPI>();
        }
    }
}
