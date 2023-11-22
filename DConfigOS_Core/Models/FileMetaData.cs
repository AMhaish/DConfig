using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DConfigOS_Core.Models
{
    public class FileMetaData
    {

        public string FileName { get; set; }

        public string FileExtension { get; set; }

        public long FileSize { get; set; }

        public string FilePath { get; set; }


        #region Constructor  

        public FileMetaData()
        {
        }

        #endregion
    }
}
