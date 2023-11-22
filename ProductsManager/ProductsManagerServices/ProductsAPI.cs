using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Data.Entity.SqlServer;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using System.Text.RegularExpressions;
using CompetitiveAnalysis.Models;
using DConfigOS_Core.Repositories.Utilities;
using System.Data.Entity.Core.Objects;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Packaging;
using Ionic.Zip;

namespace CompetitiveAnalysis.ProductsManagerServices
{
    public class ProductsAPI : IProductsAPI
    {
        public virtual List<ProductsTemplate> GetTemplates()
        {
            ProductsManager_DBContext context = new ProductsManager_DBContext();
            var templates = context.ProductsTemplates.OrderBy(m => m.Name).ToList();
            return templates;
        }

        public virtual ProductsTemplate GetTemplate(int Id)
        {
            ProductsManager_DBContext context = new ProductsManager_DBContext();
            var template = context.ProductsTemplates.Where(m => m.Id == Id).FirstOrDefault();
            return template;
        }

        public virtual List<Product> GetTemplateProducts(int Id)
        {
            ProductsManager_DBContext context = new ProductsManager_DBContext();
            var products = context.Products.Where(m => m.TemplateId == Id).OrderBy(m => m.Name).ToList();
            return products;
        }

        public virtual List<Property> GetTemplateProperties(int Id)
        {
            ProductsManager_DBContext context = new ProductsManager_DBContext();
            var properties = context.ProductsTemplates.Where(m => m.Id == Id).FirstOrDefault().Properties.ToList();
            return properties;
        }

        public virtual List<Product> GetProductsByIds(List<int> ids)
        {
            ProductsManager_DBContext context = new ProductsManager_DBContext();
            var products = context.Products.Where(m => ids.Contains(m.Id)).OrderBy(m => m.Name).ToList();
            return products;
        }

        public virtual List<Product> GetTemplateProductsByPattern(int Id, string pattern)
        {
            ProductsManager_DBContext context = new ProductsManager_DBContext();
            var products = context.Products.Where(m => m.TemplateId == Id && m.Name.Contains(pattern)).OrderBy(m => m.Name).ToList();
            return products;
        }

        public virtual List<Product> GetTemplateProductsByFilters(int Id, List<string> BrandFactoryTypes, List<string> Tags, List<Filter> andFilters, List<Filter> orFilters, DateTime?[] CreateDateRange, DateTime?[] UpdateDateRange)
        {
            ProductsManager_DBContext context = new ProductsManager_DBContext();
            List<Product> finalResults = new List<Product>();
            List<Product> products;
            products = context.Products.Include("PropertiesValues").Where(m => m.TemplateId == Id).OrderBy(m => m.Name).ToList();
            if (BrandFactoryTypes != null && BrandFactoryTypes.Count > 0)
            {
                products = products.Where(m => BrandFactoryTypes.Contains(m.BrandFactoryType)).ToList();
            }
            if (Tags != null && Tags.Count > 0)
            {
                products = products.Where(m => m.Tags.Any(n => Tags.Contains(n.Name))).ToList();
            }
            if (CreateDateRange != null && CreateDateRange.Length == 2 && CreateDateRange[0].HasValue && CreateDateRange[1].HasValue)
            {
                products = products.Where(m => m.CreatedDate > CreateDateRange[0].Value && m.CreatedDate < CreateDateRange[1].Value).ToList();
            }
            if (UpdateDateRange != null && UpdateDateRange.Length == 2 && UpdateDateRange[0].HasValue && UpdateDateRange[1].HasValue)
            {
                products = products.Where(m => m.UpdateDate > UpdateDateRange[0].Value && m.UpdateDate < UpdateDateRange[1].Value).ToList();
            }
            if (andFilters != null || orFilters != null)
            {
                foreach (Product p in products)
                {
                    bool andConditionsValid = true;
                    bool orConditionsValid = false;
                    if (andFilters != null)
                    {
                        foreach (Filter f in andFilters)
                        {
                            var property = p.PropertiesValues.Where(m => m.PropertyId == f.PropertyId).FirstOrDefault();
                            if (property != null)
                            {
                                switch (f.Type)
                                {
                                    case "Date":
                                        DateTime datevalue = Convert.ToDateTime(property.Value);
                                        if (datevalue < f.MinDate || datevalue > f.MaxDate)
                                        {
                                            andConditionsValid = false;
                                        }
                                        break;
                                    case "String":
                                    case "String - Multiple Lines":
                                    case "Predefined List":
                                    case "Predefined List - Filter/Select":
                                    case "Predefined List - Radio Buttons":
                                        if (!f.Values.Split(',').Contains(property.Value))
                                        {
                                            andConditionsValid = false;
                                        }
                                        break;
                                    case "Predefined List - Checkboxes":
                                        bool propertyFound = false;
                                        foreach (string s in f.Values.Split(','))
                                        {
                                            if (property.Value.Contains(s))
                                            {
                                                propertyFound = true;
                                                break;
                                            }
                                        }
                                        if (!propertyFound)
                                            andConditionsValid = false;
                                        break;
                                    case "Boolean":
                                        bool boolvalue = Convert.ToBoolean(property.Value);
                                        if (boolvalue != f.BoolValue)
                                        {
                                            andConditionsValid = false;
                                        }
                                        break;
                                    case "Number":
                                        double value = Convert.ToDouble(property.Value);
                                        if (value < f.MinValue || value > f.MaxValue)
                                        {
                                            andConditionsValid = false;
                                        }
                                        break;
                                }
                            }
                        }
                    }
                    if (orFilters != null)
                    {
                        foreach (Filter f in orFilters)
                        {
                            var property = p.PropertiesValues.Where(m => m.PropertyId == f.PropertyId).FirstOrDefault();
                            if (property != null)
                            {
                                switch (f.Type)
                                {
                                    case "Date":
                                        DateTime datevalue = Convert.ToDateTime(property.Value);
                                        if (datevalue >= f.MinDate && datevalue <= f.MaxDate)
                                        {
                                            orConditionsValid = true;
                                        }
                                        break;
                                    case "String":
                                    case "String - Multiple Lines":
                                    case "Predefined List":
                                    case "Predefined List - Filter/Select":
                                    case "Predefined List - Radio Buttons":
                                        if (f.Values.Split(',').Contains(property.Value))
                                        {
                                            orConditionsValid = true;
                                        }
                                        break;
                                    case "Predefined List - Checkboxes":
                                        foreach (string s in f.Values.Split(','))
                                        {
                                            if (property.Value.Contains(s))
                                            {
                                                orConditionsValid = true;
                                                break;
                                            }
                                        }
                                        break;
                                    case "Boolean":
                                        bool boolvalue = Convert.ToBoolean(property.Value);
                                        if (boolvalue == f.BoolValue)
                                        {
                                            orConditionsValid = true;
                                        }
                                        break;
                                    case "Number":
                                        double value = Convert.ToDouble(property.Value);
                                        if (value >= f.MinValue && value <= f.MaxValue)
                                        {
                                            orConditionsValid = true;
                                        }
                                        break;
                                }
                            }
                        }
                    }
                    else
                    {
                        orConditionsValid = true;
                    }
                    if (andConditionsValid && orConditionsValid)
                        finalResults.Add(p);
                }
            }
            else
            {
                return products;
            }
            return finalResults;
        }


        public virtual Product GetProduct(int Id)
        {
            ProductsManager_DBContext context = new ProductsManager_DBContext();
            var product = context.Products.Where(m => m.Id == Id).FirstOrDefault();
            return product;
        }
       
        public virtual int CreateTemplate(ProductsTemplate t)
        {
            ProductsManager_DBContext context = new ProductsManager_DBContext();
            var dbTemplate = context.ProductsTemplates.Where(m => m.Name == t.Name).FirstOrDefault();
            if (dbTemplate == null)
            {
                context.ProductsTemplates.Add(t);
                context.SaveChanges();
                return ResultCodes.Succeed;
            }
            else
            {
                return ResultCodes.ObjectNameAlreadyUsed;
            }
        }

        public virtual int CreateProduct(Product t)
        {
            ProductsManager_DBContext context = new ProductsManager_DBContext();
            var dbTemplate = context.Products.Where(m => m.Name == t.Name).FirstOrDefault();
            //if (dbTemplate == null)
            //{
            context.Products.Add(t);
            t.CreatedDate = DateTime.Now;
            context.SaveChanges();
            return ResultCodes.Succeed;
            //}
            //else
            //{
            //return ResultCodes.ObjectNameAlreadyUsed;
            //}
        }

        public virtual int UpdateTemplate(ProductsTemplate t)
        {
            ProductsManager_DBContext context = new ProductsManager_DBContext();
            var dbTemplate = context.ProductsTemplates.Where(m => m.Id == t.Id).FirstOrDefault();
            var sameTemplate = context.ProductsTemplates.Where(m => m.Name == t.Name && m.Id != t.Id).FirstOrDefault();
            if (dbTemplate != null)
            {
                if (sameTemplate == null)
                {
                    dbTemplate.Name = t.Name;
                    context.SaveChanges();
                    return ResultCodes.Succeed;
                }
                else
                {
                    return ResultCodes.ObjectNameAlreadyUsed;
                }
            }
            else
            {
                return ResultCodes.ObjectHasntFound;
            }
        }

        public virtual int UpdateProduct(Product t)
        {
            ProductsManager_DBContext context = new ProductsManager_DBContext();
            var dbProduct = context.Products.Where(m => m.Id == t.Id).FirstOrDefault();
            //var sameProduct = context.Products.Where(m => m.Name == t.Name && m.Id != t.Id).FirstOrDefault();
            if (dbProduct != null)
            {
                //if (sameProduct == null)
                //{
                dbProduct.Name = t.Name;
                dbProduct.TemplateId = t.TemplateId;
                dbProduct.BrandFactoryType = t.BrandFactoryType;
                dbProduct.CompanyId = t.CompanyId;
                dbProduct.Notes = t.Notes;
                dbProduct.Code = t.Code;
                context.SaveChanges();
                return ResultCodes.Succeed;
                //}
                //else
                //{
                //return ResultCodes.ObjectNameAlreadyUsed;
                //}
            }
            else
            {
                return ResultCodes.ObjectHasntFound;
            }
        }

        public virtual int UpdateProductDate(int Id, out Product product)
        {
            ProductsManager_DBContext context = new ProductsManager_DBContext();
            var dbProduct = context.Products.Where(m => m.Id == Id).FirstOrDefault();
            if (dbProduct != null)
            {
                dbProduct.UpdateDate = DateTime.Now;
                product = dbProduct;
                context.SaveChanges();
                return ResultCodes.Succeed;
            }
            else
            {
                product = null;
                return ResultCodes.ObjectHasntFound;
            }
        }

        public virtual int DeleteTemplate(int Id)
        {
            ProductsManager_DBContext context = new ProductsManager_DBContext();
            var dbTemplate = context.ProductsTemplates.Where(m => m.Id == Id).FirstOrDefault();
            if (dbTemplate != null)
            {
                context.ProductsTemplates.Remove(dbTemplate);
                context.SaveChanges();
                return ResultCodes.Succeed;
            }
            else
            {
                return ResultCodes.ObjectHasntFound;
            }
        }

        public virtual int DeleteProduct(int Id)
        {
            ProductsManager_DBContext context = new ProductsManager_DBContext();
            var dbProduct = context.Products.Where(m => m.Id == Id).FirstOrDefault();
            if (dbProduct != null)
            {
                context.Products.Remove(dbProduct);
                context.SaveChanges();
                return ResultCodes.Succeed;
            }
            else
            {
                return ResultCodes.ObjectHasntFound;
            }
        }


        public virtual int UpdateTemplateProperties(int templateId, List<ProductTemplatesPropertiesRelation> propertiesRelations)
        {
            ProductsManager_DBContext context = new ProductsManager_DBContext();
            var dbTemplate = context.ProductsTemplates.Where(m => m.Id == templateId).FirstOrDefault();
            if (dbTemplate != null)
            {
                dbTemplate.PropertiesRelations.Clear();
                var idsList = propertiesRelations.Select(m => m.Id).ToList();
                if (propertiesRelations != null && propertiesRelations.Count > 0)
                {
                    var properties = context.Properties.Where(m => idsList.Contains(m.Id));
                    if (properties != null)
                    {
                        foreach (var p in properties)
                        {
                            dbTemplate.PropertiesRelations.Add(new ProductTemplatesPropertiesRelation { Property = p, IsHighlight = propertiesRelations.Single(m => m.Id == p.Id).IsHighlight, InvisibileToFactoryTypes = propertiesRelations.Single(m => m.Id == p.Id).InvisibileToFactoryTypes });
                        }
                    }
                }
                context.SaveChanges();
                return ResultCodes.Succeed;
            }
            else
            {
                return ResultCodes.ObjectHasntFound;
            }
        }

        public virtual int UpdateProductPropertyValue(int productId, int propertyId, string value)
        {
            ProductsManager_DBContext context = new ProductsManager_DBContext();
            var dbProduct = context.Products.Where(m => m.Id == productId).FirstOrDefault();
            var dbProperty = context.Properties.Where(m => m.Id == propertyId).FirstOrDefault();
            if (dbProduct != null && dbProperty != null)
            {
                var dbPropertyValue = context.ProductsPropertiesValues.Where(m => m.ProductId == productId && m.PropertyId == propertyId).FirstOrDefault();
                if (!String.IsNullOrEmpty(value))
                {
                    if (dbPropertyValue != null)
                    {
                        dbPropertyValue.Value = value;
                    }
                    else
                    {
                        var newValue = new ProductPropertyValue() { Value = value };
                        dbProduct.PropertiesValues.Add(newValue);
                        dbProperty.Values.Add(newValue);
                    }
                }
                else
                {
                    dbProduct.PropertiesValues.Remove(dbPropertyValue);
                }
                context.SaveChanges();
                return ResultCodes.Succeed;
            }
            else
            {
                return ResultCodes.ObjectHasntFound;
            }
        }

        public virtual int AddPriceToProduct(int productId, Price price)
        {
            ProductsManager_DBContext context = new ProductsManager_DBContext();
            var dbProduct = context.Products.Where(m => m.Id == productId).FirstOrDefault();
            if (dbProduct != null)
            {
                price.CreateDate = DateTime.Now;
                dbProduct.Prices.Add(price);
                context.SaveChanges();
                return ResultCodes.Succeed;
            }
            else
            {
                return ResultCodes.ObjectHasntFound;
            }
        }

        public virtual int UpdateProductPrice(Price price)
        {
            ProductsManager_DBContext context = new ProductsManager_DBContext();
            var dbPrice = context.Prices.Where(m => m.Id == price.Id).FirstOrDefault();
            if (dbPrice != null)
            {

                dbPrice.PriceValue = price.PriceValue;
                dbPrice.Currency = price.Currency;
                dbPrice.PriceDate = price.PriceDate;
                dbPrice.Reference = price.Reference;
                dbPrice.PriceType = price.PriceType;
                dbPrice.QuantityFrom = price.QuantityFrom;
                dbPrice.QuantityTo = price.QuantityTo;
                context.SaveChanges();
                return ResultCodes.Succeed;

            }
            else
            {
                return ResultCodes.ObjectHasntFound;
            }
        }

        public virtual int RemovePriceFromProduct(int price)
        {
            ProductsManager_DBContext context = new ProductsManager_DBContext();
            var dbPrice = context.Prices.Where(m => m.Id == price).FirstOrDefault();
            if (dbPrice != null)
            {
                context.Prices.Remove(dbPrice);
                context.SaveChanges();
                return ResultCodes.Succeed;
            }
            else
            {
                return ResultCodes.ObjectHasntFound;
            }
        }

        public virtual int UpdateProductTags(int productId, List<string> tags)
        {
            ProductsManager_DBContext context = new ProductsManager_DBContext();
            var product = context.Products.Where(m => m.Id == productId).FirstOrDefault();
            if (product != null)
            {
                product.Tags.Clear();
                foreach (string s in tags)
                {
                    if (!String.IsNullOrEmpty(s))
                    {
                        var dbTag = context.ProductsTags.Where(m => m.Name == s).FirstOrDefault();
                        if (dbTag != null)
                        {
                            product.Tags.Add(dbTag);
                        }
                        else
                        {
                            var newTag = new ProductTag() { Name = s };
                            product.Tags.Add(newTag);
                            context.ProductsTags.Add(newTag);
                        }
                    }
                }
                context.SaveChanges();
                return ResultCodes.Succeed;
            }
            else
            {
                return ResultCodes.ObjectHasntFound;
            }
        }

        public virtual int ImportDataFromExcel(int templateId, string excelPath, int? CompanyId, string BrandFactoryType, out ImportingReport report)
        {
            report = new ImportingReport();
            report.FailedDic = new Dictionary<string, string>();
            ProductsManager_DBContext context = new ProductsManager_DBContext();
            var template = context.ProductsTemplates.Where(m => m.Id == templateId).FirstOrDefault();
            if (template != null)
            {
                SpreadsheetDocument excel = SpreadsheetDocument.Open(SABFramework.Core.SABCoreEngine.Instance.AppPhysicalPath + excelPath.Replace("/", "\\"), false);
                //var excel = new ExcelQueryFactory(SABFramework.Core.SABCoreEngine.Instance.AppPhysicalPath + excelPath.Replace("/","\\"));
                //var workSheets = excel.GetWorksheetNames().ToList();
                //var columns = excel.GetColumnNames(workSheets[0]).ToList();
                var columnsDic = new Dictionary<string, int>();
                IEnumerable<Sheet> sheets = excel.WorkbookPart.Workbook.GetFirstChild<Sheets>().Elements<Sheet>();
                string relationshipId = sheets.First().Id.Value;
                WorksheetPart worksheetPart = (WorksheetPart) excel.WorkbookPart.GetPartById(relationshipId);
                Worksheet workSheet = worksheetPart.Worksheet;
                SheetData sheetData = workSheet.GetFirstChild<SheetData>();
                IEnumerable<Row> rows = sheetData.Descendants<Row>();
                int counter = 0;
                foreach (Cell cell in rows.ElementAt(0))
                {
                    if (cell != null && cell.CellValue != null)
                    {
                        columnsDic.Add(GetCellValue(excel, cell).ToLower(), counter);
                        counter++;
                    }
                }
                //for (int i = 0; i < columns.Count; i++)
                //{
                //    columnsDic.Add(columns[i], i);
                //}
                //var excelProducts = from p in excel.Worksheet(workSheets[0]) select p;
                var indexCounter = 1;//Start from the second row for excel header
                string propertyThatHasError;
                foreach (var pro in rows)
                {
                    propertyThatHasError = "";
                    if (indexCounter == 1) { indexCounter++; continue; }
                    try
                    {
                        Product p;
                        var valuesList = pro.Descendants<Cell>().ToList();
                        if (columnsDic.ContainsKey("code"))
                        {
                            var cell = valuesList[columnsDic["code"]];
                            string code = GetCellValue(excel, cell);
                            p = context.Products.SingleOrDefault(m => m.Code== code);
                            if (p == null)
                            {
                                p = new Product();
                            }
                            p.Code = code;
                        }
                        else
                        {
                            propertyThatHasError = "Code";
                            throw new Exception("Product code couldn't be found");
                        }
                        p.TemplateId = template.Id;
                        p.CreatedDate = DateTime.Now;
                        p.CompanyId = CompanyId;
                        p.BrandFactoryType = BrandFactoryType;
                        
                        if (columnsDic.ContainsKey("name"))
                        {
                            var cell = valuesList[columnsDic["name"]];
                            p.Name = GetCellValue(excel, cell);
                        }
                        else
                        {
                            propertyThatHasError = "Name";
                            throw new Exception("Product name couldn't be found");
                        }
                        
                        if (columnsDic.ContainsKey("price"))
                        {
                            propertyThatHasError = "Price";
                            var cell = valuesList[columnsDic["price"]];
                            Price priceObj = new Price();
                            string price = GetCellValue(excel, cell).ToLower();
                            if (price.Contains("$") || price.Contains("usd"))
                            {
                                priceObj.PriceValue = double.Parse(price.Trim(' ', '$'));
                                priceObj.Currency = "USD";
                            }
                            else if (price.Contains("tl") || price.Contains("try"))
                            {
                                priceObj.PriceValue = double.Parse(price.Replace("tl", "").Replace("try", "").Trim());
                                priceObj.Currency = "TRY";
                            }
                            else if (price.Contains("euro") || price.Contains("eur"))
                            {
                                priceObj.PriceValue = double.Parse(price.Replace("euro", "").Replace("eur", "").Trim());
                                priceObj.Currency = "TRY";
                            }
                            else
                            {
                                priceObj.PriceValue = double.Parse(price.Trim());
                                priceObj.Currency = "USD";
                            }
                            priceObj.CreateDate = DateTime.Now;
                            priceObj.PriceDate = DateTime.Now;
                            priceObj.PriceType = Price.PriceType_NORM;
                            p.Prices = new List<Price>();
                            p.Prices.Add(priceObj);
                        }
                        foreach (Property prop in template.Properties)
                        {
                            propertyThatHasError = prop.Name;
                            if (!String.IsNullOrEmpty(prop.ExcelColumnName) && columnsDic.ContainsKey(prop.ExcelColumnName))
                            {
                                var cell = valuesList[columnsDic[prop.ExcelColumnName]];
                                p.PropertiesValues.Add(new ProductPropertyValue() { PropertyId = prop.Id, Value = GetCellValue(excel, cell) });
                            }
                        }
                        context.Products.Add(p);
                        context.SaveChanges();
                        report.ImportedCount++;
                    }
                    catch (Exception ex)
                    {
                        report.FailedCount++;
                        report.FailedDic.Add(indexCounter.ToString(), "Property (" + propertyThatHasError + ") " + ex.Message);
                    }
                    finally
                    {
                        indexCounter++;
                    }
                }
                return ResultCodes.Succeed;
            }
            else
            {
                return ResultCodes.ObjectHasntFound;
            }
        }

        public virtual int ImportImagesFromZipFile(string zipPath, out ImportingReport report)
        {
            ProductsManager_DBContext context = new ProductsManager_DBContext();
            report = new ImportingReport();
            report.FailedDic = new Dictionary<string, string>();
            if (zipPath.EndsWith(".zip"))
            {
                using (ZipFile zip = ZipFile.Read(SABFramework.Core.SABCoreEngine.Instance.AppPhysicalPath + zipPath.Replace("/", "\\")))
                {
                    string extractFolder = SABFramework.Core.SABCoreEngine.Instance.AppPhysicalPath + zipPath.Replace("/", "\\").Trim('p', 'i', 'z', '.');
                    while (System.IO.Directory.Exists(extractFolder))
                    {
                        extractFolder = extractFolder + new Random().Next(100000).ToString();
                    }
                    string subFolder = "";
                    foreach (var entry in zip.Entries)
                    {
                        if (entry.IsDirectory && String.IsNullOrEmpty(subFolder))
                        {
                            subFolder += '\\' + entry.FileName.TrimEnd('/','\\');
                        }
                        entry.Extract(extractFolder, ExtractExistingFileAction.OverwriteSilently);
                    }
                    var dir = new DirectoryInfo(extractFolder + subFolder);
                    foreach (FileInfo file in dir.GetFiles().OrderBy(m => m.Name))
                    {
                        bool found = false;
                        try
                        {
                            if (file.Name.Contains("_"))
                            {
                                var fileName = file.Name.Split('_');
                                var productCode = fileName[0];
                                var product = context.Products.SingleOrDefault(m => m.Code == productCode);
                                if (product != null)
                                {
                                    if (fileName[1].ToLower().StartsWith("main"))
                                    {
                                        foreach (var p in product.Template.Properties)
                                        {
                                            if (p.Type == "Image")
                                            {
                                                var ppv = product.PropertiesValues.SingleOrDefault(m => m.PropertyId == p.Id);
                                                if (ppv != null)
                                                {
                                                    ppv.Value = file.FullName.Replace(SABFramework.Core.SABCoreEngine.Instance.AppPhysicalPath, "").Replace("\\", "/");
                                                }
                                                else
                                                {
                                                    ppv = new ProductPropertyValue();
                                                    ppv.PropertyId = p.Id;
                                                    ppv.Value = file.FullName.Replace(SABFramework.Core.SABCoreEngine.Instance.AppPhysicalPath, "").Replace("\\", "/");
                                                    if (product.PropertiesValues == null)
                                                    {
                                                        product.PropertiesValues = new List<ProductPropertyValue>();
                                                    }
                                                    product.PropertiesValues.Add(ppv);
                                                }
                                                found = true;
                                                report.ImportedCount++;
                                                break;
                                            }
                                        }
                                        if (!found)
                                        {
                                            throw new Exception("No main image property found for this product");
                                        }
                                    }
                                    else
                                    {
                                        foreach (var p in product.Template.Properties)
                                        {
                                            if (p.Type == "Multiple Images")
                                            {
                                                var ppv = product.PropertiesValues.SingleOrDefault(m => m.PropertyId == p.Id);
                                                if (ppv != null)
                                                {
                                                    ppv.Value += "," + file.FullName.Replace(SABFramework.Core.SABCoreEngine.Instance.AppPhysicalPath, "").Replace("\\", "/");
                                                }
                                                else
                                                {
                                                    ppv = new ProductPropertyValue();
                                                    ppv.PropertyId = p.Id;
                                                    ppv.Value = file.FullName.Replace(SABFramework.Core.SABCoreEngine.Instance.AppPhysicalPath, "").Replace("\\", "/");
                                                    if (product.PropertiesValues == null)
                                                    {
                                                        product.PropertiesValues = new List<ProductPropertyValue>();
                                                    }
                                                    product.PropertiesValues.Add(ppv);
                                                }
                                                found = true;
                                                report.ImportedCount++;
                                                break;
                                            }
                                        }
                                        if (!found)
                                        {
                                            throw new Exception("No multiple images property found for this product");
                                        }
                                    }
                                }
                                else
                                {
                                    throw new Exception("Product for the file couldn't be found");
                                }
                            }
                            else
                            {
                                throw new Exception("The file name does not include _");
                            }
                        }
                        catch (Exception ex)
                        {
                            report.FailedCount++;
                            report.FailedDic.Add(file.Name, ex.Message);
                        }
                    }
                    context.SaveChanges();
                    return ResultCodes.Succeed;
                }
            }
            else
            {
                return ResultCodes.ObjectInvalid;
            }
        }

        public virtual string GetCellValue(SpreadsheetDocument document, Cell cell)
        {
            SharedStringTablePart stringTablePart = document.WorkbookPart.SharedStringTablePart;
            string value = cell.CellValue.InnerXml;

            if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
            {
                return stringTablePart.SharedStringTable.ChildElements[Int32.Parse(value)].InnerText;
            }
            else
            {
                return value;
            }
        }

        public virtual List<ProductTag> GetProductsTags(string pattern)
        {
            ProductsManager_DBContext context = new ProductsManager_DBContext();
            return context.ProductsTags.Where(m => m.Name.Contains(pattern)).ToList();
        }

        public class Filter
        {
            public int PropertyId { get; set; }
            public string Type { get; set; }
            public double MinValue { get; set; }
            public double MaxValue { get; set; }
            public DateTime MinDate { get; set; }
            public DateTime MaxDate { get; set; }
            public string Values { get; set; }
            public bool BoolValue { get; set; }
        }

        public class ImportingReport
        {
            public int ImportedCount { get; set; }
            public int FailedCount { get; set; }
            public Dictionary<string, string> FailedDic { get; set; }
        }


    }



}
