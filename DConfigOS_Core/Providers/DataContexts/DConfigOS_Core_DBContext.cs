using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using SABFramework.PreDefinedModules.MembershipModule.DataProviders;
using SABFramework.PreDefinedModules.MembershipModule.Models;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace DConfigOS_Core.Models
{
    public class DConfigOS_Core_DBContext<TUser> : MembershipDBContext<TUser> where TUser: ApplicationUser
    {
        public DbSet<App> Apps { get; set; }
        public DbSet<AppView> AppViews { get; set; }
        public DbSet<AppWidget> AppWidgets { get; set; }
        public FilteredDbSet<Domain> Domains { get; set; }
        public FilteredDbSet<Content> Contents { get; set; }
        public DbSet<ContentInstance> ContentInstances { get; set; }
        public DbSet<PublicViewsPackage> PublicViewsPackages { get; set; }
        public DbSet<Script> Scripts { get; set; }
        public DbSet<StyleSheet> StyleSheets { get; set; }
        public FilteredDbSet<ScriptsBundle> ScriptsBundles { get; set; }
        public FilteredDbSet<StylesBundle> StylesBundles { get; set; }
        public DbSet<ViewField> ViewFields { get; set; }
        public DbSet<FieldsType> FieldsTypes { get; set; }
        public DbSet<ViewFieldValue> ViewFieldValue { get; set; }
        public FilteredDbSet<ViewTemplate> ViewTemplates { get; set; }
        public FilteredDbSet<ViewType> ViewTypes { get; set; }
        public DbSet<DesktopWidget> DesktopWidgets { get; set; }
        public DbSet<WidgetView> WidgetViews { get; set; }
        public DbSet<WidgetClientLogic> WidgetClientLogics { get; set; }
        public DbSet<AppClientLogic> AppClientLogics { get; set; }
        public DbSet<AppExtention> AppExtentions { get; set; }
        public DbSet<AppExtentionView> AppExtentionViews { get; set; }
        public DbSet<AppExtentionClientLogic> AppExtentionClientLogics { get; set; }
        public DbSet<AppStyleSheet> AppStyleSheets { get; set; }
        public FilteredDbSet<Form> Forms { get; set; }
        public DbSet<FormsType> FormsTypes { get; set; }
        public DbSet<FormsField> FormsFields { get; set; }
        public DbSet<FormInstance> FormsInstances { get; set; }
        public DbSet<FormFieldValue> FormsFieldsValues { get; set; }
        public FilteredDbSet<FormsFieldsEnum> FormsFieldsEnums { get; set; }
        public DbSet<FormsFieldsEnumValue> FormsFieldsEnumsValues { get; set; }
        public FilteredDbSet<FormSubmitEvent> FormSubmitEvents { get; set; }
        public DbSet<FormSubmitEventType> FormSubmitEventsTypes { get; set; }
        public FilteredDbSet<ViewFieldsEnum> ViewFieldsEnums { get; set; }
        public DbSet<ViewFieldsEnumValue> ViewFieldsEnumsValues { get; set; }
        public FilteredDbSet<Stage> Stages {get; set;}
        public FilteredDbSet<ExApplicationUser> ExApplicationsUsers { get; set; }

        public DConfigOS_Core_DBContext() : base()
        {
        }

        public DConfigOS_Core_DBContext(int contextCompany):base(contextCompany)
        {
        }

        public DConfigOS_Core_DBContext(bool contextObjectsFree):base(contextObjectsFree)
        {
        }

        protected override void InitializeSets()
        {
            base.InitializeSets();
            Domains = new FilteredDbSet<Domain>(this, ContextCompanyId,ContextObjectsFree);
            Contents = new FilteredDbSet<Content>(this, ContextCompanyId, ContextObjectsFree);
            ScriptsBundles = new FilteredDbSet<ScriptsBundle>(this, ContextCompanyId, ContextObjectsFree);
            StylesBundles = new FilteredDbSet<StylesBundle>(this, ContextCompanyId, ContextObjectsFree);
            ViewTemplates = new FilteredDbSet<ViewTemplate>(this, ContextCompanyId, ContextObjectsFree);
            ViewTypes = new FilteredDbSet<ViewType>(this, ContextCompanyId, ContextObjectsFree);
            Forms = new FilteredDbSet<Form>(this, ContextCompanyId, ContextObjectsFree);
            FormSubmitEvents = new FilteredDbSet<FormSubmitEvent>(this, ContextCompanyId, ContextObjectsFree);
            ViewFieldsEnums = new FilteredDbSet<ViewFieldsEnum>(this, ContextCompanyId, ContextObjectsFree);
            WebsiteSettings = new FilteredDbSet<WebsiteSetting>(this, ContextCompanyId, ContextObjectsFree);
            ExApplicationsUsers = new FilteredDbSet<ExApplicationUser>(this, ContextCompanyId, ContextObjectsFree);
            Stages = new FilteredDbSet<Stage>(this, ContextCompanyId, ContextObjectsFree);
            FormsFieldsEnums = new FilteredDbSet<FormsFieldsEnum>(this,ContextCompanyId,ContextObjectsFree);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
            modelBuilder.Entity<ViewType>()
                            .HasMany(p => p.ChildrenTypes)
                            .WithMany(p => p.ParentTypes)
                            .Map(m => m.MapLeftKey("ParentId").MapRightKey("ChildId").ToTable("ViewTypesRelations"));
            modelBuilder.Entity<ViewTemplate>()
                           .HasOptional(p => p.LayoutTemplate)
                           .WithMany(p => p.ChildrenTemplates)
                           .HasForeignKey(m => m.LayoutTemplateId);
            modelBuilder.Entity<Content>()
                           .HasOptional(p => p.Parent)
                           .WithMany(p => p.ChildrenContents)
                           .HasForeignKey(m => m.ParentId);
            modelBuilder.Entity<AppView>()
                           .HasRequired(p => p.App)
                           .WithMany(p => p.AppViews)
                           .HasForeignKey(m => m.AppId)
                           .WillCascadeOnDelete(true);
            modelBuilder.Entity<WidgetView>()
                           .HasRequired(p => p.Widget)
                           .WithMany(p => p.WidgetsViews)
                           .HasForeignKey(m => m.WidgetId)
                           .WillCascadeOnDelete(true);
            modelBuilder.Entity<AppWidget>()
                           .HasRequired(p => p.App)
                           .WithMany(p => p.AppWidgets)
                           .HasForeignKey(m => m.AppId)
                           .WillCascadeOnDelete(true);
            modelBuilder.Entity<ViewFieldsEnumValue>()
                .HasOptional(p => p.SubEnum)
                .WithMany()
                .HasForeignKey(m => m.SubEnumId)
               .WillCascadeOnDelete(false);
            modelBuilder.Entity<Content>()
                           .HasOptional(p => p.ViewType)
                           .WithMany(p => p.TypeContents)
                           .HasForeignKey(p => p.ViewTypeId);
            modelBuilder.Entity<ContentInstance>()
                           .HasOptional(p => p.ViewTemplate)
                           .WithMany(p => p.TemplateContentInstances)
                           .HasForeignKey(p => p.ViewTemplateId);
            modelBuilder.Entity<Content>()
                        .HasOptional(p => p.Stage)    
                        .WithMany(p => p.Contents)              
                        .HasForeignKey(p => p.StageId);
            modelBuilder.Entity<ContentInstance>()
                .HasOptional(p => p.Stage)
                .WithMany(p => p.ContentInstances)
                .HasForeignKey(p => p.StageId);
            modelBuilder.Entity<PublicViewsPackage>()
                          .HasOptional(p => p.App)
                          .WithMany(p => p.AppPublicViewsPackages)
                          .HasForeignKey(p => p.AppId);
            modelBuilder.Entity<StyleSheet>()
                           .HasOptional(p => p.PublicViewsPackage)
                           .WithMany(p => p.PackageStyleSheets)
                           .HasForeignKey(p => p.PublicViewsPackageId)
                           .WillCascadeOnDelete(true);
            modelBuilder.Entity<Script>()
                            .HasOptional(p => p.PublicViewsPackage)
                            .WithMany(p => p.PackageScripts)
                            .HasForeignKey(p => p.PublicViewsPackageId)
                            .WillCascadeOnDelete(true);
            modelBuilder.Entity<ViewTemplate>()
                            .HasOptional(p => p.ViewType)
                            .WithMany(p => p.TypeTemplates)
                            .HasForeignKey(p => p.ViewTypeId);
            modelBuilder.Entity<ViewTemplate>()
                            .HasOptional(p => p.PublicViewsPackage)
                            .WithMany(p => p.PackageViewTemplates)
                            .HasForeignKey(p => p.PublicViewsPackageId);
            modelBuilder.Entity<ViewType>()
                            .HasOptional(p => p.PublicViewsPackage)
                            .WithMany(p => p.PackageViewTypes)
                            .HasForeignKey(p => p.PublicViewsPackageId)
                            .WillCascadeOnDelete(true);
            modelBuilder.Entity<ViewField>()
                            .HasRequired(p => p.TypeObj)
                            .WithMany(p => p.ViewFields)
                            .HasForeignKey(p => p.Type)
                            .WillCascadeOnDelete(false);
            modelBuilder.Entity<ViewField>()
                            .HasRequired(p => p.ViewType)
                            .WithMany(p => p.ViewFields)
                            .HasForeignKey(p => p.ViewTypeId)
                            .WillCascadeOnDelete(true);
            modelBuilder.Entity<ViewFieldValue>()
                            .HasRequired(p => p.Field)
                            .WithMany(p => p.FieldValues)
                            .HasForeignKey(p => p.FieldId)
                            .WillCascadeOnDelete(true);
            modelBuilder.Entity<ViewFieldValue>()
                            .HasRequired(p => p.Content)
                            .WithMany(p => p.FieldsValues)
                            .HasForeignKey(p => p.ContentId)
                            .WillCascadeOnDelete(true);
            modelBuilder.Entity<AppStyleSheet>()
                            .HasRequired(p => p.App)
                            .WithMany(p => p.AppStyleSheets)
                            .HasForeignKey(p => p.AppId)
                            .WillCascadeOnDelete(true);
            modelBuilder.Entity<AppClientLogic>()
                            .HasRequired(p => p.App)
                            .WithMany(p => p.AppClientLogics)
                            .HasForeignKey(p => p.AppId)
                            .WillCascadeOnDelete(true);
            modelBuilder.Entity<AppExtention>()
                            .HasRequired(p => p.App)
                            .WithMany(p => p.AppExtentions)
                            .HasForeignKey(p => p.AppId)
                            .WillCascadeOnDelete(true);
            modelBuilder.Entity<WidgetClientLogic>()
                            .HasRequired(p => p.Widget)
                            .WithMany(p => p.WidgetClientLogics)
                            .HasForeignKey(p => p.WidgetId)
                            .WillCascadeOnDelete(true);
            modelBuilder.Entity<DesktopWidget>()
                            .HasRequired(p => p.Widget)
                            .WithMany()
                            .HasForeignKey(p => p.WidgetId)
                            .WillCascadeOnDelete(true);
            modelBuilder.Entity<ContentInstance>()
                            .HasRequired(m => m.Content)
                            .WithMany(m => m.ContentInstances)
                            .HasForeignKey(m => m.ContentId)
                            .WillCascadeOnDelete(true);
            modelBuilder.Entity<Content>()
                            .HasOptional(p => p.PublicViewsPackage)
                            .WithMany(p => p.PackageViewContents)
                            .HasForeignKey(p => p.PublicViewPackageId);
            modelBuilder.Entity<Content>()
                            .HasOptional(p => p.Domain)
                            .WithMany(p => p.DomainContents)
                            .HasForeignKey(p => p.DomainId);
            modelBuilder.Entity<AppExtentionView>()
                            .HasRequired(p => p.AppExt)
                            .WithMany(p => p.AppExtViews)
                            .HasForeignKey(p => p.AppExtId)
                            .WillCascadeOnDelete(true);
            modelBuilder.Entity<AppExtentionClientLogic>()
                            .HasRequired(p => p.AppExt)
                            .WithMany(p => p.AppExtClientLogics)
                            .HasForeignKey(m => m.AppExtId)
                            .WillCascadeOnDelete(true);
            modelBuilder.Entity<Script>()
                            .HasOptional(p => p.Bundle)
                            .WithMany(p => p.Scripts)
                            .HasForeignKey(p => p.BundleId);
            modelBuilder.Entity<StyleSheet>()
                            .HasOptional(p => p.Bundle)
                            .WithMany(p => p.Styles)
                            .HasForeignKey(p => p.BundleId);
            modelBuilder.Entity<Form>()
                            .HasOptional(p => p.ParentForm)
                            .WithMany(p => p.ChildrenForms)
                            .HasForeignKey(m => m.ParentFormId)
                            .WillCascadeOnDelete(false);
            modelBuilder.Entity<FormInstance>()
                            .HasOptional(p => p.ParentInstance)
                            .WithMany(p => p.ChildrenInstances)
                            .HasForeignKey(m => m.ParentInstanceId)
                            .WillCascadeOnDelete(false);
            modelBuilder.Entity<Form>()
                            .HasOptional(p => p.PrintTemplate)
                            .WithMany()
                            .HasForeignKey(m => m.PrintTemplateId)
                            .WillCascadeOnDelete(false);
            modelBuilder.Entity<Form>()
                            .HasOptional(p => p.TypeObj)
                            .WithMany(p => p.Forms)
                            .HasForeignKey(p => p.Type)
                            .WillCascadeOnDelete(false);
            modelBuilder.Entity<Form>()
                            .HasOptional(p => p.App)
                            .WithMany(p => p.AppForms)
                            .HasForeignKey(p => p.AppId)
                            .WillCascadeOnDelete(false);
            modelBuilder.Entity<FormsField>()
                            .HasRequired(p => p.TypeObj)
                            .WithMany(p => p.FormFields)
                            .HasForeignKey(p => p.Type)
                            .WillCascadeOnDelete(false);
            modelBuilder.Entity<FormsField>()
                            .HasRequired(p => p.Form)
                            .WithMany(p => p.FormFields)
                            .HasForeignKey(p => p.FormId)
                            .WillCascadeOnDelete(true);
            modelBuilder.Entity<FormInstance>()
                            .HasRequired(p => p.Form)
                            .WithMany(p => p.FormsInstances)
                            .HasForeignKey(p => p.FormId)
                            .WillCascadeOnDelete(false);
            modelBuilder.Entity<FormFieldValue>()
                            .HasRequired(p => p.Field)
                            .WithMany(p => p.FieldsValues)
                            .HasForeignKey(p => p.FieldId)
                            .WillCascadeOnDelete(false);
            modelBuilder.Entity<FormFieldValue>()
                            .HasRequired(p => p.FormInstance)
                            .WithMany(p => p.FieldsValues)
                            .HasForeignKey(p => p.FormInstanceId)
                            .WillCascadeOnDelete(true);
            modelBuilder.Entity<FormInstance>()
                            .HasOptional(p => p.User)
                            .WithMany()
                            .HasForeignKey(p => p.UserId)
                            .WillCascadeOnDelete(false);
            modelBuilder.Entity<FormsFieldsEnumValue>()
                            .HasRequired(p => p.Enum)
                            .WithMany(p => p.Values)
                            .HasForeignKey(p => p.EnumId)
                            .WillCascadeOnDelete(true);
            modelBuilder.Entity<FormsField>()
                            .HasOptional(p => p.Enum)
                            .WithMany(p => p.FormFields)
                            .HasForeignKey(p => p.EnumId)
                            .WillCascadeOnDelete(false);
            modelBuilder.Entity<FormSubmitEvent>()
                            .HasRequired(m => m.Form)
                            .WithMany(m => m.FromSubmitEvents)
                            .HasForeignKey(m => m.FormId)
                            .WillCascadeOnDelete(true);
            modelBuilder.Entity<FormSubmitEventType>()
                            .HasRequired(m => m.App)
                            .WithMany(m => m.AppFormSubmitEventsTypes)
                            .HasForeignKey(m => m.AppId)
                            .WillCascadeOnDelete(true);
            modelBuilder.Entity<FormSubmitEvent>()
                            .HasRequired(m => m.TypeObj)
                            .WithMany(m => m.FormSubmitEvents)
                            .HasForeignKey(m => m.Type)
                            .WillCascadeOnDelete(false);
            modelBuilder.Entity<ViewField>()
                            .HasOptional(p => p.Enum)
                            .WithMany(p => p.ViewFields)
                            .HasForeignKey(p => p.EnumId)
                            .WillCascadeOnDelete(false);
            modelBuilder.Entity<ViewFieldsEnumValue>()
                            .HasRequired(p => p.Enum)
                            .WithMany(p => p.Values)
                            .HasForeignKey(p => p.EnumId)
                            .WillCascadeOnDelete(true);


            modelBuilder.Entity<Content>()
                .HasRequired(p => p.Creator)
                .WithMany()
                .HasForeignKey(p => p.CreatorId).WillCascadeOnDelete(false);
            modelBuilder.Entity<ContentInstance>()
                .HasRequired(p => p.Creator)
                .WithMany()
                .HasForeignKey(p => p.CreatorId).WillCascadeOnDelete(false);
            modelBuilder.Entity<Domain>()
                .HasRequired(p => p.Creator)
                .WithMany()
                .HasForeignKey(p => p.CreatorId).WillCascadeOnDelete(false);
            modelBuilder.Entity<Form>()
                .HasRequired(p => p.Creator)
                .WithMany()
                .HasForeignKey(p => p.CreatorId).WillCascadeOnDelete(false);
            modelBuilder.Entity<FormsFieldsEnum>()
                .HasRequired(p => p.Creator)
                .WithMany()
                .HasForeignKey(p => p.CreatorId).WillCascadeOnDelete(false);
            modelBuilder.Entity<FormSubmitEvent>()
                .HasRequired(p => p.Creator)
                .WithMany()
                .HasForeignKey(p => p.CreatorId).WillCascadeOnDelete(false);
            modelBuilder.Entity<ScriptsBundle>()
                .HasRequired(p => p.Creator)
                .WithMany()
                .HasForeignKey(p => p.CreatorId).WillCascadeOnDelete(false);
            modelBuilder.Entity<StylesBundle>()
                .HasRequired(p => p.Creator)
                .WithMany()
                .HasForeignKey(p => p.CreatorId).WillCascadeOnDelete(false);
            modelBuilder.Entity<ViewType>()
                .HasRequired(p => p.Creator)
                .WithMany()
                .HasForeignKey(p => p.CreatorId).WillCascadeOnDelete(false);
            modelBuilder.Entity<ViewTemplate>()
                .HasRequired(p => p.Creator)
                .WithMany()
                .HasForeignKey(p => p.CreatorId).WillCascadeOnDelete(false);
            modelBuilder.Entity<ViewFieldsEnum>()
                .HasRequired(p => p.Creator)
                .WithMany()
                .HasForeignKey(p => p.CreatorId).WillCascadeOnDelete(false);

            modelBuilder.Entity<App>()
                .HasMany(p => p.AppRoles)
                .WithMany()
                .Map(m => m.MapLeftKey("AppId").MapRightKey("RoleId").ToTable("AppsRolesRelations"));

            modelBuilder.Entity<Stage>()
                           .HasMany(p => p.NextStages)
                           .WithMany()
                           .Map(m => m.MapLeftKey("CurrentStageId").MapRightKey("NextStageId").ToTable("StagesRelations"));

            modelBuilder.Entity<Stage>()
                           .HasMany(p => p.Roles)
                           .WithMany()
                           .Map(m => m.MapLeftKey("StageId").MapRightKey("RoleId").ToTable("StagesRolesRelations"));
            modelBuilder.Entity<ExApplicationUser>().ToTable("ExApplicationUsers");
            //modelBuilder.Entity<ExApplicationUser>()
            //               .HasRequired(p => p.User)
            //               .WithRequiredPrincipal().WillCascadeOnDelete(true); // Making an exception
        }
    }

    public class DConfigOS_Core_DBContext : DConfigOS_Core_DBContext<ApplicationUser> {


        public DConfigOS_Core_DBContext() : base(){}

        public DConfigOS_Core_DBContext(int contextCompany) : base(contextCompany) { }

        public DConfigOS_Core_DBContext(bool contextFree) : base(contextFree) { }
    }
}
