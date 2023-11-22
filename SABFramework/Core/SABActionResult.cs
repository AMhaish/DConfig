using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SABFramework.Core.DataCore;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Net;

namespace SABFramework.Core
{
    public enum ReturnedType
    {
        View, PartialView, Download, FileStream, Json, Redirect, UserDefined, String, Empty, HttpNotFound, HttpStatusCode, XMLDocument
    }
    public class SABActionResult
    {
        public Object Model { get; set; }

        public Stream FilteStream { get; set; }

        public string ViewPath { get; set; }

        public string MasterPath { get; set; }

        public string RedirectPath { get; set; }

        public string DownloadPath { get; set; }

        public string DownloadName { get; set; }

        public ReturnedType ReturnedType { get; set; }

        public ActionResult UserDefinedResult { get; set; }

        public HttpStatusCode StatusCode { get; set; }
    }
}
