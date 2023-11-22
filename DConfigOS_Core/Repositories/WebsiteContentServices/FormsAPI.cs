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
    public class FormsAPI : IFormsAPI
    {
        public virtual List<Form> GetRootForms(string creatorId = null)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var rootForms = context.Forms.Where(m => m.ParentForm == null && (creatorId == null || m.CreatorId == creatorId)).ToList();
            return rootForms;
        }
        public virtual List<Form> GetCompanyContextForms(int companyContextId)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var list = context.Forms.Where(m => m.ContextCompanyId == companyContextId).ToList();
            return list;
        }
        public virtual List<FormsType> GetFormsTypes()
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var types = context.FormsTypes.ToList();
            return types;
        }
        public virtual List<Form> GetForms(string creatorId = null)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var forms = context.Forms.Where(m => creatorId == null || m.CreatorId == creatorId).OrderBy(m => m.Name).ToList();
            foreach (Form f in forms)
            {
                f.FormFields = f.FormFields.OrderBy(m => m.Priority).ToList();
            }
            return forms;
        }
        public virtual Form GetForm(int formId,int? contextCompanyId=null, string creatorId = null)
        {
            DConfigOS_Core_DBContext context=null;
            if(contextCompanyId.HasValue)
                context = new DConfigOS_Core_DBContext(contextCompanyId.Value);
            else
                context = new DConfigOS_Core_DBContext();
            var dbForm = context.Forms.Where(m => m.Id == formId && (creatorId == null || m.CreatorId == creatorId)).FirstOrDefault();
            if (dbForm != null)
                dbForm.FormFields = dbForm.FormFields.OrderBy(m => m.Priority).ToList();
            return dbForm;
        }
        public virtual FormInstance GetFormInstance(int formId)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var dbForm = context.FormsInstances.Where(m => m.Id == formId).FirstOrDefault();
            return dbForm;
        }
        public virtual Form GetFormByUniqueParam(Guid param)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var dbForm = context.Forms.Where(m => m.UrlParam == param).FirstOrDefault();
            return dbForm;
        }
        public virtual int CreateForm(Form form, string creatorId = null)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var dbForm = context.Forms.Where(m => m.Name == form.Name && (creatorId == null || m.CreatorId == creatorId)).FirstOrDefault();
            var cs = context.Forms.Where(m => m.ParentFormId == form.ParentFormId).Select(m => m.Priority);
            int cPrioity = (cs != null && cs.Count() > 0 ? cs.Max() : 0);
            if (dbForm == null)
            {
                form.Priority = cPrioity + 1;
                form.UrlParam = Guid.NewGuid();
                context.Forms.Add(form);
                context.SaveChanges();
                return ResultCodes.Succeed;
            }
            else
            {
                return ResultCodes.ObjectAlreadyExists;
            }
        }

        public virtual int UpdateForm(Form form, string creatorId = null)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var dbForm = context.Forms.Where(m => m.Id == form.Id && (creatorId == null || m.CreatorId == creatorId)).FirstOrDefault();
            var sameForm = context.Forms.Where(m => m.Name == form.Name && m.Id != form.Id).FirstOrDefault();
            if (dbForm != null)
            {
                if (!dbForm.AppId.HasValue)
                {
                    if (sameForm == null)
                    {
                        dbForm.Name = form.Name;
                        dbForm.Type = form.Type;
                        dbForm.AddItemButtonText = form.AddItemButtonText;
                        dbForm.RemoveItemButtonText = form.RemoveItemButtonText;
                        dbForm.NextButtonText = form.NextButtonText;
                        dbForm.BackButtonText = form.BackButtonText;
                        dbForm.SubmitButtonText = form.SubmitButtonText;
                        dbForm.SubmitRedirectUrl = form.SubmitRedirectUrl;
                        dbForm.PrintTemplateId = form.PrintTemplateId;
                        dbForm.ReCapatchaEnabled = form.ReCapatchaEnabled;
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
                    return ResultCodes.ObjectNotAllowedToBeUpdated;
                }
            }
            else
            {
                return ResultCodes.ObjectHasntFound;
            }
        }

        public virtual int DeleteForm(int id, string creatorId = null)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var dbForm = context.Forms.Where(m => m.Id == id && (creatorId == null || m.CreatorId == creatorId)).FirstOrDefault();
            if (dbForm != null)
            {
                if (!dbForm.AppId.HasValue)
                {
                    if (dbForm.FormsInstances.Count <= 0)
                    {
                        dbForm.ChildrenForms.Clear();
                        context.Forms.Remove(dbForm);
                        context.SaveChanges();
                        return ResultCodes.Succeed;
                    }
                    else
                    {
                        return ResultCodes.ObjectLinkedToAnotherObject;
                    }
                }
                else
                {
                    return ResultCodes.ObjectNotAllowedToBeDeleted;
                }
            }
            else
            {
                return ResultCodes.ObjectHasntFound;
            }
        }
    }
}
