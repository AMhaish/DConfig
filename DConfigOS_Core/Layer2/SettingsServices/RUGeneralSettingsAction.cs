using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SABFramework.Core;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.Web;
using DConfigOS_Core.Models;
using DConfigOS_Core.Repositories.WebsiteContentServices;
using DConfigOS_Core.Repositories.Utilities;
using SABFramework.PreDefinedModules.MembershipModule.DataProviders;
using SABFramework.PreDefinedModules.MembershipModule.Models;
using Ninject;

namespace DConfigOS_Core.Layer2.SettingsServices
{
    public class RUGeneralSettingsAction : SABFramework.Core.SABAction
    {
        [Inject]
        public IWebsiteSettingsAPI websiteSettingsAPI { get; set; }

        public bool DConfigIsStartPage { get; set; }
        public string LogoutAction { get; set; }
        public string LogoutRedirect { get; set; }
        public string CustomLoginUrl { get; set; }
        public string NotFoundPath { get; set; }
        public string ErrorPath { get; set; }
        public string ResetPasswordPath { get; set; }
        public string Domain { get; set; }
        public override async Task<SABActionResult> GetHandler(Controller controller)
        {
            var obj = new RUGeneralSettingsAction();
            string LogoutAction = websiteSettingsAPI.Get(WebsiteSetting.General_LogoutAction);
            if (LogoutAction!=null)
            {
                obj.LogoutAction = LogoutAction;
            }
            else
            {
                obj.LogoutAction = "DomainRoot";
            }
             obj.LogoutRedirect = websiteSettingsAPI.Get(WebsiteSetting.General_LogoutRedirect);
            string LoginUrl = websiteSettingsAPI.Get(WebsiteSetting.General_LoginUrl);
            if (LoginUrl != null && LoginUrl != "/DConfig")
            {
                obj.CustomLoginUrl = LoginUrl;
            }
            obj.DConfigIsStartPage = websiteSettingsAPI.Get(WebsiteSetting.General_DConfigIsStartPage) == "true";
            obj.NotFoundPath = websiteSettingsAPI.Get(WebsiteSetting.General_NotFoundPath);
            obj.ErrorPath = websiteSettingsAPI.Get(WebsiteSetting.General_ErrorPath);
            obj.ResetPasswordPath = websiteSettingsAPI.Get(WebsiteSetting.General_ResetPasswordPath);
            obj.Domain = websiteSettingsAPI.Get(WebsiteSetting.General_Domain);
            return Json(obj);
        }
        public override async Task<SABFramework.Core.SABActionResult> PutHandler(Controller controller)
        {
            websiteSettingsAPI.Save(WebsiteSetting.General_DConfigIsStartPage, (DConfigIsStartPage ? "true" : "false"));
            websiteSettingsAPI.Save(WebsiteSetting.General_LogoutAction, LogoutAction);
            websiteSettingsAPI.Save(WebsiteSetting.General_LogoutRedirect, LogoutRedirect);
            websiteSettingsAPI.Save(WebsiteSetting.General_NotFoundPath, NotFoundPath);
            websiteSettingsAPI.Save(WebsiteSetting.General_ErrorPath, ErrorPath);
            websiteSettingsAPI.Save(WebsiteSetting.General_Domain, Domain);
            websiteSettingsAPI.Save(WebsiteSetting.General_ResetPasswordPath, ResetPasswordPath);
            if (!String.IsNullOrEmpty(CustomLoginUrl))
                websiteSettingsAPI.Save(WebsiteSetting.General_LoginUrl, CustomLoginUrl);
            else
                websiteSettingsAPI.Save(WebsiteSetting.General_LoginUrl, "/DConfig");
            return Json(new { result = "true" });
        }

    }
}
