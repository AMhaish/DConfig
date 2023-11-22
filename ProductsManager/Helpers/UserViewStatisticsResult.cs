using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompetitiveAnalysis.Models;
using System.Web;
using Microsoft.AspNet.Identity;
using SABFramework.PreDefinedModules.MembershipModule.Models;

namespace CompetitiveAnalysis.Helpers
{
    public class UserViewTypesStatisticsResult
    {
        public virtual ProductsTemplate ProductType { get; set; }
        public double Percentage { get; set; }
    }
    public class UserViewBrandsStatisticsResult
    {
        public virtual Company Company { get; set; }
        public double Percentage { get; set; }
    }
}
