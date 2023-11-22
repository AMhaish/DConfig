using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;

namespace SABFramework.Core
{
    public interface IAction
    {
        Task<SABActionResult> GetHandler(Controller controller);
        Task<SABActionResult> PostHandler(Controller controller);
        Task<SABActionResult> PutHandler(Controller controller);
        Task<SABActionResult> DeleteHandler(Controller controller);
    }
}