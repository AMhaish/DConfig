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
using SABFramework.PreDefinedModules.MembershipModule.Models;
using SABFramework.PreDefinedModules.MembershipModule.DataProviders;
using System.Net.Mail;
using System.Net;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Ninject;

namespace DConfigOS_Core.Repositories.WebsiteContentServices
{
    public class ContentsAPI : RepositoryBase<DConfigOS_Core_DBContext>, IContentsAPI
    {
        [Inject]
        public IWebsiteSettingsAPI websiteSettingsAPI { get; set; }

        public virtual List<Domain> GetDomains(string creatorId = null, bool? contextFree = null)
        {
            DConfigOS_Core_DBContext context;
            if (contextFree.HasValue && contextFree.Value)
            {
                context = new DConfigOS_Core_DBContext(contextFree.Value);
            }
            else
            {
                context = new DConfigOS_Core_DBContext();
            }
            var domains = context.Domains.Where(m => creatorId == null || m.CreatorId == creatorId).ToList();
            return domains;
        }

        public virtual List<Content> GetCompanyContextContents(int companyContextId)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var list = context.Contents.Where(m => m.ContextCompanyId == companyContextId).ToList();
            return list;
        }

        public virtual List<Content> GetPagesContents(int? domainId = null, string creatorId = null)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var contents = context.Contents.Where(m => (domainId == null || m.DomainId == domainId) && m.ContentType == ContentTypes.Page && (creatorId == null || m.CreatorId == creatorId)).ToList();
            return contents;
        }

        public virtual List<Content> GetRootContents(string creatorId = null)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var rootContents = context.Contents.Where(m => !m.ParentId.HasValue && (creatorId == null || m.CreatorId == creatorId)).ToList();
            return rootContents;
        }

        public virtual List<Content> GetQuickContents(string creatorId = null)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var contents = context.Contents.Where(m => m.PlentyChildren == true && (creatorId == null || m.CreatorId == creatorId)).ToList();
            return contents;
        }

        public virtual List<Content> GetContentChildren(int contentId, int limit, int skip, string keyword = null, string creatorId = null)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var contents = context.Contents.Where(m => m.ParentId == contentId && (keyword == null || m.Name.Contains(keyword)) && (creatorId == null || m.CreatorId == creatorId)).OrderByDescending(m => m.CreatedDate).Skip(skip).Take(limit).ToList();
            return contents;
        }

        public virtual int GetContentChildrenCount(int contentId, string keyword = null, string creatorId = null)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var contents = context.Contents.Where(m => m.ParentId == contentId && (keyword == null || m.Name.Contains(keyword)) && (creatorId == null || m.CreatorId == creatorId)).Count();
            return contents;
        }

        public virtual List<Content> GetContentChildrenBasedOnStages(int contentId, int limit, int skip, List<int> stagesIds, string keyword = null, string creatorId = null)
        {
            if (stagesIds != null && stagesIds.Count > 0)
            {
                DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
                var contents = context.Contents.Where(
                    m => m.ParentId == contentId &&
                    m.ContentInstances.Any(n => n.StageId.HasValue && stagesIds.Contains(n.StageId.Value) && m.ContentInstances.Max(l => l.Version) == n.Version) &&
                    (keyword == null || m.Name.Contains(keyword)) &&
                    (creatorId == null || m.CreatorId == creatorId))
                    .OrderByDescending(m => m.CreatedDate)
                    .Skip(skip)
                    .Take(limit)
                    .ToList();
                return contents;
            }
            else
            {
                return new List<Content>();
            }
        }

        public virtual int GetContentChildrenBasedOnStagesCount(int contentId, List<int> stagesIds, string keyword = null, string creatorId = null)
        {
            if (stagesIds != null && stagesIds.Count > 0)
            {
                DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
                var contents = context.Contents.Where(m => m.ParentId == contentId && m.ContentInstances.Any(n => n.StageId.HasValue && stagesIds.Contains(n.StageId.Value) && m.ContentInstances.Max(l => l.Version) == n.Version) && (keyword == null || m.Name.Contains(keyword)) && (creatorId == null || m.CreatorId == creatorId)).Count();
                return contents;
            }
            else
            {
                return 0;
            }
        }

        public virtual List<ContentInstance> GetContentInstances(int contentId, string creatorId = null)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var instances = context.ContentInstances.Where(m => m.ContentId == contentId && !m.StageId.HasValue && (creatorId == null || m.CreatorId == creatorId)).ToList();
            return instances;
        }

        public virtual List<ContentInstance> GetContentInstancesBasedOnStages(int contentId, List<int> stagesIds, string creatorId = null)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            int version = context.ContentInstances.Where(m => m.ContentId == contentId && (m.StageId.HasValue && stagesIds.Contains(m.StageId.Value))).Max(m => m.Version);
            var instances = context.ContentInstances.Where(m => m.ContentId == contentId && m.Version == version && (m.StageId.HasValue && stagesIds.Contains(m.StageId.Value)) && (creatorId == null || m.CreatorId == creatorId)).ToList();
            return instances;
        }

        public virtual List<ContentInstance> GetContentInstancesPrevVersionsBasedOnStages(int contentId, List<int> stagesIds, string creatorId = null)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            int maxVersion = context.ContentInstances.Where(m => m.ContentId == contentId && (m.StageId.HasValue && stagesIds.Contains(m.StageId.Value))).Max(m => m.Version);
            var instances = context.ContentInstances.Where(m => m.ContentId == contentId && m.Version < maxVersion && (m.StageId.HasValue && stagesIds.Contains(m.StageId.Value)) && (creatorId == null || m.CreatorId == creatorId)).ToList();
            return instances;
        }

        public virtual Content GetContent(int id, string creatorId = null)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            return context.Contents.Where(m => m.Id == id && (creatorId == null || m.CreatorId == creatorId)).SingleOrDefault();
        }

        public virtual ContentInstance GetContentInstance(int id, string creatorId = null)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            return context.ContentInstances.Where(m => m.Id == id && (creatorId == null || m.CreatorId == creatorId)).SingleOrDefault();
        }

        public virtual List<ViewFieldValue> GetContentInstanceFieldsValues(int id, string creatorId = null)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var result = context.ContentInstances.Where(m => m.Id == id && (creatorId == null || m.CreatorId == creatorId)).SingleOrDefault();
            if (result != null)
            {
                return result.FieldsValues.ToList();
            }
            else
            {
                return null;
            }
        }

        public virtual List<Content> GetContnetsByViewType(int viewTypeId, int? daysAgo = null, string creatorId = null)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext(true);
            DateTime dateToCompareTo = DateTime.Now;
            if (daysAgo.HasValue)
                dateToCompareTo = DateTime.Now.AddDays(-1 * daysAgo.Value);
            return context.Contents.Where(m => m.ViewTypeId == viewTypeId && (daysAgo == null || m.CreatedDate >= dateToCompareTo) && (creatorId == null || m.CreatorId == creatorId)).ToList();
        }

        public virtual QueryResults<Content> GetContentsByViewTypeByQuery(int viewTypeId, int limit, int skip, string keyword = null, int? daysAgo = null, int? parentId = null, string keywordField = null, string sortField = null, string sortOrder = null, string creatorId = null)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext(true);
            DateTime dateToCompareTo = DateTime.Now;
            if (daysAgo.HasValue)
                dateToCompareTo = DateTime.Now.AddDays(-1 * daysAgo.Value);

            IQueryable<Content> contents = null;

            if (keywordField != null)
            {
                //To be fixed later, the problem when Id is int, it will throw an exception
                /*
                var content_property = typeof(Content).GetProperty(keywordField);
                contents = context.Contents.Where(m =>
                    m.ViewTypeId == viewTypeId &&
                    (parentId == null || m.ParentId == parentId) &&
                    (daysAgo == null || m.CreatedDate >= dateToCompareTo) &&
                    (creatorId == null || m.CreatorId == creatorId) &&
                    (keyword == null || content_property.GetValue(m) == keyword))
                    .OrderByDescending(m => m.CreatedDate);
                */
                switch (keywordField)
                {
                    case "Name":
                        contents = context.Contents.Where(m =>
                            m.ViewTypeId == viewTypeId &&
                            (parentId == null || m.ParentId == parentId) &&
                            (daysAgo == null || m.CreatedDate >= dateToCompareTo) &&
                            (creatorId == null || m.CreatorId == creatorId) &&
                            (keyword == null || m.Name.Contains(keyword)))
                            .OrderByDescending(m => m.CreatedDate);
                        break;
                    case "Id":
                        contents = context.Contents.Where(m =>
                            m.ViewTypeId == viewTypeId &&
                            (parentId == null || m.ParentId == parentId) &&
                            (daysAgo == null || m.CreatedDate >= dateToCompareTo) &&
                            (creatorId == null || m.CreatorId == creatorId) &&
                            (keyword == null || m.Id.ToString().Contains(keyword)))
                            .OrderByDescending(m => m.CreatedDate);
                        break;
                }

            }
            else
            {
                contents = context.Contents.Where(m =>
                  m.ViewTypeId == viewTypeId &&
                  (parentId == null || m.ParentId == parentId) &&
                  (daysAgo == null || m.CreatedDate >= dateToCompareTo) &&
                  (creatorId == null || m.CreatorId == creatorId) &&
                  (keyword == null || m.Name.Contains(keyword)))
                  .OrderByDescending(m => m.CreatedDate);
            }


            if (string.IsNullOrEmpty(sortField) || string.IsNullOrEmpty(sortOrder))
            {
                contents = contents.OrderBy(m => m.Priority);
            }
            else
            {
                string[] sortFieldArr = sortField.ToLower().Split(',');
                string[] sortOrderArr = sortOrder.ToLower().Split(',');

                //We should return message say there was a problem in sorting parameters
                if (sortFieldArr.Length != sortOrderArr.Length)
                    contents = contents.OrderBy(m => m.Priority);
                else
                {
                    var fieldOrderArr = sortFieldArr.Zip(sortOrderArr, (f, o) => new { field = f, order = o });

                    foreach (var item in fieldOrderArr)
                    {
                        switch (item.field)
                        {
                            case "priority":
                                if (item.order == "asc")
                                    contents = contents.OrderBy(m => m.Priority);
                                else
                                    contents = contents.OrderByDescending(m => m.Priority);
                                break;
                        }
                    }
                }
            }

            int totalCount = contents.Count();
            var items = contents
                    .Skip(skip * limit)
                    .Take(limit)
                    .ToList();

            return new QueryResults<Content>() { Items = items, TotalCount = totalCount };
        }


        public virtual int CreateContent(Content content, string creatorId = null)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var dbContent = context.Contents.Where(m => (m.UrlName != null && m.UrlName == content.UrlName) && m.ParentId == content.ParentId && (creatorId == null || m.CreatorId == creatorId)).FirstOrDefault();
            var dbParentContent = context.Contents.Where(m => m.Id == content.ParentId).FirstOrDefault();
            var cs = context.Contents.Where(m => m.ParentId == content.ParentId).Select(m => m.Priority);
            int cPrioity = (cs != null && cs.Count() > 0 ? cs.Max() : 0);
            if (dbContent == null)
            {
                if (content.ParentId.HasValue && dbParentContent != null)
                {
                    content.DomainId = dbParentContent.DomainId;
                    if (!String.IsNullOrEmpty(content.UrlName))
                    {
                        Stack<string> UrlFullCodeStack = new Stack<string>();
                        UrlFullCodeStack.Push(content.UrlName);
                        var parentContent = context.Contents.Where(m => m.Id == content.ParentId).FirstOrDefault();
                        while (parentContent.Parent != null && !String.IsNullOrEmpty(parentContent.UrlName))
                        {
                            UrlFullCodeStack.Push(parentContent.UrlName);
                            parentContent = parentContent.Parent;
                        }
                        StringBuilder UrlFullCode = new StringBuilder();
                        if (UrlFullCodeStack.Count > 0)
                        {
                            while (UrlFullCodeStack.Count > 0)
                            {
                                UrlFullCode.Append("/");
                                UrlFullCode.Append(UrlFullCodeStack.Pop());
                            }
                            content.UrlFullCode = UrlFullCode.ToString();
                        }
                    }
                    else
                    {
                        content.UrlFullCode = dbParentContent.UrlFullCode;
                    }
                }
                else if (dbParentContent == null)
                {
                    if (!content.ParentId.HasValue)
                    {
                        content.UrlFullCode = null;
                        content.DomainId = null;
                    }
                    else
                    {
                        return ResultCodes.ObjectHasntFound;
                    }
                }
                else
                {
                    if (!String.IsNullOrWhiteSpace(content.UrlName))
                    {
                        content.UrlFullCode = "/" + content.UrlName;
                    }
                    else
                    {
                        content.UrlFullCode = "/";
                    }
                }
                if (String.IsNullOrEmpty(content.Name))
                {
                    content.UrlName = content.Name.Replace(" ", "-");
                }
                content.CreateDate = DateTime.Now;
                content.Priority = cPrioity + 1;
                content.Online = content.Online;
                context.Contents.Add(content);
                context.SaveChanges();
                return ResultCodes.Succeed;
            }
            else
            {
                return ResultCodes.ObjectAlreadyExists;
            }
        }

        public virtual int CreateContentInstance(ContentInstance contentInstance, string creatorId = null)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var dbContent = context.ContentInstances.Where(m => m.Name == contentInstance.Name && m.ContentId == contentInstance.ContentId && (creatorId == null || m.CreatorId == creatorId)).FirstOrDefault();
            if (dbContent == null)
            {
                contentInstance.CreateDate = DateTime.Now;
                contentInstance.Online = contentInstance.Online;
                context.ContentInstances.Add(contentInstance);
                context.SaveChanges();
                return ResultCodes.Succeed;
            }
            else
            {
                return ResultCodes.ObjectAlreadyExists;
            }
        }

        public virtual int CloneContent(int contentId, out Content resultContent, string suffix = null, string creatorId = null, int? parentContentId = null)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var sourceContent = context.Contents.SingleOrDefault(m => m.Id == contentId && (creatorId == null || m.CreatorId == creatorId));
            if (sourceContent != null)
            {
                Content clonedContent = new Content();
                clonedContent.ContentType = sourceContent.ContentType;
                clonedContent.CreateDate = DateTime.Now;
                clonedContent.CreatorId = creatorId;
                clonedContent.DomainId = sourceContent.DomainId; // Domain id should not be cloned
                clonedContent.Name = (suffix != null ? sourceContent.Name + suffix : sourceContent.Name + " Copy");
                clonedContent.Stage = sourceContent.Stage;
                clonedContent.DueDate = sourceContent.DueDate;
                int counter = 1;
                int? parentIdToCompare = (parentContentId.HasValue ? parentContentId.Value : sourceContent.ParentId);
                while (context.Contents.Where(m => m.ParentId == parentIdToCompare && m.Name == clonedContent.Name && (creatorId == null || m.CreatorId == creatorId)).FirstOrDefault() != null)
                {
                    clonedContent.Name = sourceContent.Name + " Copy" + (++counter).ToString();
                }
                clonedContent.Online = sourceContent.Online;
                if (!parentContentId.HasValue)
                    clonedContent.ParentId = sourceContent.ParentId;
                else
                    clonedContent.ParentId = parentContentId;
                clonedContent.PlentyChildren = sourceContent.PlentyChildren;
                clonedContent.Priority = sourceContent.Priority + 1;
                if (sourceContent.UrlName != null)
                {
                    clonedContent.UrlName = (suffix != null ? sourceContent.UrlName + suffix : sourceContent.UrlName + "Copy");
                    counter = 1;
                    while (context.Contents.Where(m => m.UrlName == clonedContent.UrlName && (creatorId == null || m.CreatorId == creatorId)).FirstOrDefault() != null)
                    {
                        clonedContent.UrlName = sourceContent.UrlName + "Copy" + (++counter).ToString();
                    }
                    clonedContent.UrlFullCode = sourceContent.UrlFullCode.Remove(sourceContent.UrlFullCode.Length - sourceContent.UrlName.Length, sourceContent.UrlName.Length) + clonedContent.UrlName;
                }
                clonedContent.ViewTypeId = sourceContent.ViewTypeId;
                context.Contents.Add(clonedContent);
                context.SaveChanges();
                ContentInstance ci;
                Content c;
                foreach (Content content in sourceContent.ChildrenContents)
                {
                    CloneContent(content.Id, out c, "", creatorId, clonedContent.Id);
                }
                foreach (ContentInstance ContentInstance in sourceContent.ContentInstances)
                {
                    CloneContentInstance(ContentInstance.Id, out ci, "", creatorId, clonedContent.Id);
                }
                resultContent = clonedContent;
                return ResultCodes.Succeed;
            }
            else
            {
                resultContent = null;
                return ResultCodes.ObjectHasntFound;
            }

        }

        public virtual int CloneContentInstance(int contentInstanceId, out ContentInstance resultContentInstance, string suffix = null, string creatorId = null, int? contentId = null)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var sourceContentInstance = context.ContentInstances.SingleOrDefault(m => m.Id == contentInstanceId && (creatorId == null || m.CreatorId == creatorId));
            if (sourceContentInstance != null)
            {
                ContentInstance clonedContentInstance = new ContentInstance();
                clonedContentInstance.ContentId = (contentId.HasValue ? contentId.Value : sourceContentInstance.ContentId);
                clonedContentInstance.CreateDate = DateTime.Now;
                clonedContentInstance.CreatorId = creatorId;
                clonedContentInstance.DownloadName = sourceContentInstance.DownloadName;
                clonedContentInstance.DownloadPath = sourceContentInstance.DownloadPath;
                clonedContentInstance.Language = sourceContentInstance.Language;
                clonedContentInstance.MetaDescription = sourceContentInstance.MetaDescription;
                clonedContentInstance.MetaKeywords = sourceContentInstance.MetaKeywords;
                clonedContentInstance.Name = (suffix != null ? sourceContentInstance.Name + suffix : sourceContentInstance.Name + " Copy");
                clonedContentInstance.Data = sourceContentInstance.Data;
                int counter = 1;
                while (context.ContentInstances.Where(m => m.Name == clonedContentInstance.Name && (creatorId == null || m.CreatorId == creatorId)).FirstOrDefault() != null)
                {
                    clonedContentInstance.Name = sourceContentInstance.Name + " Copy" + (++counter).ToString();
                }
                clonedContentInstance.Online = sourceContentInstance.Online;
                clonedContentInstance.RedirectUrl = sourceContentInstance.RedirectUrl;
                clonedContentInstance.Title = sourceContentInstance.Title;
                clonedContentInstance.Version = sourceContentInstance.Version;
                clonedContentInstance.ViewTemplateId = sourceContentInstance.ViewTemplateId;
                context.ContentInstances.Add(clonedContentInstance);
                context.SaveChanges();
                clonedContentInstance.FieldsValues = new List<ViewFieldValue>();
                foreach (var prop in sourceContentInstance.FieldsValues)
                {
                    clonedContentInstance.FieldsValues.Add(
                        new ViewFieldValue()
                        {
                            ContentId = clonedContentInstance.Id,
                            FieldId = prop.FieldId,
                            Value = prop.Value
                        }
                    );
                }
                context.SaveChanges();
                resultContentInstance = clonedContentInstance;
                return ResultCodes.Succeed;
            }
            else
            {
                resultContentInstance = null;
                return ResultCodes.ObjectHasntFound;
            }
        }

        public virtual int StagingContentInstance(int contentInstanceId, int nextStageId, string comments, out ContentInstance resultContentInstance, string creatorId = null, int? contentId = null)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var sourceContentInstance = context.ContentInstances.SingleOrDefault(m => m.Id == contentInstanceId);

            if (sourceContentInstance != null)
            {
                ContentInstance stagedContentInstance = new ContentInstance();
                stagedContentInstance.ContentId = (contentId.HasValue ? contentId.Value : sourceContentInstance.ContentId);
                stagedContentInstance.CreateDate = DateTime.Now;
                stagedContentInstance.CreatorId = creatorId;
                stagedContentInstance.DownloadName = sourceContentInstance.DownloadName;
                stagedContentInstance.DownloadPath = sourceContentInstance.DownloadPath;
                stagedContentInstance.Language = sourceContentInstance.Language;
                stagedContentInstance.MetaDescription = sourceContentInstance.MetaDescription;
                stagedContentInstance.MetaKeywords = sourceContentInstance.MetaKeywords;
                stagedContentInstance.Name = sourceContentInstance.Name;
                stagedContentInstance.Version = sourceContentInstance.Version + 1;
                stagedContentInstance.Name = sourceContentInstance.Name;
                stagedContentInstance.StageId = nextStageId;
                stagedContentInstance.RedirectUrl = sourceContentInstance.RedirectUrl;
                stagedContentInstance.Title = sourceContentInstance.Title;
                stagedContentInstance.ViewTemplateId = sourceContentInstance.ViewTemplateId;
                stagedContentInstance.Comments = comments;
                context.ContentInstances.Add(stagedContentInstance);
                context.SaveChanges();
                stagedContentInstance.FieldsValues = new List<ViewFieldValue>();
                foreach (var prop in sourceContentInstance.FieldsValues)
                {
                    stagedContentInstance.FieldsValues.Add(
                        new ViewFieldValue()
                        {
                            ContentId = stagedContentInstance.Id,
                            FieldId = prop.FieldId,
                            Value = prop.Value
                        }
                    );
                }
                context.SaveChanges();
                resultContentInstance = stagedContentInstance;
                // Sending notifications
                var dbStage = context.Stages.Where(m => m.Id == nextStageId).FirstOrDefault();
                var stageRoles = dbStage.Roles.Select(r => r.Id).ToList();
                List<ApplicationUser> potenialUsers = context.Users.Where(x => x.Roles.Any(r => stageRoles.Contains(r.RoleId))).Distinct().ToList();
                foreach (var user in potenialUsers)
                {
                    //SendEmailToUserForStage(resultContentInstance, dbStage, user);
                }
                return ResultCodes.Succeed;
            }
            else
            {
                resultContentInstance = null;
                return ResultCodes.ObjectHasntFound;
            }
        }

        public virtual int UpdateContent(Content content, string creatorId = null)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var dbContent = context.Contents.Where(m => m.Id == content.Id && (creatorId == null || m.CreatorId == creatorId)).FirstOrDefault();
            var dbParentContent = context.Contents.Where(m => m.Id == content.ParentId).FirstOrDefault();
            var sameContent = context.Contents.Where(m => m.Name == content.Name && (m.UrlName != null && m.UrlName == content.UrlName) && m.ParentId == content.ParentId && m.Id != content.Id && (creatorId == null || m.CreatorId == creatorId)).FirstOrDefault();
            if (dbContent != null)
            {
                if (sameContent == null)
                {

                    if (content.ParentId.HasValue)
                    {
                        if (!String.IsNullOrEmpty(content.UrlName))
                        {
                            Stack<string> UrlFullCodeStack = new Stack<string>();
                            UrlFullCodeStack.Push(content.UrlName);
                            var parentContent = context.Contents.Where(m => m.Id == content.ParentId).FirstOrDefault();
                            while (parentContent != null)
                            {
                                if (parentContent.UrlName != null)
                                {
                                    UrlFullCodeStack.Push(parentContent.UrlName);
                                }
                                parentContent = parentContent.Parent;
                            }
                            StringBuilder UrlFullCode = new StringBuilder();
                            while (UrlFullCodeStack.Count > 0)
                            {
                                UrlFullCode.Append("/");
                                UrlFullCode.Append(UrlFullCodeStack.Pop());

                            }
                            content.UrlFullCode = UrlFullCode.ToString();
                        }
                        else
                        {
                            content.UrlFullCode = dbParentContent.UrlFullCode;
                        }
                    }
                    else
                    {
                        if (!String.IsNullOrWhiteSpace(content.UrlName))
                        {
                            content.UrlFullCode = "/" + content.UrlName;
                        }
                        else
                        {
                            content.UrlFullCode = "/";
                        }
                    }
                    dbContent.UrlFullCode = content.UrlFullCode;
                    dbContent.UrlName = content.UrlName;
                    ResetChildrenUrls(dbContent);
                    dbContent.Name = content.Name;
                    dbContent.Online = content.Online;
                    dbContent.ParentId = content.ParentId;
                    dbContent.PlentyChildren = content.PlentyChildren;
                    dbContent.StageId = content.StageId;
                    dbContent.DueDate = content.DueDate;
                    dbContent.Priority = content.Priority;
                    //dbContent.ViewTypeId = content.ViewTypeId;
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

        public virtual int UpdateContentsOrder(List<Content> contents, string creatorId = null)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            Dictionary<int, int> ids = contents.ToDictionary(m => m.Id, m => m.Priority);
            var dbContents = context.Contents.Where(m => ids.Keys.Contains(m.Id) && (creatorId == null || m.CreatorId == creatorId));
            if (dbContents != null && dbContents.Count() > 0)
            {
                foreach (Content c in dbContents)
                {
                    c.Priority = ids[c.Id];
                }
                context.SaveChanges();
                return ResultCodes.Succeed;
            }
            else
            {
                return ResultCodes.ObjectHasntFound;
            }
        }

        public virtual void ResetChildrenUrls(Content content)
        {
            if (content != null)
            {
                foreach (Content c in content.ChildrenContents)
                {
                    if (!String.IsNullOrEmpty(c.UrlName))
                    {
                        if (content.UrlFullCode != "/")
                        {
                            c.UrlFullCode = content.UrlFullCode + "/" + c.UrlName;
                        }
                        else
                        {
                            c.UrlFullCode = content.UrlFullCode + c.UrlName;
                        }
                    }
                    else
                    {
                        c.UrlFullCode = content.UrlFullCode;
                    }
                    ResetChildrenUrls(c);
                }
            }
        }

        public virtual int UpdateContentInstance(ContentInstance contentInstance, string creatorId = null)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var dbContent = context.ContentInstances.Where(m => m.Id == contentInstance.Id && (creatorId == null || m.CreatorId == creatorId)).FirstOrDefault();
            var sameContent = context.ContentInstances.Where(m => m.Name == contentInstance.Name && m.ContentId == contentInstance.ContentId && m.Id != contentInstance.Id && m.Version == contentInstance.Version && (creatorId == null || m.CreatorId == creatorId)).FirstOrDefault();
            if (dbContent != null)
            {
                //To-Do: this was sameContent == null which was returning object not found
                if (sameContent == null)
                {
                    dbContent.Name = contentInstance.Name;
                    dbContent.Online = contentInstance.Online;
                    dbContent.ViewTemplateId = contentInstance.ViewTemplateId;
                    dbContent.Language = contentInstance.Language;
                    dbContent.MetaDescription = contentInstance.MetaDescription;
                    dbContent.MetaKeywords = contentInstance.MetaKeywords;
                    dbContent.Title = contentInstance.Title;
                    dbContent.Version = contentInstance.Version;
                    dbContent.RedirectUrl = contentInstance.RedirectUrl;
                    dbContent.DownloadPath = contentInstance.DownloadPath;
                    dbContent.DownloadName = contentInstance.DownloadName;
                    dbContent.StageId = contentInstance.StageId;
                    if (contentInstance.Data != null)
                        dbContent.Data = contentInstance.Data;
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

        public virtual int UpdateContentParent(int contentId, int? parentId, string creatorId = null)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var dbContent = context.Contents.Where(m => m.Id == contentId && (creatorId == null || m.CreatorId == creatorId)).FirstOrDefault();
            if (dbContent != null)
            {
                dbContent.ParentId = parentId;
                context.SaveChanges();
                return ResultCodes.Succeed;
            }
            else
            {
                return ResultCodes.ObjectHasntFound;
            }
        }

        public virtual int UpdateContentInstanceParentContent(int contentInstanceId, int parentContentId, string creatorId = null)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var dbContent = context.ContentInstances.Where(m => m.Id == contentInstanceId && (creatorId == null || m.CreatorId == creatorId)).FirstOrDefault();
            if (dbContent != null)
            {
                dbContent.ContentId = parentContentId;
                context.SaveChanges();
                return ResultCodes.Succeed;
            }
            else
            {
                return ResultCodes.ObjectHasntFound;
            }
        }

        public virtual int ActivateContent(int id, string creatorId = null)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var dbContent = context.Contents.Where(m => m.Id == id && (creatorId == null || m.CreatorId == creatorId)).FirstOrDefault();
            if (dbContent != null)
            {
                if (!dbContent.Online)
                {
                    dbContent.Online = true;
                    context.SaveChanges();
                    return ResultCodes.Succeed;
                }
                else
                {
                    return ResultCodes.ObjectAlreadyUpdated;
                }
            }
            else
            {
                return ResultCodes.ObjectHasntFound;
            }
        }

        public virtual int DeactivateContent(int id, string creatorId = null)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var dbContent = context.Contents.Where(m => m.Id == id && (creatorId == null || m.CreatorId == creatorId)).FirstOrDefault();
            if (dbContent != null)
            {
                if (dbContent.Online)
                {
                    dbContent.Online = false;
                    context.SaveChanges();
                    return ResultCodes.Succeed;
                }
                else
                {
                    return ResultCodes.ObjectAlreadyUpdated;
                }
            }
            else
            {
                return ResultCodes.ObjectHasntFound;
            }
        }

        public virtual int DeleteContent(int id, string creatorId = null)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var dbContent = context.Contents.Where(m => m.Id == id && (creatorId == null || m.CreatorId == creatorId)).FirstOrDefault();
            if (dbContent != null)
            {
                RecursiveDelete(context, dbContent);
                context.SaveChanges();
                return ResultCodes.Succeed;
            }
            else
            {
                return ResultCodes.ObjectHasntFound;
            }
        }

        public virtual void RecursiveDelete(DConfigOS_Core_DBContext context, Content c)
        {
            if (c.ChildrenContents != null && c.ChildrenContents.Count > 0)
            {
                foreach (Content cc in c.ChildrenContents.ToList())
                {
                    RecursiveDelete(context, cc);
                }
            }
            context.Contents.Remove(c);
        }

        public virtual int DeleteContentInstance(int id, string creatorId = null)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var dbContent = context.ContentInstances.Where(m => m.Id == id && (creatorId == null || m.CreatorId == creatorId)).FirstOrDefault();
            if (dbContent != null)
            {
                context.ContentInstances.Remove(dbContent);
                context.SaveChanges();
                Providers.DataContexts.DConfigOS_Core_ElasticSearchContext elasticContext = new Providers.DataContexts.DConfigOS_Core_ElasticSearchContext();
                elasticContext.ContentInstancesValues.UnIndex(dbContent);
                return ResultCodes.Succeed;
            }
            else
            {
                return ResultCodes.ObjectHasntFound;
            }
        }

        public virtual int AddDomainToContent(int contentId, string domain, string creatorId = null)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var dbContent = context.Contents.Where(m => m.Id == contentId && (creatorId == null || m.CreatorId == creatorId)).FirstOrDefault();
            var dbDomain = context.Domains.Where(m => m.DomainAliases.Contains(";" + domain + ";") && (creatorId == null || m.CreatorId == creatorId)).FirstOrDefault();
            if (dbContent != null)
            {
                if (dbContent.Domain == null)
                {
                    if (dbDomain == null || dbContent.DomainId != dbDomain.Id)
                    {
                        Domain d = new Domain();
                        d.DomainAliases = ";" + domain + ";";
                        d.CreatorId = creatorId;
                        dbContent.Domain = d;
                        context.Domains.Add(d);
                        UpdateContentChildrenDomain(dbContent, d);
                    }
                    else
                    {
                        dbContent.Domain = dbDomain;
                        UpdateContentChildrenDomain(dbContent, dbDomain);
                    }
                    context.SaveChanges();
                    return ResultCodes.Succeed;
                }
                else
                {
                    if (dbDomain == null)
                    {
                        dbContent.Domain.DomainAliases += domain + ";";
                        context.SaveChanges();
                        return ResultCodes.Succeed;
                    }
                    else
                    {
                        return ResultCodes.ObjectNameAlreadyUsed;
                    }
                }
            }
            else
            {
                return ResultCodes.ObjectHasntFound;
            }
        }

        public virtual void UpdateContentChildrenDomain(Content c, Domain d)
        {
            foreach (Content cc in c.ChildrenContents)
            {
                cc.Domain = d;
                UpdateContentChildrenDomain(cc, d);
            }
        }

        public virtual void SendEmailToUserForStage(ContentInstance content, Stage stage, ApplicationUser user)
        {
            try
            {
                var smtp_host = websiteSettingsAPI.Get(WebsiteSetting.SMTP_Host);
                var smtp_port = websiteSettingsAPI.Get(WebsiteSetting.SMTP_Port);
                var smtp_username = websiteSettingsAPI.Get(WebsiteSetting.SMTP_UserName);
                var smtp_password = websiteSettingsAPI.Get(WebsiteSetting.SMTP_Password);
                var smtp_from = websiteSettingsAPI.Get(WebsiteSetting.SMTP_From);
                SmtpClient sc = new SmtpClient(smtp_host, Convert.ToInt32(smtp_port));
                sc.Credentials = new NetworkCredential(smtp_username, smtp_password);
                ServicePointManager.ServerCertificateValidationCallback += (o, c, ch, er) => true;

                MailMessage m = new MailMessage(smtp_from, user.Email);
                m.Body = "The content with the name (" + content.Name + ") has been moved under " + stage.Name + " staging phase";
                m.Subject = "Content Staging Notification";
                sc.Send(m);
            }
            catch (Exception ex)
            {
                SABFramework.Core.SABCoreEngine.Instance.ErrorHandler.ProcessError("Error in sending stage notification", ex);
            }
        }

        public virtual int RemoveDomainFromContent(int contentId, string domain, string creatorId = null)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var dbContent = context.Contents.Where(m => m.Id == contentId && (creatorId == null || m.CreatorId == creatorId)).FirstOrDefault();
            if (dbContent != null && dbContent.Domain != null)
            {
                if (dbContent.Domain.DomainAliases == ";" + domain + ";")
                {
                    var dbDomain = context.Domains.Where(m => m.DomainAliases.Contains(";" + domain + ";")).FirstOrDefault();
                    dbContent.Domain = null;
                    UpdateContentChildrenDomain(dbContent, null);
                    context.Domains.Remove(dbDomain);
                }
                else
                {
                    dbContent.Domain.DomainAliases = dbContent.Domain.DomainAliases.Replace(";" + domain + ";", ";");
                }
                context.SaveChanges();
                return ResultCodes.Succeed;
            }
            else
            {
                return ResultCodes.ObjectHasntFound;
            }
        }

        public virtual int ImportDataFromExcel(int contentId, string excelPath, int viewtypeId, out ImportingReport report)
        {
            report = new ImportingReport();
            report.FailedDic = new Dictionary<string, string>();

            /*string Path = excelPath.Replace(SABFramework.Core.SABCoreEngine.Instance.Settings[SABFramework.Core.SABSettings.SABSettings_CDN], "").Replace("\\", "/");
            string _excelPath = "";
            if (SABFramework.Core.SABCoreEngine.Instance.Settings.ContainsKey(SABFramework.Core.SABSettings.SABSettings_CDN))
            {
                _excelPath = (SABFramework.Core.SABCoreEngine.Instance.AppPhysicalPath + Path).Replace("/", "\\");
            }
            else
            {
                _excelPath = Path;
            }*/
            var request = WebRequest.Create(excelPath);
            var response = request.GetResponse();
            var memoryStream = new MemoryStream();
            using (var networkStream = response.GetResponseStream())
            {
                if (networkStream != null)
                {
                    // Copy the network stream to an in-memory variable
                    networkStream.CopyTo(memoryStream);
                    // Move the position of the stream to the beginning
                    memoryStream.Seek(0, SeekOrigin.Begin);
                }
            }

            Dictionary<string, Content> contents = new Dictionary<string, Content>();

            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var content = context.Contents.Where(m => m.Id == contentId).FirstOrDefault();
            if (content != null)
            {
                var indexCounter = 1;//Start from the second row for excel header
                try
                {
                    //SpreadsheetDocument excel = SpreadsheetDocument.Open(_excelPath, false);
                    SpreadsheetDocument excel = SpreadsheetDocument.Open(memoryStream, false);
                    //var excel = new ExcelQueryFactory(SABFramework.Core.SABCoreEngine.Instance.AppPhysicalPath + excelPath.Replace("/","\\"));
                    //var workSheets = excel.GetWorksheetNames().ToList();
                    //var columns = excel.GetColumnNames(workSheets[0]).ToList();
                    var columnsDic = new Dictionary<string, int>();
                    IEnumerable<Sheet> sheets = excel.WorkbookPart.Workbook.GetFirstChild<Sheets>().Elements<Sheet>();
                    string relationshipId = sheets.First().Id.Value;
                    WorksheetPart worksheetPart = (WorksheetPart)excel.WorkbookPart.GetPartById(relationshipId);
                    Worksheet workSheet = worksheetPart.Worksheet;
                    SheetData sheetData = workSheet.GetFirstChild<SheetData>();
                    IEnumerable<Row> rows = sheetData.Descendants<Row>();
                    int counter = 0;

                    foreach (Cell cell in rows.ElementAt(0))
                    {
                        if (cell != null && cell.CellValue != null && !string.IsNullOrEmpty(GetCellValue(excel, cell)))
                        {
                            columnsDic.Add(GetCellValue(excel, cell).ToLower(), counter);
                            counter++;
                        }
                    }
                    //for (int i = 0; i < columns.Count; i++)
                    //{
                    //    columnsDic.Add(columns[i], i);
                    //}
                    //var excelProducts = from p in excel.Worksheet(workSheets[0]) select p;

                    string propertyThatHasError;
                    foreach (var row in rows)
                    {
                        propertyThatHasError = "";
                        if (indexCounter == 1) { indexCounter++; continue; }

                        try
                        {
                            bool isExistContent = false;
                            string id = "";
                            Content c = null;
                            ContentInstance ci = new ContentInstance();
                            ci.CreateDate = DateTime.Now;
                            ci.Online = true;
                            //This is wrong, as blank cells is skipped and data are stacked with wrong index
                            //var valuesList = pro.Descendants<Cell>().ToList();                       
                            IEnumerable<Cell> valuesList = SpreedsheetHelper.GetRowCells(row);

                            if (columnsDic.ContainsKey("id"))
                            {
                                var cell = valuesList.ElementAt(columnsDic["id"]);
                                if (cell.CellValue != null)
                                    id = GetCellValue(excel, cell);

                                if (contents.ContainsKey(id))
                                {
                                    c = contents[id];
                                    isExistContent = true;
                                    ci.Online = c.Online;
                                }
                                else
                                {
                                    c = new Content();
                                    c.CreatedDate = DateTime.Now;
                                    c.Online = true;
                                    c.DomainId = content.DomainId;
                                    c.CreatorId = content.CreatorId;
                                }
                            }
                            else
                            {
                                propertyThatHasError = "Id";
                                throw new Exception("Content Id couldn't be found");
                            }

                            if (columnsDic.ContainsKey("name"))
                            {
                                var cell = valuesList.ElementAt(columnsDic["name"]);
                                if (cell.CellValue != null)
                                {
                                    string name = GetCellValue(excel, cell);

                                    if (!isExistContent)
                                        c.Name = name;
                                    ci.Name = name;
                                }
                            }
                            else
                            {
                                propertyThatHasError = "Name";
                                throw new Exception("Content name couldn't be found");
                            }

                            if (columnsDic.ContainsKey("urlname"))
                            {
                                var cell = valuesList.ElementAt(columnsDic["urlname"]);
                                if (cell.CellValue != null)
                                {
                                    string urlname = GetCellValue(excel, cell);

                                    if (!isExistContent)
                                        c.UrlName = urlname;

                                }
                            }
                            else
                            {
                                propertyThatHasError = "UrlName";
                                throw new Exception("Content urlname couldn't be found");
                            }

                            if (columnsDic.ContainsKey("title"))
                            {
                                var cell = valuesList.ElementAt(columnsDic["title"]);
                                if (cell.CellValue != null)
                                {
                                    string title = GetCellValue(excel, cell);

                                    ci.Title = title;
                                }
                            }
                            else
                            {
                                propertyThatHasError = "Title";
                                throw new Exception("Content Instance title couldn't be found");
                            }

                            if (columnsDic.ContainsKey("description"))
                            {
                                var cell = valuesList.ElementAt(columnsDic["description"]);
                                if (cell.CellValue != null)
                                {
                                    string description = GetCellValue(excel, cell);

                                    ci.MetaDescription = description;
                                }
                            }
                            else
                            {
                                propertyThatHasError = "Description";
                                throw new Exception("Content Instance description couldn't be found");
                            }

                            if (columnsDic.ContainsKey("language"))
                            {
                                var cell = valuesList.ElementAt(columnsDic["language"]);
                                if (cell.CellValue != null)
                                {
                                    string language = GetCellValue(excel, cell);

                                    ci.Language = language;
                                }
                            }
                            else
                            {
                                propertyThatHasError = "Language";
                                throw new Exception("Content Instance Language couldn't be found");
                            }

                            if (columnsDic.ContainsKey("templateid"))
                            {
                                var cell = valuesList.ElementAt(columnsDic["templateid"]);
                                if (cell.CellValue != null)
                                {
                                    string templateIdUnparsed = GetCellValue(excel, cell);
                                    int templateId;
                                    if (int.TryParse(templateIdUnparsed, out templateId))
                                    {
                                        ci.ViewTemplateId = templateId;
                                    }
                                    else
                                    {
                                        throw new Exception("Content Instance Template id is not parsed correctly");
                                    }
                                }
                            }

                            var viewType = context.ViewTypes.Where(x => x.Id == viewtypeId).SingleOrDefault();
                            List<ViewFieldValue> contentInstanceFields = new List<ViewFieldValue>();

                            foreach (var viewfield in viewType.ViewFields)
                            {
                                try
                                {
                                    propertyThatHasError = viewfield.Name;
                                    if (!String.IsNullOrEmpty(viewfield.ColumnName) && columnsDic.ContainsKey(viewfield.ColumnName.ToLower()))
                                    {
                                        var cell = valuesList.ElementAt(columnsDic[viewfield.ColumnName.ToLower()]);
                                        if (cell.CellValue != null)
                                        {
                                            contentInstanceFields.Add(new ViewFieldValue() { FieldId = viewfield.Id, Value = GetCellValue(excel, cell) });
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    throw new Exception("Problem in a field " + viewfield.Name + " " + ex.Message);
                                }
                            }

                            ci.FieldsValues = new List<ViewFieldValue>();
                            ci.FieldsValues = contentInstanceFields;
                            if (!isExistContent)
                            {
                                Content newContent = context.Contents.Add(c);
                                newContent.ViewTypeId = viewType.Id;
                                contents.Add(id, newContent);
                                content.ChildrenContents.Add(newContent);
                                ci.Content = newContent;
                                ci.ContentId = newContent.Id;
                            }
                            else
                            {
                                ci.Content = c;
                                ci.ContentId = c.Id;
                            }
                            context.ContentInstances.Add(ci);
                            context.SaveChanges();
                            report.ImportedCount++;
                        }
                        catch (Exception ex)
                        {
                            report.FailedCount++;
                            report.FailedDic.Add(indexCounter.ToString(), "Property (" + propertyThatHasError + ") " + ex.Message);
                        }
                        finally
                        {
                            indexCounter++;
                        }

                    }
                    return ResultCodes.Succeed;
                }
                catch (Exception ex)
                {
                    report.FailedCount++;
                    report.FailedDic.Add(indexCounter.ToString(), " " + ex.Message);

                    return ResultCodes.ObjectNameAlreadyUsed;
                }
            }
            else
            {
                return ResultCodes.ObjectHasntFound;
            }
        }

        public virtual string ExportTemplateAsExcel(int contentId, int viewtypeId)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var content = context.Contents.Where(m => m.Id == contentId).FirstOrDefault();
            if (content != null)
            {
                var directory = Directory.CreateDirectory(SABFramework.Core.SABCoreEngine.Instance.AppPhysicalPath + "\\temp");

                string guid = Guid.NewGuid().ToString();
                string excelPath = directory.FullName + "\\" + guid + ".xlsx";
                string virtualPath = "~\\temp\\" + guid + ".xlsx";
                using (SpreadsheetDocument excel = SpreadsheetDocument.Create(excelPath, DocumentFormat.OpenXml.SpreadsheetDocumentType.Workbook))
                {
                    // Add a WorkbookPart to the document.
                    WorkbookPart workbookPart = excel.AddWorkbookPart();
                    workbookPart.Workbook = new Workbook();
                    // Add a WorksheetPart to the WorkbookPart.
                    WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                    worksheetPart.Worksheet = new Worksheet(new SheetData());

                    var sheets = workbookPart.Workbook.AppendChild(new Sheets());

                    Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Template" };
                    sheets.Append(sheet);

                    // Get the sheetData cell table.
                    SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

                    // Add a row to the cell table.
                    Row row;
                    row = new Row() { RowIndex = 1 };
                    sheetData.Append(row);

                    Cell id = new Cell() { CellValue = new CellValue("Id") };
                    id.DataType = new DocumentFormat.OpenXml.EnumValue<CellValues>(CellValues.String);
                    sheetData.First().AppendChild(id);


                    Cell name = new Cell() { CellValue = new CellValue("Name") };
                    name.DataType = new DocumentFormat.OpenXml.EnumValue<CellValues>(CellValues.String);
                    sheetData.First().AppendChild(name);

                    Cell urlname = new Cell() { CellValue = new CellValue("UrlName") };
                    urlname.DataType = new DocumentFormat.OpenXml.EnumValue<CellValues>(CellValues.String);
                    sheetData.First().AppendChild(urlname);

                    Cell title = new Cell() { CellValue = new CellValue("Title") };
                    title.DataType = new DocumentFormat.OpenXml.EnumValue<CellValues>(CellValues.String);
                    sheetData.First().AppendChild(title);


                    Cell description = new Cell() { CellValue = new CellValue("Description") };
                    description.DataType = new DocumentFormat.OpenXml.EnumValue<CellValues>(CellValues.String);
                    sheetData.First().AppendChild(description);


                    Cell language = new Cell() { CellValue = new CellValue("Language") };
                    language.DataType = new DocumentFormat.OpenXml.EnumValue<CellValues>(CellValues.String);
                    sheetData.First().AppendChild(language);

                    var viewType = context.ViewTypes.Where(x => x.Id == viewtypeId).SingleOrDefault();
                    foreach (var viewfield in viewType.ViewFields)
                    {
                        if (viewfield != null && !string.IsNullOrEmpty(viewfield.ColumnName))
                        {
                            Cell viewfieldcell = new Cell() { CellValue = new CellValue(viewfield.ColumnName) };
                            viewfieldcell.DataType = new DocumentFormat.OpenXml.EnumValue<CellValues>(CellValues.String);
                            sheetData.First().AppendChild(viewfieldcell);
                        }
                    }


                    workbookPart.Workbook.Save();
                    excel.Close();
                    return virtualPath;
                }

            }
            return "";
        }

        public class ImportingReport
        {
            public int ImportedCount { get; set; }
            public int FailedCount { get; set; }
            public Dictionary<string, string> FailedDic { get; set; }
        }

        private string GetCellValue(SpreadsheetDocument document, Cell cell)
        {
            SharedStringTablePart stringTablePart = document.WorkbookPart.SharedStringTablePart;
            string value = cell.CellValue.InnerXml;

            if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
            {
                return stringTablePart.SharedStringTable.ChildElements[Int32.Parse(value)].InnerText;
            }
            else
            {
                return value;
            }
        }

        public List<Content> GetContentChildrenBasedOnStages(int contentId, int limit, int skip, List<int> stagesIds, string creatorId = null)
        {
            throw new NotImplementedException();
        }

        public int GetContentChildrenBasedOnStagesCount(int contentId, List<int> stagesIds, string creatorId = null)
        {
            throw new NotImplementedException();
        }
    }
}
