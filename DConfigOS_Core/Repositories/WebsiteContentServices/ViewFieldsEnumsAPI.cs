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
    public class ViewFieldsEnumsAPI : IViewFieldsEnumsAPI
    {

        public virtual List<ViewFieldsEnum> GetViewFieldsEnums(string creatorId = null)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var enums = context.ViewFieldsEnums.Where(m => (creatorId == null || m.CreatorId == creatorId)).OrderBy(m => m.Name).ToList();
            return enums;
        }

        public virtual ViewFieldsEnum GetViewFieldsEnum(int id, string creatorId = null)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var e = context.ViewFieldsEnums.Where(m => m.Id == id && (creatorId == null || m.CreatorId == creatorId)).SingleOrDefault();
            return e;
        }

        public virtual ViewFieldsEnumValue GetViewFieldsEnumValue(int id)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var viewFieldsEnum = context.ViewFieldsEnumsValues.Where(m => m.Id == id).SingleOrDefault();
            return viewFieldsEnum;
        }

        public virtual QueryResults<ViewFieldsEnumValue> GetViewFieldsEnumValues(int id, int limit, int skip, string keyword = null, string creatorId = null, string sortField = null, string sortOrder = null)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var viewFieldsEnum = context.ViewFieldsEnums.Where(m => m.Id == id && (creatorId == null || m.CreatorId == creatorId)).SingleOrDefault();

            IEnumerable<ViewFieldsEnumValue> result = viewFieldsEnum.Values.Where(m =>
                (keyword == null || m.Value.Contains(keyword)));

            //We should return message say there was a problem in sorting parameters
            if (string.IsNullOrEmpty(sortField) || string.IsNullOrEmpty(sortOrder))
            {
                result = result.OrderBy(m => m.Priority);
            }
            else
            {
                string[] sortFieldArr = sortField.ToLower().Split(',');
                string[] sortOrderArr = sortOrder.ToLower().Split(',');

                //We should return message say there was a problem in sorting parameters
                if (sortFieldArr.Length != sortOrderArr.Length)
                    result = result.OrderBy(m => m.Priority);
                else
                {
                    var fieldOrderArr = sortFieldArr.Zip(sortOrderArr, (f, o) => new { field = f, order = o });

                    foreach (var item in fieldOrderArr)
                    {
                        switch (item.field)
                        {
                            case "value":
                                if (item.order == "asc")
                                    result = result.OrderBy(m => m.Value.Trim());
                                else
                                    result = result.OrderByDescending(m => m.Value.Trim());
                                break;
                            case "priority":
                                if (item.order == "asc")
                                    result = result.OrderBy(m => m.Priority);
                                else
                                    result = result.OrderByDescending(m => m.Priority);
                                break;
                        }
                    }
                }                
            }

            int totalCount = result.Count();
            var items = result
                    .Skip(skip * limit)
                    .Take(limit)
                    .ToList();

            return new QueryResults<ViewFieldsEnumValue>() { Items = items, TotalCount = totalCount };
        }



        public virtual List<ViewFieldsEnumValue> GetViewFieldsEnumValues(int id, string creatorId = null)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var e = context.ViewFieldsEnums.SingleOrDefault(m => m.Id == id && (creatorId == null || m.CreatorId == creatorId));
            if (e != null)
            {
                return e.Values.OrderBy(m => m.Priority).ToList();
            }
            return null;
        }

        public virtual int CreateViewFieldsEnum(ViewFieldsEnum fieldsEnum, string creatorId = null)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var dbEnum = context.ViewFieldsEnums.Where(m => m.Name == fieldsEnum.Name && (creatorId == null || m.CreatorId == creatorId)).FirstOrDefault();
            if (dbEnum == null)
            {
                context.ViewFieldsEnums.Add(fieldsEnum);
                context.SaveChanges();
                return ResultCodes.Succeed;
            }
            else
            {
                return ResultCodes.ObjectAlreadyExists;
            }
        }

        public virtual int CreateViewFieldsEnumValue(int enumId, string Value, int? subEnumId = null, string creatorId = null, string langValueJson = null, int? priority = null)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var dbEnum = context.ViewFieldsEnums.Where(m => m.Id == enumId && (creatorId == null || m.CreatorId == creatorId)).FirstOrDefault();
            if (dbEnum != null)
            {
                ViewFieldsEnumValue viewFieldsEnumValue = subEnumId != null && subEnumId.HasValue ?
                    new ViewFieldsEnumValue() { Value = Value, SubEnumId = subEnumId.Value, LangValueJson = langValueJson, Priority = priority ?? null } :
                    new ViewFieldsEnumValue() { Value = Value, LangValueJson = langValueJson, Priority = priority ?? null };
                dbEnum.Values.Add(viewFieldsEnumValue);
                context.SaveChanges();
                return ResultCodes.Succeed;
            }
            else
            {
                return ResultCodes.ObjectAlreadyExists;
            }
        }

        public virtual int UpdateViewFieldsEnumValue(int EnumId, int Id, string Value, int? subEnumId, string creatorId = null, string langValueJson = null, int? priority = null)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var dbEnum = context.ViewFieldsEnums.Where(m => m.Id == EnumId && (creatorId == null || m.CreatorId == creatorId)).FirstOrDefault();
            var dbEnumValue = context.ViewFieldsEnumsValues.Where(m => m.Id == Id).FirstOrDefault();
            if (dbEnum != null && dbEnumValue != null)
            {
                dbEnumValue.Value = Value;
                dbEnumValue.LangValueJson = langValueJson;
                dbEnumValue.Priority = priority;

                if (subEnumId.HasValue && subEnumId.Value > 0)
                    dbEnumValue.SubEnumId = subEnumId.Value;
                if (EnumId != dbEnumValue.EnumId)
                {
                    var dbEnumOldParent = context.ViewFieldsEnums.Where(m => m.Id == dbEnumValue.EnumId && (creatorId == null || m.CreatorId == creatorId)).FirstOrDefault();
                    dbEnumOldParent.Values.Remove(dbEnumValue);
                    dbEnum.Values.Add(dbEnumValue);
                }

                context.SaveChanges();
                return ResultCodes.Succeed;
            }
            else
            {
                return ResultCodes.ObjectAlreadyExists;
            }
        }

        public virtual int UpdateFormsFieldsEnum(ViewFieldsEnum fieldsEnum, string creatorId = null)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var dbEnum = context.ViewFieldsEnums.Where(m => m.Id == fieldsEnum.Id && (creatorId == null || m.CreatorId == creatorId)).FirstOrDefault();
            var dbSameEnum = context.ViewFieldsEnums.Where(m => m.Name == fieldsEnum.Name && m.Id != fieldsEnum.Id && (creatorId == null || m.CreatorId == creatorId)).FirstOrDefault();
            if (dbEnum != null)
            {
                if (dbSameEnum == null)
                {
                    dbEnum.Name = fieldsEnum.Name;
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

        public virtual int DeleteFormsFieldsEnum(int id, string creatorId = null)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var dbEnum = context.ViewFieldsEnums.Where(m => m.Id == id && (creatorId == null || m.CreatorId == creatorId)).FirstOrDefault();
            if (dbEnum != null)
            {
                if (dbEnum.ViewFields.Count <= 0)
                {
                    context.ViewFieldsEnums.Remove(dbEnum);
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
                return ResultCodes.ObjectHasntFound;
            }
        }
        public virtual int UpdateViewFieldsEnumValues(int enumId, List<ViewFieldsEnumValue> values, string creatorId = null)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var dbEnum = context.ViewFieldsEnums.Where(m => m.Id == enumId && (creatorId == null || m.CreatorId == creatorId)).FirstOrDefault();
            if (dbEnum != null)
            {
                var oldValues = context.ViewFieldsEnumsValues.Where(w => w.EnumId == dbEnum.Id).ToList();
                foreach (var val in oldValues)
                {
                    context.ViewFieldsEnumsValues.Remove(val);
                }
                if (values != null)
                {
                    foreach (var value in values)
                    {
                        dbEnum.Values.Add(new ViewFieldsEnumValue() { Value = value.Value, SubEnumId = value.SubEnumId, LangValueJson = value.LangValueJson });
                    }
                }
                context.SaveChanges();
                return ResultCodes.Succeed;

            }
            else
            {
                return ResultCodes.ObjectHasntFound;
            }
        }

        public virtual int DeleteFormsFieldsEnumValue(int id, string creatorId = null)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var dbValue = context.ViewFieldsEnumsValues.Where(m => m.Id == id && (creatorId == null || m.Enum.CreatorId == creatorId)).FirstOrDefault();
            if (dbValue != null)
            {
                context.ViewFieldsEnumsValues.Remove(dbValue);
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
