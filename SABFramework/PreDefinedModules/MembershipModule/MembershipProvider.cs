using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SABFramework.PreDefinedModules.MembershipModule.Models;
using System.Security.Claims;
using SABFramework.Core;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Web;
using Microsoft.Owin.Security;
using Microsoft.Owin;
using Owin;
using SABFramework.PreDefinedModules.MembershipModule.DataProviders;
using Ninject;

namespace SABFramework.PreDefinedModules.MembershipModule
{
    public class MembershipProvider
    {
        private IUsersAPI usersAPI;

        public IUsersAPI UsersAPI
        {
            get { return usersAPI; }
        }

        public MembershipProvider(IUsersAPI usersAPI) {
            this.usersAPI = usersAPI;
        }

        private static MembershipProvider _Instance;

        public static MembershipProvider Instance
        {
            get
            {
                if (_Instance == null && SABCoreEngine.Instance.DInjector!=null)
                {
                    _Instance = new MembershipProvider(SABCoreEngine.Instance.DInjector.Get<IUsersAPI>());
                }
                return _Instance;
            }
        }

        //Users session info============================
        private ApplicationUser _CurrentUser; // Used for API sessionless calls, its being set in ApplicationOAuthProvider
        public string CurrentUserId
        {
            get
            {
                if (CurrentUser != null)
                    return CurrentUser.Id;
                else
                    return null;
            }
        }
        public ApplicationUser CurrentUser
        {
            get
            {
                if (_CurrentUser != null)
                {
                    return _CurrentUser;
                }
                if (HttpContext.Current != null && HttpContext.Current.Session != null)
                {
                    if (HttpContext.Current.Session["User"] == null)
                    {
                        if (HttpContext.Current.User != null && HttpContext.Current.User.Identity != null && !String.IsNullOrEmpty(HttpContext.Current.User.Identity.Name))
                        {
                            var user = UsersAPI.GetUser(HttpContext.Current.User.Identity.Name);
                            HttpContext.Current.Session["User"] = user;
                        }
                        else
                        {
                            return null;
                        }
                    }
                    return (ApplicationUser)HttpContext.Current.Session["User"];
                }
                return null;
            }
            internal set
            {
                _CurrentUser = value;
            }
        }
        private bool? _CurrentUserIsAdministrator; // Used for API sessionless calls, its being set in ApplicationOAuthProvider
        public bool CurrentUserIsAdministrator
        {
            get
            {
                if (_CurrentUserIsAdministrator != null)
                {
                    return _CurrentUserIsAdministrator.Value;
                }
                if (HttpContext.Current == null || HttpContext.Current.Session == null || !HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    return false;
                }
                if (HttpContext.Current.Session["UserIsAdministrator"] == null)
                {
                    if (!String.IsNullOrEmpty(CurrentUserId))
                    {
                        HttpContext.Current.Session["UserIsAdministrator"] = UserManager.IsInRole(CurrentUserId, "Administrators");
                    }
                    else
                    {
                        return false;
                    }
                }
                return (bool)HttpContext.Current.Session["UserIsAdministrator"];
            }
            internal set
            {
                _CurrentUserIsAdministrator = value;
            }
        }
        public int? ContextCompanyId
        {
            get
            {
                if (HttpContext.Current == null || HttpContext.Current.Session == null)
                {
                    return null;
                }
                if (HttpContext.Current.Session["ContextCompanyId"] == null)
                {
                    if (CurrentUser != null)
                    {
                        var contextCompany = CurrentUser.Companies.FirstOrDefault();
                        if (contextCompany != null)
                        {
                            HttpContext.Current.Session["ContextCompanyId"] = contextCompany.Id;
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
                return (int?)HttpContext.Current.Session["ContextCompanyId"];
            }
            set
            {
                HttpContext.Current.Session["ContextCompanyId"] = value;
            }
        }
        //=====================================================

        public void SetUserManager(ApplicationUserManager userManager)
        {
            _userManager = userManager;
        }

        private ApplicationSignInManager _signInManager;

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.Current.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        private ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //public void SetApiUserManager(ApplicationUserManagerApi userManagerApi)
        //{
        //    _userManagerApi = userManagerApi;
        //}

        //private ApplicationUserManagerApi _userManagerApi;

        //public ApplicationUserManagerApi UserManagerApi
        //{
        //    get
        //    {
        //        return _userManagerApi ?? HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManagerApi>();
        //    }
        //    private set
        //    {
        //        _userManagerApi = value;
        //    }
        //}

        public async Task SignIn(HttpContextBase httpContext, ApplicationUser user, bool isPersistent)
        {
            httpContext.Session.Clear();
            httpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            httpContext.GetOwinContext().Authentication.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
        }

        public void SignOut(HttpContextBase httpContext)
        {
            httpContext.Session.Clear();
            httpContext.GetOwinContext().Authentication.SignOut();
        }

        public bool ChechUser(string userName, string password)
        {
            var result = SABFramework.PreDefinedModules.MembershipModule.MembershipProvider.Instance.UserManager.Find(userName, password);
            return result != null;
        }

        public bool IsInRole(string role)
        {
            return IsInRole(CurrentUserId, role);
        }

        public bool IsInRole(string userId, string role)
        {
            return UserManager.IsInRole(userId, role);
        }

        public async Task<Microsoft.AspNet.Identity.Owin.ExternalLoginInfo> GetExternalLoginInfo(HttpContextBase httpContext)
        {
            return await httpContext.GetOwinContext().Authentication.GetExternalLoginInfoAsync();
        }

        protected const string XsrfKey = "XsrfId";

        public class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                // this line did the trick
                context.RequestContext.HttpContext.Response.SuppressFormsAuthenticationRedirect = true;

                var properties = new AuthenticationProperties() { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
    }

}
