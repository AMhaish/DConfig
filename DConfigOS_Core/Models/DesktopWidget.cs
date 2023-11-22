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
    public class DesktopWidget
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public int WidgetId { get; set; }
        //[ForeignKey("AppId")]
        [JsonIgnore]
        public virtual AppWidget Widget { get; set; }
        [Required]
        public int Order { get; set; }
        [Required]
        public string WidgetClass { get; set; }
    }
}
