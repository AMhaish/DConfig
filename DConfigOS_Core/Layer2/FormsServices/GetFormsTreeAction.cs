using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DConfigOS_Core.Layer2.ActionsModels;
using DConfigOS_Core.Repositories.WebsiteContentServices;
using DConfigOS_Core.Models;
using Ninject;

namespace DConfigOS_Core.Layer2.FormsServices
{
    public class GetFormsTreeAction : UserActionsBase
    {
        [Inject]
        public IFormsAPI formsAPI { get; set; }

        public override async Task<SABFramework.Core.SABActionResult> GetHandler(System.Web.Mvc.Controller controller)
        {
            var contents = formsAPI.GetRootForms();
            var result = new List<TreeNodeModel>();
            BuildFormsTreeNodes(result, contents);
            return Json(result);
        }

        private void BuildFormsTreeNodes(List<TreeNodeModel> dest, IEnumerable<Form> src)
        {
            if (src != null && src.Count() > 0)
            {
                foreach (Form c in src)
                {
                    var node = new TreeNodeModel()
                    {
                        id = c.Id.ToString(),
                        obj = c,
                        text = c.Name,
                        children = new List<TreeNodeModel>()
                    };
                    dest.Add(node);
                    BuildFormsTreeNodes(node.children, c.ChildrenForms);
                }
            }
        }
    }
}
