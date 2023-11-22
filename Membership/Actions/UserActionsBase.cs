﻿using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DConfigOS_Core.Layer2.ActionsModels;
using SABFramework.Core;
using Microsoft.AspNet.Identity;
using System.Web.Mvc;
using DConfigOS_Core.Repositories.WebsiteContentServices;
using DConfigOS_Core.Models;
using SABFramework.PreDefinedModules.MembershipModule.Models;
using Ninject;

namespace Membership.Actions
{
    [Bind(Exclude = "UserId,UserIsAdministrator,User")]
    public class UserActionsBase : SABFramework.Core.SABAction
    {
        [Inject]
        public IStagesAPI stagesAPI { get; set; }

        public string UserId
        {
            get
            {
                return SABFramework.PreDefinedModules.MembershipModule.MembershipProvider.Instance.CurrentUserId;
            }
        }

        protected bool UserBasedApps
        {
            get
            {
                if (HttpContext.Current.Session["UserBasedApps"] == null)
                {
                    HttpContext.Current.Session["UserBasedApps"] = SABFramework.Core.SABCoreEngine.Instance.Settings["UserBasedApps"] == "True" && !UserIsAdministrator;
                }
                return (bool) HttpContext.Current.Session["UserBasedApps"];
            }
        }

        public bool UserIsAdministrator
        {
            get
            {
                return SABFramework.PreDefinedModules.MembershipModule.MembershipProvider.Instance.CurrentUserIsAdministrator;
            }
        }

        public ApplicationUser User
        {
            get
            {
                return SABFramework.PreDefinedModules.MembershipModule.MembershipProvider.Instance.CurrentUser;
            }
        }

        public ExApplicationUser ExUser
        {
            get
            {
                if (HttpContext.Current.Session["ExUser"] == null)
                {
                    var context = new DConfigOS_Core_DBContext();
                    var exUser = context.ExApplicationsUsers.Where(m => m.UserId == User.Id).FirstOrDefault();
                    HttpContext.Current.Session["ExUser"] = exUser;
                }
                return (ExApplicationUser) HttpContext.Current.Session["ExUser"];
            }
        }

        public List<Stage> UserStages
        {
            get
            {
                var roles = SABFramework.PreDefinedModules.MembershipModule.MembershipProvider.Instance.UsersAPI.GetUserRoles(UserId);
                return stagesAPI.GetStagesbyRole(roles.Select(m => m.RoleId).ToList());
            }
        }

        public override async Task<SABActionResult> GetHandler(System.Web.Mvc.Controller controller)
        {
            return null;
        }

        public override async Task<SABActionResult> PostHandler(System.Web.Mvc.Controller controller)
        {
            return null;
        }

        public override async Task<SABActionResult> PutHandler(System.Web.Mvc.Controller controller)
        {
            return null;
        }


        public override async Task<SABActionResult> DeleteHandler(System.Web.Mvc.Controller controller)
        {
            return null;
        }
    }
}
