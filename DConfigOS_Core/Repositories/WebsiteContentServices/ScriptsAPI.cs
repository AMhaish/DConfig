using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DConfigOS_Core.Models;
using DConfigOS_Core.Models.Utilities;
using System.Reflection;
using System.IO;
using System.Text.RegularExpressions;
using DConfigOS_Core.Repositories.Utilities;

namespace DConfigOS_Core.Repositories.WebsiteContentServices
{
    public class ScriptsAPI:IScriptsAPI
    {
        public virtual List<Script> GetScripts()
        {
            List<Script> results = new List<Script>();
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var scriptBundles = context.ScriptsBundles.ToList();
            foreach (var sb in scriptBundles)
            {
                results.AddRange(sb.Scripts);
            }
            results = results.OrderBy(m => m.Name).ToList();
            return results;
        }

        public virtual Script GetScript(int id)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            return context.Scripts.Where(m => m.Id == id).SingleOrDefault();
        }

        public virtual int CreateScript(Script script)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var dbScriptExists = context.ScriptsBundles.Any(m => m.Scripts.Any(n => n.Name == script.Name));
            if (!dbScriptExists)
            {
                script.CreateDate = DateTime.Now;
                script.Path = "~" + script.Path.Replace('\\', '/');
                context.Scripts.Add(script);
                context.SaveChanges();
                return ResultCodes.Succeed;
            }
            else
            {
                return ResultCodes.ObjectAlreadyExists;
            }
        }

        public virtual int UpdateScript(Script script)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var dbScript = context.Scripts.Where(m => m.Id == script.Id).FirstOrDefault();
            var sameScriptExists = context.ScriptsBundles.Any(m => m.Scripts.Any(n => n.Name == script.Name));
            if (dbScript != null)
            {
                if (!sameScriptExists)
                {
                    dbScript.Name = script.Name;
                    dbScript.Path = "~" + script.Path.Replace('\\', '/');
                    context.SaveChanges();
                    return ResultCodes.Succeed;
                }
                else
                {
                    return ResultCodes.ObjectNameAlreadyUsed;
                }
            }
            else
            {
                return ResultCodes.ObjectHasntFound;
            }
        }

        public virtual int UpdateScriptsOrder(List<Script> scripts, string creatorId = null)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            Dictionary<int, int> ids = scripts.ToDictionary(m => m.Id, m => m.Priority);
            var dbScripts = context.Scripts.Where(m => ids.Keys.Contains(m.Id));
            if (dbScripts != null && dbScripts.Count() > 0)
            {
                foreach (Script c in dbScripts)
                {
                    c.Priority = ids[c.Id];
                }
                context.SaveChanges();
                return ResultCodes.Succeed;
            }
            else
            {
                return ResultCodes.ObjectHasntFound;
            }
        }

        public virtual int DeleteScript(int id)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var dbScript = context.Scripts.Where(m => m.Id == id).FirstOrDefault();
            if (dbScript != null)
            {
                context.Scripts.Remove(dbScript);
                context.SaveChanges();
                return ResultCodes.Succeed;
            }
            else
            {
                return ResultCodes.ObjectHasntFound;
            }
        }


    }
}
