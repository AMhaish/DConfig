using DConfigOS_Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DConfigOS_Core.Repositories.WebsiteContentServices
{
    public interface ITemplatesAPI
    {
        List<ViewTemplate> GetCompanyContextTemplates(int companyContextId);
        List<ViewTemplate> GetRootTemplates(string creatorId = null);
        List<ViewTemplate> GetTemplates(string creatorId = null);
        ViewTemplate GetTemplate(int id, string creatorId = null);
        int CreateTemplate(ViewTemplate template, string creatorId = null);
        int UpdateTemplate(ViewTemplate template, string creatorId = null);
        int UpdateTemplateLayout(int templateId, int? layoutId, string creatorId = null);
        int DeleteTemplate(int id, string creatorId = null);
        int CloneTemplate(int templateId, out ViewTemplate resultTemplate, string suffix = null, string creatorId = null, int? parentTemplateId = null);
    }
}
