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
    public class WidgetView
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        [Required]
        [Index()]
        public int WidgetId { get; set; }
        [JsonIgnore]
        public virtual AppWidget Widget { get; set; }
        [Required]
        [MaxLength(500)]
        public string Path { get; set; }
    }
}
