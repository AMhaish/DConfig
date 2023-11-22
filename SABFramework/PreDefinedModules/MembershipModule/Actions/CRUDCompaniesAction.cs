using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SABFramework.Core;
using SABFramework.PreDefinedModules.MembershipModule;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Security.Claims;
using SABFramework.PreDefinedModules.MembershipModule.DataProviders;
using SABFramework.PreDefinedModules.MembershipModule.Models;
using Ninject;

namespace SABFramework.PreDefinedModules.MembershipModule.Actions
{
    public class CRUDCompaniesAction : SABFramework.Core.SABAction
    {
        [Inject]
        public ICompaniesAPI companiesAPI { get; set; }

        [Required]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Website { get; set; }
        public string PhoneNumber { get; set; }
        public string TaxOffice { get; set; }
        public string TaxNumber { get; set; }
        public string ContactPersonName { get; set; }
        public string ContactPersonEmail { get; set; }
        public string ContactPersonPhoneNumber { get; set; }
        public string OwnerId { get; set; }
        public string RoleName { get; set; }
        public string SubscriptionType { get; set; }
        public override async Task<SABFramework.Core.SABActionResult> GetHandler(System.Web.Mvc.Controller controller)
        {
            List<Company> companies;
            if (String.IsNullOrEmpty(Name))
            {
                companies = companiesAPI.GetCompanies();
            }
            else
            {
                companies = companiesAPI.GetCompaniesByName(Name);
            }
            return Json(companies);
        }

        public override async Task<SABFramework.Core.SABActionResult> PostHandler(System.Web.Mvc.Controller controller)
        {
            if (controller.ModelState.IsValid && !String.IsNullOrEmpty(Name))
            {
                var p = new Company()
                {
                    Id = Id,
                    Name = Name,
                    Address = Address,
                    City = City,
                    Country = Country,
                    Website = Website,
                    PhoneNumber = PhoneNumber,
                    TaxOffice = TaxOffice,
                    TaxNumber = TaxNumber,
                    ContactPersonEmail = ContactPersonEmail,
                    ContactPersonName = ContactPersonName,
                    ContactPersonPhoneNumber = ContactPersonPhoneNumber,
                    SubscriptionType = SubscriptionType,
                };
                var result = companiesAPI.CreateDConfigCompany(p, OwnerId);
                if (!String.IsNullOrEmpty(OwnerId) && !String.IsNullOrEmpty(RoleName) && RoleName != "Administrators")
                {
                    IdentityResult x = await MembershipProvider.Instance.UserManager.AddToRoleAsync(OwnerId, RoleName);
                    if (x.Succeeded)
                    {
                        return Json(new { result = "true", obj = p });
                    }
                    else
                    {
                        return Json(new { result = "false", message = "Couldn't assign the user to the target role" });
                    }
                }
                switch (result)
                {
                    case true:
                        return Json(new { result = "true", obj = p });
                    case false:
                        return Json(new { result = "false", message = "Company with the same name is already exists" });
                }
            }
            else
            {
                return Json(new { result = "false", message = "Name is required to add a company" });
            }
            return Json(new { result = "false" });
        }

        public override async Task<SABFramework.Core.SABActionResult> PutHandler(System.Web.Mvc.Controller controller)
        {
            if (controller.ModelState.IsValid)
            {
                var p = new Company()
                {
                    Id = Id,
                    Name = Name,
                    Address = Address,
                    City = City,
                    Country = Country,
                    Website = Website,
                    PhoneNumber = PhoneNumber,
                    TaxOffice = TaxOffice,
                    TaxNumber = TaxNumber,
                    ContactPersonEmail = ContactPersonEmail,
                    ContactPersonName = ContactPersonName,
                    ContactPersonPhoneNumber = ContactPersonPhoneNumber,
                    SubscriptionType = SubscriptionType
                };
                var result = companiesAPI.UpdateDConfigCompany(p);
                switch (result)
                {
                    case true:
                        return Json(new { result = "true", obj = p });
                    case false:
                        return Json(new { result = "false", message = "Company hasn't been found to be updated, or there is already a company with the same name" });
                }
            }
            else
            {
                return Json(new { result = "false", message = "Company Id is required to be updated" });
            }
            return Json(new { result = "false" });
        }

        public override async Task<SABFramework.Core.SABActionResult> DeleteHandler(System.Web.Mvc.Controller controller)
        {
            if (Id != 0)
            {
                var result = companiesAPI.DeleteDConfigCompany(Id);
                switch (result)
                {
                    case true:
                        return Json(new { result = "true" });
                    case false:
                        return Json(new { result = "false", message = "Company hasn't been found to be deleted" });
                }
            }
            else
            {
                return Json(new { result = "false", message = "Id is required to delete the instance" });
            }
            return Json(new { result = "false" });
        }
    }
}
