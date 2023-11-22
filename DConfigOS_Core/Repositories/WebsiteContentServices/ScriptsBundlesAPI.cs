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
    public class ScriptsBundlesAPI : IScriptsBundlesAPI
    {
        public virtual List<ScriptsBundle> GetScriptsBundles( string creatorId = null)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var scriptsBundles = context.ScriptsBundles.Where(m => creatorId == null || m.CreatorId == creatorId).ToList();
            return scriptsBundles;
        }

        public virtual ScriptsBundle GetScriptBundle(int id, string creatorId = null)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            return context.ScriptsBundles.Where(m => m.Id == id && (creatorId == null || m.CreatorId == creatorId)).SingleOrDefault();
        }

        public virtual int CreateScriptsBundles(ScriptsBundle scriptBundle, string creatorId = null)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var dbScript = context.ScriptsBundles.Where(m => m.Name == scriptBundle.Name && (creatorId == null || m.CreatorId == creatorId)).FirstOrDefault();
            if (dbScript == null)
            {
                scriptBundle.CreateDate = DateTime.Now;
                context.ScriptsBundles.Add(scriptBundle);
                context.SaveChanges();
                return ResultCodes.Succeed;
            }
            else
            {
                return ResultCodes.ObjectAlreadyExists;
            }
        }

        public virtual int UpdateScriptsBundle(ScriptsBundle scriptBundle, string creatorId = null)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var dbScript = context.ScriptsBundles.Where(m => m.Id == scriptBundle.Id && (creatorId == null || m.CreatorId == creatorId)).FirstOrDefault();
            var sameScript = context.ScriptsBundles.Where(m => m.Name == scriptBundle.Name && m.Id != scriptBundle.Id && (creatorId == null || m.CreatorId == creatorId)).FirstOrDefault();
            if (dbScript != null)
            {
                if (sameScript == null)
                {
                    dbScript.Name = scriptBundle.Name;
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
       
        public virtual int DeleteScriptsBundle(int id, string creatorId = null)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var dbScriptBundle = context.ScriptsBundles.Where(m => m.Id == id && (creatorId == null || m.CreatorId == creatorId)).FirstOrDefault();
            if (dbScriptBundle != null)
            {
                var scripts = dbScriptBundle.Scripts.ToList();
                foreach (var s in scripts)
                {
                    context.Scripts.Remove(s);
                }
                context.ScriptsBundles.Remove(dbScriptBundle);
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
