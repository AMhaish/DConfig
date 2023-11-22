using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DConfigOS_Core.Layer2.ActionsModels;
using DConfigOS_Core.Repositories.IOServices;
using DConfigOS_Core.Models;
using System.IO;
using Ninject;

namespace DConfigOS_Core.Layer2.IOServices
{
    public class GetResourcesFoldersTreeAction : UserActionsBase
    {
        [Inject]
        public IFoldersAPI foldersAPI { get; set; }

        public string RootPath { get; set; }

        public override async Task<SABFramework.Core.SABActionResult> GetHandler(System.Web.Mvc.Controller controller)
        {
            var r = await foldersAPI.GetResourcesFolder(RootPath, (ExUser != null ? ExUser.ResourcesRootPath : null));
            var list = new List<IFolder>();
            var result = new List<TreeNodeModel>();
            list.Add(r);
            await BuildFoldersTreeNodes(result, list, "");
            return Json(result);
        }

        private async Task BuildFoldersTreeNodes(List<TreeNodeModel> dest, IEnumerable<IFolder> src, string parentPath)
        {
            if (src != null && src.Count() > 0)
            {
                foreach (IFolder c in src.OrderBy(m => m.Name))
                {
                    string path = parentPath + "/" + c.Name;
                    var node = new TreeNodeModel()
                    {
                        id = path,
                        obj = c,
                        text = c.Name,
                        children = new List<TreeNodeModel>(),
                        type = ContentsTreeNodeType.Container
                    };
                    dest.Add(node);
                    await BuildFoldersTreeNodes(node.children, await foldersAPI.GetChildFolders(c.Path), path);
                }
            }
        }
    }
}
