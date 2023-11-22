using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SABFramework.Core;
using System.Data.Entity;
using CompetitiveAnalysis.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System.IO;

namespace CompetitiveAnalysis
{
    public class CompetitiveAnalysisInitializer : IInitializer
    {
        public void Initialize()
        {
            //Database.SetInitializer(new SABFramework.ModulesUtilities.CreateTablesOnlyIfTheyDontExist<ProductsManager_DBContext>());
            //Initizlize resources folder for products
            if(!Directory.Exists(SABFramework.Core.SABCoreEngine.Instance.AppPhysicalPath + "/Resources/Produtcs"))
            {
                Directory.CreateDirectory(SABFramework.Core.SABCoreEngine.Instance.AppPhysicalPath + "/Resources/Produtcs");
            }
        }
    }
}
