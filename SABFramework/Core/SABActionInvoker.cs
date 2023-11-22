using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Caching;
using SABFramework.Core.DataCore;
using System.Threading.Tasks;
using System.Web.Mvc.Async;

namespace SABFramework.Core
{
    internal class SABActionInvoker : AsyncControllerActionInvoker
    {
        public SABActionInfo ActionInfo { get; set; }

        public int? CacheMinutes { get; set; }

        public override bool InvokeAction(ControllerContext controllerContext, string actionName)
        {
            //ActionResult actionResult = null;
            //if (CacheMinutes != null)
            //{
            //    if (controllerContext.HttpContext.Cache[ActionInfo.ActionName.ToString() + ActionInfo.RequestType] != null)
            //    {
            //        actionResult = (ActionResult)controllerContext.HttpContext.Cache[ActionInfo.ActionName.ToString() + ActionInfo.RequestType];
            //    }
            //    else
            //    {
            //        actionResult = ActionInfo.Result(ActionInfo.ActionHandler);
            //        controllerContext.HttpContext.Cache.Add(ActionInfo.ActionName.ToString() + ActionInfo.RequestType, actionResult, null, DateTime.Now.Add(new TimeSpan(0, CacheMinutes.Value, 0)), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            //    }
            //}
            //else
            //{
            //    actionResult = ActionInfo.Result(ActionInfo.ActionHandler);
            //}
            //base.InvokeActionResult(controllerContext, actionResult);
#if DEBUG
            try
            {
                base.InvokeAction(controllerContext, "ActionHandlerExecuter");
            }
            catch (Exception ex)
            {
                SABFramework.Core.SABCoreEngine.Instance.ErrorHandler.ProcessError("Error in invoking an action",ex);
            }
#else
            base.InvokeAction(controllerContext, "ActionHandlerExecuter");
#endif

            return true;
        }

        public override IAsyncResult BeginInvokeAction(ControllerContext controllerContext, string actionName, AsyncCallback callback, object state)
        {
            return base.BeginInvokeAction(controllerContext, "ActionHandlerExecuter", callback, state);
        }

    }
}