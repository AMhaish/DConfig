using CompetitiveAnalysis.Models;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CompetitiveAnalysis.ProductsManagerServices.ProductsAPI;

namespace CompetitiveAnalysis.ProductsManagerServices
{
    public interface IProductsAPI
    {
        List<ProductsTemplate> GetTemplates();
        ProductsTemplate GetTemplate(int Id);
        List<Product> GetTemplateProducts(int Id);
        List<Property> GetTemplateProperties(int Id);
        List<Product> GetProductsByIds(List<int> ids);
        List<Product> GetTemplateProductsByPattern(int Id, string pattern);
        List<Product> GetTemplateProductsByFilters(int Id, List<string> BrandFactoryTypes, List<string> Tags, List<ProductsAPI.Filter> andFilters, List<ProductsAPI.Filter> orFilters, DateTime?[] CreateDateRange, DateTime?[] UpdateDateRange);
        Product GetProduct(int Id);
        int CreateTemplate(ProductsTemplate t);
        int CreateProduct(Product t);
        int UpdateTemplate(ProductsTemplate t);
        int UpdateProduct(Product t);
        int UpdateProductDate(int Id, out Product product);
        int DeleteTemplate(int Id);
        int DeleteProduct(int Id);
        int UpdateTemplateProperties(int templateId, List<ProductTemplatesPropertiesRelation> propertiesRelations);
        int UpdateProductPropertyValue(int productId, int propertyId, string value);
        int AddPriceToProduct(int productId, Price price);
        int UpdateProductPrice(Price price);
        int RemovePriceFromProduct(int price);
        int UpdateProductTags(int productId, List<string> tags);
        int ImportDataFromExcel(int templateId, string excelPath, int? CompanyId, string BrandFactoryType, out ImportingReport report);
        int ImportImagesFromZipFile(string zipPath, out ImportingReport report);
        string GetCellValue(SpreadsheetDocument document, Cell cell);
        List<ProductTag> GetProductsTags(string pattern);
    }
}

  
