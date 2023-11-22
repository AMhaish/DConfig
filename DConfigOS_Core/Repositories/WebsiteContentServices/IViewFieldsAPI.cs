using DConfigOS_Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DConfigOS_Core.Repositories.WebsiteContentServices
{
    public interface IViewFieldsAPI
    {
        List<FieldsType> GetViewFieldTypes();
        List<ViewField> GetViewTypeFields(int viewTypeId);
        ViewField GetViewTypeField(int fieldId);
        int AddFieldToViewType(int viewTypeId, ViewField field, string creatorId = null);
        int UpdateViewField(ViewField field, string creatorId = null);
        int RemoveFieldFromViewType(int fieldId, string creatorId = null);
        int UpdateViewFieldValue(int contentId, int fieldId, string value, string creatorId = null);

    }
}
