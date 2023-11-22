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
using System.Web.Optimization;

namespace DConfigOS_Core.Layer2.WebsiteContentServices
{
    public class URScriptCodeAction : UserActionsBase
    {
        public int Id { get; set; }
        public string PostedCode { get; set; }
        public override async Task<SABFramework.Core.SABActionResult> GetHandler(System.Web.Mvc.Controller controller)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var script = context.Scripts.Where(m => m.Id == Id && (UserBasedApps != true || m.Bundle.CreatorId == UserId)).FirstOrDefault();
            if (script != null)
            {
                string view = "";
                string path = SABFramework.Core.SABCoreEngine.Instance.AppPhysicalPath + script.Path.TrimStart('~');
                if (File.Exists(path))
                {
                    var str = new StreamReader(path);
                    view = await str.ReadToEndAsync();
                    str.Close();
                }
                return Str(view);
            }
            else
            {
                return Str("Error!, Script not found!");
            }
        }

        public override async Task<SABFramework.Core.SABActionResult> PutHandler(System.Web.Mvc.Controller controller)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var script = context.Scripts.Where(m => m.Id == Id && (UserBasedApps != true || m.Bundle.CreatorId == UserId)).FirstOrDefault();
            if (script != null)
            {
                string path = SABFramework.Core.SABCoreEngine.Instance.AppPhysicalPath + script.Path.Replace("/","\\").TrimStart('~');
                if (File.Exists(path))
                {
                    var str = new StreamWriter(path);
                    if (PostedCode != null)
                    {
                        await str.WriteLineAsync(PostedCode);
                    }
                    else
                    {
                        await str.WriteLineAsync("");
                    }
                    str.Close();
                }
                else
                {
                    var str = new StreamWriter(path);
                    await str.WriteLineAsync(PostedCode);
                    str.Close();
                }
                Providers.ResourcesProviders.BundlesProvider.Instance.ReInitializeScriptBundle(script.Bundle);
                return Json(new { result = "true" });
            }
            else
            {
                return Json(new { result = "false", message = "Script not found!" });
            }
        }

    }
}
