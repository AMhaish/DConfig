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
    public class FormFieldsAPI:  RepositoryBase<DConfigOS_Core_DBContext>, IFormFieldsAPI
    {
        public virtual List<FieldsType> GetFormFieldsTypes()
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var types = context.FieldsTypes.OrderBy(m => m.Name).ToList();
            return types;
        }


        public virtual FormsField GetField(int fieldId)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var field = context.FormsFields.Where(m => m.Id == fieldId).SingleOrDefault();
            return field;
        }

        public virtual List<FormsField> GetFormFields(int formId)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var fields = context.FormsFields.Where(m => m.FormId == formId).OrderBy(m => m.Priority).ToList();
            return fields;
        }

        public virtual int AddFieldToForm(int formId, FormsField field, string creatorId = null)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var dbForm = context.Forms.Where(m => m.Id == formId && (creatorId == null || m.CreatorId == creatorId)).FirstOrDefault();
            var sameField = dbForm.FormFields.Where(m => m.Name == field.Name).FirstOrDefault();
            if (dbForm != null)
            {
                if(!dbForm.AppId.HasValue)
                { 
                if (sameField == null)
                {
                    dbForm.FormFields.Add(field);
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

        public virtual int UpdateFormField(FormsField field, string creatorId = null)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var dbField = context.FormsFields.Where(m => m.Id == field.Id && (creatorId == null || m.Form.CreatorId == creatorId)).FirstOrDefault();
            var sameField = context.FormsFields.Where(m => m.Name == field.Name && m.Id != field.Id && m.FormId==field.FormId).FirstOrDefault();
            if (dbField != null)
            {
                if (!dbField.Form.AppId.HasValue)
                {
                    if (sameField == null)
                    {
                        dbField.Name = field.Name;
                        dbField.Type = field.Type;
                        dbField.EnumId = field.EnumId;
                        dbField.Title = field.Title;
                        dbField.Priority = field.Priority;
                        dbField.Invisible = field.Invisible;
                        dbField.Form = context.Forms.SingleOrDefault(m => m.Id == dbField.FormId);
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

        public virtual int RemoveFieldFromForm(int fieldId, string creatorId = null)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var dbField = context.FormsFields.Where(m => m.Id == fieldId && (creatorId == null || m.Form.CreatorId == creatorId)).FirstOrDefault();
            if (dbField != null)
            {
                if (!dbField.Form.AppId.HasValue)
                {
                    context.FormsFields.Remove(dbField);
                    context.SaveChanges();
                    return ResultCodes.Succeed;
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

        public virtual int UpdateFormFieldValue(int formInstanceId, int fieldId, string value, string creatorId = null)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var dbForm = context.FormsInstances.Where(m => m.Id == formInstanceId && (creatorId == null || m.Form.CreatorId == creatorId)).FirstOrDefault();
            var dbField = context.FormsFields.Where(m => m.Id == fieldId).FirstOrDefault();
            var dbFieldValue = context.FormsFieldsValues.Where(m => m.FormInstanceId == formInstanceId && m.FieldId == fieldId).FirstOrDefault();
            if (dbForm != null && dbField != null)
            {
                if (!String.IsNullOrEmpty(value))
                {
                    if (dbFieldValue != null)
                    {
                        dbFieldValue.Value = value;
                    }
                    else
                    {
                        var newValue = new FormFieldValue() { Value = value };
                        dbForm.FieldsValues.Add(newValue);
                        dbField.FieldsValues.Add(newValue);
                    }
                }
                else
                {
                    dbForm.FieldsValues.Remove(dbFieldValue);
                }
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
