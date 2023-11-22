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
    public class ViewTypesAPI : IViewTypesAPI
    {
        public virtual List<ViewType> GetRootViewTypes(string creatorId = null)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var rootTypes = context.ViewTypes.Where(m => m.ParentTypes.Count <= 0 && (creatorId == null || m.CreatorId == creatorId)).ToList();
            return rootTypes;
        }

        public virtual List<ViewType> GetRootViewTypesByContextId(int contextId)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext(true);
            var rootTypes = context.ViewTypes.Where(m => m.ContextCompanyId == contextId && m.ParentTypes.Count <= 0).ToList();
            return rootTypes;
        }

        public virtual List<ViewType> GetViewTypes(string creatorId = null)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var types = context.ViewTypes.Where(m => creatorId == null || m.CreatorId == creatorId).OrderBy(m => m.Name).ToList();
            for (int i = 0; i < types.Count; i++)
            {
                types[i].ViewFields = types[i].ViewFields.OrderBy(n => n.Priority).ToList();
            }
            return types;
        }

        public virtual List<ViewType> GetViewTypesByContextId(int contextId)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext(true);
            var types = context.ViewTypes.Where(m => m.ContextCompanyId == contextId).OrderBy(m => m.Name).ToList();
            for (int i = 0; i < types.Count; i++)
            {
                types[i].ViewFields = types[i].ViewFields.OrderBy(n => n.Priority).ToList();
            }
            return types;
        }

        public virtual ViewType GetViewType(int typeId, string creatorId = null)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var dbType = context.ViewTypes.Where(m => m.Id == typeId && (creatorId == null || m.CreatorId == creatorId)).FirstOrDefault();
            dbType.ViewFields = dbType.ViewFields.OrderBy(n => n.Priority).ToList();
            return dbType;
        }

        public virtual List<ViewType> GetViewTypeChildren(int typeId, string creatorId = null)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var dbType = context.ViewTypes.Where(m => m.Id == typeId && (creatorId == null || m.CreatorId == creatorId)).FirstOrDefault();
            if (dbType != null)
            {
                return dbType.ChildrenTypes.ToList();
            }
            else
            {
                return null;
            }
        }

        public virtual List<ViewType> GetViewTypesChildrenByContextId(int typeId, int contextId)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext(true);
            var dbType = context.ViewTypes.Where(m => m.Id == typeId && m.ContextCompanyId == contextId).FirstOrDefault();
            if (dbType != null)
            {
                return dbType.ChildrenTypes.ToList();
            }
            else
            {
                return null;
            }
        }

        public virtual int UpdateViewTypeChildren(int typeId, List<int> childrenIds, string creatorId = null)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var dbType = context.ViewTypes.Where(m => m.Id == typeId && (creatorId == null || m.CreatorId == creatorId)).FirstOrDefault();
            if (dbType != null)
            {
                dbType.ChildrenTypes.Clear();
                if (childrenIds != null && childrenIds.Count > 0)
                {
                    var children = context.ViewTypes.Where(m => childrenIds.Contains(m.Id));
                    if (children != null)
                    {
                        foreach (var child in children)
                        {
                            dbType.ChildrenTypes.Add(child);
                        }
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

        public virtual int CreateViewType(ViewType type, string creatorId = null)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var dbType = context.ViewTypes.Where(m => m.Name == type.Name && (creatorId == null || m.CreatorId == creatorId)).FirstOrDefault();
            if (dbType == null)
            {
                context.ViewTypes.Add(type);
                context.SaveChanges();
                return ResultCodes.Succeed;
            }
            else
            {
                return ResultCodes.ObjectAlreadyExists;
            }
        }

        public virtual int UpdateViewType(ViewType type, string creatorId = null)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var dbType = context.ViewTypes.Where(m => m.Id == type.Id && (creatorId == null || m.CreatorId == creatorId)).FirstOrDefault();
            var sameType = context.ViewTypes.Where(m => m.Name == type.Name && m.Id != type.Id && (creatorId == null || m.CreatorId == creatorId)).FirstOrDefault();
            if (dbType != null)
            {
                if (sameType == null)
                {
                    dbType.Name = type.Name;
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

        public virtual int DeleteViewType(int id, string creatorId = null)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var dbType = context.ViewTypes.Where(m => m.Id == id && (creatorId == null || m.CreatorId == creatorId)).FirstOrDefault();
            if (dbType != null)
            {
                if (dbType.TypeTemplates.Count <= 0 && dbType.TypeContents.Count <= 0)
                {
                    dbType.ChildrenTypes.Clear();
                    dbType.ParentTypes.Clear();
                    context.ViewTypes.Remove(dbType);
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
    }
}
