using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using Microsoft.AspNet.Identity.EntityFramework;

namespace DConfigOS_Core.Models
{
    [Serializable]
    public class App
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        [Index(IsUnique=true)]
        public string Name { get; set; }
        [MaxLength(10)]
        public string Version { get; set; }
        [Required]
        [MaxLength(500)]
        public string DllPath { get; set; }
        [Required]
        [MaxLength(500)]
        public string ConfigPath { get; set; }
        [Required] 
        public DateTime InstalledDate { get; set; }
        [JsonIgnore]
        public virtual ICollection<AppView> AppViews { get; set; }
        [JsonIgnore]
        public virtual ICollection<PublicViewsPackage> AppPublicViewsPackages { get; set; }
        [JsonIgnore]
        public virtual ICollection<AppWidget> AppWidgets { get; set; }
        [JsonIgnore]
        public virtual ICollection<AppClientLogic> AppClientLogics { get; set; }
        [JsonIgnore]
        public virtual ICollection<AppExtention> AppExtentions { get; set; }
        [JsonIgnore]
        public virtual ICollection<AppStyleSheet> AppStyleSheets { get; set; }
        [JsonIgnore]
        public virtual ICollection<Form> AppForms { get; set; }
        [JsonIgnore]
        public virtual ICollection<FormSubmitEventType> AppFormSubmitEventsTypes { get; set; }
        public string IconPath { get; set; }
        [Required]
        public bool BuiltInApp { get; set; }
        [Required]
        public string StartPath { get; set; }
        [Required]
        public string LaunchPath { get; set; }
        [NotMapped]
        public string DisplayName { get; set; }
        public virtual ICollection<IdentityRole> AppRoles { get; set; }
    }
}
