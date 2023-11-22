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
    public class UDUserFieldEnumValuesAction : SABFramework.Core.SABAction
    {
        [Required]
        public int Id { get; set; }
        public List<UserFieldEnumValue> Values { get; set; }

        [Inject]
        public IUserFieldsEnumsAPI UserFieldEnumsAPI { get; set; }

        public override async Task<SABFramework.Core.SABActionResult> PutHandler(System.Web.Mvc.Controller controller)
        {
            var result = UserFieldEnumsAPI.UpdateUserFieldEnumValues(Id, Values.Select(m => m.Value).ToList());
            var e = UserFieldEnumsAPI.GetUserFieldEnum(Id);
            if (result==ResultCodes.Succeed)
            {
                return Json(new { result = "true", obj = e });
            }
            else
            {
                return Json(new { result = "false", message = "Predefined List hasn't been found to be updated" });
            }
        }

        public override async Task<SABActionResult> DeleteHandler(Controller controller)
        {
            var result = UserFieldEnumsAPI.DeleteUserFieldEnumValue(Id);
            if (result==ResultCodes.Succeed)
            {
                return Json(new { result = "true" });
            }
            else
            {
                return Json(new { result = "false", message = "Value hasn't been found to be deleted" });
            }
        }
    }
}
