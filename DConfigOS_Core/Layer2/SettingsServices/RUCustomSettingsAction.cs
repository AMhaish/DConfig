using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Threading.Tasks;
using SABFramework.Core;
using SABFramework.PreDefinedModules.MembershipModule.DataProviders;
using SABFramework.PreDefinedModules.MembershipModule.Models;
using Ninject;

namespace DConfigOS_Core.Layer2.SettingsServices
{
    class RUCustomSettingsAction : SABFramework.Core.SABAction
    {
        [Inject]
        public IWebsiteSettingsAPI websiteSettingsAPI { get; set; }

        public string Key { get; set; }
        public string Value { get; set; }

        public override async Task<SABActionResult> GetHandler(Controller controller)
        {               
            string _Value = websiteSettingsAPI.Get(Key);
            if (_Value == null)
            {
                websiteSettingsAPI.Save(Key, "");
                _Value = "";
            }
            return Json(_Value);
        }


        public override async Task<SABFramework.Core.SABActionResult> PutHandler(Controller controller)
        {
            try
            {
                websiteSettingsAPI.Save(Key, Value);               
                return Json(new { result = "true" });
            }
            catch (Exception ex)
            {
                return Json(new { result = "false", message = ex.Message });
            }
        }

    }
}
