using DConfigOS_Core.Models;
using SABFramework.PreDefinedModules.MembershipModule.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DConfigOS_Core.Repositories.WebsiteContentServices.ContentsAPI;

namespace DConfigOS_Core.Repositories.WebsiteContentServices
{
    public interface IContentsAPI : IDisposable
    {
        List<Domain> GetDomains(string creatorId = null, bool? contextFree = null);
        List<Content> GetCompanyContextContents(int companyContextId);
        List<Content> GetPagesContents(int? domainId = null, string creatorId = null);
        List<Content> GetRootContents(string creatorId = null);
        List<Content> GetQuickContents(string creatorId = null);
        List<Content> GetContentChildren(int contentId, int limit, int skip, string keyword=null, string creatorId = null);
        int GetContentChildrenCount(int contentId, string keyword=null,string creatorId = null);
        List<Content> GetContentChildrenBasedOnStages(int contentId, int limit, int skip, List<int> stagesIds, string keyword=null, string creatorId = null);
        int GetContentChildrenBasedOnStagesCount(int contentId, List<int> stagesIds, string keyword=null ,string creatorId = null);
        List<ContentInstance> GetContentInstances(int contentId, string creatorId = null);
        List<ContentInstance> GetContentInstancesBasedOnStages(int contentId, List<int> stagesIds, string creatorId = null);
        List<ContentInstance> GetContentInstancesPrevVersionsBasedOnStages(int contentId, List<int> stagesIds, string creatorId = null);
        Content GetContent(int id, string creatorId = null);
        ContentInstance GetContentInstance(int id, string creatorId = null);
        List<ViewFieldValue> GetContentInstanceFieldsValues(int id, string creatorId = null);
        List<Content> GetContnetsByViewType(int viewTypeId, int? daysAgo=null, string creatorId = null);
        int CreateContent(Content content, string creatorId = null);
        int CreateContentInstance(ContentInstance contentInstance, string creatorId = null);
        int CloneContent(int contentId, out Content resultContent, string suffix = null, string creatorId = null, int? parentContentId = null);
        int CloneContentInstance(int contentInstanceId, out ContentInstance resultContentInstance, string suffix = null, string creatorId = null, int? contentId = null);
        int StagingContentInstance(int contentInstanceId, int nextStageId, string comments, out ContentInstance resultContentInstance, string creatorId = null, int? contentId = null);
        int UpdateContent(Content content, string creatorId = null);
        int UpdateContentsOrder(List<Content> contents, string creatorId = null);
        void ResetChildrenUrls(Content content);
        int UpdateContentInstance(ContentInstance contentInstance, string creatorId = null);
        int UpdateContentParent(int contentId, int? parentId, string creatorId = null);
        int UpdateContentInstanceParentContent(int contentInstanceId, int parentContentId, string creatorId = null);
        int ActivateContent(int id, string creatorId = null);
        int DeactivateContent(int id, string creatorId = null);
        int DeleteContent(int id, string creatorId = null);
        void RecursiveDelete(DConfigOS_Core_DBContext context, Content c);
        int DeleteContentInstance(int id, string creatorId = null);
        int AddDomainToContent(int contentId, string domain, string creatorId = null);
        void UpdateContentChildrenDomain(Content c, Domain d);
        void SendEmailToUserForStage(ContentInstance content, Stage stage, ApplicationUser user);
        int RemoveDomainFromContent(int contentId, string domain, string creatorId = null);
        int ImportDataFromExcel(int contentId, string excelPath, int viewtypeId, out ImportingReport report);
        string ExportTemplateAsExcel(int contentId, int viewtypeId);
        QueryResults<Content> GetContentsByViewTypeByQuery(int viewTypeId, int limit, int skip, string keyword = null, int? daysAgo = null, int? parentId = null, string keywordField=null, string sortField=null, string sortOrder=null, string creatorId = null);
    }
}
