using DConfigOS_Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DConfigOS_Core.Repositories.WebsiteContentServices
{
    public interface IViewTypesAPI
    {
        List<ViewType> GetRootViewTypes(string creatorId = null);
        List<ViewType> GetRootViewTypesByContextId(int contextId);
        List<ViewType> GetViewTypes(string creatorId = null);
        List<ViewType> GetViewTypesByContextId(int contextId);
        ViewType GetViewType(int typeId, string creatorId = null);
        List<ViewType> GetViewTypeChildren(int typeId, string creatorId = null);
        List<ViewType> GetViewTypesChildrenByContextId(int typeId, int contextId);
        int UpdateViewTypeChildren(int typeId, List<int> childrenIds, string creatorId = null);
        int CreateViewType(ViewType type, string creatorId = null);
        int UpdateViewType(ViewType type, string creatorId = null);
        int DeleteViewType(int id, string creatorId = null);

    }
}
