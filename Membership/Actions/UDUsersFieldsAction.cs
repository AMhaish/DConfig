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
    public class UDUsersFieldsAction : SABFramework.Core.SABAction
    {
        [Inject]
        public IUsersFieldsAPI usersFieldsAPI { get; set; }

        public int Id { get; set; }
        public List<UserField> DefinedUserProporties { get; set; }
        public override async Task<SABFramework.Core.SABActionResult> PutHandler(System.Web.Mvc.Controller controller)
        {
            if (DefinedUserProporties != null)
            {
                foreach (UserField vf in DefinedUserProporties)
                {
                    if (vf.Id == 0)
                    {
                        if (usersFieldsAPI.AddUserField(vf) == ResultCodes.ObjectNameAlreadyUsed)
                        {
                            return Json(new { result = "false", message = "Field with name " + vf.Name + " already exists" });
                        }
                    }
                    else
                    {
                        var res = usersFieldsAPI.UpdateUserField(vf);
                        if (res == ResultCodes.ObjectHasntFound)
                        {
                            return Json(new { result = "false", message = "Some fields hasn't been found, please do refresh" });
                        }
                        if (res == ResultCodes.ObjectNameAlreadyUsed)
                        {
                            return Json(new { result = "false", message = "Field with name " + vf.Name + " already exists" });
                        }
                    }
                }
                return Json(new { result = "true" });
            }
            else
            {
                return Json(new { result = "false", message = "No fields to be updated" });
            }
        }

        public override async Task<SABActionResult> DeleteHandler(Controller controller)
        {
            var result = usersFieldsAPI.DeleteUserField(Id);
            if (result == ResultCodes.Succeed)
            {
                return Json(new { result = "true" });
            }
            else
            {
                return Json(new { result = "false", message = "Field hasn't been found to be deleted" });
            }
        }
    }
}
