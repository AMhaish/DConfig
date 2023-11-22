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
    public class CUDComparisonsAction : SABFramework.Core.SABAction 
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Tags { get; set; }
        public string BrandFactoryTypes { get; set; }
        public List<ComparisonFilter> Filters { get; set; }
        public DateTime? CreateDate_From { get; set; }
        public DateTime? CreateDate_To { get; set; }
        public DateTime? UpdateDate_From { get; set; }
        public DateTime? UpdateDate_To { get; set; }
        private IComparisonAPI ComparisonAPI { get; set; }
        public override async Task<SABFramework.Core.SABActionResult> PostHandler(Controller controller)
        {
            if (controller.ModelState.IsValid)
            {
                var g = new Comparison()
                {
                    Id = Id,
                    Name = Name,
                    Tags = Tags,
                    BrandFactoryTypes = BrandFactoryTypes,
                    CreateDate_From = CreateDate_From,
                    CreateDate_To = CreateDate_To,
                    UpdateDate_From = UpdateDate_From,
                    UpdateDate_To = UpdateDate_To
                };
                var result = ComparisonAPI.CreateComparison(g, Filters);
                var gg = ComparisonAPI.GetComparison(g.Id);
                switch (result)
                {
                    case ResultCodes.Succeed:
                        return Json(new { result = "true", obj = gg });
                    case ResultCodes.ObjectNameAlreadyUsed:
                        return Json(new { result = "false", message = "Comparison report with the same name is already exists" });
                    default:
                        return Json(new { result = "false" });
                }
            }
            else
            {
                return Json(new { result = "false", message = "Name is required to add a comparison report" });
            }
        }

        public override async Task<SABFramework.Core.SABActionResult> PutHandler(Controller controller)
        {
            if (controller.ModelState.IsValid)
            {
                var g = new Comparison()
                {
                    Id = Id,
                    Name = Name,
                    Tags = Tags,
                    BrandFactoryTypes = BrandFactoryTypes,
                    CreateDate_From = CreateDate_From,
                    CreateDate_To = CreateDate_To,
                    UpdateDate_From = UpdateDate_From,
                    UpdateDate_To = UpdateDate_To
                };
                var result = ComparisonAPI.UpdateComparison(g, Filters);
                var gg = ComparisonAPI.GetComparison(g.Id);
                switch (result)
                {
                    case ResultCodes.Succeed:
                        return Json(new { result = "true", obj = gg });
                    case ResultCodes.ObjectHasntFound:
                        return Json(new { result = "false", message = "Comparison report hasn't been found to be updated" });
                    case ResultCodes.ObjectNameAlreadyUsed:
                        return Json(new { result = "false", message = "There is already a comparison report with the same name" });
                    default:
                        return Json(new { result = "false" });
                }
            }
            else
            {
                return Json(new { result = "false", message = "Id and Name are required to update a comparison report" });
            }
        }

        public override async Task<SABFramework.Core.SABActionResult> DeleteHandler(Controller controller)
        {
            if (controller.ModelState.IsValid)
            {
                var result = ComparisonAPI.DeleteComparison(Id);
                switch (result)
                {
                    case ResultCodes.Succeed:
                        return Json(new { result = "true" });
                    case ResultCodes.ObjectHasntFound:
                        return Json(new { result = "false", message = "Comparison report hasn't been found to be deleted" });
                    default:
                        return Json(new { result = "false" });
                }
            }
            else
            {
                return Json(new { result = "false", message = "Id and Name are required to delete a comparison report" });
            }
        }
    }
}
