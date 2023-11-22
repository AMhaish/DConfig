using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNet.Identity.EntityFramework;
using Newtonsoft.Json;

namespace SABFramework.PreDefinedModules.MembershipModule.Models
{
    public class Company
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        [MaxLength(100)]
        public string Address { get; set; }
        [MaxLength(20)]
        public string City { get; set; }
        [MaxLength(20)]
        public string Country { get; set; }
        [MaxLength(50)]
        public string Website { get; set; }
        public DateTime CreateDate { get; set; }
        [MaxLength(50)]
        public string PhoneNumber { get; set; }
        [MaxLength(50)]
        public string TaxOffice { get; set; }
        [MaxLength(50)]
        public string TaxNumber { get; set; }
        [MaxLength(50)]
        public string ContactPersonName { get; set; }
        [MaxLength(50)]
        public string ContactPersonEmail { get; set; }
        [MaxLength(50)]
        public string ContactPersonPhoneNumber { get; set; }

        public virtual ICollection<SABFramework.PreDefinedModules.MembershipModule.Models.ApplicationUser> Users { get; set; }
        public string CompanyUserId { get; set; }

        [JsonIgnore]
        public virtual SABFramework.PreDefinedModules.MembershipModule.Models.ApplicationUser CompanyUser { get; set; }

        [NotMapped]
        public bool Current { get; set; }

        [MaxLength(50)]
        public string SubscriptionType { get; set; }

        [MaxLength(50)]
        public string AccountId { get; set; }
        //[Index]
        //public int? ContextCompanyId { get; set; }
        //[JsonIgnore]
        //[ForeignKey("ContextCompanyId")]
        //public virtual Company ContextOwnerCompany { get; set; }

        //public Company()
        //{
        //    if (MembershipProvider.Instance.ContextCompanyId.HasValue)
        //    {
        //        ContextCompanyId = MembershipProvider.Instance.ContextCompanyId.Value;
        //    }
        //}
    }
}
