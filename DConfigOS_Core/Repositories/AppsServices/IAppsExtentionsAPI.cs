using DConfigOS_Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DConfigOS_Core.Repositories.AppsServices
{
    public interface IAppsExtentionsAPI : IDisposable
    {
        AppExtention GetAppExtention(int id);
        List<AppExtentionClientLogic> GetAppExtentionClientLogics(int id);
        List<AppExtention> GetIntentExtentions(string intentName);
    }
}
