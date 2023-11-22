using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SABFramework.Core;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.Web;
using CompetitiveAnalysis.Models;
using CompetitiveAnalysis.ProductsManagerServices;
using DConfigOS_Core.Repositories.Utilities;
namespace CompetitiveAnalysis.Actions
{
    public class CUDAdvancedPrivilegesAction : SABFramework.Core.SABAction
    {
        public int? Id { get; set; }
        [Required]
        public int CompanyId { get; set; }
        public string VisibleSections { get; set; }
        public string RelatedBrandFactoyTypes { get; set; }
        public List<int> RelatedProdutTemplatesIds { get; set; }
        public IAdvancedPrivilegesAPI AdvancedPrivilegesAPI { get; set; }
        public override async Task<SABFramework.Core.SABActionResult> PostHandler(Controller controller)
        {
            if (controller.ModelState.IsValid)
            {
                var g = new AdvancedPrivilege()
                {
                    CompanyId=CompanyId,
                    RelatedBrandFactoyTypes=RelatedBrandFactoyTypes,
                    VisibleSections=VisibleSections
                };
                var result = AdvancedPrivilegesAPI.CreatePrivilege(g, RelatedProdutTemplatesIds);
                var obj = AdvancedPrivilegesAPI.GetPrivilege(g.Id);
                switch (result)
                {
                    case ResultCodes.Succeed:
                        return Json(new { result = "true", obj = obj });
                    case ResultCodes.ObjectAlreadyExists:
                        return Json(new { result = "false", message = "Advanced privilege on the same company is already exists" });
                    default:
                        return Json(new { result = "false" });
                }
            }
            else
            {
                return Json(new { result = "false", message = "Company is required to add an advanced privilege" });
            }
        }

        public override async Task<SABFramework.Core.SABActionResult> PutHandler(Controller controller)
        {
            if (Id.HasValue && controller.ModelState.IsValid)
            {
                var g = new AdvancedPrivilege()
                {
                    Id=Id.Value,
                    CompanyId = CompanyId,
                    RelatedBrandFactoyTypes = RelatedBrandFactoyTypes,
                    VisibleSections = VisibleSections
                };
                var result = AdvancedPrivilegesAPI.UpdatePrivilege(g, RelatedProdutTemplatesIds);
                var obj = AdvancedPrivilegesAPI.GetPrivilege(g.Id);
                switch (result)
                {
                    case ResultCodes.Succeed:
                        return Json(new { result = "true", obj = obj });
                    case ResultCodes.ObjectHasntFound:
                        return Json(new { result = "false", message = "Advanced privilege hasn't been found to be updated" });
                    default:
                        return Json(new { result = "false" });
                }
            }
            else
            {
                return Json(new { result = "false", message = "Id and company id are required to update an advance privilege" });
            }
        }

        public override async Task<SABFramework.Core.SABActionResult> DeleteHandler(Controller controller)
        {
            if (Id.HasValue)
            {
                var result = AdvancedPrivilegesAPI.DeletePrivilege(Id.Value);
                switch (result)
                {
                    case ResultCodes.Succeed:
                        return Json(new { result = "true" });
                    case ResultCodes.ObjectHasntFound:
                        return Json(new { result = "false", message = "Advanced privilege hasn't been found to be deleted" });
                    default:
                        return Json(new { result = "false" });
                }
            }
            else
            {
                return Json(new { result = "false", message = "Id is required to delete an advanced privilege" });
            }
        }
    }
}
