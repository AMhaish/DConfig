using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DConfigOS_Core.Models;
using DConfigOS_Core.Models.Utilities;
using System.Reflection;
using System.IO;
using System.Text.RegularExpressions;
using DConfigOS_Core.Repositories.Utilities;

namespace DConfigOS_Core.Repositories.WebsiteContentServices
{
    public class FormSubmitEventsAPI : IFormSubmitEventsAPI
    {
        public virtual List<FormSubmitEvent> GetFormSubmitEvents(int formId, string creatorId = null)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var events = context.FormSubmitEvents.Where(m => m.FormId == formId && (creatorId == null || m.CreatorId == creatorId)).ToList();
            return events;
        }

        public virtual FormSubmitEvent GetFormSubmitEvent(int eventId, string creatorId = null)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var dbEvent = context.FormSubmitEvents.Where(m => m.Id == eventId && (creatorId == null || m.CreatorId == creatorId)).FirstOrDefault();
            return dbEvent;
        }

        public virtual List<FormSubmitEventType> GetFormSubmitEventsTypes()
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var eventsTypes = context.FormSubmitEventsTypes.ToList();
            return eventsTypes;
        }

        public virtual int CreateSubmitEmailEvent(FormSubmitEmailEvent eventObj, string creatorId = null)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            eventObj.CreateDate = DateTime.Now;
            eventObj.Type = FormSubmitEventType.PredefinedTypes.EmailType;
            context.FormSubmitEvents.Add(eventObj);
            context.SaveChanges();
            return ResultCodes.Succeed;
        }

        public virtual int UpdateSubmitEmailEvent(FormSubmitEmailEvent eventObj, string creatorId = null)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var dbEvent = context.FormSubmitEvents.OfType<FormSubmitEmailEvent>().Where(m => m.Id == eventObj.Id && (creatorId == null || m.CreatorId == creatorId)).FirstOrDefault();
            var sameEvent = context.FormSubmitEvents.Where(m => m.Name == eventObj.Name && m.Id != eventObj.Id && m.FormId == eventObj.FormId && (creatorId == null || m.CreatorId == creatorId)).FirstOrDefault();
            if (dbEvent != null)
            {
                if (sameEvent == null)
                {
                    dbEvent.Name = eventObj.Name;
                    dbEvent.Bcc = eventObj.Bcc;
                    dbEvent.Cc = eventObj.Cc;
                    dbEvent.To = eventObj.To;
                    dbEvent.From = eventObj.From;
                    dbEvent.Subject = eventObj.Subject;
                    dbEvent.TemplateId = eventObj.TemplateId;
                    dbEvent.BccBindedFieldId = eventObj.BccBindedFieldId;
                    dbEvent.CcBindedFieldId = eventObj.CcBindedFieldId;
                    dbEvent.FromBindedFieldId = eventObj.FromBindedFieldId;
                    dbEvent.ToBindedFieldId = eventObj.ToBindedFieldId;
                    context.SaveChanges();
                    return ResultCodes.Succeed;
                }
                else
                {
                    return ResultCodes.ObjectNameAlreadyUsed;
                }
            }
            else
            {
                return ResultCodes.ObjectHasntFound;
            }
        }

        public virtual int DeleteSubmitEmailEvent(int id, string creatorId = null)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var dbEvent = context.FormSubmitEvents.Where(m => m.Id == id && (creatorId == null || m.CreatorId == creatorId)).FirstOrDefault();
            if (dbEvent != null)
            {
                context.FormSubmitEvents.Remove(dbEvent);
                context.SaveChanges();
                return ResultCodes.Succeed;
            }
            else
            {
                return ResultCodes.ObjectHasntFound;
            }
        }
    }
}
