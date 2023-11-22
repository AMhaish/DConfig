using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DConfigOS_Core.Layer2.ActionsModels;
using DConfigOS_Core.Repositories.IOServices;
using DConfigOS_Core.Models;
using System.IO;
using Ninject;

namespace DConfigOS_Core.Layer2.IOServices
{
    public class GetResourcesFilesAction : SABFramework.Core.SABAction
    {
        public string Path { get; set; }

        [Inject]
        public IFoldersAPI foldersAPI { get; set; }

        public override async Task<SABFramework.Core.SABActionResult> GetHandler(System.Web.Mvc.Controller controller)
        {
            var result = await foldersAPI.GetFolderFiles(Path);
            return Json(result);
        }


    }
}
