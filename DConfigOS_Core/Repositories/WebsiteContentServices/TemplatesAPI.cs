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
    public class TemplatesAPI : ITemplatesAPI
    {
        public virtual List<ViewTemplate> GetCompanyContextTemplates(int companyContextId)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var templates = context.ViewTemplates.Where(m => m.ContextCompanyId == companyContextId).ToList();
            return templates;
        }

        public virtual List<ViewTemplate> GetRootTemplates(string creatorId = null)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var rootTemplates = context.ViewTemplates.Where(m => !m.LayoutTemplateId.HasValue && (creatorId == null || m.CreatorId == creatorId)).ToList();
            return rootTemplates;
        }

        public virtual List<ViewTemplate> GetTemplates(string creatorId = null)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            return context.ViewTemplates.Where(m => creatorId == null || m.CreatorId == creatorId).ToList();
        }

        public virtual ViewTemplate GetTemplate(int id, string creatorId = null)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            return context.ViewTemplates.Where(m => m.Id == id && (creatorId == null || m.CreatorId == creatorId)).SingleOrDefault();
        }

        public virtual int CreateTemplate(ViewTemplate template, string creatorId = null)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var dbTemplate = context.ViewTemplates.Where(m => m.Name==template.Name && m.LayoutTemplateId==template.LayoutTemplateId && (creatorId == null || m.CreatorId == creatorId)).FirstOrDefault();
            if (dbTemplate == null)
            {
                template.IsActive = true;
                template.Path = "~" + template.Path.Replace('\\', '/');
                context.ViewTemplates.Add(template);
                context.SaveChanges();
                return ResultCodes.Succeed;
            }
            else
            {
                return ResultCodes.ObjectAlreadyExists;
            }
        }

        public virtual int UpdateTemplate(ViewTemplate template, string creatorId = null)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var dbTemplate = context.ViewTemplates.Where(m => m.Id == template.Id && (creatorId == null || m.CreatorId == creatorId)).FirstOrDefault();
            var sameTemplate = context.ViewTemplates.Where(m => m.LayoutTemplateId == template.LayoutTemplateId && m.Name==template.Name && m.Id != template.Id && (creatorId == null || m.CreatorId == creatorId)).FirstOrDefault();
            if (dbTemplate != null)
            {
                if (sameTemplate == null)
                {
                    dbTemplate.Name = template.Name;
                    dbTemplate.IsActive = template.IsActive;
                    dbTemplate.Path = "~" + template.Path.Replace('\\','/');
                    dbTemplate.ViewTypeId = template.ViewTypeId;
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

   
        public virtual int UpdateTemplateLayout(int templateId, int? layoutId, string creatorId = null)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var dbTemplate = context.ViewTemplates.Where(m => m.Id == templateId && (creatorId == null || m.CreatorId == creatorId)).FirstOrDefault();
            if (dbTemplate != null)
            {
                dbTemplate.LayoutTemplateId = layoutId;
                context.SaveChanges();
                return ResultCodes.Succeed;
            }
            else
            {
                return ResultCodes.ObjectHasntFound;
            }
        }

       
        public virtual int DeleteTemplate(int id, string creatorId = null)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var dbTemplate = context.ViewTemplates.Where(m => m.Id == id && (creatorId == null || m.CreatorId == creatorId)).FirstOrDefault();
            if (dbTemplate != null)
            {
                if(dbTemplate.TemplateContentInstances.Count <= 0)
                {
                    foreach (var s in dbTemplate.ChildrenTemplates)
                    {
                        context.ViewTemplates.Remove(s);
                    }
                    context.ViewTemplates.Remove(dbTemplate);
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

        public virtual int CloneTemplate(int templateId , out ViewTemplate resultTemplate, string suffix=null ,string creatorId = null, int? parentTemplateId = null)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var sourceTemplate = context.ViewTemplates.SingleOrDefault(m => m.Id == templateId && (creatorId == null || m.CreatorId == creatorId));
            if (sourceTemplate != null)
            {
                ViewTemplate clonedTemplate = new ViewTemplate();
                clonedTemplate.CreatorId = sourceTemplate.CreatorId;
                clonedTemplate.CreatedDate = DateTime.Now;
                clonedTemplate.IsActive = sourceTemplate.IsActive;
                clonedTemplate.IsContainer = sourceTemplate.IsContainer;
                clonedTemplate.LayoutTemplateId = sourceTemplate.LayoutTemplateId;
                if (parentTemplateId == null)
                    clonedTemplate.LayoutTemplateId = sourceTemplate.LayoutTemplateId;
                else
                    clonedTemplate.LayoutTemplateId = parentTemplateId;
                clonedTemplate.ViewTypeId = sourceTemplate.ViewTypeId;
                clonedTemplate.Name = (suffix!=null? sourceTemplate.Name + suffix: sourceTemplate.Name + " Copy");
                int counter = 1;
                while (context.ViewTemplates.Where(m => m.Name == clonedTemplate.Name && (creatorId == null || m.CreatorId == creatorId)).FirstOrDefault() != null)
                {
                    clonedTemplate.Name = sourceTemplate.Name + " Copy" + (++counter).ToString();
                }
                //Copy template code
                string source_path = SABFramework.Core.SABCoreEngine.Instance.AppPhysicalPath + sourceTemplate.Path.Replace("/", "\\").TrimStart('~');
                string newpath =  "\\Views\\PublicViews\\Templates\\" + SABFramework.PreDefinedModules.MembershipModule.MembershipProvider.Instance.ContextCompanyId + "\\" + clonedTemplate.Name + ".cshtml";
                string destination_path = SABFramework.Core.SABCoreEngine.Instance.AppPhysicalPath + newpath;
                if (File.Exists(source_path))
                {
                    File.Copy(source_path, destination_path);
                }
                clonedTemplate.Path = "~" + newpath.Replace("\\", "/");
                context.ViewTemplates.Add(clonedTemplate);
                context.SaveChanges();
                ViewTemplate c;
                foreach (ViewTemplate v in sourceTemplate.ChildrenTemplates)
                {
                    CloneTemplate(v.Id, out c, suffix, creatorId, clonedTemplate.Id);
                }
                resultTemplate = clonedTemplate;
                return ResultCodes.Succeed;
            }
            else
            {
                resultTemplate = null;
                return ResultCodes.ObjectHasntFound;
            }
        }
    }
}
