using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using SABFramework.PreDefinedModules.MembershipModule.Models;
using DConfigOS_Core.Models;

namespace Membership.Models
{
    public class Membership_DBContext : DConfigOS_Core_DBContext
    {
        public FilteredDbSet<UserField> UserFields { get; set; }
        public DbSet<UserFieldsValue> UserFieldsValues { get; set; }
        public FilteredDbSet<ContentPrivilege> ContentPrivileges { get; set; }
        public FilteredDbSet<UserFieldEnum> UserFieldEnums { get; set; }
        public DbSet<UserFieldEnumValue> UserFieldEnumsValues { get; set; }
        
        public Membership_DBContext()
        {
            UserFields = new FilteredDbSet<UserField>(this,ContextCompanyId, ContextObjectsFree);
            ContentPrivileges = new FilteredDbSet<ContentPrivilege>(this, ContextCompanyId, ContextObjectsFree);
            UserFieldEnums = new FilteredDbSet<UserFieldEnum>(this, ContextCompanyId, ContextObjectsFree);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<UserFieldsValue>()
                            .HasRequired(p => p.User)
                            .WithMany()
                            .HasForeignKey(p => p.UserId)
                            .WillCascadeOnDelete(true);
            modelBuilder.Entity<UserFieldsValue>()
                            .HasRequired(p => p.Field)
                            .WithMany(p => p.FieldValues)
                            .HasForeignKey(p => p.FieldId)
                            .WillCascadeOnDelete(true);
            modelBuilder.Entity<ContentPrivilege>()
                            .HasMany(p => p.Roles)
                            .WithMany()
                            .Map(m => m.MapLeftKey("ContentId").MapRightKey("RoleId").ToTable("ContentPrivilegesRoles"));
            modelBuilder.Entity<UserFieldEnumValue>()
                            .HasRequired(m => m.Enum)
                            .WithMany(m => m.Values)
                            .HasForeignKey(m => m.EnumId);
            modelBuilder.Entity<UserField>()
                            .HasRequired(m => m.TypeObj)
                            .WithMany()
                            .HasForeignKey(m => m.Type);
            modelBuilder.Entity<UserField>()
                            .HasOptional(m => m.Enum)
                            .WithMany(m => m.UserFields)
                            .HasForeignKey(m => m.EnumId);
            modelBuilder.Entity<ContentPrivilege>()
                            .HasRequired(p => p.Content).WithRequiredDependent().WillCascadeOnDelete(true);

        }
    }
}
