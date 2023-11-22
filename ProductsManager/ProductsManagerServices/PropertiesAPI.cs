using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using System.Text.RegularExpressions;
using CompetitiveAnalysis.Models;
using DConfigOS_Core.Models;
using DConfigOS_Core.Repositories.Utilities;
using SABFramework.ModulesUtilities;

namespace CompetitiveAnalysis.ProductsManagerServices
{
    public class PropertiesAPI : IPropertiesAPI
    {
        public virtual List<Property> GetProperties()
        {
            ProductsManager_DBContext context = new ProductsManager_DBContext();
            var ps = context.Properties.OrderBy(m => m.Priority).ToList();
            return ps;
        }
        public virtual List<Property> GetGroupProperties(int groupId)
        {
            ProductsManager_DBContext context = new ProductsManager_DBContext();
            var ps = context.Properties.Where(m => m.GroupId == groupId).OrderBy(m => m.Priority).ToList();
            return ps;
        }
        public virtual List<FieldsType> GetPropertiesTypes()
        {
            ProductsManager_DBContext context = new ProductsManager_DBContext();
            var types = context.FieldsTypes.OrderBy(m => m.Name).ToList();
            return types;
        }

        public virtual PropertiesGroup GetPropertiesGroup(int groupId)
        {
            ProductsManager_DBContext context = new ProductsManager_DBContext();
            var g = context.PropertiesGroups.Where(m => m.Id == groupId).SingleOrDefault();
            g.Properties = g.Properties.OrderBy(m => m.Priority).ToList();
            return g;
        }
        public virtual List<PropertiesGroup> GetPropertiesGroups()
        {
            ProductsManager_DBContext context = new ProductsManager_DBContext();
            var groups = context.PropertiesGroups.OrderBy(m => m.Priority).ToList();
            foreach(PropertiesGroup pg in groups)
            {
                pg.Properties = pg.Properties.OrderBy(m => m.Priority).ToList();
            }
            return groups.ToList();
        }

        public virtual Property GetProperty(int propertyId)
        {
            ProductsManager_DBContext context = new ProductsManager_DBContext();
            var p = context.Properties.Where(m => m.Id == propertyId).SingleOrDefault();
            return p;
        }

        public virtual int CreateProperty(Property p)
        {
            ProductsManager_DBContext context = new ProductsManager_DBContext();
            var dbProperty = context.Properties.Where(m => m.Name == p.Name).FirstOrDefault();
            if (dbProperty == null)
            {

                context.Properties.Add(p);
                context.SaveChanges();
                return ResultCodes.Succeed;
            }
            else
            {
                return ResultCodes.ObjectNameAlreadyUsed;
            }
        }

        public virtual int CreatePropertiesGroup(PropertiesGroup g)
        {
            ProductsManager_DBContext context = new ProductsManager_DBContext();
            var dbGroup = context.PropertiesGroups.Where(m => m.Name == g.Name).FirstOrDefault();
            if (dbGroup == null)
            {
                context.PropertiesGroups.Add(g);
                context.SaveChanges();
                return ResultCodes.Succeed;
            }
            else
            {
                return ResultCodes.ObjectNameAlreadyUsed;
            }
        }

        public virtual int UpdateProperty(Property p)
        {
            ProductsManager_DBContext context = new ProductsManager_DBContext();
            var dbProperty = context.Properties.Where(m => m.Id == p.Id).FirstOrDefault();
            if (dbProperty != null)
            {
                dbProperty.EnumId = p.EnumId;
                dbProperty.GroupId = p.GroupId;
                dbProperty.Name = p.Name;
                dbProperty.Type = p.Type;
                dbProperty.Priority = p.Priority;
                dbProperty.LargerIsBetter = p.LargerIsBetter;
                dbProperty.Unit = p.Unit;
                dbProperty.Notes = p.Notes;
                dbProperty.DisplayAs = p.DisplayAs;
                dbProperty.ExcelColumnName = p.ExcelColumnName;
                context.SaveChanges();
                return ResultCodes.Succeed;
            }
            else
            {
                return ResultCodes.ObjectHasntFound;
            }
        }

        public virtual int UpdatePropertiesGroup(PropertiesGroup p)
        {
            ProductsManager_DBContext context = new ProductsManager_DBContext();
            var dbGroup = context.PropertiesGroups.Where(m => m.Id == p.Id).FirstOrDefault();
            var sameGroup = context.PropertiesGroups.Where(m => m.Name == p.Name && m.Id!=p.Id).FirstOrDefault();
            if (dbGroup != null)
            {
                if (sameGroup == null)
                {
                    dbGroup.Name = p.Name;
                    dbGroup.Priority = p.Priority;
                    dbGroup.DisplayAs = p.DisplayAs;
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

        public virtual int UpdateGroupsOrder(List<PropertiesGroup> groups)
        {
            ProductsManager_DBContext context = new ProductsManager_DBContext();
            Dictionary<int, int> ids = groups.ToDictionary(m => m.Id, m => m.Priority);
            var dbGroups = context.PropertiesGroups.Where(m => ids.Keys.Contains(m.Id));
            if (dbGroups != null && dbGroups.Count() > 0)
            {
                foreach (PropertiesGroup g in dbGroups)
                {
                    g.Priority = ids[g.Id];
                }
                context.SaveChanges();
                return ResultCodes.Succeed;
            }
            else
            {
                return ResultCodes.ObjectHasntFound;
            }
        }

        public virtual int DeleteProperty(int Id)
        {
            ProductsManager_DBContext context = new ProductsManager_DBContext();
            var dbProperty = context.Properties.Where(m => m.Id == Id).FirstOrDefault();
            if (dbProperty != null)
            {
                context.Properties.Remove(dbProperty);
                context.SaveChanges();
                return ResultCodes.Succeed;
            }
            else
            {
                return ResultCodes.ObjectHasntFound;
            }
        }

        public virtual int DeletePropertiesGroup(int Id)
        {
            ProductsManager_DBContext context = new ProductsManager_DBContext();
            var dbGroup = context.PropertiesGroups.Where(m => m.Id == Id).FirstOrDefault();
            if (dbGroup != null)
            {
                context.PropertiesGroups.Remove(dbGroup);
                context.SaveChanges();
                return ResultCodes.Succeed;
            }
            else
            {
                return ResultCodes.ObjectHasntFound;
            }
        }

        public virtual int AddPropertyToGroup(int groupId, Property property)
        {
            ProductsManager_DBContext context = new ProductsManager_DBContext();
            var dbGroup = context.PropertiesGroups.Where(m => m.Id == groupId).FirstOrDefault();
            var sameProperty = dbGroup.Properties.Where(m => m.Name == property.Name).FirstOrDefault();
            if (dbGroup != null)
            {
                if (sameProperty == null)
                {
                    dbGroup.Properties.Add(property);
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

        public virtual int UpdateGroupProperty(Property property)
        {
            ProductsManager_DBContext context = new ProductsManager_DBContext();
            var dbProperty = context.Properties.Where(m => m.Id == property.Id).FirstOrDefault();
            var sameProperty = context.Properties.Where(m => m.Name == property.Name && m.Id != property.Id && m.GroupId == property.GroupId).FirstOrDefault();
            if (dbProperty != null)
            {
                if (sameProperty == null)
                {
                    dbProperty.Name = property.Name;
                    dbProperty.Type = property.Type;
                    dbProperty.EnumId = property.EnumId;
                    dbProperty.Priority = property.Priority;
                    dbProperty.LargerIsBetter = property.LargerIsBetter;
                    dbProperty.Group = context.PropertiesGroups.SingleOrDefault(m => m.Id == dbProperty.GroupId);
                    dbProperty.Unit = property.Unit;
                    dbProperty.Notes = property.Notes;
                    dbProperty.DisplayAs = property.DisplayAs;
                    dbProperty.ExcelColumnName = property.ExcelColumnName;
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

        public virtual int RemovePropertyFromGroup(int propertId)
        {
            ProductsManager_DBContext context = new ProductsManager_DBContext();
            var dbProperty = context.Properties.Where(m => m.Id == propertId).FirstOrDefault();
            if (dbProperty != null)
            {
                context.Properties.Remove(dbProperty);
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
