using DConfigOS_Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DConfigOS_Core.Repositories.AppsServices
{
    public interface IWidgetsAPIRepository : IDisposable
    {
       List<AppWidget> GetInstalledWidgets();
       List<DesktopWidget> GetDesktopWidgets();
       int AddWidgetToDesktop(int widgetId, int order);
       int RemoveWidgetFromDesktop(int id);
    }
}
