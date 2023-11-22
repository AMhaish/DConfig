using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.IO.Compression;
using SABFramework.Core.DataCore;
using SABFramework.Core.ReturnedTypesHandlers;
using System.Web.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Net;
using SABFramework.PreDefinedModules.MembershipModule.DataProviders;
using Ninject;
using System.Reflection;

namespace SABFramework.Core
{

    [PreDefinedModules.MembershipModule.SABAuthorize()]
    [PreDefinedModules.MembershipModule.SABRequireHttps()]
    [PreDefinedModules.MembershipModule.SABAllowCROS()]
    internal class SABActionControllerBase : System.Web.Mvc.Controller
    {
        private IWebsiteSettingsAPI websitesSettings;

        public SABActionInfo ActionInfo { get; set; }

        public SABActionControllerBase(IWebsiteSettingsAPI websettings) { this.websitesSettings = websettings; }

        public async Task<ActionResult> ActionHandlerExecuter(IAction action)
        {
            try
            {
                ActionInfo.ActionHandler = action;
                SABActionResult result = null;
                string output = "";
                if (ActionInfo.ActionHandler != null)
                {
                    if (SABPreActionEventsRegistrar.Instance.CheckAndExecuteRegularHandlers(HttpContext) &&
                        SABPreActionEventsRegistrar.Instance.CheckAndExecuteHandlers(HttpContext, ActionInfo.ControllerName, ActionInfo.ActionName, ActionInfo.RequestType, out output))
                    {
                        SABCoreEngine.Instance.DInjector.Inject(action);
                        switch (ActionInfo.RequestType)
                        {
                            case RequestType.Get:
                                result = await action.GetHandler(this);
                                break;
                            case RequestType.Post:
                                result = await action.PostHandler(this);
                                break;
                            case RequestType.Put:
                                result = await action.PutHandler(this);
                                break;
                            case RequestType.Delete:
                                result = await action.DeleteHandler(this);
                                break;
                        }
                    }
                    else
                    {
                        result = new SABActionResult();
                        result.ReturnedType = ReturnedType.Json;
                        result.Model = new { result = "false", message = output };
                    }
                    SABPostActionEventsRegistrar.Instance.CheckHandlerExistenceAndExecuteIt(HttpContext, ActionInfo.ControllerName, ActionInfo.ActionName, ActionInfo.RequestType, this, action, result, out output);
                    SABPostActionEventsRegistrar.Instance.CheckAndExecuteRegularHandlers(HttpContext, ActionInfo, action, out output);
                    if (!String.IsNullOrEmpty(output))
                    {
                        SABFramework.Core.SABCoreEngine.Instance.ErrorHandler.ProcessError(output);
                    }
                }
                string viewpath = (String.IsNullOrEmpty(ActionInfo.ModuleInfo.ViewsPath) ? ActionInfo.ViewLocation : ActionInfo.ModuleInfo.ViewsPath + ActionInfo.ViewLocation);
                if (result != null)
                {
                    switch (result.ReturnedType)
                    {
                        case ReturnedType.View:
                            if (String.IsNullOrEmpty(result.MasterPath))
                            {
                                return View((String.IsNullOrEmpty(result.ViewPath) ? viewpath : result.ViewPath), result.Model);
                            }
                            else
                            {
                                return View((String.IsNullOrEmpty(result.ViewPath) ? viewpath : result.ViewPath), result.MasterPath, result.Model);
                            }
                        case ReturnedType.PartialView:
                            return PartialView((String.IsNullOrEmpty(result.ViewPath) ? viewpath : result.ViewPath), result.Model);
                        case ReturnedType.Redirect:
                            return Redirect((String.IsNullOrEmpty(result.RedirectPath) ? ActionInfo.RedirectPath : result.RedirectPath));
                        case ReturnedType.Json:
                            string json;
                            json = JsonConvert.SerializeObject(result.Model, Formatting.None);
                            Response.ContentType = "application/json";
                            //return Json(action.ModelResult,JsonRequestBehavior.AllowGet);
                            return this.Content(json);
                        case ReturnedType.String:
                            Response.ContentType = "text/plain";
                            return this.Content(result.Model.ToString());
                        case ReturnedType.Download:
                            return new DownloadResult { VirtualPath = result.DownloadPath, FileDownloadName = result.DownloadName };
                        case ReturnedType.UserDefined:
                            return (result.UserDefinedResult);
                        case ReturnedType.FileStream:
                            return File(result.FilteStream, "application/octet-stream");
                        case ReturnedType.HttpStatusCode:
                            return new HttpStatusCodeResult(result.StatusCode);
                        case ReturnedType.Empty:
                            return new EmptyResult();
                        case ReturnedType.HttpNotFound:
                            string notFoundPath = websitesSettings.Get("General_NotFoundPath");
                            if (!String.IsNullOrEmpty(notFoundPath))
                            {
                                return Redirect(notFoundPath);
                            }
                            else
                            {
                                return new HttpStatusCodeResult(404);
                            }
                        case ReturnedType.XMLDocument:
                            return this.Content(new System.IO.StreamReader(result.FilteStream).ReadToEnd(), "text/xml");
                        default:
                            return View(viewpath);
                    }
                }
                else
                {
                    if (!String.IsNullOrEmpty(viewpath))
                    {
                        return View(viewpath);
                    }
                    else
                    {
                        return HttpNotFound();
                    }
                }

            }
            catch (Exception ex)
            {
                SABFramework.Core.SABCoreEngine.Instance.ErrorHandler.ProcessError("Error in executing action:", ex);
                if (Request.Headers["Ajax"] == "true")
                {
                    var json = JsonConvert.SerializeObject(new { result = "false", message = "Unknown error happened, please return to DConfig support team." }, Formatting.None);
                    Response.ContentType = "application/json";
                    return this.Content(json);
                }
                else
                {
                    string errorPath = websitesSettings.Get("General_ErrorPath");
                    if (!String.IsNullOrEmpty(errorPath))
                    {
                        return Redirect(errorPath);
                    }
                    else
                    {
                        Response.ContentType = "text/plain";
                        return this.Content("Unknown error happened, please return to DConfig support team.");
                    }
                }
            }
        }
    }
}