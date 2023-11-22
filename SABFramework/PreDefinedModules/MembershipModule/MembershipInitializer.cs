using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Threading;
using System.Web.Mvc;
using System.Net;
using SABFramework.Core;
using SABFramework.PreDefinedModules.MembershipModule.Models;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security.Facebook;
using Microsoft.Owin.Security.Twitter;
using Microsoft.Owin.Security.MicrosoftAccount;
using System.Security.Claims;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using SABFramework.PreDefinedModules.MembershipModule.DataProviders;
using System.Web;
using System.Net.Http;
using Newtonsoft.Json;
using Microsoft.Owin.Security.OAuth;
using Ninject;

[assembly: OwinStartup(typeof(SABFramework.PreDefinedModules.MembershipModule.MembershipStartup))]
namespace SABFramework.PreDefinedModules.MembershipModule
{
    public class MembershipInitializer : IInitializer
    {
        public void Initialize()
        {
            //Database.SetInitializer(new SABFramework.ModulesUtilities.CreateTablesOnlyIfTheyDontExist<MembershipDBContext>());

        }

    }

    public class MembershipStartup
    {
        public static string PublicClientId { get; private set; }
        public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }

        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            ConfigureExternalAuth(app);
        }

        public void ConfigureAuth(IAppBuilder app)
        {
            // Configure the db context and user manager to use a single instance per request
            app.CreatePerOwinContext(MembershipDBContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

            #region Cookie Init
            // Enable the application to use a cookie to store information for the signed in user
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                // LoginPath = new PathString((SABFramework.Core.SABCoreEngine.Instance.Settings.ContainsKey("LoginUrl") ? SABFramework.Core.SABCoreEngine.Instance.Settings["LoginUrl"] : "/DConfig/Account")),
                ReturnUrlParameter = "returnUrl"
            });
            #endregion

            #region Token Init
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            // Configure the application for OAuth based flow
            PublicClientId = "self";
            OAuthOptions = new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("/Token"),
                Provider = new ApplicationOAuthProvider(PublicClientId),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(14),
                // In production mode set AllowInsecureHttp = false
                AllowInsecureHttp = true,
            };
            // Enable the application to use bearer tokens to authenticate users
            app.UseOAuthBearerTokens(OAuthOptions);
            #endregion
        }

        public virtual void ConfigureExternalAuth(IAppBuilder app)
        {
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);
            if (SABCoreEngine.Instance.Settings.ContainsKey(WebsiteSetting.Security_MicrosoftClientId) && SABCoreEngine.Instance.Settings.ContainsKey(WebsiteSetting.Security_MicrosoftClientSecret) && !String.IsNullOrEmpty(SABCoreEngine.Instance.Settings[WebsiteSetting.Security_MicrosoftClientId]) && !String.IsNullOrEmpty(SABCoreEngine.Instance.Settings[WebsiteSetting.Security_MicrosoftClientSecret]))
            {
                var microsoftAuthenticationOptions = new MicrosoftAccountAuthenticationOptions
                {
                    ClientId = SABCoreEngine.Instance.Settings[WebsiteSetting.Security_MicrosoftClientId],
                    ClientSecret = SABCoreEngine.Instance.Settings[WebsiteSetting.Security_MicrosoftClientSecret]
                };
                app.UseMicrosoftAccountAuthentication(microsoftAuthenticationOptions);
            }

            if (SABCoreEngine.Instance.Settings.ContainsKey(WebsiteSetting.Security_TwitterClientId) && SABCoreEngine.Instance.Settings.ContainsKey(WebsiteSetting.Security_TwitterClientSecret) && !String.IsNullOrEmpty(SABCoreEngine.Instance.Settings[WebsiteSetting.Security_TwitterClientId]) && !String.IsNullOrEmpty(SABCoreEngine.Instance.Settings[WebsiteSetting.Security_TwitterClientSecret]))
                app.UseTwitterAuthentication(
                   consumerKey: SABFramework.Core.SABCoreEngine.Instance.Settings[WebsiteSetting.Security_TwitterClientId],
                   consumerSecret: SABFramework.Core.SABCoreEngine.Instance.Settings[WebsiteSetting.Security_TwitterClientSecret]);

            if (SABCoreEngine.Instance.Settings.ContainsKey(WebsiteSetting.Security_FacebookClientId) && SABCoreEngine.Instance.Settings.ContainsKey(WebsiteSetting.Security_FacebookClientSecret) && !String.IsNullOrEmpty(SABCoreEngine.Instance.Settings[WebsiteSetting.Security_FacebookClientId]) && !String.IsNullOrEmpty(SABCoreEngine.Instance.Settings[WebsiteSetting.Security_FacebookClientSecret]))
            {
                var facebookAuthenticationOptions = new FacebookAuthenticationOptions
                {
                    AppId = SABFramework.Core.SABCoreEngine.Instance.Settings[WebsiteSetting.Security_FacebookClientId],
                    AppSecret = SABFramework.Core.SABCoreEngine.Instance.Settings[WebsiteSetting.Security_FacebookClientSecret],
                    //BackchannelHttpHandler = new FacebookBackChannelHandler(),
                    //Fields = { "name", "email" },
                    Provider = new FacebookAuthenticationProvider() { }

                    //UserInformationEndpoint = "https://graph.facebook.com/v2.8/me?fields=id,name,email,first_name,last_name",
                    //Scope = { "email" },
                    //UserInformationEndpoint = "https://graph.facebook.com/v2.4/me?fields=id,name,email,first_name,last_name,location",
                    //Provider = new FacebookAuthenticationProvider()
                    //{
                    //    OnAuthenticated = (context) =>
                    //    {
                    //        context.Identity.AddClaim(new Claim("urn:facebook:accesstoken", context.AccessToken, ClaimValueTypes.String, "Facebook"));
                    //        foreach (var claim in context.User)
                    //        {
                    //            var claimType = string.Format("urn:facebook:{0}", claim.Key);
                    //            string claimValue = claim.Value.ToString();
                    //            if (!context.Identity.HasClaim(claimType, claimValue))
                    //                context.Identity.AddClaim(new System.Security.Claims.Claim(claimType, claimValue, "XmlSchemaString", "Facebook"));

                    //        }
                    //        return Task.FromResult(0);
                    //    }
                    //}
                };
                //facebookAuthenticationOptions.Scope.Add("public_profile");
                //add this for facebook to actually return the email and name
                facebookAuthenticationOptions.Fields.Add("email");
                facebookAuthenticationOptions.Fields.Add("name");
                app.UseFacebookAuthentication(facebookAuthenticationOptions);
            }

            if (SABCoreEngine.Instance.Settings.ContainsKey(WebsiteSetting.Security_GoogleClientId) && SABCoreEngine.Instance.Settings.ContainsKey(WebsiteSetting.Security_GoogleClientSecret) && !String.IsNullOrEmpty(SABCoreEngine.Instance.Settings[WebsiteSetting.Security_GoogleClientId]) && !String.IsNullOrEmpty(SABCoreEngine.Instance.Settings[WebsiteSetting.Security_GoogleClientSecret]))
            {
                var googleOAuth2AuthenticationOptions = new GoogleOAuth2AuthenticationOptions
                {
                    ClientId = SABFramework.Core.SABCoreEngine.Instance.Settings[WebsiteSetting.Security_GoogleClientId],
                    ClientSecret = SABFramework.Core.SABCoreEngine.Instance.Settings[WebsiteSetting.Security_GoogleClientSecret],
                    Provider = new GoogleOAuth2AuthenticationProvider()
                    {
                        //OnAuthenticated = async context =>
                        //{
                        //    context.Identity.AddClaim(new Claim(ClaimTypes.Name, context.User.GetValue("picture").ToString()));
                        //}
                    }
                };
                googleOAuth2AuthenticationOptions.Scope.Add("email");
                app.UseGoogleAuthentication(googleOAuth2AuthenticationOptions);
            }
        }
    }

    public class SABAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(System.Web.HttpContextBase httpContext)
        {
            var usermanager = httpContext.Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var user = httpContext.User;
            var dbUser = MembershipProvider.Instance.UsersAPI.GetUser(user.Identity.Name);
            var controller = httpContext.Request.RequestContext.RouteData.Values["controller"].ToString().ToLower();
            var action = httpContext.Request.RequestContext.RouteData.Values["action"].ToString().ToLower();
            var requestType = httpContext.Request.RequestType;
            var dataContext = new MembershipDBContext();
            var prv = dataContext.Privileges.Where(m => m.Controller == controller).ToList();
            List<IdentityRole> prvRoles;
            if (requestType == "OPTIONS")
            {
                return ExecuteCustomAuthHandlers(httpContext);
            }
            if (controller == "Membership" && action == "authorize"){
                if (dbUser != null)
                    return true;
                else
                    return false;
            }
            if (prv != null || prv.Count > 0)
            {
                var prvL3 = prv.Where(m => (m.Action != null && m.Action.ToLower() == action) && m.RequestType == requestType).SingleOrDefault();
                if (prvL3 != null)
                {
                    if (prvL3.NeedAuthentication)
                    {
                        if (dbUser != null)
                        {
                            if (prvL3.NeedAuthorization)
                            {
                                prvRoles = prvL3.Roles.ToList();
                                foreach (IdentityRole r in prvRoles)
                                {
                                    if (usermanager.IsInRole(dbUser.Id, r.Name))
                                    {
                                        return ExecuteCustomAuthHandlers(httpContext);
                                    }
                                }
                                return false;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    var prvL2 = prv.Where(m => (m.Action != null && m.Action.ToLower() == action) && m.RequestType == null).SingleOrDefault();
                    if (prvL2 != null)
                    {
                        if (prvL2.NeedAuthentication)
                        {
                            if (dbUser != null)
                            {
                                if (prvL2.NeedAuthorization)
                                {
                                    prvRoles = prvL2.Roles.ToList();
                                    foreach (IdentityRole r in prvRoles)
                                    {
                                        if (usermanager.IsInRole(dbUser.Id, r.Name))
                                        {
                                            return ExecuteCustomAuthHandlers(httpContext);
                                        }
                                    }
                                    return false;
                                }
                            }
                            else
                            {
                                return false;
                            }
                        }

                    }
                    else
                    {
                        var prvL1 = prv.Where(m => m.Action == null).SingleOrDefault();
                        if (prvL1 != null)
                        {
                            if (prvL1.NeedAuthentication)
                            {
                                if (dbUser != null)
                                {
                                    if (prvL1.NeedAuthorization)
                                    {
                                        prvRoles = prvL1.Roles.ToList();
                                        foreach (IdentityRole r in prvRoles)
                                        {

                                            if (usermanager.IsInRole(dbUser.Id, r.Name))
                                            {
                                                return ExecuteCustomAuthHandlers(httpContext);
                                            }
                                        }
                                        return false;
                                    }
                                }
                                else
                                {
                                    return false;
                                }
                            }
                        }
                    }
                }
            }
            return ExecuteCustomAuthHandlers(httpContext);
        }
        protected bool ExecuteCustomAuthHandlers(System.Web.HttpContextBase httpContext)
        {
            var result = true;
            var eventsList = AuthEventsRegistrar.Instance.Events;
            if (AuthEventsRegistrar.Instance.Events.Count > 0)
            {
                foreach (AuthEventsRegistrar.AuthEvent e in eventsList)
                {
                    result = e(httpContext);
                }
            }
            return result;
        }
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            var httpContext = filterContext.HttpContext;
            var request = httpContext.Request;
            var response = httpContext.Response;
            var user = httpContext.User;

            if (request.Headers["Ajax"] == "true")
            {
                response.Clear();
                if (user.Identity.IsAuthenticated == false)
                    response.StatusCode = (int)HttpStatusCode.Unauthorized;
                else
                    response.StatusCode = (int)HttpStatusCode.Forbidden;

                filterContext.Result = new EmptyResult();
                //response.SuppressFormsAuthenticationRedirect = true;
                response.End();
            }
            else if (!String.IsNullOrEmpty(SABFramework.Core.SABCoreEngine.Instance.Settings["LoginUrl"]))
            {
                if (HttpContext.Current.Request.Url.PathAndQuery.ToLower().Contains("dconfig"))
                {
                    filterContext.Result = new RedirectResult("/DConfig/Account" + "?returnUrl=" + HttpUtility.UrlEncode(HttpContext.Current.Request.Url.PathAndQuery));
                }
                else
                {
                    filterContext.Result = new RedirectResult(SABFramework.Core.SABCoreEngine.Instance.Settings["LoginUrl"] + "?returnUrl=" + HttpUtility.UrlEncode(HttpContext.Current.Request.Url.PathAndQuery));
                }
            }
            else
                base.HandleUnauthorizedRequest(filterContext);
        }
    }

    public class SABRequireHttpsAttribute : RequireHttpsAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            var controller = filterContext.HttpContext.Request.RequestContext.RouteData.Values["controller"].ToString();
            var action = filterContext.HttpContext.Request.RequestContext.RouteData.Values["action"].ToString();
            var requestType = filterContext.HttpContext.Request.RequestType;
            var dataContext = new MembershipDBContext();
            var prv = dataContext.Privileges.Where(m => m.Controller == controller).ToList();
            if (prv != null || prv.Count > 0)
            {
                var prvL3 = prv.Where(m => m.Action == action && m.RequestType == requestType).SingleOrDefault();
                if (prvL3 != null)
                {
                    if (prvL3.RequireHttps)
                    {
                        base.OnAuthorization(filterContext);
                    }
                }
                else
                {
                    var prvL2 = prv.Where(m => m.Action == action && m.RequestType == null).SingleOrDefault();
                    if (prvL2 != null)
                    {
                        if (prvL2.RequireHttps)
                        {
                            base.OnAuthorization(filterContext);
                        }
                    }
                    else
                    {
                        var prvL1 = prv.Where(m => m.Action == null).SingleOrDefault();
                        if (prvL1 != null)
                        {
                            if (prvL1.RequireHttps)
                            {
                                base.OnAuthorization(filterContext);
                            }
                        }
                    }
                }
            }
            //if (ExecuteCustomCheckingSecureConnectionHandlers(filterContext.HttpContext))
            //{
            //    base.OnAuthorization(filterContext);
            //}
        }

        protected bool ExecuteCustomCheckingSecureConnectionHandlers(System.Web.HttpContextBase httpContext)
        {
            var result = true;
            var eventsList = CheckingSecureConnectionEventsRegistrar.Instance.Events;
            if (CheckingSecureConnectionEventsRegistrar.Instance.Events.Count > 0)
            {
                foreach (CheckingSecureConnectionEventsRegistrar.CheckingSecureConnectionEvent e in eventsList)
                {
                    result = e(httpContext);
                }
            }
            return result;
        }

        protected override void HandleNonHttpsRequest(AuthorizationContext filterContext)
        {
            // The base only redirects GET, but we added HEAD as well. This avoids exceptions for bots crawling using HEAD.
            // The other requests will throw an exception to ensure the correct verbs are used. 
            // We fall back to the base method as the mvc exceptions are marked as internal. 
            if (!String.Equals(filterContext.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase)
                && !String.Equals(filterContext.HttpContext.Request.HttpMethod, "HEAD", StringComparison.OrdinalIgnoreCase))
            {
                base.HandleNonHttpsRequest(filterContext);
            }
            // Redirect to HTTPS version of page
            // We updated this to redirect using 301 (permanent) instead of 302 (temporary).
            string url = "https://" + filterContext.HttpContext.Request.Url.Host + filterContext.HttpContext.Request.RawUrl;
            if (string.Equals(filterContext.HttpContext.Request.Url.Host, "localhost", StringComparison.OrdinalIgnoreCase))
            {
                // For localhost requests, default to IISExpress https default port (44300)
                url = "https://" + filterContext.HttpContext.Request.Url.Host + ":44300" + filterContext.HttpContext.Request.RawUrl;
            }
            filterContext.Result = new RedirectResult(url, true);
        }
    }

    public class SABAllowCROSAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Request.HttpMethod == "OPTIONS")
            {
                // do nothing let IIS deal with reply!
                filterContext.Result = new EmptyResult();
            }
            else
            {
                base.OnActionExecuting(filterContext);
            }
        }
    }

    public class FacebookBackChannelHandler : HttpClientHandler
    {
        protected override async System.Threading.Tasks.Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            if (!request.RequestUri.AbsolutePath.Contains("/oauth"))
            {
                request.RequestUri = new Uri(request.RequestUri.AbsoluteUri.Replace("?access_token", "&access_token"));
            }

            var result = await base.SendAsync(request, cancellationToken);
            if (!request.RequestUri.AbsolutePath.Contains("/oauth"))
            {
                return result;
            }

            var content = await result.Content.ReadAsStringAsync();
            var facebookOauthResponse = JsonConvert.DeserializeObject<FacebookOauthResponse>(content);

            var outgoingQueryString = HttpUtility.ParseQueryString(string.Empty);
            outgoingQueryString.Add("access_token", facebookOauthResponse.access_token);
            outgoingQueryString.Add("expires_in", facebookOauthResponse.expires_in + string.Empty);
            outgoingQueryString.Add("token_type", facebookOauthResponse.token_type);
            var postdata = outgoingQueryString.ToString();

            var modifiedResult = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(postdata)
            };

            return modifiedResult;
        }
    }

    public class FacebookOauthResponse
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
    }
}
