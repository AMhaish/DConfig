using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace DConfigOS_Core.Models
{
    [Serializable]
    public class AppExtention
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        [Required]
        [Index()]
        public int AppId { get; set; }
        //[ForeignKey("AppId")]
        [JsonIgnore]
        public virtual App App { get; set; }
        [Required]
        [MaxLength(50)]
        [Index(IsUnique=false)]
        public string IntentName { get; set; }
        [JsonIgnore]
        public virtual ICollection<AppExtentionView> AppExtViews { get; set; }
        [JsonIgnore]
        public virtual ICollection<AppExtentionClientLogic> AppExtClientLogics { get; set; }
    }
}
