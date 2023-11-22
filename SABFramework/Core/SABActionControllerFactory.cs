using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using SABFramework.Core.DataCore;
using SABFramework.PreDefinedModules.MembershipModule.DataProviders;
using Ninject;

namespace SABFramework.Core
{
    public class SABActionControllerFactory : DefaultControllerFactory
    {
        public override IController CreateController(System.Web.Routing.RequestContext requestContext, string controllerName)
        {
            var settingsProvider = SABFramework.Core.SABCoreEngine.Instance.DInjector.Get<IWebsiteSettingsAPI>();
            System.Web.Mvc.Controller controller = new SABActionControllerBase(settingsProvider);
            controller.ControllerContext = new ControllerContext(requestContext, controller);
            SABActionControllerBase castedController = (controller as SABActionControllerBase);
            string areaName = (requestContext.RouteData.Values["area"] != null ? (requestContext.RouteData.Values["area"] as string).ToLower() : "");
            string actionName = (requestContext.RouteData.Values["action"] != null ? (requestContext.RouteData.Values["action"] as string).ToLower() : "");
            RequestType requestType = SABActionInfo.ResolveRequestType(requestContext);
            if (requestType == RequestType.Options)
            {
                controller.ActionInvoker = new SABActionInvoker() { ActionInfo = null, CacheMinutes = null };
                return controller; 
            }
            var action = SABCoreEngine.Instance.ModulesProxy.GetActionDescriptor(areaName, controllerName, actionName, requestType);
            if (action != null)
            {
                Type actionType = SABCoreEngine.Instance.ModulesProxy.GetTypeFromModule(action.ActionModule.Namespace, action.HandlerTypeName);
                castedController.ActionInfo = new SABActionInfo()
                            {
                                ActionName = action.Name,
                                ViewLocation = action.ViewPath,
                                Result = castedController.ActionHandlerExecuter,
                                RequestType = requestType,
                                RedirectPath = action.RedirectPath,
                                ControllerName = controllerName,
                                ActionType = actionType,
                                ModuleInfo = action.ActionModule
                            };
                controller.ActionInvoker = new SABActionInvoker() { ActionInfo = castedController.ActionInfo, CacheMinutes = action.CacheMinutes };
                return controller;
            }
            else
            {
#if DEBUG

                try
                {
                    return base.CreateController(requestContext, controllerName);
                }
                catch
                {
                    throw new Exception("Action hasn't been found to be executed");
                }
#else
                return base.CreateController(requestContext, controllerName);
#endif
            }
        }
    }
}