using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SABFramework.Core.DataCore;

namespace SABFramework.Core
{
    public class SABPreActionEventsRegistrar
    {
        private const string REGULAR = "R";
        public delegate bool AuthEvent(System.Web.HttpContextBase httpContext, out string message);
        internal Dictionary<int, AuthEvent> Events { get; set; }
        internal List<AuthEvent> RegularEvents { get; set; }
        private static SABPreActionEventsRegistrar _instance;
        private SABPreActionEventsRegistrar()
        {
            Events = new Dictionary<int, AuthEvent>();
            RegularEvents = new List<AuthEvent>();
        }

        public static SABPreActionEventsRegistrar Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SABPreActionEventsRegistrar();
                }
                return _instance;
            }
        }

        public void RegisterEvent(string controllerName, string actionName, RequestType requestType, AuthEvent handler)
        {
            string result = controllerName + "_" + actionName + "_" + requestType;
            Events.Add(result.GetHashCode(), handler);
        }

        public void RegisterRegularEvent(AuthEvent handler)
        {
            RegularEvents.Add(handler);
        }

        internal bool CheckAndExecuteHandlers(System.Web.HttpContextBase httpContext, string controllerName, string actionName, RequestType requestType, out string output)
        {
            string result = controllerName + "_" + actionName + "_" + requestType;
            int hash = result.GetHashCode();
            if (Events.ContainsKey(hash))
            {
                return Events[hash](httpContext, out output);
            }
            else
            {
                output = null;
                return true;
            }
        }

        internal bool CheckAndExecuteRegularHandlers(System.Web.HttpContextBase httpContext)
        {
            string result;
            bool? boolResult = true;
            foreach (AuthEvent handler in RegularEvents)
            {
                boolResult=handler?.Invoke(httpContext, out result);
                if (!boolResult.Value)
                {
                    break;
                }
            }
            return boolResult.Value;
        }
    }
}
