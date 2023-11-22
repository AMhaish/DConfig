using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace CompetitiveAnalysis.Models
{
    public class Price
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [MaxLength(500)]
        public string Reference { get; set; }
        public double? PriceValue { get; set; }
        [MaxLength(10)]
        public string Currency {get;set;}
        public DateTime CreateDate { get; set; }
        public DateTime PriceDate { get; set; }
        [Required]
        public int ProductId { get; set; }
        [JsonIgnore]
        public virtual Product Product { get; set; }
        [MaxLength(6)]
        public string PriceType { get; set; }// SSRP-NORM-QYT
        public int? QuantityFrom { get; set; }
        public int? QuantityTo { get; set; }

        public const string PriceType_SSRP = "SSRP";
        public const string PriceType_NORM = "NORM";
        public const string PriceType_QYT = "QYT";
    }
}
