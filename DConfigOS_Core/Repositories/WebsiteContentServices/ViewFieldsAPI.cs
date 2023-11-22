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
using System.Data.Entity;

namespace DConfigOS_Core.Repositories.WebsiteContentServices
{
    public class ViewFieldsAPI :IViewFieldsAPI
    {
        public virtual List<FieldsType> GetViewFieldTypes()
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var types = context.FieldsTypes.OrderBy(m => m.Name).ToList();
            return types;
        }

        public virtual List<ViewField> GetViewTypeFields(int viewTypeId)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var fields = context.ViewFields.Where(m => m.ViewTypeId == viewTypeId).ToList();
            return fields;
        }

        public virtual ViewField GetViewTypeField(int fieldId)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var field = context.ViewFields.Where(m => m.Id == fieldId).SingleOrDefault();
            return field;
        }

        public virtual int AddFieldToViewType(int viewTypeId, ViewField field, string creatorId = null)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var dbType = context.ViewTypes.Where(m => m.Id == viewTypeId && (creatorId == null || m.CreatorId == creatorId)).FirstOrDefault();
            var sameField = dbType.ViewFields.Where(m => m.Name == field.Name).FirstOrDefault();
            if (dbType != null)
            {
                if (sameField == null)
                {
                    dbType.ViewFields.Add(field);
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
                return ResultCodes.ObjectHasntFound;
            }
        }

        public virtual int UpdateViewField(ViewField field, string creatorId = null)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var dbField = context.ViewFields.Where(m => m.Id == field.Id && (creatorId == null || m.ViewType.CreatorId == creatorId)).FirstOrDefault();
            var sameField = context.ViewFields.Where(m => m.Name == field.Name && m.Id != field.Id && m.ViewTypeId == field.ViewTypeId).FirstOrDefault();
            if (dbField != null)
            {
                if (sameField == null)
                {
                    dbField.Name = field.Name;
                    dbField.ColumnName = field.ColumnName;
                    dbField.Type = field.Type;
                    dbField.ViewType = context.ViewTypes.SingleOrDefault(m => m.Id == dbField.ViewTypeId);
                    dbField.EnumId = field.EnumId;
                    dbField.Priority = field.Priority;
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
                return ResultCodes.ObjectHasntFound;
            }
        }

        public virtual int RemoveFieldFromViewType(int fieldId, string creatorId = null)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var dbField = context.ViewFields.Where(m => m.Id == fieldId && (creatorId == null || m.ViewType.CreatorId == creatorId)).FirstOrDefault();
            if (dbField != null)
            {
                context.ViewFields.Remove(dbField);
                context.SaveChanges();
                return ResultCodes.Succeed;
            }
            else
            {
                return ResultCodes.ObjectHasntFound;
            }
        }

        public static int UpdateViewFieldValues(int contentId, List<ViewFieldValue> FieldsValues)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var dbContent = context.ContentInstances.Where(m => m.Id == contentId).Include(m => m.FieldsValues).FirstOrDefault();
            if (dbContent != null)
            {
                var dbFields = dbContent.Content.ViewType.ViewFields.ToList();
                foreach (var field in FieldsValues)
                {
                    var dbField = dbFields.SingleOrDefault(m => m.Id == field.FieldId);
                    var dbFieldValue = dbContent.FieldsValues.Where(m => m.FieldId == field.FieldId && m.ContentId == contentId).FirstOrDefault();
                    if (!String.IsNullOrEmpty(field.Value) && dbField.Type != "Auto Increment Number")
                    {
                        if (dbFieldValue != null)
                        {
                            dbFieldValue.Value = field.Value;
                        }
                        else
                        {
                            var newValue = new ViewFieldValue() { Value = field.Value };
                            dbContent.FieldsValues.Add(newValue);
                            dbField.FieldValues.Add(newValue);
                        }
                    }
                    else if (dbField.Type == "Auto Increment Number" && dbFieldValue == null)
                    {
                        var dbFieldLastValue = context.ViewFieldValue.Where(m => m.FieldId == dbField.Id);
                        int lastValue = 1;
                        if (dbFieldLastValue != null && dbFieldLastValue.Count() > 0)
                        {
                            field.Value = (dbFieldLastValue.ToList().Max(m => int.Parse(m.Value)) + 1).ToString();
                        }
                        else
                        {
                            field.Value = lastValue.ToString();
                        }
                        var newValue = new ViewFieldValue() { Value = field.Value };
                        dbContent.FieldsValues.Add(newValue);
                        dbField.FieldValues.Add(newValue);
                    }
                    else
                    {
                        dbContent.FieldsValues.Remove(dbFieldValue);
                    }
                }
                context.SaveChanges();
                //Task.Run(() =>
                //{
                //    Providers.DataContexts.DConfigOS_Core_ElasticSearchContext elasticContext = new Providers.DataContexts.DConfigOS_Core_ElasticSearchContext();
                //    dbContent = context.ContentInstances.Where(m => m.Id == contentId).FirstOrDefault();
                //    elasticContext.ContentInstancesValues.Index(dbContent);
                //});
                return ResultCodes.Succeed;
            }
            else
            {
                return ResultCodes.ObjectHasntFound;
            }
        }

        public virtual int UpdateViewFieldValue(int contentId, int fieldId, string value, string creatorId = null)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var dbContent = context.ContentInstances.Where(m => m.Id == contentId).FirstOrDefault();
            var dbField = context.ViewFields.Where(m => m.Id == fieldId && (creatorId == null || m.ViewType.CreatorId == creatorId)).FirstOrDefault();
            var dbFieldValue = context.ViewFieldValue.Where(m => m.ContentId == contentId && m.FieldId == fieldId).FirstOrDefault();
            if (dbContent != null && dbField != null)
            {
                if (dbField.Type != "Auto Increment Number" && !String.IsNullOrEmpty(value))
                {
                    if (dbFieldValue != null)
                    {
                        dbFieldValue.Value = value;
                    }
                    else
                    {
                        var newValue = new ViewFieldValue() { Value = value };
                        dbContent.FieldsValues.Add(newValue);
                        dbField.FieldValues.Add(newValue);
                    }
                }
                else if (dbField.Type == "Auto Increment Number" && dbFieldValue == null)
                {
                    var dbFieldLastValue = context.ViewFieldValue.Where(m => m.FieldId == dbField.Id);
                    int lastValue = 1;
                    if (dbFieldLastValue != null && dbFieldLastValue.Count() > 0)
                    {
                        value = (dbFieldLastValue.ToList().Max(m => int.Parse(m.Value)) + 1).ToString();
                    }
                    else
                    {
                        value = lastValue.ToString();
                    }
                    var newValue = new ViewFieldValue() { Value = value };
                    dbContent.FieldsValues.Add(newValue);
                    dbField.FieldValues.Add(newValue);
                }
                else
                {
                    dbContent.FieldsValues.Remove(dbFieldValue);
                }
                context.SaveChanges();
                //Task.Run(() =>
                //{
                //    Providers.DataContexts.DConfigOS_Core_ElasticSearchContext elasticContext = new Providers.DataContexts.DConfigOS_Core_ElasticSearchContext();
                //    dbContent = context.ContentInstances.Where(m => m.Id == contentId).FirstOrDefault();
                //    elasticContext.ContentInstancesValues.Index(dbContent);
                //});
                return ResultCodes.Succeed;
            }
            else
            {
                return ResultCodes.ObjectHasntFound;
            }
        }
    }
}
