using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DConfigOS_Core.Models;
using System.Web;
using Microsoft.AspNet.Identity;

namespace DConfigOS_Core.Providers.HttpContextProviders
{
    public class DConfigModel
    {
        public DConfigModel()
        {

        }

        public Content ActiveContent { get; set; }
        public ContentInstance ActiveContentInstance { get; set; }
        public Content RootContent { get; set; }
        public ContentInstance RootContentInstance { get; set; }
        public Dictionary<int, string> FieldsDictionaryOnId { get; set; }
        public Dictionary<string, string> FieldsDictionaryOnName { get; set; }
        public DConfigFormModel CurrentPageFormModel { get; set; }

        public string UserId
        {
            get
            {
                if (User != null)
                {
                    return User.Id;
                }
                else
                    return null;
            }
        }

        public string Path
        {
            get
            {
                if (ActiveContent != null)
                {
                    return ActiveContent.Path;
                }
                return null;
            }
        }

        public SABFramework.PreDefinedModules.MembershipModule.Models.ApplicationUser User
        {
            get
            {
                if (HttpContext.Current.Session["User"] == null && HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    var user = SABFramework.PreDefinedModules.MembershipModule.MembershipProvider.Instance.UsersAPI.GetUser(HttpContext.Current.User.Identity.Name);
                    HttpContext.Current.Session["User"] = user;
                }
                return (SABFramework.PreDefinedModules.MembershipModule.Models.ApplicationUser)HttpContext.Current.Session["User"];
            }
        }

        public bool UserIsAdministrator
        {
            get
            {
                if (HttpContext.Current.Session["UserIsAdministrator"] == null)
                {
                    var usermanager = SABFramework.PreDefinedModules.MembershipModule.MembershipProvider.Instance.UserManager;
                    HttpContext.Current.Session["UserIsAdministrator"] = usermanager.IsInRole(UserId, "Administrators");
                }
                return (bool)HttpContext.Current.Session["UserIsAdministrator"];
            }
        }

        public string this[int index]
        {
            get
            {
                if (this.ActiveContentInstance != null)
                {
                    if (FieldsDictionaryOnId == null)
                    {
                        this.FieldsDictionaryOnId = this.ActiveContentInstance.FieldsValues.ToList().ToDictionary(m => m.FieldId, m => m.Value);
                    }
                    if (FieldsDictionaryOnId.ContainsKey(index))
                    {
                        return FieldsDictionaryOnId[index];
                    }
                    else
                    {
                        return "";
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        public string this[string key]
        {
            get
            {
                if (this.ActiveContentInstance != null)
                {
                    if (FieldsDictionaryOnName == null)
                    {
                        this.FieldsDictionaryOnName = this.ActiveContentInstance.FieldsValues.ToList().ToDictionary(m => m.Field.Name, m => m.Value);
                    }
                    if (FieldsDictionaryOnName.ContainsKey(key))
                    {
                        return FieldsDictionaryOnName[key];
                    }
                    else
                    {
                        return "";
                    }
                }
                else
                {
                    return null;
                }

            }
        }

        public static DConfigModel BuildDConfigModel(Content content, int? FormId = null)
        {
            DConfigModel model = new DConfigModel();
            model.ActiveContent = content;
            model.ActiveContentInstance = content.ActiveContentInstance;
            Content rootContent = content;
            while (rootContent.Parent != null)
            {
                rootContent = rootContent.Parent;
            }
            model.RootContent = rootContent;
            model.RootContentInstance = rootContent.ActiveContentInstance;
            //Check if the current page has a form model and set it
            if (FormId.HasValue)
            {
                model.CurrentPageFormModel = Layer2.FormsServices.FormBaseAction.GetFormFromSession(FormId.Value);
            }
            //=====================================================
            return model;
        }

        //public static IEnumerable<Content> Search()
        //{

        //}
    }
}
