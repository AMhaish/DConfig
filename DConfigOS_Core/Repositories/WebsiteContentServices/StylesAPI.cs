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
    public class StylesAPI : IStylesAPI
    {
        public virtual List<StyleSheet> GetStyles()
        {
            List<StyleSheet> results = new List<StyleSheet>();
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var styleBundles = context.StylesBundles.ToList();
            foreach (var sb in styleBundles)
            {
                results.AddRange(sb.Styles);
            }
            results = results.OrderBy(m => m.Name).ToList();
            return results;
        }

        public virtual StyleSheet GetStyle(int id)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            return context.StyleSheets.Where(m => m.Id == id).SingleOrDefault();
        }

        public virtual int CreateStyle(StyleSheet styleSheet)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var dbStyleExists = context.StylesBundles.Any(m => m.Styles.Any(n => n.Name == styleSheet.Name));
            if (!dbStyleExists)
            {
                styleSheet.CreateDate = DateTime.Now;
                styleSheet.Path = "~" + styleSheet.Path.Replace('\\', '/');
                context.StyleSheets.Add(styleSheet);
                context.SaveChanges();
                return ResultCodes.Succeed;
            }
            else
            {
                return ResultCodes.ObjectAlreadyExists;
            }
        }

        public virtual int UpdateStyle(StyleSheet styleSheet)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var dbStyle = context.StyleSheets.Where(m => m.Id == styleSheet.Id).FirstOrDefault();
            var sameStyleExists = context.StylesBundles.Any(m => m.Styles.Any(n => n.Name == styleSheet.Name));
            if (dbStyle != null)
            {
                if (!sameStyleExists)
                {
                    dbStyle.Name = styleSheet.Name;
                    dbStyle.Path = "~" + styleSheet.Path.Replace('\\', '/');
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

        public virtual int UpdateStylesOrder(List<StyleSheet> styles, string creatorId = null)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            Dictionary<int, int> ids = styles.ToDictionary(m => m.Id, m => m.Priority);
            var dbStyles = context.StyleSheets.Where(m => ids.Keys.Contains(m.Id));
            if (dbStyles != null && dbStyles.Count() > 0)
            {
                foreach (StyleSheet c in dbStyles)
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

        public virtual int DeleteStyle(int id)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var dbStyle = context.StyleSheets.Where(m => m.Id == id).FirstOrDefault();
            if (dbStyle != null)
            {
                context.StyleSheets.Remove(dbStyle);
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
