using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SABFramework.PreDefinedModules.MembershipModule.DataProviders
{
    public interface IWebsiteSettingsAPI
    {
        string Get(string key, int? contextCompanyId = null);
        void Save(string key, string value, int? contextCompanyId = null);


    }
}
