using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DConfigOS_Core.Models
{
    [NotMapped]
    public class LangValue
    {       
        public string Lang { get; set; }
        public string Value { get; set; }        
    }
}
