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
    public class WidgetsAPIRepository : RepositoryBase<DConfigOS_Core_DBContext>, IWidgetsAPIRepository
    {
        public virtual List<AppWidget> GetInstalledWidgets()
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            return context.AppWidgets.ToList();
        }

        public virtual List<DesktopWidget> GetDesktopWidgets()
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            return context.DesktopWidgets.OrderBy(m => m.Order).ToList();
        }

        public virtual int AddWidgetToDesktop(int widgetId, int order)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            DesktopWidget dw = new DesktopWidget();
            var widget = context.AppWidgets.SingleOrDefault(m => m.Id == widgetId);
            if (widget != null)
            {
                dw.Order = order;
                context.DesktopWidgets.Add(dw);
                context.SaveChanges();
                return ResultCodes.Succeed;
            }
            else
                return ResultCodes.ObjectHasntFound;
        }

        public virtual int RemoveWidgetFromDesktop(int id)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var widget = context.DesktopWidgets.SingleOrDefault(m => m.Id == id);
            if (widget != null)
            {
                context.DesktopWidgets.Remove(widget);
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
