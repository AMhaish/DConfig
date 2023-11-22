using DConfigOS_Core.Repositories.IOServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DConfigOS_Core.Models
{
    public class Blob : BlobContainer, IFile
    {
        public string Type { get; set; }
        public long Length { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
    }
}
