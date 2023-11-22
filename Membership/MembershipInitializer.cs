using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SABFramework.Core;
using Membership.Models;
using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using Owin;
using DConfigOS_Core.Providers.HttpContextProviders;
using SABFramework.PreDefinedModules.MembershipModule.DataProviders;
using SABFramework.PreDefinedModules.MembershipModule.Models;
using Microsoft.Owin.Security.MicrosoftAccount;
using Microsoft.Owin.Security.Facebook;
using SABFramework.PreDefinedModules.MembershipModule;
using Microsoft.Owin.Security.Google;
using Ninject;

namespace Membership
{
    public class MembershipInitializer : IInitializer
    {
        public void Initialize()
        {
            //Membership_DBContext DBContext = new Membership_DBContext();
            //DBContext.Database.CreateIfNotExists();
            //Database.SetInitializer(new SABFramework.ModulesUtilities.CreateTablesOnlyIfTheyDontExist<Membership_DBContext>());
            SABFramework.PreDefinedModules.MembershipModule.AuthEventsRegistrar.Instance.RegisterEvent((httpContext) =>
            {
                string path = (httpContext.Request.RequestContext.RouteData.Values["path"] != null ? httpContext.Request.RequestContext.RouteData.Values["path"].ToString() : null);
                Membership_DBContext context = new Membership_DBContext();
                if (!String.IsNullOrEmpty(path))
                {
                    var content = context.Contents.Where(m => m.UrlFullCode == "/" + path).FirstOrDefault();
                    var user = httpContext.User;
                    var dbUser = MembershipProvider.Instance.UsersAPI.GetUser(user.Identity.Name);
                    if (content != null)
                    {
                        var prv = context.ContentPrivileges.Where(m => m.ContentId == content.Id).SingleOrDefault();
                        if (prv != null)
                        {
                            if (prv.NeedAuthentication)
                            {
                                if (dbUser != null)
                                {
                                    if (prv.NeedAuthorization)
                                    {
                                        var prvRoles = prv.Roles.ToList();
                                        foreach (IdentityRole r in prvRoles)
                                        {
                                            if (httpContext.User.IsInRole(r.Name))
                                            {
                                                return true;
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
                return true;
            });
            SABFramework.PreDefinedModules.MembershipModule.CheckingSecureConnectionEventsRegistrar.Instance.RegisterEvent((httpContext) =>
            {
                string path = (httpContext.Request.RequestContext.RouteData.Values["path"] != null ? httpContext.Request.RequestContext.RouteData.Values["path"].ToString() : null);
                Membership_DBContext context = new Membership_DBContext();
                if (!String.IsNullOrEmpty(path))
                {
                    var content = context.Contents.Where(m => m.UrlFullCode == "/" + path).FirstOrDefault();
                    if (content != null)
                    {
                        var prv = context.ContentPrivileges.Where(m => m.ContentId == content.Id).SingleOrDefault();
                        if (prv != null)
                        {
                            if (prv.RequireHttps)
                            {
                                return true;
                            }
                        }
                    }
                }
                return false;
            });
        }
    }
    public class MembershipStartup : SABFramework.PreDefinedModules.MembershipModule.MembershipStartup
    {
        private IWebsiteSettingsAPI websiteSettingsAPI;
        public MembershipStartup()
        {
            websiteSettingsAPI = SABFramework.Core.SABCoreEngine.Instance.DInjector.Get<IWebsiteSettingsAPI>();
        }

        public override void ConfigureExternalAuth(IAppBuilder app)
        {
            app.MapWhen(ctx => DConfigRequestContext.Domains.ContainsKey(ctx.Request.Headers.Get("Host")), app2 =>
            {
                if (DConfigRequestContext.Current != null)
                {
                    var contextId = DConfigRequestContext.Current.ContextId;
                    var Security_MicrosoftClientId = websiteSettingsAPI.Get(WebsiteSetting.Security_MicrosoftClientId, contextId);
                    var Security_MicrosoftClientSecret = websiteSettingsAPI.Get(WebsiteSetting.Security_MicrosoftClientSecret, contextId);
                    var Security_TwitterClientId = websiteSettingsAPI.Get(WebsiteSetting.Security_TwitterClientId, contextId);
                    var Security_TwitterClientSecret = websiteSettingsAPI.Get(WebsiteSetting.Security_TwitterClientSecret, contextId);
                    var Security_FacebookClientId = websiteSettingsAPI.Get(WebsiteSetting.Security_FacebookClientId, contextId);
                    var Security_FacebookClientSecret = websiteSettingsAPI.Get(WebsiteSetting.Security_FacebookClientSecret, contextId);
                    var Security_GoogleClientId = websiteSettingsAPI.Get(WebsiteSetting.Security_GoogleClientId, contextId);
                    var Security_GoogleClientSecret = websiteSettingsAPI.Get(WebsiteSetting.Security_GoogleClientSecret, contextId);
                    if (!String.IsNullOrEmpty(Security_MicrosoftClientId) && !String.IsNullOrEmpty(Security_MicrosoftClientSecret))
                    {
                        var microsoftAuthenticationOptions = new MicrosoftAccountAuthenticationOptions
                        {
                            ClientId = Security_MicrosoftClientId,
                            ClientSecret = Security_MicrosoftClientSecret
                        };
                        app.UseMicrosoftAccountAuthentication(microsoftAuthenticationOptions);
                    }
                    if (!String.IsNullOrEmpty(Security_TwitterClientId) && !String.IsNullOrEmpty(Security_TwitterClientSecret))
                        app.UseTwitterAuthentication(
                           consumerKey: Security_TwitterClientId,
                           consumerSecret: Security_TwitterClientSecret);

                    if (!String.IsNullOrEmpty(Security_FacebookClientId) && !String.IsNullOrEmpty(Security_FacebookClientSecret))
                    {
                        var facebookAuthenticationOptions = new FacebookAuthenticationOptions
                        {
                            AppId = Security_FacebookClientId,
                            AppSecret = Security_FacebookClientSecret,
                            BackchannelHttpHandler = new FacebookBackChannelHandler(),
                            UserInformationEndpoint = "https://graph.facebook.com/v2.4/me?fields=id,name,email,first_name,last_name"
                        };
                        facebookAuthenticationOptions.Scope.Add("public_profile");
                        //add this for facebook to actually return the email and name
                        facebookAuthenticationOptions.Fields.Add("email");
                        facebookAuthenticationOptions.Fields.Add("name");
                        app.UseFacebookAuthentication(facebookAuthenticationOptions);
                    }

                    if (!String.IsNullOrEmpty(Security_GoogleClientId) && !String.IsNullOrEmpty(Security_GoogleClientSecret))
                    {
                        var googleOAuth2AuthenticationOptions = new GoogleOAuth2AuthenticationOptions
                        {
                            ClientId = Security_GoogleClientId,
                            ClientSecret = Security_GoogleClientSecret,
                            Provider = new GoogleOAuth2AuthenticationProvider() { }
                        };
                        googleOAuth2AuthenticationOptions.Scope.Add("email");
                        app.UseGoogleAuthentication(googleOAuth2AuthenticationOptions);
                    }
                }
            });
        }
    }
}
