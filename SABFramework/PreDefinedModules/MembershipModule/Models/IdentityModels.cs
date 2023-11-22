using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNet.Identity;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SABFramework.PreDefinedModules.MembershipModule.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    [Table("AspNetUsers")]
    public class ApplicationUser : IdentityUser
    {
        public bool IsEnabled { get; set; }
        [JsonIgnore]
        public virtual ICollection<Company> Companies { get; set; }

        //[Index]
        //public int? ContextCompanyId { get; set; }
        //[JsonIgnore]
        //[ForeignKey("ContextCompanyId")]
        //public virtual Company ContextOwnerCompany { get; set; }

        //public ApplicationUser()
        //{
        //    if (MembershipProvider.Instance.ContextCompanyId.HasValue)
        //    {
        //        ContextCompanyId = MembershipProvider.Instance.ContextCompanyId.Value;
        //    }
        //}

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager,string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Add custom user claims here
            return userIdentity;
        }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

}