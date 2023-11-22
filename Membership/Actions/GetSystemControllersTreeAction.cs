using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Membership.Models;
using Membership.MembershipServices;
using DConfigOS_Core.Layer2.ActionsModels;

namespace Membership.Actions
{
    public class GetSystemControllersTreeAction : SABFramework.Core.SABAction
    {
        public override async Task<SABFramework.Core.SABActionResult> GetHandler(System.Web.Mvc.Controller controller)
        {
            Membership_DBContext context = new Models.Membership_DBContext();
            var prvs = context.Privileges.ToList();
            var controllers = SABFramework.Core.SABCoreEngine.Instance.ModulesProxy.RegisteredControllers.OrderBy(m => m.Name).Select(a => new TreeNodeModel()
            {
                id = "C_" + a.Name,
                obj = a,
                text = (String.IsNullOrEmpty(a.Description) ? a.Name : a.Description),
                addObj = prvs.Where(m => m.Controller == a.Name && m.Action == null && m.RequestType == null).SingleOrDefault(),
                type = ContentsTreeNodeType.Container,
                children = a.ActionsDic.Values.OrderBy(m => m.Name).Select(aa => new TreeNodeModel()
                {
                    id = a.Name + "_" + aa.Name + "_" + aa.RequestType,
                    obj = aa,
                    text = (String.IsNullOrEmpty(aa.Description)?aa.Name:aa.Description),
                    addObj = prvs.Where(m => m.Controller == a.Name && m.Action == aa.Name && m.RequestType == null).SingleOrDefault(),
                    type = ContentsTreeNodeType.Item
                }).ToList()
            });
            return Json(controllers);
        }
    }
}
