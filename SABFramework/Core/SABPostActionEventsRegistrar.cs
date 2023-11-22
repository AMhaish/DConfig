using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SABFramework.Core.DataCore;

namespace SABFramework.Core
{
    public class SABPostActionEventsRegistrar
    {
        public delegate void PostEvent(System.Web.HttpContextBase httpContext, System.Web.Mvc.Controller controller, IAction action, SABActionResult actionResult, out string message);
        public delegate void RegularPostEvent(System.Web.HttpContextBase httpContext, SABActionInfo actionDetails, IAction actionModel, out string message);
        internal Dictionary<int, List<PostEvent>> Events { get; set; }
        internal List<RegularPostEvent> RegularEvents { get; set; }
        private static SABPostActionEventsRegistrar _instance;
        private SABPostActionEventsRegistrar()
        {
            Events = new Dictionary<int, List<PostEvent>>();
            RegularEvents = new List<RegularPostEvent>();
        }

        public static SABPostActionEventsRegistrar Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SABPostActionEventsRegistrar();
                }
                return _instance;
            }
        }

        public void RegisterEvent(string controllerName, string actionName, RequestType requestType, PostEvent handler)
        {
            string result = controllerName.ToLower() + "_" + actionName.ToLower() + "_" + requestType;
            int hash = result.GetHashCode();
            if (Events.ContainsKey(hash))
                Events[hash].Add(handler);
            else
            {
                var list = new List<PostEvent>();
                list.Add(handler);
                Events.Add(result.GetHashCode(), list);
            }
        }

        public void RegisterRegularEvent(RegularPostEvent handler)
        {
            RegularEvents.Add(handler);
        }

        internal void CheckAndExecuteRegularHandlers(System.Web.HttpContextBase httpContext, SABActionInfo actionDetails, IAction actionModel, out string output)
        {
            output = null;
            string temp;
            StringBuilder ss = new StringBuilder();
            foreach (RegularPostEvent p in RegularEvents)
            {
                p(httpContext, actionDetails, actionModel, out temp);
                if (temp != null)
                    ss.AppendLine(temp);
            }
            output = ss.ToString();
        }

        internal void CheckHandlerExistenceAndExecuteIt(System.Web.HttpContextBase httpContext, string controllerName, string actionName, RequestType requestType, System.Web.Mvc.Controller controller, IAction action, SABActionResult actionResult, out string output)
        {
            string result = controllerName.ToLower() + "_" + actionName.ToLower() + "_" + requestType;
            int hash = result.GetHashCode();
            if (Events.ContainsKey(hash))
            {
                StringBuilder ss = new StringBuilder();
                string temp;
                foreach (PostEvent p in Events[hash])
                {
                    p(httpContext, controller, action, actionResult, out temp);
                    ss.AppendLine(temp);
                }
                output = ss.ToString();
            }
            else
            {
                output = null;
            }
        }
    }
}
