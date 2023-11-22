﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace SABFramework.Core.ReturnedTypesHandlers
{
    internal class DownloadResult : ActionResult
    {

        public DownloadResult()
        {
        }

        public DownloadResult(string virtualPath)
        {
            this.VirtualPath = virtualPath;
        }

        public string VirtualPath
        {
            get;
            set;
        }

        public string FileDownloadName
        {
            get;
            set;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            if (!String.IsNullOrEmpty(FileDownloadName))
            {
                string filePath = context.HttpContext.Server.MapPath(this.VirtualPath);

                if (File.Exists(filePath))
                {
                    context.HttpContext.Response.AddHeader("content-disposition","attachment; filename=" + this.FileDownloadName);
                    context.HttpContext.Response.TransmitFile(filePath);
                }
            }
        }
    }
}