﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DConfigOS_Core.Layer2.ActionsModels;
using DConfigOS_Core.Repositories.WebsiteContentServices;
using DConfigOS_Core.Models;
using Ninject;

namespace DConfigOS_Core.Layer2.FormsServices
{
    public class GetRootFormsTree : UserActionsBase
    {
        [Inject]
        public IFormsAPI formsAPI { get; set; }

        public override async Task<SABFramework.Core.SABActionResult> GetHandler(System.Web.Mvc.Controller controller)
        {
            var forms = formsAPI.GetRootForms().Select(c => new TreeNodeModel()
            {
                id = c.Id.ToString(),
                obj = c,
                text = c.Name
            });
            return Json(forms);
        }

    }
}
