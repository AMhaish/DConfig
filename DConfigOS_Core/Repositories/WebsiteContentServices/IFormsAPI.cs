using DConfigOS_Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DConfigOS_Core.Repositories.WebsiteContentServices
{
    public interface IFormsAPI
    {
        List<Form> GetRootForms(string creatorId = null);
        List<Form> GetCompanyContextForms(int companyContextId);
        List<FormsType> GetFormsTypes();
        List<Form> GetForms(string creatorId = null);
        Form GetForm(int formId, int? contextCompanyId = null, string creatorId = null);
        Form GetFormByUniqueParam(Guid param);
        FormInstance GetFormInstance(int formId);
        int CreateForm(Form form, string creatorId = null);
        int UpdateForm(Form form, string creatorId = null);
        int DeleteForm(int id, string creatorId = null);

    }
}
