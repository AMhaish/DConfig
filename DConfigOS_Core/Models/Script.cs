using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using System.Xml.Serialization;
using System.Xml;

namespace DConfigOS_Core.Models
{
    [Serializable]
    public class Script
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        [XmlAttribute("name")]
        public string Name { get; set; }
        [Required]
        [MaxLength(500)]
        [XmlAttribute("path")]
        public string Path { get; set; }
        public int? PublicViewsPackageId { get; set; }
        //[ForeignKey("PublicViewsPackageId")]
        [JsonIgnore]
        public virtual PublicViewsPackage PublicViewsPackage { get; set; }
        [Index()]
        public int? BundleId { get; set; }
        [JsonIgnore]
        public virtual ScriptsBundle Bundle { get; set; }
        [Required]
        public DateTime CreateDate { get; set; }
        [XmlAttribute("priority")]
        public int Priority { get; set; }


    }
}
