using DConfigOS_Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DConfigOS_Core.Repositories.WebsiteContentServices
{
    public interface IViewFieldsEnumsAPI
    {
        List<ViewFieldsEnum> GetViewFieldsEnums(string creatorId = null);
        ViewFieldsEnum GetViewFieldsEnum(int id, string creatorId = null);
        QueryResults<ViewFieldsEnumValue> GetViewFieldsEnumValues(int id, int limit, int skip, string keyword = null, string creatorId = null, string sortField = null, string sortOrder = null);
        List<ViewFieldsEnumValue> GetViewFieldsEnumValues(int id, string creatorId = null);
        int CreateViewFieldsEnum(ViewFieldsEnum fieldsEnum, string creatorId = null);
        int UpdateFormsFieldsEnum(ViewFieldsEnum fieldsEnum, string creatorId = null);
        int DeleteFormsFieldsEnum(int id, string creatorId = null);
        int UpdateViewFieldsEnumValues(int enumId, List<ViewFieldsEnumValue> values, string creatorId = null);
        int DeleteFormsFieldsEnumValue(int id, string creatorId = null);
        ViewFieldsEnumValue GetViewFieldsEnumValue(int id);
        int CreateViewFieldsEnumValue(int enumId, string Value, int? subEnumId, string creatorId = null, string langValueJson = null, int? priority = null);
        int UpdateViewFieldsEnumValue(int EnumId, int Id, string Value, int? subEnumId, string creatorId = null, string langValueJson = null, int? priority = null);
    }
}
