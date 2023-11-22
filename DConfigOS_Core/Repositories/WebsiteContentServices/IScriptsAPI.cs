using DConfigOS_Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DConfigOS_Core.Repositories.WebsiteContentServices
{
    public interface IScriptsAPI 
    {
        List<Script> GetScripts();
        Script GetScript(int id);
        int CreateScript(Script script);
        int UpdateScript(Script script);
        int UpdateScriptsOrder(List<Script> scripts, string creatorId = null);
        int DeleteScript(int id);

    }
}
