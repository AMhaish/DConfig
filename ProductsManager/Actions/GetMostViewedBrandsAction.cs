﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompetitiveAnalysis.Models;
using CompetitiveAnalysis.ProductsManagerServices;
using DConfigOS_Core.Layer2.ActionsModels;
using Ninject;

namespace CompetitiveAnalysis.Actions
{
    public class GetMostViewedBrandsAction : AppBaseAction
    {
        [Inject]
        public IUserProductViewsAPI userProductViewsAPI { get; set; }

        public override async Task<SABFramework.Core.SABActionResult> GetHandler(System.Web.Mvc.Controller controller)
        {
            var result = userProductViewsAPI.GetMostViewedProductsBrands(User.Id);
            return Json(result);
        }
    }
}
