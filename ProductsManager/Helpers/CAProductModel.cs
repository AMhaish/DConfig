using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompetitiveAnalysis.Models;
using System.Web;
using Microsoft.AspNet.Identity;

namespace CompetitiveAnalysis.Helpers
{
    public class CAProductModel
    {
        public CAProductModel()
        {

        }

        public Product ActiveProduct { get; set; }
        public Dictionary<string, string> FieldsDictionaryOnName { get; set; }

        public string this[string key]
        {
            get
            {
                if (this.ActiveProduct != null)
                {
                    if (FieldsDictionaryOnName == null)
                    {
                        this.FieldsDictionaryOnName = this.ActiveProduct.PropertiesValues.ToDictionary(m => m.Property.Name, m => m.Value);
                    }
                    if (FieldsDictionaryOnName.ContainsKey(key))
                    {
                        return FieldsDictionaryOnName[key];
                    }
                    else
                    {
                        return "";
                    }
                }
                else
                {
                    return null;
                }

            }
        }

        public double? Price
        {
            get
            {
                Models.ProductsManager_DBContext context = new ProductsManager_DBContext();
                var price = ActiveProduct.Prices.FirstOrDefault(m => m.PriceType == Models.Price.PriceType_NORM);
                var priceOverride = context.AdvancedPrivileges.FirstOrDefault(m => m.CompanyId == ActiveProduct.CompanyId);
                if (price != null)
                {
                    if (priceOverride != null)
                    {
                        return price.PriceValue + (price.PriceValue * priceOverride.PriceOverridePercentage);
                    }
                    else
                    {
                        return price.PriceValue;
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        public static CAProductModel BuildCAProductModel(Product p)
        {
            CAProductModel model = new CAProductModel();
            model.ActiveProduct = p;
            return model;
        }
    }
}
