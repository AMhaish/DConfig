using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DConfigOS_Core.Layer2.ActionsModels;
using CompetitiveAnalysis.ProductsManagerServices;
using CompetitiveAnalysis.Models;
using Ninject;

namespace CompetitiveAnalysis.Actions
{
    public class RenderProductDetailsAction : SABFramework.Core.SABAction
    {
        [Inject]
        public IProductsAPI productsAPI { get; set; }
        [Inject]
        public IPropertiesAPI propertiesAPI { get; set; }

        public int Id { get; set; }
        public string PropertiesIds { get; set; }
        public override async Task<SABFramework.Core.SABActionResult> GetHandler(System.Web.Mvc.Controller controller)
        {
            var product = productsAPI.GetProduct(Id);
            List<int> propIds = new List<int>();
            if (!String.IsNullOrEmpty(PropertiesIds))
            {
                var props = PropertiesIds.Split(',');
                foreach (string p in props)
                {
                    int id;
                    if (int.TryParse(p, out id))
                    {
                        propIds.Add(id);
                    }
                }
            }
            var groups = propertiesAPI.GetPropertiesGroups();
            List<PropertiesGroup> model = new List<PropertiesGroup>();
            PropertiesGroup currentGroup = null;
            bool groupExists;
            if (product != null)
            {
                foreach (PropertiesGroup g in groups)
                {
                    groupExists = false;
                    foreach (ProductPropertyValue p in product.PropertiesValues)
                    {
                        if (g.Id == p.Property.GroupId && (propIds.Count <= 0 || propIds.Contains(p.PropertyId)))
                        {
                            if (!groupExists)
                            {
                                groupExists = true;
                                currentGroup = new PropertiesGroup() { Id = g.Id, Name = g.Name, DisplayAs=g.DisplayAs ,Properties = new List<Property>() };
                                p.Property.Value = p.Value;
                                currentGroup.Properties.Add(p.Property);
                                model.Add(currentGroup);
                            }
                            else
                            {
                                p.Property.Value = p.Value;
                                currentGroup.Properties.Add(p.Property);

                            }
                        }
                    }
                    if (currentGroup != null && currentGroup.Properties != null)
                        currentGroup.Properties = currentGroup.Properties.OrderBy(m => m.Priority).ToList();
                }
            }
            return View(model);
        }
    }
}
