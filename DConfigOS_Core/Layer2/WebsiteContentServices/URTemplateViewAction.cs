using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DConfigOS_Core.Layer2.ActionsModels;
using DConfigOS_Core.Repositories.WebsiteContentServices;
using DConfigOS_Core.Models;
using System.IO;
using Newtonsoft.Json;
using Ninject;

namespace DConfigOS_Core.Layer2.WebsiteContentServices
{
    public class URTemplateViewAction : UserActionsBase
    {
        public int Id { get; set; }
        public string PostedView { get; set; }
        public override async Task<SABFramework.Core.SABActionResult> GetHandler(System.Web.Mvc.Controller controller)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var template = context.ViewTemplates.Where(m => m.Id == Id && (UserBasedApps != true || m.CreatorId == UserId)).FirstOrDefault();
            if (template != null)
            {
                string view = "";
                string path = SABFramework.Core.SABCoreEngine.Instance.AppPhysicalPath + template.Path.TrimStart('~');
                if (File.Exists(path))
                {
                    var str = new StreamReader(path);
                    view = await str.ReadToEndAsync();
                    str.Close();
                }
                if (view.StartsWith("@model DConfigModel"))
                {
                    return Str(view.Remove(0, 21));//Deleting @model Content from the top of the view
                }
                else
                {
                    return Str(view);
                }
            }
            else
            {
                return Str("Error!, Template not found!");
            }
        }

        public override async Task<SABFramework.Core.SABActionResult> PutHandler(System.Web.Mvc.Controller controller)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var template = context.ViewTemplates.Where(m => m.Id == Id && (UserBasedApps !=true || m.CreatorId==UserId)).FirstOrDefault();
            if (template != null)
            {
                string path = SABFramework.Core.SABCoreEngine.Instance.AppPhysicalPath + template.Path.Replace("/", "\\").TrimStart('~');
                if (PostedView == null)
                {
                    PostedView = "";
                }
                if (!PostedView.StartsWith("@model DConfigModel"))
                {
                    PostedView = "@model DConfigModel\r\n" + PostedView;
                }
                if (File.Exists(path))
                {
                    var utf8EmitBOM = new UTF8Encoding(true);
                    var str = new StreamWriter(path, false, utf8EmitBOM);
                    await str.WriteLineAsync(PostedView);
                    str.Close();
                }
                else
                {
                    var str = new StreamWriter(path);
                    await str.WriteLineAsync(PostedView);
                    str.Close();
                }
                return Json(new { result = "true" });
            }
            else
            {
                return Json(new { result = "false", message = "Template not found!" });
            }
        }

    }
}
