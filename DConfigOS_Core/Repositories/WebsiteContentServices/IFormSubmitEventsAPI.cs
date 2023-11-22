using DConfigOS_Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DConfigOS_Core.Repositories.WebsiteContentServices
{
    public interface IFormSubmitEventsAPI
    {
        List<FormSubmitEvent> GetFormSubmitEvents(int formId, string creatorId = null);
        FormSubmitEvent GetFormSubmitEvent(int eventId, string creatorId = null);
        List<FormSubmitEventType> GetFormSubmitEventsTypes();
        int CreateSubmitEmailEvent(FormSubmitEmailEvent eventObj, string creatorId = null);
        int UpdateSubmitEmailEvent(FormSubmitEmailEvent eventObj, string creatorId = null);
        int DeleteSubmitEmailEvent(int id, string creatorId = null);

    }
}
