using DConfigOS_Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DConfigOS_Core.Repositories.WebsiteContentServices
{
    public interface IScriptsBundlesAPI
    {
         List<ScriptsBundle> GetScriptsBundles(string creatorId = null);
         ScriptsBundle GetScriptBundle(int id, string creatorId = null);
         int CreateScriptsBundles(ScriptsBundle scriptBundle, string creatorId = null);
         int UpdateScriptsBundle(ScriptsBundle scriptBundle, string creatorId = null);
         int DeleteScriptsBundle(int id, string creatorId = null);

    }
}
