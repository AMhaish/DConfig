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
    public class RenderComparisonAction : SABFramework.Core.SABAction 
    {
        [Inject]
        public IPropertiesAPI propertiesAPI { get; set; }

        public int Id { get; set; }
        public string PropertiesIds { get; set; }
        private IComparisonAPI ComparisonsAPI { get; set; }
        public override async Task<SABFramework.Core.SABActionResult> GetHandler(System.Web.Mvc.Controller controller)
        {
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
            var com = ComparisonsAPI.GetComparison(Id);
            var groups = propertiesAPI.GetPropertiesGroups();
            List<PropertiesGroup> model = new List<PropertiesGroup>();
            Dictionary<int, Dictionary<int, string>> valuesDic = new Dictionary<int, Dictionary<int, string>>();
            PropertiesGroup currentGroup = null;
            bool groupExists;
            bool groupExitsWithItsValues;
            if (com!=null && com.Products != null && com.Products.Count > 0)
            {
                foreach (PropertiesGroup g in groups)
                {
                    groupExists = false;
                    groupExitsWithItsValues = false;
                    foreach (Product pr in com.Products)
                    {
                        foreach (ProductPropertyValue p in pr.PropertiesValues)
                        {
                            if (g.Id == p.Property.GroupId && (propIds.Count <=0 || propIds.Contains(p.PropertyId)))
                            {
                                if (!groupExitsWithItsValues)
                                {
                                    if (!groupExists)
                                    {
                                        groupExists = true;
                                        currentGroup = new PropertiesGroup() { Id = g.Id, Name = g.Name, DisplayAs=g.DisplayAs,Properties = new List<Property>() };
                                        currentGroup.Properties.Add(p.Property);
                                        model.Add(currentGroup);
                                    }
                                    else
                                    {
                                        currentGroup.Properties.Add(p.Property);
                                    }
                                }
                                if (!valuesDic.Keys.Contains(pr.Id))
                                {
                                    valuesDic.Add(pr.Id,new Dictionary<int, string>());
                                }
                                valuesDic[pr.Id].Add(p.PropertyId, p.Value);
                            }
                        }
                        groupExitsWithItsValues = true;
                    }
                }
            }
            return View(new ComparisonModel { groups = model, values = valuesDic, comparison = com });

        }
        public class ComparisonModel
        {
            public List<PropertiesGroup> groups { get; set; }
            public Dictionary<int, Dictionary<int, string>> values { get; set; }
            public Comparison comparison { get; set; }
        }
    }

}
