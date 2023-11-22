using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SABFramework.Core;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.Web;
using Membership.Models;
using Membership.MembershipServices;
using DConfigOS_Core.Repositories.Utilities;
using Ninject;

namespace Membership.Actions
{
    public class RUUsersFieldsValuesAction : SABFramework.Core.SABAction
    {
        [Inject]
        public IUsersFieldsAPI usersFieldsAPI { get; set; }

        public string Id { get; set; }
        public List<UserField> DefinedUserProporties { get; set; }
        public override async Task<SABFramework.Core.SABActionResult> PutHandler(System.Web.Mvc.Controller controller)
        {
            if (DefinedUserProporties != null)
            {
                foreach (UserField f in DefinedUserProporties)
                {
                    var res = usersFieldsAPI.UpdateUserFieldValue(Id, f.Id, f.Value);
                    if (res == ResultCodes.ObjectHasntFound)
                    {
                        return Json(new { result = "false", message = "Content hasn't been found, please do refresh" });
                    }
                }
                return Json(new { result = "true" });
                //var contentInstance = ContentsAPI.GetContentInstance(Id);
                //if (contentInstance != null)
                //{
                //    return Json(new { result = "true", obj = contentInstance });
                //}
                //else
                //{
                //    return Json(new { result = "false", message = "Content hasn't been found to be updated" });
                //}
            }
            else
            {
                return Json(new { result = "true", message = "No fields to be updated" });
            }
        }

        public override async Task<SABActionResult> GetHandler(Controller controller)
        {
            return Json(usersFieldsAPI.GetUserFieldsValues(Id));
        }
    }
}
