using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using SABFramework.PreDefinedModules.MembershipModule.Models;


namespace OperationsManager.Models
{
    public class OperationsManager_DBContext : DConfigOS_Core.Models.DConfigOS_Core_DBContext
    {
        public DbSet<CompanyOperation> CompanyOperations { get; set; }
        public DbSet<Operation> Operations { get; set; }
        public DbSet<OperationCheckListItem> OperationCheckListItems { get; set; }
        public DbSet<OperationCheckListItemInstance> OperationCheckListItemInstances { get; set; }
        public DbSet<OperationInstance> OperationInstances { get; set; }
        public DbSet<OperationsCategory> OperationsCategories { get; set; }
        public DbSet<OperationInstanceStatus> OperationInstanceStatuses { get; set; }
        
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<CompanyOperation>()
                .HasRequired(m => m.Operation)
                .WithMany(m => m.CompanyOperations)
                .HasForeignKey(m => m.OperationId);
            modelBuilder.Entity<CompanyOperation>()
                .HasRequired(m => m.ServedCompany)
                .WithMany()
                .HasForeignKey(m => m.ServedCompanyId);
            modelBuilder.Entity<Operation>()
                .HasOptional(m => m.Category)
                .WithMany(m => m.Operations)
                .HasForeignKey(m => m.CategoryId);
            modelBuilder.Entity<Operation>()
                .HasRequired(m => m.CreatingUser)
                .WithMany()
                .HasForeignKey(m => m.CreatingUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<OperationInstanceStatus>()
                .HasRequired(m => m.User)
                .WithMany()
                .HasForeignKey(m => m.UserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<OperationCheckListItem>()
                .HasRequired(m => m.Operation)
                .WithMany(m => m.OperationCheckListItems)
                .HasForeignKey(m => m.OperationId).WillCascadeOnDelete(true);
            modelBuilder.Entity<OperationInstance>()
                .HasRequired(m => m.Operation)
                .WithMany(m => m.OperationInstances)
                .HasForeignKey(m => m.OperationId).WillCascadeOnDelete(false);
            modelBuilder.Entity<OperationInstance>()
                .HasRequired(m => m.ServedCompany)
                .WithMany()
                .HasForeignKey(m => m.ServedCompanyId);
            modelBuilder.Entity<OperationInstance>()
                .HasMany(m => m.AssigneesUsers)
                .WithMany()
                .Map(m => m.MapLeftKey("OperationId").MapRightKey("UserId").ToTable("OperationInstancesAssignees"));
            modelBuilder.Entity<CompanyOperation>()
                .HasMany(m => m.Assignees)
                .WithMany()
                .Map(m => m.MapLeftKey("CompanyOperationId").MapRightKey("UserId").ToTable("CompanyOperationsAssignees"));
            modelBuilder.Entity<OperationCheckListItemInstance>()
                .HasRequired(m => m.OperationCheckListItem)
                .WithMany()
                .HasForeignKey(m => m.OperationCheckListItemId);
            modelBuilder.Entity<OperationCheckListItemInstance>()
                .HasRequired(m => m.OperationInstance)
                .WithMany(m => m.OperationCheckListItems)
                .HasForeignKey(m => m.OperationInstanceId).WillCascadeOnDelete(true);
            modelBuilder.Entity<OperationInstanceStatus>()
                .HasRequired(m => m.OperationInstance)
                .WithMany(m => m.OperationStatuses)
                .HasForeignKey(m => m.OperationInstanceId).WillCascadeOnDelete(true);
        }

        
    }
}
