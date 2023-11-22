using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SABFramework.PreDefinedModules.MembershipModule.Models;

namespace SABFramework.PreDefinedModules.MembershipModule.DataProviders
{
    public  class WebsiteSettingsAPI : IWebsiteSettingsAPI
    {

        public virtual string Get(string key,int? contextCompanyId=null)
        {
            MembershipDBContext context;
            if(contextCompanyId.HasValue)
            {
                context = new MembershipDBContext(contextCompanyId.Value);
            }
            else
            {
                context = new MembershipDBContext();
            }
            var stringCon = context.WebsiteSettings.FirstOrDefault(m => m.Key==key);
            return (stringCon != null ? stringCon.Value : null);
        }

        public virtual void Save(string key,string value, int? contextCompanyId = null)
        {
            MembershipDBContext context;
            if (contextCompanyId.HasValue)
            {
                context = new MembershipDBContext(contextCompanyId.Value);
            }
            else
            {
                context = new MembershipDBContext();
            }
            var stringCon = context.WebsiteSettings.SingleOrDefault(m => m.Key == key);
            if(stringCon==null)
            {
                stringCon = new WebsiteSetting();
                stringCon.Key = key;
                stringCon.Value = value;
                stringCon.Id = Guid.NewGuid();
                context.WebsiteSettings.Add(stringCon);
            }
            else
            {
                stringCon.Value = value;
            }
            context.SaveChanges();
        }
    }
}
