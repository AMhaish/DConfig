using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SABFramework.Core;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.Web;
using DConfigOS_Core.Models;
using DConfigOS_Core.Repositories.WebsiteContentServices;
using DConfigOS_Core.Repositories.Utilities;
using System.Net.Mail;
using System.Net;
using SABFramework.PreDefinedModules.MembershipModule.DataProviders;
using SABFramework.PreDefinedModules.MembershipModule.Models;
using SABFramework.Providers;
using Ninject;
using Microsoft.AspNet.Identity;
using static SABFramework.Providers.EmailProvider;

namespace DConfigOS_Core.Layer2.FormsServices
{
    public class CUDFormSubmitEmailEventsAction : UserActionsBase
    {
        [Inject]
        public IEmailProvider emailProvider { private get; set; }
        [Inject]
        public IFormSubmitEventsAPI formsSubmitEventsAPI { get; set; }

        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public int? FormId { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Bcc { get; set; }
        public string Cc { get; set; }
        public string Subject { get; set; }
        public int? FromBindedFieldId { get; set; }
        public int? ToBindedFieldId { get; set; }
        public int? BccBindedFieldId { get; set; }
        public int? CcBindedFieldId { get; set; }
        public int? TemplateId { get; set; }
        public override async Task<SABFramework.Core.SABActionResult> PostHandler(Controller controller)
        {
            if (controller.ModelState.IsValid && FormId.HasValue && TemplateId.HasValue && !String.IsNullOrEmpty(To) && !String.IsNullOrEmpty(Subject))
            {
                var eventObj = new FormSubmitEmailEvent()
                {
                    Id = Id,
                    Name = Name,
                    Bcc = Bcc,
                    Cc = Cc,
                    From = From,
                    FormId = FormId.Value,
                    Subject = Subject,
                    TemplateId = TemplateId.Value,
                    To = To,
                    FromBindedFieldId = FromBindedFieldId,
                    ToBindedFieldId = ToBindedFieldId,
                    BccBindedFieldId = BccBindedFieldId,
                    CcBindedFieldId = CcBindedFieldId,
                    CreatorId = UserId
                };

                //try
                //{
                //    SABIdentityMessage m = new SABIdentityMessage();
                //    m.Body = "test";
                //    m.Subject = "test";
                //    Task.Run(() => emailProvider.SendAsync(m)).Wait();
                //}
                //catch
                //{
                //    return Json(new { result = "false", message = "From email is not valid, please try again" });
                //}


                var result = formsSubmitEventsAPI.CreateSubmitEmailEvent(eventObj);
                switch (result)
                {
                    case ResultCodes.Succeed:
                        return Json(new { result = "true", obj = eventObj });
                    case ResultCodes.ObjectAlreadyExists:
                        return Json(new { result = "false", message = "Form submit event with the same name is already exists" });
                }
            }
            else
            {
                return Json(new { result = "false", message = "Some required fileds are missed" });
            }
            return null;
        }

        public override async Task<SABFramework.Core.SABActionResult> PutHandler(Controller controller)
        {
            if (controller.ModelState.IsValid && FormId.HasValue && TemplateId.HasValue && !String.IsNullOrEmpty(To) && !String.IsNullOrEmpty(Subject))
            {
                var eventObj = new FormSubmitEmailEvent()
                {
                    Id = Id,
                    Name = Name,
                    Bcc = Bcc,
                    From = From,
                    Cc = Cc,
                    FormId = FormId.Value,
                    Subject = Subject,
                    TemplateId = TemplateId.Value,
                    To = To,
                    FromBindedFieldId = FromBindedFieldId,
                    ToBindedFieldId = ToBindedFieldId,
                    BccBindedFieldId = BccBindedFieldId,
                    CcBindedFieldId = CcBindedFieldId
                };
                eventObj.Type = FormSubmitEventType.PredefinedTypes.EmailType;

                //try
                //{
                //    SABIdentityMessage m = new SABIdentityMessage();
                //    m.Body = "test";
                //    m.Subject = "test";
                //    Task.Run(() => emailProvider.SendAsync(m)).Wait();
                //}
                //catch (SmtpException smtpEx)
                //{
                //    return Json(new { result = "false", message = "From email is not valid, please try again" });
                //}

                var result = formsSubmitEventsAPI.UpdateSubmitEmailEvent(eventObj);
                switch (result)
                {
                    case ResultCodes.Succeed:
                        return Json(new { result = "true", obj = eventObj });
                    case ResultCodes.ObjectHasntFound:
                        return Json(new { result = "false", message = "Form submit event hasn't been found to be updated" });
                }
            }
            else
            {
                return Json(new { result = "false", message = "Some required fileds are missed" });
            }
            return null;
        }

        public override async Task<SABFramework.Core.SABActionResult> DeleteHandler(Controller controller)
        {
            if (controller.ModelState.IsValid)
            {
                var result = formsSubmitEventsAPI.DeleteSubmitEmailEvent(Id);
                switch (result)
                {
                    case ResultCodes.Succeed:
                        return Json(new { result = "true" });
                    case ResultCodes.ObjectHasntFound:
                        return Json(new { result = "false", message = "Form submit event hasn't been found to be deleted" });
                }
            }
            else
            {
                return Json(new { result = "false", message = "Some required fileds are missed" });
            }
            return null;
        }
    }
}
