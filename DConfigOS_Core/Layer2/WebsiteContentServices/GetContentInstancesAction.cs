using System.Threading.Tasks;
using DConfigOS_Core.Repositories.WebsiteContentServices;
using System.Linq;
using Ninject;

namespace DConfigOS_Core.Layer2.WebsiteContentServices
{
    public class GetContentInstancesAction : UserActionsBase
    {
        public int Id { get; set; }
        [Inject]
        public IContentsAPI contentsAPI { get; set; }

        public override async Task<SABFramework.Core.SABActionResult> GetHandler(System.Web.Mvc.Controller controller)
        {
            if (UserIsAdministrator || UserStages.Count <=0)
            {
                return Json(contentsAPI.GetContentInstances(Id));
            }
            else
            {
                if (!controller.User.Identity.IsAuthenticated)
                {
                    return Json(contentsAPI.GetContentInstances(Id));
                }
                else
                {
                    return Json(contentsAPI.GetContentInstancesBasedOnStages(Id, UserStages.Select(m => m.Id).ToList()));
                }
            }
        }
    }
}
