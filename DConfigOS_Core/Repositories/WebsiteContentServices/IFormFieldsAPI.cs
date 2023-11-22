using DConfigOS_Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DConfigOS_Core.Repositories.WebsiteContentServices
{
    public interface IFormFieldsAPI : IDisposable
    {
        List<FieldsType> GetFormFieldsTypes();
        List<FormsField> GetFormFields(int formId);
        int AddFieldToForm(int formId, FormsField field, string creatorId = null);
        int UpdateFormField(FormsField field, string creatorId = null);
        int RemoveFieldFromForm(int fieldId, string creatorId = null);
        int UpdateFormFieldValue(int formInstanceId, int fieldId, string value, string creatorId = null);

    }
}
