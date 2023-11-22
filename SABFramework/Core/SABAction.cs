using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Web.Routing;
using System.Web.Configuration;
using Ninject;

namespace SABFramework.Core
{
    public abstract class SABAction:IAction
    {
        public virtual Task<SABActionResult> GetHandler(Controller controller)
        {
            return Task.FromResult(Json("Not implemented."));
        }

        public virtual Task<SABActionResult> PostHandler(Controller controller)
        {
            return Task.FromResult(Json("Not implemented."));
        }

        public virtual Task<SABActionResult> PutHandler(Controller controller)
        {
            return Task.FromResult(Json("Not implemented."));
        }

        public virtual Task<SABActionResult> DeleteHandler(Controller controller)
        {
            return Task.FromResult(Json("Not implemented."));
        }

        public virtual SABActionResult View(string viewPath, Object model)
        {
            SABActionResult result = new SABActionResult();
            result.ReturnedType = ReturnedType.View;
            result.ViewPath = viewPath;
            result.Model = model;
            return result;
        }

        public virtual SABActionResult View(string viewPath,string masterPath ,Object model)
        {
            SABActionResult result = new SABActionResult();
            result.ReturnedType = ReturnedType.View;
            result.ViewPath = viewPath;
            result.Model = model;
            result.MasterPath = masterPath;
            return result;
        }

        public virtual SABActionResult View(string viewPath)
        {
            SABActionResult result = new SABActionResult();
            result.ReturnedType = ReturnedType.View;
            result.ViewPath = viewPath;
            return result;
        }

        public virtual SABActionResult View(string viewPath,string masterPath)
        {
            SABActionResult result = new SABActionResult();
            result.ReturnedType = ReturnedType.View;
            result.ViewPath = viewPath;
            result.MasterPath = masterPath;
            return result;
        }

        public virtual SABActionResult View(Object model)
        {
            SABActionResult result = new SABActionResult();
            result.ReturnedType = ReturnedType.View;
            result.Model = model;
            return result;
        }

        public virtual SABActionResult PartialView(string viewPath, Object model)
        {
            SABActionResult result = new SABActionResult();
            result.ReturnedType = ReturnedType.PartialView;
            result.ViewPath = viewPath;
            result.Model = model;
            return result;
        }

        public virtual SABActionResult PartialView(string viewPath)
        {
            SABActionResult result = new SABActionResult();
            result.ReturnedType = ReturnedType.PartialView;
            result.ViewPath = viewPath;
            return result;
        }

        public virtual SABActionResult PartialView(Object model)
        {
            SABActionResult result = new SABActionResult();
            result.ReturnedType = ReturnedType.PartialView;
            result.Model = model;
            return result;
        }

        public virtual SABActionResult EmptyResult()
        {
            SABActionResult result = new SABActionResult();
            result.ReturnedType = ReturnedType.Empty;
            return result;
        }

        public virtual SABActionResult Download(string filePath,string fileName)
        {
            SABActionResult result = new SABActionResult();
            result.ReturnedType = ReturnedType.Download;
            result.DownloadName = fileName;
            result.DownloadPath = filePath;
            return result;
        }

        public virtual SABActionResult FileStream(Stream fileStream)
        {
            SABActionResult result = new SABActionResult();
            result.ReturnedType = ReturnedType.FileStream;
            result.FilteStream = fileStream;
            return result;
        }

        public virtual SABActionResult HttpStatusCode(HttpStatusCode statusCode)
        {
            SABActionResult result = new SABActionResult();
            result.ReturnedType = ReturnedType.HttpStatusCode;
            result.StatusCode = statusCode;
            return result;
        }

        public virtual SABActionResult UserDefinedResult(ActionResult actionResult)
        {
            SABActionResult result = new SABActionResult();
            result.ReturnedType = ReturnedType.UserDefined;
            result.UserDefinedResult = actionResult;
            return result;
        }

        public virtual SABActionResult Redirect(string redirectPath)
        {
            SABActionResult result = new SABActionResult();
            result.ReturnedType = ReturnedType.Redirect;
            result.RedirectPath = redirectPath;
            return result;
        }

        public virtual SABActionResult Json(Object model)
        {
            SABActionResult result = new SABActionResult();
            result.ReturnedType = ReturnedType.Json;
            result.Model = model;
            return result;
        }

        public virtual SABActionResult Str(Object model)
        {
            SABActionResult result = new SABActionResult();
            result.ReturnedType = ReturnedType.String;
            result.Model = model;
            return result;
        }

        public virtual SABActionResult HttpNotFound()
        {
            SABActionResult result = new SABActionResult();
            result.ReturnedType = ReturnedType.HttpNotFound;
            return result;
        }

        public virtual SABActionResult XMLDocument(FileStream stream)
        {
            SABActionResult result = new SABActionResult();
            result.ReturnedType = ReturnedType.XMLDocument;
            result.FilteStream = stream;
            return result;
        }

        public static string RenderViewToString(Controller controller, string viewName, object model)
        {
            controller.ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(controller.ControllerContext, viewName);
                var viewContext = new ViewContext(controller.ControllerContext, viewResult.View, controller.ViewData, controller.TempData, sw);
                viewContext.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(controller.ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }

        public static string RenderViewToString(Controller controller, string viewName)
        {
            return RenderViewToString(controller,viewName,null);
        }

        public static string RenderViewToString(string viewName,object model)
        {
            Controller fakeController = new FakeController();
            var routeData = new RouteData();
            routeData.Values.Add("controller", "FakeController");
            var fakeControllerContext = new ControllerContext(new HttpContextWrapper(new HttpContext(new HttpRequest(null, "http://" + SABFramework.Core.SABCoreEngine.Instance.Settings[SABSettings.SABSettings_Domain], null), new HttpResponse(null))), routeData, fakeController);
            fakeController.ControllerContext = fakeControllerContext;
            return RenderViewToString(fakeController, viewName, model);
        }

        public class FakeController : Controller { }
    }
}
