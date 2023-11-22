using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using SABFramework.PreDefinedModules.MembershipModule.Models;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace CompetitiveAnalysis.Models
{
    public class ProductsManager_DBContext : DConfigOS_Core.Models.DConfigOS_Core_DBContext
    {
        public DbSet<Property> Properties { get; set; }
        public DbSet<PropertiesGroup> PropertiesGroups { get; set; }
        public DbSet<ProductsTemplate> ProductsTemplates { get; set; }
        public FilteredDbSet<Product> Products { get; set; }
        public DbSet<ProductPropertyValue> ProductsPropertiesValues { get; set; }
        public DbSet<PropertyEnum> PropertyEnums { get; set; }
        public DbSet<PropertyEnumValue> PropertyEnumValues { get; set; }
        public DbSet<Price> Prices { get; set; }
        public DbSet<Comparison> Comparisons { get; set; }
        public DbSet<ProductTag> ProductsTags { get; set; }
        public DbSet<ComparisonFilter> ComparisonFilters { get; set; }
        public DbSet<ProductTemplatesPropertiesRelation> ProductTemplatesPropertiesRelations { get; set; }
        public DbSet<AdvancedPrivilege> AdvancedPrivileges { get; set; }
        public DbSet<UserProductView> UserProductsViews { get; set; }

        public ProductsManager_DBContext() {
            Products = new FilteredDbSet<Product>(this, ContextCompanyId, ContextObjectsFree);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Entity<Property>()
                .HasRequired(m => m.Group)
                .WithMany(m => m.Properties)
                .HasForeignKey(m => m.GroupId);
            modelBuilder.Entity<Property>()
                .HasRequired(m => m.TypeObj)
                .WithMany()
                .HasForeignKey(m => m.Type);
            modelBuilder.Entity<Product>()
                .HasRequired(m => m.Template)
                .WithMany(m => m.Products)
                .HasForeignKey(m => m.TemplateId);
            modelBuilder.Entity<ProductPropertyValue>()
                .HasRequired(m => m.Product)
                .WithMany(m => m.PropertiesValues)
                .HasForeignKey(m => m.ProductId)
                .WillCascadeOnDelete(true);
            modelBuilder.Entity<ProductPropertyValue>()
                .HasRequired(m => m.Property)
                .WithMany(m => m.Values)
                .HasForeignKey(m => m.PropertyId);
            modelBuilder.Entity<ProductTemplatesPropertiesRelation>()
                .HasRequired(p => p.Template)
                .WithMany(p => p.PropertiesRelations)
                .HasForeignKey(m => m.TemplateId);
            modelBuilder.Entity<ProductTemplatesPropertiesRelation>()
                .HasRequired(p => p.Property)
                .WithMany(p => p.Templates)
                .HasForeignKey(m => m.PropertyId);
            modelBuilder.Entity<PropertyEnumValue>()
                .HasRequired(m => m.Enum)
                .WithMany(m => m.Values)
                .HasForeignKey(m => m.EnumId);
            modelBuilder.Entity<Property>()
               .HasOptional(m => m.Enum)
               .WithMany(m => m.Properties)
               .HasForeignKey(m => m.EnumId);
            modelBuilder.Entity<Price>()
                .HasRequired(m => m.Product)
                .WithMany(m => m.Prices)
                .HasForeignKey(m => m.ProductId)
                .WillCascadeOnDelete(true);
            modelBuilder.Entity<Product>()
                .HasOptional(m => m.Company)
                .WithMany()
                .HasForeignKey(m => m.CompanyId)
                .WillCascadeOnDelete(false);
            modelBuilder.Entity<Comparison>()
                .HasMany(p => p.Products)
                .WithMany()
                .Map(m => m.MapLeftKey("ComparisonId").MapRightKey("ProductId").ToTable("ComparisonsProductsRelations"));
            modelBuilder.Entity<Product>()
                .HasMany(p => p.Tags)
                .WithMany(p => p.Products)
                .Map(m => m.MapLeftKey("ProductId").MapRightKey("TagId").ToTable("ProductsTagsRelations"));
            modelBuilder.Entity<ComparisonFilter>()
                .HasRequired(m => m.Comparison)
                .WithMany(m => m.Filters)
                .HasForeignKey(m => m.ComparisonId).WillCascadeOnDelete(false);
            modelBuilder.Entity<ComparisonFilter>()
                .HasRequired(m => m.Property)
                .WithMany()
                .HasForeignKey(m => m.PropertyId).WillCascadeOnDelete(true);
            modelBuilder.Entity<AdvancedPrivilege>()
                .HasMany(m => m.RelatedProdutTemplates)
                .WithMany()
                .Map(m => m.MapLeftKey("TemplateId").MapRightKey("PrivilegeId").ToTable("AdvancedPrivilegesProductTypes"));
            modelBuilder.Entity<AdvancedPrivilege>()
                .HasRequired(m => m.Company)
                .WithMany()
                .HasForeignKey(m => m.CompanyId)
                .WillCascadeOnDelete(true);
        }

        
    }
}
