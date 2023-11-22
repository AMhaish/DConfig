using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using System.Linq;
using SABFramework.Providers;
using SABFramework.PreDefinedModules.MembershipModule.Models;

namespace SABFramework.PreDefinedModules.MembershipModule.DataProviders
{
    public class MembershipDBContext<TUser> : IdentityDbContext<TUser>, IDBContext where TUser: ApplicationUser
    {
        public int? ContextCompanyId { get; set; }
        public DbSet<Privilege> Privileges { get; set; }
        public DbSet<Company> Companies { get; set; }
        public FilteredDbSet<WebsiteSetting> WebsiteSettings { get; set; }
        public FilteredDbSet<Webhook> Webhooks { get; set; }

        public bool ContextObjectsFree { get; set; }

        public MembershipDBContext()
            : base((Core.SABCoreEngine.Instance.Settings != null &&  Core.SABCoreEngine.Instance.Settings.ContainsKey("SystemDatabaseConnectionStringName") ? SABFramework.Core.SABCoreEngine.Instance.Settings["SystemDatabaseConnectionStringName"] : "System_Datastore"))
        {
            InitializeSets();
        }

        public MembershipDBContext(int ContextCompanyId) : base((Core.SABCoreEngine.Instance.Settings != null &&  Core.SABCoreEngine.Instance.Settings.ContainsKey("SystemDatabaseConnectionStringName") ? SABFramework.Core.SABCoreEngine.Instance.Settings["SystemDatabaseConnectionStringName"] : "System_Datastore"))
        {
            this.ContextCompanyId = ContextCompanyId;
            ContextObjectsFree = false;
            InitializeSets();
        }

        public MembershipDBContext(bool contextObjectsFree) : base((Core.SABCoreEngine.Instance.Settings!=null && Core.SABCoreEngine.Instance.Settings.ContainsKey("SystemDatabaseConnectionStringName") ? SABFramework.Core.SABCoreEngine.Instance.Settings["SystemDatabaseConnectionStringName"] : "System_Datastore"))
        {
            ContextObjectsFree = contextObjectsFree;
            InitializeSets();
        }

        protected virtual void InitializeSets()
        {
            WebsiteSettings = new FilteredDbSet<WebsiteSetting>(this, ContextCompanyId,ContextObjectsFree);
            Webhooks = new FilteredDbSet<Webhook>(this, ContextCompanyId, ContextObjectsFree);
        }

        public static MembershipDBContext Create()
        {
            return new MembershipDBContext();
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<WebsiteSetting>().ToTable("WebsiteSettings");
            modelBuilder.Entity<Webhook>().ToTable("Webhooks");
            modelBuilder.Entity<Privilege>()
                .HasMany(p => p.Roles)
                .WithMany()
                .Map(m => m.MapLeftKey("PrivilegeId").MapRightKey("RoleId").ToTable("PrivilegesRoles"));
            modelBuilder.Entity<Company>()
                .HasOptional(m => m.CompanyUser)
                .WithMany()
                .HasForeignKey(m => m.CompanyUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<Company>()
                .HasMany(m => m.Users)
                .WithMany(m => m.Companies)
                .Map(m => m.MapLeftKey("CompanyId").MapRightKey("UserId").ToTable("CompanyMemberships"));         
        }

        
    }

    public class MembershipDBContext: MembershipDBContext<ApplicationUser>{
        public MembershipDBContext(bool contextObjectsFree) : base(contextObjectsFree) { }
        public MembershipDBContext(int ContextCompanyId) : base(ContextCompanyId) { }
        public MembershipDBContext() : base() { }
    }
}
