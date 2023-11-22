using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DConfigOS_Core.Providers.HttpContextProviders;
using System.Web.Mvc;
using System.Web;
using DConfigOS_Core.Models;
using DConfigOS_Core.Repositories.WebsiteContentServices;
using DConfigOS_Core.Repositories.Utilities;
using System.Net.Mail;
using Newtonsoft.Json;
using System.Net;
using SABFramework.PreDefinedModules.MembershipModule.DataProviders;
using SABFramework.PreDefinedModules.MembershipModule.Models;
using SABFramework.Providers;
using Ninject;
using Microsoft.AspNet.Identity;
using static SABFramework.Providers.EmailProvider;
using DConfigOS_Core.Repositories.IOServices;
using DConfigOS_Core.Responses;
using BitArmory.ReCaptcha;

namespace DConfigOS_Core.Layer2.FormsServices
{
    public abstract class FormBaseAction : SABFramework.Core.SABAction
    {
        [Inject]
        [JsonIgnore]
        public IEmailProvider emailProvider { protected get; set; }
        [Inject]
        [JsonIgnore]
        public IFormsAPI formsAPI { get; set; }
        [Inject]
        [JsonIgnore]
        public IFoldersAPI foldersAPI { get; set; }
        [Inject]
        [JsonIgnore]
        public IFormsInstancesAPI formsInstancesAPI { get; set; }
        [Inject]
        [JsonIgnore]
        public IUsersAPI usersAPI { get; set; }

        public int Id { get; set; }
        public string PageUrl { get; set; }
        [JsonIgnore]
        public string captcha { get; set; }

        public DConfigFormModel CurrentFormModel { get; set; }

        public static DConfigFormModel GetFormFromSession(int id)
        {
            return (DConfigFormModel)HttpContext.Current.Session["Form" + id];
        }

        protected static void SaveFormToSession(DConfigFormModel model)
        {
            HttpContext.Current.Session["Form" + model.PageForm.Id] = model;
        }

        protected void ClearFormFromSession(DConfigFormModel model)
        {
            HttpContext.Current.Session["Form" + model.PageForm.Id] = null;
        }

        protected DConfigFormModel BuildCurrentFormModel()
        {
            var model = GetFormFromSession(Id);
            if (model == null)
            {
                Form form = formsAPI.GetForm(Id, DConfigRequestContext.Current.ContextId);
                model = new DConfigFormModel();
                model.PageForm = form;
                model.PageUrl = PageUrl;
                model.UniqueFormName = form.Name;
                switch (model.PageForm.Type)
                {
                    case "Single Form":
                    default:
                        break;
                    case "Multiple Sections Form":
                        if (form.ChildrenForms.Count > 0)
                        {
                            model.ChildrenModels = new List<DConfigFormModel>();
                            foreach (Form f in form.ChildrenForms)
                            {
                                model.ChildrenModels.Add(new DConfigFormModel()
                                {
                                    PageForm = f,
                                    PageUrl = PageUrl,
                                    UniqueFormName = f.Name
                                });
                            }
                        }
                        break;
                    case "Multiple Same Child":
                        if (form.ChildrenForms.Count > 0)
                        {
                            model.ChildrenModels = new List<DConfigFormModel>();
                            foreach (Form f in form.ChildrenForms)
                            {
                                model.ChildrenModels.Add(new DConfigFormModel()
                                {
                                    PageForm = f,
                                    PageUrl = PageUrl,
                                    UniqueFormName = f.Name
                                });
                            }
                        }
                        break;
                    case "Multiple Steps Form":
                        break;
                }
            }
            return model;
        }

        protected bool InitializeFormStateAndValidateIt(Controller controller, DConfigFormModel model)
        {
            bool formValid = true;
            model.PageFormFieldsValues = new Dictionary<int, string>();
            model.ModelState.Clear();
            if (model.PageForm.Type == "Multiple Same Child")
            {
                BuildMultipleSameChildFormsFromState(controller, model);
            }
            MapFormsDataAndCheckValidations(controller, model, !String.IsNullOrEmpty(model.PageForm.CustomSubmitPath));
            model.IsValid = formValid;
            SaveFormToSession(model);
            return formValid;
        }

        protected void BuildMultipleSameChildFormsFromState(Controller controller, DConfigFormModel model)
        {
            var indexes = controller.Request.Form["DConfigChildrenIndexes"];
            int[] indexesObj = JsonConvert.DeserializeObject<int[]>(indexes);

            if (model.ChildrenModels != null && model.ChildrenModels.Count > 0 && indexesObj != null && indexesObj.Length > 0)
            {
                var mainModel = model.ChildrenModels[0];
                string mainName = mainModel.PageForm.Name;
                for (int i = 0; i < indexesObj.Length; i++)
                {
                    if (i != 0)
                    {
                        model.ChildrenModels.Add(new DConfigFormModel() { PageForm = mainModel.PageForm, PageUrl = mainModel.PageUrl });
                    }
                    model.ChildrenModels[i].UniqueFormName = mainName + indexesObj[i];
                }
            }

        }

        protected void SetFormValidaty(DConfigFormModel model, bool validity)
        {
            model.IsValid = validity;
            SaveFormToSession(model);
        }

        protected async Task<bool> MapFormsDataAndCheckValidations(Controller controller, DConfigFormModel model, bool builtIn)
        {
            bool formValid = false;
            model.ModelFieldsValues = new Dictionary<int, string>();//The variable used to saved in the database for form instance
            foreach (var field in model.PageForm.FormFields)
            {
                foreach (string item in (field.Type == "File" ? controller.Request.Files.Keys : controller.Request.Form.Keys))
                {
                    if (item == model.UniqueFormName + "_" + field.Name || item == field.Name)
                    {
                        if (field.Required && String.IsNullOrEmpty(controller.Request.Form[item]))
                        {
                            formValid = false;
                            if (!builtIn)
                            {
                                model.ModelState.AddModelError(model.PageForm.Name + "_" + field.Name, field.Name + " is required");
                            }
                            else
                            {
                                model.ModelState.AddModelError(field.Name, field.Name + " is required");
                            }
                        }
                        else
                        {
                            if (field.Type == "File")
                            {
                                string filePath = await CheckAndSavePostedFile(controller, field, model.UniqueFormName);
                                if (!String.IsNullOrEmpty(filePath))
                                {
                                    model.PageFormFieldsValues.Add(field.Id, filePath);
                                    model.ModelFieldsValues.Add(field.Id, filePath);
                                }
                                else
                                {
                                    if (!builtIn)
                                    {
                                        model.ModelState.AddModelError(model.PageForm.Name + "_" + field.Name, field.Name + " file is corrupted");
                                    }
                                    else
                                    {
                                        model.ModelState.AddModelError(field.Name, field.Name + " file is corrupted");
                                    }
                                }
                            }
                            else
                            {
                                if (field.Type != "Password")
                                {
                                    model.PageFormFieldsValues.Add(field.Id, controller.Request.Form[item]);
                                    model.ModelFieldsValues.Add(field.Id, controller.Request.Form[item]);
                                }
                            }
                        }
                        break;
                    }
                }
            }
            if (model.PageForm.Type == "Multiple Sections Form" || model.PageForm.Type == "Multiple Same Child")
            {
                foreach (DConfigFormModel childModel in model.ChildrenModels)
                {
                    childModel.ModelFieldsValues = new Dictionary<int, string>();
                    foreach (var field in childModel.PageForm.FormFields)
                    {
                        foreach (string item in controller.Request.Form.Keys)
                        {
                            if (item == childModel.UniqueFormName + "_" + field.Name || item == field.Name)
                            {
                                if (field.Required && String.IsNullOrEmpty(controller.Request.Form[item]))
                                {
                                    formValid = false;
                                    if (!builtIn)
                                    {
                                        model.ModelState.AddModelError(model.PageForm.Name + "_" + field.Name, field.Name + " is required");
                                    }
                                    else
                                    {
                                        model.ModelState.AddModelError(field.Name, field.Name + " is required");
                                    }
                                }
                                else
                                {
                                    if (field.Type == "File")
                                    {
                                        string filePath = await CheckAndSavePostedFile(controller, field, model.UniqueFormName);
                                        if (!String.IsNullOrEmpty(filePath))
                                        {
                                            if (model.PageForm.Type != "Multiple Same Child")
                                                model.PageFormFieldsValues.Add(field.Id, controller.Request.Form[item]);
                                            childModel.ModelFieldsValues.Add(field.Id, controller.Request.Form[item]);
                                        }
                                        else
                                        {
                                            if (!builtIn)
                                            {
                                                model.ModelState.AddModelError(model.PageForm.Name + "_" + field.Name, field.Name + " file is corrupted");
                                            }
                                            else
                                            {
                                                model.ModelState.AddModelError(field.Name, field.Name + " file is corrupted");
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (field.Type != "Password")
                                        {
                                            if (model.PageForm.Type != "Multiple Same Child")
                                                model.PageFormFieldsValues.Add(field.Id, controller.Request.Form[item]);
                                            childModel.ModelFieldsValues.Add(field.Id, controller.Request.Form[item]);
                                        }
                                    }
                                }
                                break;
                            }
                        }
                    }
                }
            }
            return formValid;
        }

        protected async Task<string> CheckAndSavePostedFile(Controller controller, FormsField field, string uniqueFormName)
        {
            if (controller.HttpContext.Request.Files.Count > 0)
            {
                string[] filesKeys = controller.HttpContext.Request.Files.AllKeys;
                foreach (string s in filesKeys)
                {
                    if (s == field.Name || s == uniqueFormName + "_" + field.Name)
                    {
                        HttpPostedFileBase file = controller.HttpContext.Request.Files[s];
                        if (file != null && file.ContentLength > 0)
                        {
                            string savingPath;
                            var result = await foldersAPI.CreateFile("/globalresources/" + DConfigRequestContext.Current.ContextId + "/Resources/FormsFiles/", file, out savingPath);
                            if (result == ResultCodes.Succeed)
                            {
                                return savingPath;
                            }
                        }
                    }
                }
            }
            return null;
        }

        protected async Task ProcessFormSubmitEvents(Controller controller, DConfigFormModel model)
        {
            if (model.PageForm.FromSubmitEvents != null && model.PageForm.FromSubmitEvents.Count >= 0)
            {
                foreach (FormSubmitEvent e in model.PageForm.FromSubmitEvents)
                {
                    switch (e.Type)
                    {
                        case FormSubmitEventType.PredefinedTypes.EmailType:
                            DConfigEmailModel emailModel = new DConfigEmailModel();
                            emailModel.FieldsDictionaryOnName = new Dictionary<string, string>();
                            emailModel.FieldsDictionaryOnId = new Dictionary<int, string>();
                            foreach (var fv in model.PageForm.FormFields)
                            {
                                if (model.ModelFieldsValues.ContainsKey(fv.Id))
                                {
                                    emailModel.FieldsDictionaryOnName.Add(fv.Name, model.ModelFieldsValues[fv.Id]);
                                    emailModel.FieldsDictionaryOnId.Add(fv.Id, model.ModelFieldsValues[fv.Id]);
                                }
                            }
                            emailModel.SubModels = new List<DConfigEmailModel>();
                            if (model.ChildrenModels != null && model.ChildrenModels.Count > 0)
                            {
                                foreach (DConfigFormModel subModel in model.ChildrenModels)
                                {
                                    var subEmailModel = new DConfigEmailModel();
                                    subEmailModel.FieldsDictionaryOnName = new Dictionary<string, string>();
                                    subEmailModel.FieldsDictionaryOnId = new Dictionary<int, string>();
                                    foreach (var fv in subModel.PageForm.FormFields)
                                    {
                                        subEmailModel.FieldsDictionaryOnName.Add(fv.Name, subModel.ModelFieldsValues[fv.Id]);
                                        subEmailModel.FieldsDictionaryOnId = subModel.ModelFieldsValues;
                                    }
                                    emailModel.SubModels.Add(subEmailModel);
                                }
                            }
                            FormSubmitEmailEvent email_e = (FormSubmitEmailEvent)e;
                            SABIdentityMessage message = new SABIdentityMessage();
                            message.From = email_e.From;
                            if (email_e.BccBindedFieldId.HasValue)
                            {
                                if (!String.IsNullOrEmpty(emailModel.FieldsDictionaryOnId[email_e.BccBindedFieldId.Value]))
                                {
                                    message.BCCs.Add(emailModel.FieldsDictionaryOnId[email_e.BccBindedFieldId.Value]);
                                }
                            }
                            else
                            {
                                if (!String.IsNullOrEmpty(email_e.Bcc))
                                    message.BCCs.Add(email_e.Bcc);
                            }
                            if (email_e.CcBindedFieldId.HasValue)
                            {
                                if (!String.IsNullOrEmpty(emailModel.FieldsDictionaryOnId[email_e.CcBindedFieldId.Value]))
                                {
                                    message.CCs.Add(emailModel.FieldsDictionaryOnId[email_e.CcBindedFieldId.Value]);
                                }
                            }
                            else
                            {
                                if (!String.IsNullOrEmpty(email_e.Cc))
                                    message.CCs.Add(email_e.Cc);
                            }
                            //if (email_e.FromBindedFieldId.HasValue)
                            //{
                            //    if (!String.IsNullOrEmpty(emailModel.FieldsDictionaryOnId[email_e.FromBindedFieldId.Value]))
                            //    {
                            //        message. = new MailAddress(emailModel.FieldsDictionaryOnId[email_e.FromBindedFieldId.Value]);
                            //    }
                            //}
                            //else
                            //{
                            //    if (!String.IsNullOrEmpty(email_e.From))
                            //        m.From = new MailAddress(email_e.From);
                            //}
                            if (email_e.ToBindedFieldId.HasValue)
                            {
                                if (!String.IsNullOrEmpty(emailModel.FieldsDictionaryOnId[email_e.ToBindedFieldId.Value]))
                                {
                                    message.Destination = (emailModel.FieldsDictionaryOnId[email_e.ToBindedFieldId.Value]);
                                }
                            }
                            else
                            {
                                if (!String.IsNullOrEmpty(email_e.To))
                                    message.Destination = (email_e.To);
                            }
                            message.Subject = email_e.Subject;
                            //var viewName= email_e.Template.Path.Substring(email_e.Template.Path.LastIndexOf('/') + 1 , email_e.Template.Path.Length - email_e.Template.Path.LastIndexOf('/') - 1);
                            message.View = email_e.Template.Path;
                            message.Model = emailModel;
                            await emailProvider.SendAsync(message, DConfigRequestContext.Current.ContextId.Value);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        protected void CreateFormInstancesForFormModels(Controller controller, DConfigFormModel model)
        {
            string userId = null;
            if (controller.User.Identity.IsAuthenticated)
            {
                userId = usersAPI.GetUser(controller.User.Identity.Name).Id;
            }
            FormInstance fi = new FormInstance();
            if (!String.IsNullOrEmpty(userId))
            {
                fi.UserId = userId;
            }
            fi.FormId = Id;
            formsInstancesAPI.CreateFormInstance(fi, model.ModelFieldsValues);
            if (model.PageForm.Type == "Multiple Sections Form" || model.PageForm.Type == "Multiple Same Child")
            {
                foreach (DConfigFormModel childModel in model.ChildrenModels)
                {
                    FormInstance childFi = new FormInstance();
                    if (!String.IsNullOrEmpty(userId))
                    {
                        childFi.UserId = userId;
                    }
                    childFi.FormId = childModel.PageForm.Id;
                    childFi.ParentInstanceId = fi.Id;
                    formsInstancesAPI.CreateFormInstance(childFi, childModel.ModelFieldsValues);
                }
            }
        }

        protected async Task<bool> ValidateRecapatcha(Controller controller, string formName)
        {
            //1. Get the client IP address in your chosen web framework
            string clientIp = controller.Request.UserHostAddress;
            string secret = "6Lc_VIYUAAAAAAxoxyRwJjMsq6f5lpB3ya3VvqMz";
            //2. Validate the reCAPTCHA with Google
            var captchaApi = new ReCaptchaService();
            var result = await captchaApi.Verify3Async(this.captcha, clientIp, secret);
            if (!result.IsSuccess || result.Action != formName || result.Score < 0.5)
            {
                // The POST is not valid
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
