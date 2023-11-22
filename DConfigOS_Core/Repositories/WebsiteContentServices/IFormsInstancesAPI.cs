using DConfigOS_Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DConfigOS_Core.Repositories.WebsiteContentServices
{
    public interface IFormsInstancesAPI
    {
        Task<List<FormInstance>> GetFormInstances(int formId, string creatorId = null);
        FormInstance GetFormInstance(int instanceId, string creatorId = null);
        int CreateFormInstance(FormInstance formIns, Dictionary<int, string> fieldsValues);
        int DeleteFormInstance(int id, string creatorId = null);

    }
}
