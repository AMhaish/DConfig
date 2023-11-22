using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DConfigOS_Core.Models;
using System.Reflection;
using System.IO;
using System.Text.RegularExpressions;
using DConfigOS_Core.Repositories.Utilities;

namespace DConfigOS_Core.Repositories.AppsServices
{
    public class AppsExtentionsAPI : RepositoryBase<DConfigOS_Core_DBContext>, IAppsExtentionsAPI
    {
    
        public virtual AppExtention GetAppExtention(int id)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            return context.AppExtentions.SingleOrDefault(m => m.Id == id);
        }

        public virtual List<AppExtentionClientLogic> GetAppExtentionClientLogics(int id)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            return context.AppExtentions.SingleOrDefault(m => m.Id == id).AppExtClientLogics.ToList();
        }

        public virtual List<AppExtention> GetIntentExtentions(string intentName)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            return context.AppExtentions.Where(m => m.IntentName == intentName).ToList();
        }
    }
}
