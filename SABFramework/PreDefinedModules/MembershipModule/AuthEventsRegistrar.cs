using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SABFramework.PreDefinedModules.MembershipModule
{
    public class AuthEventsRegistrar
    {
        public delegate bool AuthEvent(System.Web.HttpContextBase httpContext);
        internal List<AuthEvent> Events { get; set; }
        private static AuthEventsRegistrar _instance;
        private AuthEventsRegistrar()
        {
            Events = new List<AuthEvent>();
        }

        public static AuthEventsRegistrar Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AuthEventsRegistrar();
                }
                return _instance;
            }
        }

        public void RegisterEvent(AuthEvent handler)
        {
            Events.Add(handler);
        }
    }
}
