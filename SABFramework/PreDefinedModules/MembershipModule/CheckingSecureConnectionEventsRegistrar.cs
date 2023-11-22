using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SABFramework.PreDefinedModules.MembershipModule
{
    public class CheckingSecureConnectionEventsRegistrar
    {
        public delegate bool CheckingSecureConnectionEvent(System.Web.HttpContextBase httpContext);
        internal List<CheckingSecureConnectionEvent> Events { get; set; }
        private static CheckingSecureConnectionEventsRegistrar _instance;
        private CheckingSecureConnectionEventsRegistrar()
        {
            Events = new List<CheckingSecureConnectionEvent>();
        }

        public static CheckingSecureConnectionEventsRegistrar Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new CheckingSecureConnectionEventsRegistrar();
                }
                return _instance;
            }
        }

        public void RegisterEvent(CheckingSecureConnectionEvent handler)
        {
            Events.Add(handler);
        }
    }
}
