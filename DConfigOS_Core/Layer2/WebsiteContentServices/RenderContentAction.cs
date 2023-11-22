using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DConfigOS_Core.Layer2.ActionsModels;
using DConfigOS_Core.Repositories.WebsiteContentServices;
using DConfigOS_Core.Models;
using DConfigOS_Core.Providers.HttpContextProviders;
using SABFramework.PreDefinedModules.MembershipModule.DataProviders;
using Ninject;

namespace DConfigOS_Core.Layer2.WebsiteContentServices
{
    public class RenderContentAction : SABFramework.Core.SABAction
    {
        public string Path { get; set; }
        public int? PageFormId { get; set; }

        [Inject]
        public IWebsiteSettingsAPI websiteSettingsAPI { get; set; }

        public override async Task<SABFramework.Core.SABActionResult> GetHandler(System.Web.Mvc.Controller controller)
        {
            var currentDomain = controller.HttpContext.Request.Url.Host;
            if(currentDomain=="localhost")
            {
                currentDomain = SABFramework.Core.SABCoreEngine.Instance.Settings["Domain"];
            }
            if (DConfigRequestContext.Domains.ContainsKey(currentDomain))
            {
                DConfigRequestContext.Current.DomainId = DConfigRequestContext.Domains[currentDomain];
                DConfigRequestContext.Current.ContextId = DConfigRequestContext.Contexts[currentDomain];
            }
            else
            {
                SABFramework.Core.SABCoreEngine.Instance.ErrorHandler.ProcessError(@"Couldn't find the domain '" + currentDomain  + "' in domains cache - " +
                    "currently there are " + DConfigRequestContext.Domains.Count + " domains stored in the cache - they are: (" + String.Join("-",DConfigRequestContext.Domains.Select(m => m.Key).ToArray()) + ")");
                return HttpStatusCode(System.Net.HttpStatusCode.NotFound);
            }
            if (DConfigRequestContext.Current.ContextId.HasValue)
            {
                DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext(DConfigRequestContext.Current.ContextId.Value);
                Content page;
                if (!String.IsNullOrEmpty(Path) && Path.EndsWith("/"))
                    Path = Path.TrimEnd('/');
                if (!String.IsNullOrEmpty(Path))
                {
                    if (Path.Contains('/'))
                    {
                        var indexOfFirstPath = Path.IndexOf('/');
                        if (indexOfFirstPath < 0 || indexOfFirstPath > 2)
                        {
                            page = context.Contents.Where(m => m.DomainId == DConfigRequestContext.Current.DomainId && m.UrlName != null && m.UrlFullCode == "/" + Path && m.Online == true).FirstOrDefault();
                        }
                        else
                        {
                            var language = Path.Substring(0, indexOfFirstPath);
                            var realPath = Path.Substring(indexOfFirstPath + 1, Path.Length - indexOfFirstPath - 1);
                            DConfigRequestContext.Current.Language = language.ToUpper();
                            page = context.Contents.Where(m =>  m.DomainId == DConfigRequestContext.Current.DomainId && m.UrlName != null && m.UrlFullCode == "/" + realPath && m.Online == true).FirstOrDefault();
                        }
                    }
                    else
                    {
                        if (Path.Length < 3)
                        {
                            DConfigRequestContext.Current.Language = Path.ToUpper();
                            page = context.Contents.Where(m =>  m.DomainId == DConfigRequestContext.Current.DomainId && !m.ParentId.HasValue && m.Online == true).FirstOrDefault();
                        }
                        else
                        {
                            page = context.Contents.Where(m =>  m.DomainId == DConfigRequestContext.Current.DomainId && m.UrlName != null && m.UrlFullCode == "/" + Path && m.Online == true).FirstOrDefault();
                        }
                    }
                }
                else
                {
                    page = context.Contents.Where(m => m.DomainId == DConfigRequestContext.Current.DomainId && !m.ParentId.HasValue && m.Online == true).FirstOrDefault();
                }
                if (page != null)
                {
                    var model = DConfigModel.BuildDConfigModel(page, PageFormId);
                    if (model.ActiveContentInstance != null)
                    {
                        switch (page.ContentType)
                        {
                            case ContentTypes.Page:
                                if (model.ActiveContentInstance.ViewTemplate != null && !String.IsNullOrEmpty(model.ActiveContentInstance.ViewTemplate.Path))
                                    return View(model.ActiveContentInstance.ViewTemplate.Path, model);
                                else
                                    return HttpStatusCode(System.Net.HttpStatusCode.NoContent);
                            case ContentTypes.Partial:
                                return PartialView(model.ActiveContentInstance.ViewTemplate.Path, model);
                            case ContentTypes.Redirect:
                                return Redirect(model.ActiveContentInstance.RedirectUrl);
                            case ContentTypes.Download:
                                if (!String.IsNullOrEmpty(model.ActiveContentInstance.DownloadName))
                                    return Download(model.ActiveContentInstance.DownloadPath, model.ActiveContentInstance.DownloadName);
                                else
                                    return Download(model.ActiveContentInstance.DownloadPath, model.ActiveContentInstance.Name);
                        }
                    }
                    SABFramework.Core.SABCoreEngine.Instance.ErrorHandler.ProcessError(@"Couldn't find a return type for the following content id:" + page.Id);
                    return HttpStatusCode(System.Net.HttpStatusCode.NotFound);
                }
                else
                {
                    SABFramework.Core.SABCoreEngine.Instance.ErrorHandler.ProcessError(@"Couldn't find the requested page:" + Path);
                    string notFoundPath = websiteSettingsAPI.Get("General_NotFoundPath");
                    if (!String.IsNullOrEmpty(notFoundPath))
                    {
                        page = context.Contents.Where(m => m.DomainId == DConfigRequestContext.Current.DomainId && m.UrlName != null && m.UrlFullCode == notFoundPath  && m.Online == true).FirstOrDefault();
                        if (page != null)
                        {
                            var model = DConfigModel.BuildDConfigModel(page, PageFormId);
                            if (model.ActiveContentInstance != null)
                            {
                                switch (page.ContentType)
                                {
                                    case ContentTypes.Page:
                                        if (model.ActiveContentInstance.ViewTemplate != null && !String.IsNullOrEmpty(model.ActiveContentInstance.ViewTemplate.Path))
                                            return View(model.ActiveContentInstance.ViewTemplate.Path, model);
                                        else
                                            return HttpStatusCode(System.Net.HttpStatusCode.NoContent);
                                }
                            }
                            SABFramework.Core.SABCoreEngine.Instance.ErrorHandler.ProcessError(@"Couldn't find a return type for the following content id:" + page.Id);
                            return HttpStatusCode(System.Net.HttpStatusCode.NotFound);
                        }
                        else
                        {
                            return HttpStatusCode(System.Net.HttpStatusCode.NotFound);
                        }
                    }
                    else
                    {
                        return HttpStatusCode(System.Net.HttpStatusCode.NotFound);
                    }
                }
            }
            else
            {
                SABFramework.Core.SABCoreEngine.Instance.ErrorHandler.ProcessError(@"Couldn't find context id for the domain '" + currentDomain + "' in context ids cache -" +
                   "currently there are " + DConfigRequestContext.Contexts.Count + " context stored in the cache - they are: (" + String.Join("-", DConfigRequestContext.Domains.Select(m => m.Key).ToArray()) + ")");
                return HttpStatusCode(System.Net.HttpStatusCode.NotFound);
            }
        }
    }
}
