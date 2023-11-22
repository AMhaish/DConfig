using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DConfigOS_Core.Layer2.ActionsModels;
using Membership.MembershipServices;
using Membership.Models;
using Ninject;

namespace Membership.Actions
{
    public class GetUserFieldEnumsTreeAction : SABFramework.Core.SABAction
    {
        [Inject]
        public IUserFieldsEnumsAPI UserFieldsEnumsAPI { get; set; }

        public override async Task<SABFramework.Core.SABActionResult> GetHandler(System.Web.Mvc.Controller controller)
        {
            var result = UserFieldsEnumsAPI.GetUserFieldEnums().Select(a => new TreeNodeModel()
            {
                id = a.Id.ToString(),
                obj = a,
                text = a.Name
            });
            return Json(result);
        }
    }
}
