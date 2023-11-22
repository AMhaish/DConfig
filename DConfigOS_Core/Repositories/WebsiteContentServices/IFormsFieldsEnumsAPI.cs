using DConfigOS_Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DConfigOS_Core.Repositories.WebsiteContentServices
{
    public interface IFormsFieldsEnumsAPI
    {
        List<FormsFieldsEnum> GetFormsFieldsEnums(string creatorId = null);
        FormsFieldsEnum GetFormsFieldsEnum(int id, string creatorId = null);
        int CreateFormsFieldsEnum(FormsFieldsEnum fieldsEnum, string creatorId = null);
        int UpdateFormsFieldsEnum(FormsFieldsEnum fieldsEnum, string creatorId = null);
        int DeleteFormsFieldsEnum(int id, string creatorId = null);
        int UpdateFormsFieldsEnumValues(int enumId, List<string> values, string creatorId = null);
        int DeleteFormsFieldsEnumValue(int id, string creatorId = null);

    }
}
