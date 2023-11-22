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
using System.Linq.Expressions;
using System.Data.Entity;

namespace DConfigOS_Core.Repositories.WebsiteContentServices
{
    public class FormsInstancesAPI : IFormsInstancesAPI
    {
        public async virtual Task<List<FormInstance>> GetFormInstances(int formId, string creatorId = null)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var formsInstancesQuery = context.FormsInstances.Include(m => m.FieldsValues).Where(m => m.FormId == formId).OrderByDescending(m => m.CreateDate).Take(1000);
            var formsInstances = await formsInstancesQuery.ToListAsync();
            return formsInstances;
        }

        public virtual FormInstance GetFormInstance(int instanceId, string creatorId = null)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var dbFormInstance = context.FormsInstances.Where(m => m.Id == instanceId && (creatorId == null || m.Form.CreatorId == creatorId)).FirstOrDefault();
            return dbFormInstance;
        }


        public virtual int CreateFormInstance(FormInstance formIns, Dictionary<int, string> fieldsValues)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            formIns.FieldsValues = new List<FormFieldValue>();
            foreach (KeyValuePair<int, string> pair in fieldsValues)
            {
                formIns.FieldsValues.Add(new FormFieldValue() { FieldId = pair.Key, Value = pair.Value });
            }
            formIns.CreateDate = DateTime.Now;
            context.FormsInstances.Add(formIns);
            context.SaveChanges();
            return ResultCodes.Succeed;
        }


        public virtual int DeleteFormInstance(int id, string creatorId = null)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var dbForm = context.FormsInstances.Where(m => m.Id == id && (creatorId == null || m.Form.CreatorId == creatorId)).FirstOrDefault();
            if (dbForm != null)
            {
                context.FormsInstances.Remove(dbForm);
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
