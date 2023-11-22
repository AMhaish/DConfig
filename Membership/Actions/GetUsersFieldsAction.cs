using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Membership.Models;
using Membership.MembershipServices;
using Ninject;

namespace Membership.Actions
{
    public class GetUsersFieldsAction : SABFramework.Core.SABAction
    {
        [Inject]
        public IUsersFieldsAPI usersFieldsAPI { get; set; }

        public override async Task<SABFramework.Core.SABActionResult> GetHandler(System.Web.Mvc.Controller controller)
        {
            var result = usersFieldsAPI.GetUsersFields();
            return Json(result);
        }
    }
}
