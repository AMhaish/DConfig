﻿using DConfigOS_Core.Repositories.IOServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DConfigOS_Core.Models
{
    public class BlobDirectory : IFolder
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string CDNPath { get; set; }
    }
}
