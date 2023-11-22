using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;
using System.Web.Mvc;
using SABFramework.Core.DataCore;
using System.Threading.Tasks;

namespace SABFramework.Core
{
    public class SABActionInfo
    {
        public string ActionName { get; set; }
        public string ControllerName { get; set; }
        public string ViewLocation { get; set; }
        public IAction ActionHandler { get; set; }
        public Type ActionType { get; set; }
        public Func<IAction, Task<ActionResult>> Result { get; set; }
        public RequestType RequestType { get; set; }
        public string RedirectPath { get; set; }
        public string PreviousViewLocation { get; set; }

        public Core.DataCore.Module ModuleInfo { get; set; }

        //internal static Type ResolveAction(DataCore.Module module, string ModelTypeName, RequestType requestType, ControllerContext controllerContext)
        //{
        //    Type actionType = null;
        //    if (!String.IsNullOrEmpty(ModelTypeName) && module != null)
        //    {
        //        Assembly assembly;
        //        //IAction action = null;
        //        if (module.BuiltIn)
        //        {
        //            actionType = Type.GetType(module.Namespace + "." + ModelTypeName);
        //            //action = (IAction)Activator.CreateInstance(actionType);
        //        }
        //        else
        //        {
        //            try
        //            {
        //                assembly = Assembly.LoadFrom(module.PhysicalPath);
        //                actionType = assembly.GetType(module.Namespace + "." + ModelTypeName);
        //                //action = (IAction)Activator.CreateInstance(actionType);
        //            }
        //            catch (System.IO.FileNotFoundException ex)
        //            {
        //                SABCoreEngine.Instance.ErrorHandler.ProcessError(" (Can't find dll in the path \"" + module.PhysicalPath + "\")", ex);
        //            }
        //            catch (BadImageFormatException ex2)
        //            {
        //                SABCoreEngine.Instance.ErrorHandler.ProcessError(" (The Dll in the path \"" + module.PhysicalPath + "\" is corrupted)", ex2);
        //            }
        //            catch (System.Security.SecurityException ex3)
        //            {
        //                SABCoreEngine.Instance.ErrorHandler.ProcessError(" (The Application doesn't have permissions to access the Dll in the path \"" + module.PhysicalPath + "\")", ex3);
        //            }
        //            catch (ArgumentException ex3)
        //            {
        //                SABCoreEngine.Instance.ErrorHandler.ProcessError(" (Couldn't find the type " + module.PhysicalPath + " in the dll)", ex3);
        //            }
        //            catch (InvalidCastException ex4)
        //            {
        //                SABCoreEngine.Instance.ErrorHandler.ProcessError("(Couldn't cast the action into IAction)", ex4);
        //            }
        //        }
        //        //string value;
        //        //IModelBinder binder = ModelBinders.Binders.GetBinder(actionType);
        //        //ModelBindingContext bindingContext = new ModelBindingContext()
        //        //{
        //        //    FallbackToEmptyPrefix = (parameterDescriptor.BindingInfo.Prefix == null), // only fall back if prefix not specified
        //        //    ModelName = parameterName,
        //        //    ModelState = controllerContext.Controller.ViewData.ModelState,
        //        //    ModelType = parameterType,
        //        //    PropertyFilter = propertyFilter,
        //        //    ValueProvider = controllerContext.Controller.ValueProvider
        //        //};
        //        //action = (IAction)binder.BindModel(controllerContext, bindingContext) ;
        //        //if (action != null)
        //        //{
        //        //    foreach (PropertyInfo p in action.GetType().GetProperties())
        //        //    {
        //        //        var attr = p.GetCustomAttributes(typeof(Dependency), false).Where(m => ((Dependency)m).RequestType == requestType).FirstOrDefault();
        //        //        if (attr != null)
        //        //        {
        //        //            value = (controllerContext.HttpContext.Request.Form[p.Name] != null ? AntiXss.HtmlEncode(controllerContext.HttpContext.Request.Form[p.Name]) : null);
        //        //            if (value == null)
        //        //            {
        //        //                value = (controllerContext.HttpContext.Request.QueryString[p.Name] != null ? AntiXss.HtmlEncode(controllerContext.HttpContext.Request.QueryString[p.Name]) : null);
        //        //            }
        //        //            if (value == null)
        //        //            {
        //        //                value = (controllerContext.RouteData.Values[p.Name] != null ? AntiXss.HtmlEncode(controllerContext.RouteData.Values[p.Name].ToString()) : null);
        //        //            }
        //        //            if (value == null)
        //        //            {
        //        //                value = (controllerContext.HttpContext.Request.ServerVariables[p.Name] != null ? AntiXss.HtmlEncode(controllerContext.HttpContext.Request.ServerVariables[p.Name]) : null);
        //        //            }
        //        //            if (!String.IsNullOrEmpty(value))
        //        //            {
        //        //                if (p.PropertyType == typeof(string))
        //        //                {
        //        //                    p.SetValue(action, value, null);
        //        //                }
        //        //                else if (p.PropertyType.IsValueType)
        //        //                {
        //        //                    if (p.PropertyType == typeof(bool))
        //        //                        value = value.Replace("true,false", "true");
        //        //                    try
        //        //                    {
        //        //                        object resultValue;
        //        //                        if (p.PropertyType.IsPrimitive)
        //        //                        {
        //        //                            resultValue = Convert.ChangeType(value, p.PropertyType);
        //        //                        }
        //        //                        else
        //        //                        {
        //        //                            resultValue = Convert.ChangeType(value, p.PropertyType.GetGenericArguments()[0]);
        //        //                        }
        //        //                        p.SetValue(action, resultValue, null);
        //        //                    }
        //        //                    catch (Exception ex)
        //        //                    {
        //        //                        SABCoreEngine.Instance.ErrorHandler.ProcessError("The value passed to the property '" + p.Name + "' in the action is invalid: ", ex);
        //        //                    }
        //        //                }
        //        //            }
        //        //            else if (((Dependency)attr).DependencyType == DependencyType.Required)
        //        //            {
        //        //                SABCoreEngine.Instance.ErrorHandler.ProcessError("Some required properties for the action is missing from the request posted data");
        //        //            }
        //        //        }
        //        //    }
        //        //}
        //        //return action;
        //        return actionType;
        //    }
        //    return null;
        //}

        internal static RequestType ResolveRequestType(System.Web.Routing.RequestContext requestContext)
        {

            switch (requestContext.HttpContext.Request.RequestType)
            {
                case "GET":
                default:
                    return RequestType.Get;
                case "POST":
                    return RequestType.Post;
                case "PUT":
                    return RequestType.Put;
                case "DELETE":
                    return RequestType.Delete;
                case "OPTIONS":
                    return RequestType.Options;
            }

        }

        public static string RequestTypeToString(RequestType requestType)
        {
            switch (requestType)
            {
                case RequestType.Get:
                default:
                    return "GET";
                case RequestType.Post:
                    return "POST";
                case RequestType.Put:
                    return "PUT";
                case RequestType.Delete:
                    return "DELETE";
                case RequestType.Options:
                    return "OPTIONS";
            }
        }
        //internal static ReturnedType parseReturnedValueType(string retValueType)
        //{
        //    switch (retValueType)
        //    {
        //        case "View":
        //            return ReturnedType.View;
        //        case "PartialView":
        //            return ReturnedType.PartialView;
        //        case "Redirect":
        //            return ReturnedType.Redirect;
        //        case "Download":
        //            return ReturnedType.Download;
        //        case "Json":
        //            return ReturnedType.Json;
        //        case "UserDefined":
        //            return ReturnedType.UserDefined;
        //        case "UserDefinedView":
        //            return ReturnedType.UserDefinedView;
        //        default:
        //            return ReturnedType.Json;
        //    }
        //}
    }
}